// Client/Handlers/AuthorizationHeaderHandler.cs
using System.Net.Http.Headers;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;

public class AuthorizationHeaderHandler : DelegatingHandler
{
    private readonly ISessionStorageService _sessionStorage;
    private const string AuthTokenKey = "authToken";

    // O DelegatingHandler deve receber o serviço de armazenamento
    public AuthorizationHeaderHandler(ISessionStorageService sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // 1. Verifica se a requisição já tem um cabeçalho (evita anexar duas vezes)
        if (request.Headers.Authorization?.Scheme != "Bearer")
        {
            // 2. Obtém o token do armazenamento
            var token = await _sessionStorage.GetItemAsync<string>(AuthTokenKey);

            if (!string.IsNullOrEmpty(token))
            {
                // 3. ANEXA o token Bearer na requisição
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // 4. Continua o processamento da requisição
        return await base.SendAsync(request, cancellationToken);
    }
}