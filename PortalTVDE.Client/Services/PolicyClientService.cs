using PortalTVDE.Client.Services.Interfaces;
using PortalTVDE.Shared.ModelsDTOs;
using System.Net.Http.Json;

namespace PortalTVDE.Client.Services
{
    public class PolicyClientService : IPolicyClientService
    {
        private readonly HttpClient _http;

        public PolicyClientService(HttpClient http)
        {
            _http = http;
        }

        public async Task<PolicyDto?> GetPolicyByIdAsync(int id)
            => await _http.GetFromJsonAsync<PolicyDto>($"api/policies/{id}");

        public async Task<List<PolicyDto>> GetAllPoliciesAsync()
        {
            var result = await _http.GetFromJsonAsync<List<PolicyDto>>("api/policies");
            return result ?? new List<PolicyDto>();
        }
    }
}
