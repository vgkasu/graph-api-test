using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace GraphAPITest
{
    internal class GraphServiceClientFactory : IGraphServiceClientFactory
    {
        private readonly IConfidentialClientApplication confidentialClientApplication;
        private readonly ApplicationConfig graphServiceClientConfig;

        public GraphServiceClientFactory(
            IConfidentialClientApplication confidentialClientApplication, 
            ApplicationConfig graphServiceClientConfig)
        {
            this.confidentialClientApplication = confidentialClientApplication;
            this.graphServiceClientConfig = graphServiceClientConfig;
        }

        public GraphServiceClient Create(string[] scopes)
        {
            GraphServiceClient graphServiceClient = new($"{graphServiceClientConfig.ApiUrl}/V1.0/", new DelegateAuthenticationProvider(async (requestMessage) =>
                    {
                        AuthenticationResult result = await confidentialClientApplication.AcquireTokenForClient(scopes).ExecuteAsync();
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                    }));

            return graphServiceClient;
        }
    }
}
