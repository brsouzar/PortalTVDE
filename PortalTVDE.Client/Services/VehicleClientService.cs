using PortalTVDE.Client.Services.Interfaces;
using PortalTVDE.Shared.ModelsDTOs;
using System.Net.Http.Json;

namespace PortalTVDE.Client.Services
{
       public class VehicleClientService : IVehicleClientService
    {
        private readonly HttpClient _http;

        public VehicleClientService(HttpClient http)
        {
            _http = http;
        }

        public async Task<PaginatedVehiclesResponse> GetVehiclesAsync(VehicleQueryDto query)
        {
            // 1. Inicia a query string com os parâmetros obrigatórios
            var url = $"api/vehicles?page={query.Page}&pageSize={query.PageSize}";

            // 2. Adiciona os filtros de forma segura, apenas se não estiverem nulos/vazios

            if (!string.IsNullOrEmpty(query.LicensePlate))
            {
                url += $"&LicensePlate={Uri.EscapeDataString(query.LicensePlate)}";
            }

            if (!string.IsNullOrEmpty(query.Make))
            {
                url += $"&Make={Uri.EscapeDataString(query.Make)}";
            }

            if (!string.IsNullOrEmpty(query.Model))
            {
                url += $"&Model={Uri.EscapeDataString(query.Model)}";
            }

            // 3. Executa a requisição com a URL construída
            var result = await _http.GetFromJsonAsync<PaginatedVehiclesResponse>(url);

            return result ?? new PaginatedVehiclesResponse();
        }

        public async Task<VehicleDto?> GetVehicleByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<VehicleDto>($"api/vehicles/{id}");
        }

        public async Task<List<VehicleDto>> GetAllSimpleVehiclesAsync()
        {
            var result = await _http.GetFromJsonAsync<List<VehicleDto>>("api/vehicles/all");

            // Retorna a lista, ou uma lista vazia se for nulo
            return result ?? new List<VehicleDto>();
        }

        public async Task CreateVehicleAsync(VehicleCreateDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/vehicles", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateVehicleAsync(int id, VehicleCreateDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/vehicles/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteVehicleAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/vehicles/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}

