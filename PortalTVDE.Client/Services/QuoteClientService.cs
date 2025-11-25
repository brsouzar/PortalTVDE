// PortalTVDE.Client.Services.QuoteClientService.cs
using System.Net.Http.Json;
using PortalTVDE.Client.Services.Interfaces;
using PortalTVDE.Shared.ModelsDTOs;

namespace PortalTVDE.Client.Services
{
    public class QuoteClientService : IQuoteClientService
    {
        private readonly HttpClient _http;

        public QuoteClientService(HttpClient http)
        {
            _http = http;
        }

        public async Task<QuotePricedDto?> PriceQuoteAsync(QuotePriceRequestDto request)
        {
            var response = await _http.PostAsJsonAsync("api/quotes/price", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<QuotePricedDto>();
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                throw new ApplicationException(error?.Message ?? "Erro ao processar cotação");
            }
        }
            public async Task<List<QuoteDto>?> GetQuotesByClientAsync(int clientId)
        {
            var response = await _http.GetAsync($"api/quotes/client/{clientId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<QuoteDto>>();
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                throw new ApplicationException(error?.Message ?? "Erro ao buscar histórico de cotações");
            }
        }

        public async Task<BindResultDto> BindQuoteAsync(QuoteBindRequestDto request)
        {
           
            var response = await _http.PostAsJsonAsync($"api/quotes/{request.QuoteId}/bind", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                throw new ApplicationException(error?.Message ?? "Erro ao emitir apólice (bindar cotação)");
            }

            var result = await response.Content.ReadFromJsonAsync<BindResultDto>();

            if (result == null)
                throw new ApplicationException("Resposta inválida do servidor após a emissão.");

            return result;
        }

    }

    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}
