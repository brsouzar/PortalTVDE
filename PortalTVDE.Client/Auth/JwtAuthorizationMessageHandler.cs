using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace PortalTVDE.Client.Auth
{
    public class JwtAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;
        private const string AuthTokenKey = "authToken";

        public JwtAuthorizationMessageHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
           HttpRequestMessage request,
           CancellationToken cancellationToken)
        {
            // 1. Tenta obter o token JWT do armazenamento local
            // O GetItemAsStringAsync retorna null se a chave não existir
            var token = await _localStorage.GetItemAsStringAsync(AuthTokenKey);

            // 2. Se o token existir, injeta-o no cabeçalho de Autorização
            if (!string.IsNullOrWhiteSpace(token))
            {
                // Verifica se a requisição já tem um cabeçalho de Autorização
                if (request.Headers.Authorization == null)
                {
                    // Formato: Bearer <token>
                    request.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }
            }

            // 3. Permite que a requisição continue para o próximo handler ou para o destino
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
