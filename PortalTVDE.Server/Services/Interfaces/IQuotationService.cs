using PortalTVDE.Shared.ModelsDTOs;

namespace PortalTVDE.Server.Services.Interfaces
{
    public interface IQuotationService
    {
        Task<QuotePricedDto> PriceQuoteAsync(QuotePriceRequestDto request);
        Task<int> BindQuoteAsync(QuoteBindRequestDto request);
    }
}
