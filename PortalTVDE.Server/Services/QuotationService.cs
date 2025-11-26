// PortalTVDE.Server.Services.QuotationService.cs
using Microsoft.EntityFrameworkCore;
using PortalTVDE.Server.Data;
using PortalTVDE.Server.Models;
using PortalTVDE.Server.Services.Interfaces;
using PortalTVDE.Shared.ModelsDTOs;

public class QuotationService : IQuotationService
{
    private readonly ApplicationDbContext _db;

    public QuotationService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<QuotePricedDto> PriceQuoteAsync(QuotePriceRequestDto request)
    {
        var client = await _db.Clients.FindAsync(request.ClientId)
                     ?? throw new Exception("Cliente não encontrado");

       var vehicle = await _db.Vehicles.FindAsync(request.VehicleId)
                      ?? throw new Exception("Veículo não encontrado");

        int age = DateTime.Today.Year - client.BirthDate.Year;
        if (client.BirthDate.Date > DateTime.Today.AddYears(-age)) age--;
        if (age < 18)
            throw new InvalidOperationException("Cliente deve ter pelo menos 18 anos.");

        if (vehicle.Year < 2000)
            throw new InvalidOperationException("Veículo deve ser de 2000 ou mais recente.");

        decimal basePremium = 200m; // Exemplo
        decimal surcharges = 50m;   // Exemplo (Ajuste por idade, uso, cidade)
        decimal discounts = 10m;    // Exemplo (Bônus sem sinistro)
        decimal optionalCovers = 60m; // Exemplo (Vidros/Assistência)
        decimal totalPremium = basePremium + surcharges - discounts + optionalCovers;

        var breakdownResult = new QuoteBreakdownDto
        {
            BasePremium = basePremium,
            Total = totalPremium,
            CitySurcharge = surcharges,
            NcbDiscount = discounts,
            OptionalCoverages = optionalCovers,
            CoverageItems = new List<string>() // Ex: "GLASS", "ROADSIDE"
        };

        string quoteNumber = $"Q-{DateTime.UtcNow.Ticks}";
        // cálculo simplificado

        var newQuote = new Quote
        {
            Number = quoteNumber,
            ClientId = request.ClientId,
            VehicleId = request.VehicleId,
            MediatorId = request.MediatorId,
            BasePremium = basePremium,
            Surcharges = surcharges + optionalCovers,
            Discounts = discounts,
            TotalPremium = totalPremium,
            Status = "Priced", // O status é "Priced" após o cálculo do preço
               //newQuote.CoverageItems = MapCoverageItems(request);
            IsDeleted = false,
            RowVersion = new byte[0]
        };

        _db.Quotes.Add(newQuote);
        await _db.SaveChangesAsync();

        return new QuotePricedDto
        {
            Id = newQuote.Id, // ID gerado após o SaveChangesAsync
            Number = newQuote.Number,
            Status = newQuote.Status,
            Breakdown = breakdownResult
        };

    }

   

    public async Task<int> BindQuoteAsync(QuoteBindRequestDto request)
    {
        // 1. Busque a cotação (Quote)
        var qoutes = await _db.Quotes.ToListAsync();
        var quote = await _db.Quotes
            .FirstOrDefaultAsync(q => q.Id == request.QuoteId);

        if (quote == null)
        {
            throw new InvalidOperationException($"Cotação ID {request.QuoteId} não encontrada.");
        }

        // 2. ATUALIZE O MEDIATOR ID NA COTAÇÃO
        // Salva a informação de quem emitiu a apólice na cotação.
        quote.MediatorId = request.MediatorId;

        // 3. Lógica de Criação da Apólice (Policy)
        var policy = new Policy
        {
            // Campos existentes na sua Policy
            QuoteId = quote.Id,
            PolicyNumber = GerarNumeroUnicoDePolicy(), // Assumindo método para gerar número
            EffectiveFrom = DateTime.Today,
            EffectiveTo = DateTime.Today.AddYears(1),
            TotalPremium = 1200.00m, // Exemplo
            Commission = 120.00m,   // Exemplo
            IssuedAt = DateTime.UtcNow,
            IsDeleted = false,
            RowVersion = new byte[0]
        };

        // 4. Salvar as alterações
        _db.Policies.Add(policy);
        await _db.SaveChangesAsync();

        return policy.Id;
    }

    
    private string GerarNumeroUnicoDePolicy()
    {
        // Lógica para gerar um número único, por exemplo, "POL-2025-0001"
        return $"POL-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}";
    }

}
