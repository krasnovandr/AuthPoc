using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using Swashbuckle.AspNetCore.Swagger;
using UserManagement.API.Identity;
using UserManagement.DI;
using UserManagement.IdentityManagement;
using UserManagement.Notification;
using UserManagement.Notification.EmailBuilders;
using UserManagement.Notification.EmailClient;
using UserManagement.Notification.EmailTemplates;
using UserManagement.Notification.Models;

namespace UserManagement.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.InstallIdentityDependencies(Configuration);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "User Management API", Version = "v1" });
            });

            services.AddSingleton(sp =>
            {
                var emailServerConnectionSettings = new EmailServerConnectionSettings
                {
                    ApiKey = Configuration["SendGridApiKey"],
                    SenderEmailAddress = Configuration["SendGridSenderEmail"],
                    SenderName = Configuration["SendGridSenderName"]
                };

                return emailServerConnectionSettings;
            });

            //services.AddTransient<IEmailHtmlContentBuilder, EmailHtmlContentBuilder>();
            services.AddTransient<EmailBuilder<UserActivationEmailModel>, UserActivationEmailBuilder>();
            services.AddTransient<EmailBuilder<ResetPasswordEmailModel>, ForgotPasswordEmailBuilder>();

            services.AddTransient<INotificationService, NotificationService>((serviceProvider) =>
            {
                var emailConnectionSettings = serviceProvider.GetRequiredService<EmailServerConnectionSettings>();
                var emailFactory = new EmailClientFactory();
                var sendGridClient = emailFactory.CreateClient(emailConnectionSettings.ApiKey);
                var emailBuilderRetriever = new EmailBuilderRetriever(serviceProvider, emailConnectionSettings);
                return new NotificationService(sendGridClient, emailBuilderRetriever);
            });
            services.AddMvc();


            var assembly = typeof(INotificationService).GetTypeInfo().Assembly;
            var embeddedFileProvider = new EmbeddedFileProvider(
                assembly,
                "ViewComponentLibrary"
            );

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Clear();
                options.FileProviders.Add(new PhysicalFileProvider(Directory.GetCurrentDirectory()));
            });

            services.AddTransient<IRazorViewToStringRenderer, RazorViewToStringRenderer>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(Configuration["SwaggerEndpoint"], "User Management API");
            });
            app.UseMvc();
        }
    }
}
