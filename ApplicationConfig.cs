using System.Globalization;

namespace GraphAPITest
{
    internal class ApplicationConfig
    {
        private string Instance { get; }
        private string Tenant { get; }

        public string ApiUrl { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }

        public string Authority { get { return String.Format(CultureInfo.InvariantCulture, Instance, Tenant); } }

        public ApplicationConfig(string apiUrl,string clientId, string clientSecret, string instance, string tenant)
        {
            ApiUrl = apiUrl.TrimEnd('/');
            ClientId = clientId;
            ClientSecret = clientSecret;
            Instance = instance;
            Tenant = tenant;
        }
    }
}
