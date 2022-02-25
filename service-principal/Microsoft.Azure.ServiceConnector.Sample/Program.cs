using System;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Azure.ServiceConnector.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        //
        // This example uses the Microsoft.Azure.AppConfiguration.AspNetCore NuGet package:
        // - Establishes the connection to Azure App Configuration using DefaultAzureCredential, since ServiceConnector enabled systemManagedIdentity auth between WebApp and App Config.
        // - Loads configuration from Azure App Configuration.
        // - Sets up dynamic configuration refresh triggered by a sentinel key.

        // Prerequisite
        // - An Azure App Configuration store is created
        // - The application identity is granted "App Configuration Data Reader" role in the App Configuration store
        // - "AzureAppConfigurationEndpoint" is set to the App Configuration endpoint from WebApp's AppSetting.
        // - The test configuration items are imported to the App Configuration store
        // - Key 'SampleApplication:Settings:Messsages" in App Configuration to signal the refresh of configuration

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration(builder =>
                    {
                        var settings = builder.Build();
                        // Service Connector configured AZURE_APPCONFIGURATION_ENDPOINT at Azure WebApp's AppSetting already.
                        string appConfigurationEndpoint = Environment.GetEnvironmentVariable("AZURE_APPCONFIGURATION_ENDPOINT");
                        string clientId = Environment.GetEnvironmentVariable("AZURE_APPCONFIGURATION_CLIENTID");
                        string clientSecret = Environment.GetEnvironmentVariable("AZURE_APPCONFIGURATION_CLIENTSECRET");
                        string tenantId = Environment.GetEnvironmentVariable("AZURE_APPCONFIGURATION_TENANTID");
                        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                        
                        if (!string.IsNullOrEmpty(appConfigurationEndpoint))
                        {
                            builder.AddAzureAppConfiguration(options =>
                            {
                                // Service Connector configured system Managed Identity of the Azure WebApp, and grant App Configuration Data Reader role of the AppConfig to it.
                                options.Connect(new Uri(appConfigurationEndpoint), credential)
                                       .Select(keyFilter: "SampleApplication:*")
                                       .ConfigureRefresh((refreshOptions) =>
                                       {
                                           // Indicates that all configuration should be refreshed when the given key has changed.
                                           refreshOptions.Register(key: "SampleApplication:Settings:Messages", refreshAll: true);
                                       });
                            });
                        }
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}
