using Microsoft.AspNetCore.WebUtilities;
using PortalTVDE.Client.Services.Interfaces;
using PortalTVDE.Shared.ModelsDTOs;
using System.Net.Http.Json;

namespace PortalTVDE.Client.Services
{
    public class MediatorClientService : IMediatorClientService
    {
        private readonly HttpClient _httpClient;

        public MediatorClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PaginatedResultDto<MediatorDto>> GetMediatorsAsync(MediatorQueryDto query)
        {
            var queryParams = new Dictionary<string, string>
        {
            { "Page", query.Page.ToString() },
            { "PageSize", query.PageSize.ToString() }
        };
            if (!string.IsNullOrWhiteSpace(query.Name))
                queryParams.Add("Name", query.Name);
            if (query.Tier.HasValue)
                queryParams.Add("Tier", query.Tier.Value.ToString());

            var url = QueryHelpers.AddQueryString("/api/mediators", queryParams);

            return await _httpClient.GetFromJsonAsync<PaginatedResultDto<MediatorDto>>(url);
        }

        public async Task<MediatorDto> CreateMediatorAsync(MediatorCreateDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/mediators", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MediatorDto>();
        }

        public async Task<MediatorDto> UpdateMediatorAsync(MediatorUpdateDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync("/api/mediators", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MediatorDto>();
        }
    }
}
