using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using PortalTVDE.Client.Auth;
using PortalTVDE.Client.Services.Interfaces;
using PortalTVDE.Shared.ModelsDTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PortalTVDE.Client.Services
{
    public class ClientAuthService : IClientAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ISessionStorageService _sessionStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        private const string AuthTokenKey = "authToken";

        public ClientAuthService(
            HttpClient httpClient,
            ISessionStorageService sessionStorage,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _sessionStorage = sessionStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<LoginResult> Login(LoginRequestDto loginRequest)
        {
            var loginAsJson = new StringContent(
                JsonSerializer.Serialize(loginRequest),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("api/auth/login", loginAsJson);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonSerializer.Deserialize<ErrorResponseDto>(errorBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return new LoginResult
                {
                    Successful = false,
                    ErrorMessage = errorResponse?.Message ?? "Erro desconhecido ao tentar fazer login."
                };
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
            {
                // Armazena token na sessão
                await _sessionStorage.SetItemAsync(AuthTokenKey, loginResponse.Token);

                // Atualiza AuthenticationStateProvider
                ((CustomAuthStateProvider)_authenticationStateProvider)
                    .MarkUserAsAuthenticated(loginResponse.Token);

                
                return new LoginResult { Successful = true, UserInfo = loginResponse };
            }

            return new LoginResult { Successful = false, ErrorMessage = "Resposta do login inválida." };
        }

        public async Task<LoginResult> Register(RegisterRequestDto registerRequest)
        {
            var registerAsJson = new StringContent(
                JsonSerializer.Serialize(registerRequest),
                Encoding.UTF8,
                "application/json"
            );

            // O endpoint a ser chamado é "api/auth/register"
            var response = await _httpClient.PostAsync("api/auth/register", registerAsJson);

            if (!response.IsSuccessStatusCode)
            {
                // Se a resposta for um erro (400, 500, etc.), tentamos desserializar a mensagem de erro
                var errorBody = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonSerializer.Deserialize<ErrorResponseDto>(errorBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return new LoginResult
                {
                    Successful = false,
                    ErrorMessage = errorResponse?.Message ?? "Erro desconhecido ao tentar registrar o usuário."
                };
            }

            // Se a resposta for sucesso (200 OK), desserializamos a LoginResponseDto
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
            {
                // Armazenamento do Token e atualização do estado de autenticação, assim como no Login
                await _sessionStorage.SetItemAsync(AuthTokenKey, loginResponse.Token);

                ((CustomAuthStateProvider)_authenticationStateProvider)
                    .MarkUserAsAuthenticated(loginResponse.Token);

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", loginResponse.Token);

                // Retorna o resultado de sucesso
                return new LoginResult { Successful = true, UserInfo = loginResponse };
            }

            return new LoginResult { Successful = false, ErrorMessage = "Resposta de registro inválida." };
        }

        public async Task Logout()
        {
            // Remove token da sessão
            await _sessionStorage.RemoveItemAsync(AuthTokenKey);

            // Remove header do HttpClient
            _httpClient.DefaultRequestHeaders.Authorization = null;

            // Notifica AuthenticationStateProvider
            ((CustomAuthStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _sessionStorage.GetItemAsync<string>(AuthTokenKey);
        }
    }

    public class ErrorResponseDto
    {
        public string Message { get; set; } = "";
    }
}
