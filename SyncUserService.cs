using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

namespace GraphAPITest
{
    internal class SyncUserService : BackgroundService
    {
        private readonly ILogger<SyncUserService> logger;
        private readonly IGraphServiceClientFactory graphServiceClientFactory;
        private readonly IHostApplicationLifetime hostApplicationLifetime;
        private readonly string[] userListScopes = new string[] { "https://graph.microsoft.com/.default" };

        public SyncUserService(
            ILogger<SyncUserService> logger,
            IGraphServiceClientFactory graphServiceClientFactory, 
            IHostApplicationLifetime hostApplicationLifetime)
        {
            this.logger = logger;
            this.graphServiceClientFactory = graphServiceClientFactory;
            this.hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var graphServiceClient = graphServiceClientFactory.Create(userListScopes);
                    IGraphServiceUsersCollectionPage users = await graphServiceClient.Users.Request().GetAsync(stoppingToken);
                    foreach (var user in users)
                    {
                        logger.LogInformation("User: {User}", user.UserPrincipalName);
                    }
                    await Task.Delay(30000, stoppingToken);
                }
                catch (TaskCanceledException taskCanceledException)
                {
                    logger.LogError(taskCanceledException, "Service stopped");
                    return;
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "Fatal error occured");
                    hostApplicationLifetime.StopApplication();
                    return;
                }
            }
        }
    }
}
