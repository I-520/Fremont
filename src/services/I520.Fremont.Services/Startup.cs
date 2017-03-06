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
        private const string ClientId = "b0cb06bd-c43d-4045-9efc-5b83dc44c691";

        private const string DocumentDbAccountKey = "";

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            AddCustomConfiguration(services);

            services.AddSwaggerGen(
                c =>
                {
                    var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                    var xmlPath = Path.Combine(basePath, "I520.Fremont.Services.xml");

                    c.IncludeXmlComments(xmlPath);
                });

            services.AddSingleton<IProjectRepository>(provider =>
            {
                var url = Configuration.GetValue<string>("DocumentDbUrl");

                return new ProjectDocumentDbRepository(url, DocumentDbAccountKey);
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

        private void AddCustomConfiguration(IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<ApplicationConfiguration>(Configuration.GetSection("DocumentDbSettings"));
        }

        /* private static async Task<string> GetKeyValueSecret(string keyValueUrl, string secretKey)
        {
            var cerificateThumbprint = "‎e42c77220919b18215a97986c5824bd4ab9e1716";
            var authenticationClientId = "1c2cd3de-87e4-4e65-a0d7-23b543e6a530";

            var certificate = FindCertificateByThumbprint(cerificateThumbprint);
            var assertionCert = new ClientAssertionCertificate(authenticationClientId, certificate);

            var client = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(
                    (authority, resource, scope) => GetAccessTokenAsync(authority, resource, scope, assertionCert)),
                new System.Net.Http.HttpClient());

            var secret = await client.GetSecretAsync(keyValueUrl, "fremontDocumentDbKey");

            return secret.Value;
        }

        public static async Task<string> GetAccessTokenAsync(string authority, string resource, string scope, ClientAssertionCertificate assertionCert)
        {
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);

            var result = await context.AcquireTokenAsync(resource, assertionCert);

            return result.AccessToken;
        }

        private static X509Certificate2 FindCertificateByThumbprint(string certificateThumbprint)
        {
            if (certificateThumbprint == null)
            {
                throw new ArgumentNullException("certificateThumbprint");
            }

            using (X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection col = store.Certificates.Find(X509FindType.FindByThumbprint, certificateThumbprint, false); // Don't validate certs, since the test root isn't installed.

                if (col == null || col.Count == 0)
                {
                    throw new Exception(
                        string.Format("Could not find the certificate with thumbprint {0} in the Local Machine's Personal certificate store.",
                        certificateThumbprint));
                }

                return col[0];
            }
        }*/
    }
}
