using Microsoft.Graph;

namespace GraphAPITest
{
    internal interface IGraphServiceClientFactory
    {
        GraphServiceClient Create(string[] scopes);
    }
}