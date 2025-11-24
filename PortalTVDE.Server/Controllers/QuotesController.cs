// PortalTVDE.Server.Controllers.QuotesController.cs
using Microsoft.AspNetCore.Mvc;
using PortalTVDE.Server.Services.Interfaces;
using PortalTVDE.Shared.ModelsDTOs;

[ApiController]
[Route("api/quotes")]
public class QuotesController : ControllerBase
{
    private readonly IQuotationService _service;

    public QuotesController(IQuotationService service)
    {
        _service = service;
    }

    [HttpPost("price")]
    public async Task<ActionResult<QuotePricedDto>> PriceQuote([FromBody] QuotePriceRequestDto request)
    {
        try
        {
            var result = await _service.PriceQuoteAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao processar cotação.", detail = ex.Message });
        }
    }

    [HttpPost("{id}/bind")]
    public async Task<IActionResult> BindQuote(int id, [FromBody] QuoteBindRequestDto request)
    {
        try
        {
            int policyId = await _service.BindQuoteAsync(request);

            return Ok(new BindResultDto
            {
                PolicyId = policyId
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

}
