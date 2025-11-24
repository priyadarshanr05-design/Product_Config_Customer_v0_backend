using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Product_Config_Customer_v0.Services
{
    public class ParentAPI_GenToken_Service
    {
        private readonly DomainManagementDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ParentAPI_GenToken_Service> _logger;

        private string? _cachedToken;
        private DateTime _tokenExpiry;
        private readonly SemaphoreSlim _tokenLock = new(1, 1);

        public ParentAPI_GenToken_Service(
            DomainManagementDbContext dbContext,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<ParentAPI_GenToken_Service> logger)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        
        public string ModelUrl => _configuration["ExternalModelService:ModelUrl"];

        public async Task<bool> CanAccessDomainAsync(string domainName, bool isAuthenticated)
        {
            var domainConfig = await _dbContext.AnonymousRequestControls
                .FirstOrDefaultAsync(d => d.DomainName == domainName);

            if (domainConfig == null)
                throw new KeyNotFoundException($"Domain '{domainName}' not found.");

            return domainConfig.AllowAnonymousRequest || isAuthenticated;
        }

        public async Task<string> GetBearerTokenAsync(string domainName)
        {
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
                return _cachedToken!;

            await _tokenLock.WaitAsync();
            try
            {
                if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
                    return _cachedToken!;

                var tokenUrl = Environment.GetEnvironmentVariable("EXTERNALMODEL_TOKEN_URL") ?? string.Empty;
                var username = Environment.GetEnvironmentVariable("EXTERNALMODEL_USERNAME") ?? string.Empty;
                var password = Environment.GetEnvironmentVariable("EXTERNALMODEL_PASSWORD") ?? string.Empty;

                var client = _httpClientFactory.CreateClient();
                var tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenUrl);

                var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
                tokenRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);
                tokenRequest.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var payload = JsonSerializer.Serialize(new { DomainName = domainName });
                tokenRequest.Content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(tokenRequest);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Token request failed: {response.StatusCode} {content}");

                using var jsonDoc = JsonDocument.Parse(content);
                if (jsonDoc.RootElement.TryGetProperty("Token", out var tokenProp))
                {
                    var token = tokenProp.GetString();
                    if (!string.IsNullOrEmpty(token))
                    {
                        _cachedToken = token;
                        _tokenExpiry = DateTime.UtcNow.AddMinutes(50);
                        _logger.LogInformation("Token acquired successfully for domain {Domain}", domainName);
                        return token;
                    }
                }

                throw new Exception("Bearer token not found in response.");
            }
            finally
            {
                _tokenLock.Release();
            }
        }
    }
}
