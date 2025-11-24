using PortalTVDE.Shared.ModelsDTOs;

namespace PortalTVDE.Client.Services.Interfaces
{
    public interface IQuoteClientService
    {
        Task<QuotePricedDto?> PriceQuoteAsync(QuotePriceRequestDto request);
        Task<List<QuoteDto>?> GetQuotesByClientAsync(int clientId);
        Task<BindResultDto> BindQuoteAsync(QuoteBindRequestDto request);


    }
}
