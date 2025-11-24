using System.Net.Http.Json;
using PortalTVDE.Shared.ModelsDTOs;
using PortalTVDE.Client.Services.Interfaces;
using System.Net; // Necessário para HttpStatusCode
using System.Net.Http; // Necessário para GetAsync e HttpRequestException

namespace PortalTVDE.Client.Services
{
    public class ClientClientService : IClientClientService
    {
        private readonly HttpClient _http;

        public ClientClientService(HttpClient http)
        {
            _http = http;
        }

        // =======================================================================
        // MÉTODO AJUSTADO PARA TRATAMENTO DE 401/403
        // =======================================================================
        public async Task<PaginatedClientsResponse> GetClientsAsync(ClientQueryDto query)
        {
            var url = $"api/clients?name={query.Name}&email={query.Email}&nif={query.NIF}&page={query.Page}&pageSize={query.PageSize}";

            // 1. Usa GetAsync em vez de GetFromJsonAsync para verificar o status code
            var response = await _http.GetAsync(url);

            // 2. Verifica explicitamente os status de falha de segurança
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                // Lança uma exceção que o componente .razor deve capturar para redirecionar.
                // Isso interrompe o processamento de JSON inválido (o HTML).
                throw new HttpRequestException(
                    "Token de autenticação inválido ou permissão negada.",
                    null,
                    response.StatusCode);
            }

            // 3. Verifica se a requisição foi bem-sucedida (Status 200)
            if (response.IsSuccessStatusCode)
            {
                // Tenta ler o conteúdo JSON. O ?? garante um valor padrão em caso de null.
                return await response.Content.ReadFromJsonAsync<PaginatedClientsResponse>()
                       ?? new PaginatedClientsResponse();
            }

            // 4. Trata outros erros HTTP (404, 500, 400, etc.)
            throw new HttpRequestException($"Erro na API: {response.ReasonPhrase}", null, response.StatusCode);
        }

     
        public async Task<ClientDtoWithId?> GetClientByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/clients/{id}");

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                throw new HttpRequestException("Token inválido ou permissão negada.", null, response.StatusCode);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null; // Trata o 404 de forma específica para o GetById

            response.EnsureSuccessStatusCode(); // Lança exceção para outros códigos de erro
            return await response.Content.ReadFromJsonAsync<ClientDtoWithId>();
        }

        public async Task CreateClientAsync(ClientDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/clients", dto);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                throw new HttpRequestException("Token inválido ou permissão negada.", null, response.StatusCode);

            response.EnsureSuccessStatusCode();
        }

        // Opcional: Aplicar a mesma lógica de tratamento de 401/403 em Update e Delete para ser robusto.
        public async Task UpdateClientAsync(int id, ClientDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/clients/{id}", dto);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                throw new HttpRequestException("Token inválido ou permissão negada.", null, response.StatusCode);

            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteClientAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/clients/{id}");

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                throw new HttpRequestException("Token inválido ou permissão negada.", null, response.StatusCode);

            response.EnsureSuccessStatusCode();
        }
    }
}