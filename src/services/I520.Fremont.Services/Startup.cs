using I520.Fremont.Services.Configuration;
using I520.Fremont.Services.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace I520.Fremont.Services
{
    public class Startup
    {
        public Startup(IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            BuildConfiguration(environment);
        }

        public IConfigurationRoot Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            AddCustomConfiguration(services);

            AddSwaggerConfiguration(services);

            services.AddSingleton<IProjectRepository>(provider =>
            {
                return new ProjectDocumentDbRepository(Configuration["FremontDocumentDb:Url"], Configuration["FremontDocumentDb:AccountKey"]);
            });

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUi();
        }

        private static void AddSwaggerConfiguration(IServiceCollection services)
        {
            services.AddSwaggerGen(
                c =>
                {
                    var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                    var xmlPath = Path.Combine(basePath, "I520.Fremont.Services.xml");

                    c.IncludeXmlComments(xmlPath);
                });
        }

        private void BuildConfiguration(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            /*  TODO: <tmerkel> Currently there is a bug that causes an error here.  Looks like it's fixed in 1.1.1.
             *  We'll wait... https://www.google.com/url?sa=t&rct=j&q=&esrc=s&source=web&cd=2&cad=rja&uact=8&ved=0ahUKEwjWxY-xm8HSAhUW4mMKHVKlAyQQFggiMAE&url=https%3A%2F%2Fgithub.com%2Faspnet%2FConfiguration%2Fissues%2F569&usg=AFQjCNGRAc1l0KSGDanfliGeU9bTN3fztg&sig2=xc0w3aky1AHLbiInoE7ikg
            builder.AddAzureKeyVault(
                Configuration["KeyVault:Name"],
                Configuration["AzureAd:ClientId"],
                Configuration["AzureAd:ClientSecret"]);
                */
        }

        private void AddCustomConfiguration(IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<ApplicationConfiguration>(Configuration.GetSection("DocumentDbSettings"));
            services.Configure<IConfiguration>(Configuration);
        }
    }
}
