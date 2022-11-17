using GraphAPITest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(appConfiguraion =>
    {
        appConfiguraion.AddJsonFile("appsettings.json", false, false);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton((serviceProvider) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var applicationConfig = configuration.GetSection("ApplicationConfig").Get<ApplicationConfig>();
            if (applicationConfig == null ||
               string.IsNullOrWhiteSpace(applicationConfig.ClientId) ||
               string.IsNullOrWhiteSpace(applicationConfig.ClientSecret) ||
               string.IsNullOrWhiteSpace(applicationConfig.Authority))
            {
                throw new ApplicationException("Graph service client configuration is missing. ClientId, ClientSecret and Authority is mandatory");
            }
            return applicationConfig;
        });

        services.AddSingleton((serviceProvider) =>
        {
            var graphServiceClientConfig = serviceProvider.GetRequiredService<ApplicationConfig>();
            var confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(graphServiceClientConfig.ClientId)
                              .WithClientSecret(graphServiceClientConfig.ClientSecret)
                              .WithAuthority(new Uri(graphServiceClientConfig.Authority))
                              .Build();
            confidentialClientApplication.AddInMemoryTokenCache();
            return confidentialClientApplication;
        });

        services.AddSingleton<IGraphServiceClientFactory, GraphServiceClientFactory>();
        services.AddHostedService<SyncUserService>();
    })
    .Build();

await host.RunAsync();