using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ICM_ROTA_MVC.Services
{
    public class OutlookService
    {
        private readonly GraphServiceClient _graphClient;
        private readonly string _tenantId;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public OutlookService(IConfiguration configuration)
        {
            _tenantId = configuration["AzureAd:TenantId"];
            _clientId = configuration["AzureAd:ClientId"];
            _clientSecret = configuration["AzureAd:ClientSecret"];

            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var clientSecretCredential = new ClientSecretCredential(_tenantId, _clientId, _clientSecret, options);
            _graphClient = new GraphServiceClient(clientSecretCredential);
        }

        public async Task<List<Microsoft.Graph.Models.Message>> GetRecentEmailsAsync(string userEmail)
        {
            try
            {
                var result = await _graphClient.Users[userEmail].Messages.GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Top = 10;
                    requestConfiguration.QueryParameters.Select = new[] { "subject", "bodyPreview", "body", "from", "receivedDateTime" };
                    requestConfiguration.QueryParameters.Orderby = new[] { "receivedDateTime desc" };
                });

                return result?.Value ?? new List<Microsoft.Graph.Models.Message>();
            }
            catch (Exception ex)
            {
                // Log exception
                return new List<Microsoft.Graph.Models.Message>();
            }
        }
    }
}
