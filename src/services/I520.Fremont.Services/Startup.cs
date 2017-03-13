using I520.Fremont.Services.Data;
using I520.Fremont.Services.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

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

            AddAzureKeyVaultAsConfigurationSource(builder);
        }

        private void AddAzureKeyVaultAsConfigurationSource(IConfigurationBuilder builder)
        {
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));

            builder.AddAzureKeyVault(
                Configuration["KeyVault:Name"],
                keyVaultClient,
                new DefaultKeyVaultSecretManager());

            Configuration = builder.Build();
        }

        private ClientAssertionCertificate GetCert()
        {
            X509Certificate2 cert;

            using (X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                cert = store.FindCertificateByThumbprint(Configuration["AzureAd:CertThumbprint"]);
            }

            return new ClientAssertionCertificate(Configuration["AzureAd:ClientId"], cert);
        }

        private async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            var result = await context.AcquireTokenAsync(resource, GetCert());
            return result.AccessToken;
        }

        private void AddCustomConfiguration(IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<IConfiguration>(Configuration);
        }
    }
}
