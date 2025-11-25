using Xunit;
using Microsoft.EntityFrameworkCore;
using PortalTVDE.Server.Data;
using PortalTVDE.Server.Models;
using PortalTVDE.Shared.ModelsDTOs;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace PortalTVDE.Server.Tests.Services
{
   public class QuotationServiceTests
    {
      private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new ApplicationDbContext(options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            return dbContext;
        }

        // --- DADOS BASE DE TESTE ---
        private void SeedQuotationData(ApplicationDbContext dbContext)
        {
            // Placeholder para RowVersion (necessário para EF Core In-Memory)
            var placeholderRowVersion = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };

            // 1. Mediator (Necessário para Quote creation)
            var mediator = new Mediator { Id = 1002, Email = "test@mediator.com", Name = "Test Mediator", RowVersion = placeholderRowVersion };
            dbContext.Mediators.Add(mediator);

            // 2. Clients
            // Cliente Válido: Adulto (25 anos)
            var clientAdult = new Clientt { Id = 1, NIF = "111111111", BirthDate = DateTime.Today.AddYears(-25), RowVersion = placeholderRowVersion };
            // Cliente Inválido: Menor (17 anos)
            var clientMinor = new Clientt { Id = 2, NIF = "222222222", BirthDate = DateTime.Today.AddYears(-17), RowVersion = placeholderRowVersion };
            dbContext.Clients.AddRange(clientAdult, clientMinor);

            // 3. Vehicles
            // Veículo Válido (Ano >= 2000)
            var vehicleNew = new Vehicle { Id = 10, LicensePlate = "A1-10-AA", Year = 2010, RowVersion = placeholderRowVersion, IsDeleted = false };
            // Veículo Inválido (Ano < 2000)
            var vehicleOld = new Vehicle { Id = 11, LicensePlate = "B2-11-BB", Year = 1999, RowVersion = placeholderRowVersion, IsDeleted = false };
            dbContext.Vehicles.AddRange(vehicleNew, vehicleOld);

            // 4. Quote para Teste de Bind (Policy)
            var quoteForBind = new Quote
            {
                Id = 200,
                Number = "Q-BIND-TEST",
                ClientId = 1,
                VehicleId = 10,
                MediatorId = 1002,
                TotalPremium = 1000m,
                Status = "Priced",
                RowVersion = placeholderRowVersion,
                IsDeleted = false
                
            };
            dbContext.Quotes.Add(quoteForBind);

            dbContext.SaveChanges();
        }

        // ====================================================================
        // TESTES PARA PRICEQUOTEASYNC (CALCULAR PREÇO E CRIAR COTAÇÃO)
        // ====================================================================

        [Fact]
        public async Task PriceQuoteAsync_ShouldCalculateAndCreateQuote_OnSuccess()
        {
            // ARRANGE
            var dbName = "PriceQuoteSuccessDB";
            using var dbContext = GetDbContext(dbName);
            SeedQuotationData(dbContext);
            var service = new QuotationService(dbContext);

            var request = new QuotePriceRequestDto
            {
                ClientId = 1, // Cliente válido (Adulto)
                VehicleId = 10, // Veículo válido (Ano >= 2000)
                MediatorId = 1002,
            };

            // ACT
            var result = await service.PriceQuoteAsync(request);

            // ASSERT
            // 1. Verifica o retorno do DTO
            Assert.True(result.Id > 0);
            Assert.Equal("Priced", result.Status);
            // 2. Verifica se o cálculo está correto (200 + 50 - 10 + 60 = 300)
            Assert.Equal(300m, result.Breakdown.Total);

            // 3. Verifica a persistência no DB
            var savedQuote = await dbContext.Quotes.FindAsync(result.Id);
            Assert.NotNull(savedQuote);
            Assert.Equal(request.VehicleId, savedQuote.VehicleId);
            Assert.Equal(300m, savedQuote.TotalPremium);
        }

        [Fact]
        public async Task PriceQuoteAsync_ShouldThrowException_WhenClientNotFound()
        {
            // ARRANGE
            var dbName = "PriceQuoteClientNotFoundDB";
            using var dbContext = GetDbContext(dbName);
            SeedQuotationData(dbContext);
            var service = new QuotationService(dbContext);

            var request = new QuotePriceRequestDto { ClientId = 999, VehicleId = 10 };

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<Exception>(() => service.PriceQuoteAsync(request));
            Assert.Contains("Cliente não encontrado", exception.Message);
        }

        [Fact]
        public async Task PriceQuoteAsync_ShouldThrowException_WhenVehicleNotFound()
        {
            // ARRANGE
            var dbName = "PriceQuoteVehicleNotFoundDB";
            using var dbContext = GetDbContext(dbName);
            SeedQuotationData(dbContext);
            var service = new QuotationService(dbContext);

            var request = new QuotePriceRequestDto { ClientId = 1, VehicleId = 10, NcbYears = 2005};

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<Exception>(() => service.PriceQuoteAsync(request));
            Assert.Contains("Veículo não encontrado", exception.Message);
        }

        [Fact]
        public async Task PriceQuoteAsync_ShouldThrowInvalidOperationException_WhenClientIsMinor()
        {
            // ARRANGE
            var dbName = "PriceQuoteMinorClientDB";
            using var dbContext = GetDbContext(dbName);
            SeedQuotationData(dbContext);
            var service = new QuotationService(dbContext);

            var request = new QuotePriceRequestDto { ClientId = 2, VehicleId = 10 }; // ClientId 2 é menor de idade

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.PriceQuoteAsync(request));
            Assert.Contains("Cliente deve ter pelo menos 18 anos.", exception.Message);
        }

        [Fact]
        public async Task PriceQuoteAsync_ShouldThrowInvalidOperationException_WhenVehicleIsTooOld()
        {
            // ARRANGE
            var dbName = "PriceQuoteOldVehicleDB";
            using var dbContext = GetDbContext(dbName);
            SeedQuotationData(dbContext);
            var service = new QuotationService(dbContext);

            var request = new QuotePriceRequestDto { ClientId = 1, VehicleId = 11 }; // VehicleId 11 é 1999

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.PriceQuoteAsync(request));
            Assert.Contains("Veículo deve ser de 2000 ou mais recente.", exception.Message);
        }

        // ====================================================================
        // TESTES PARA BINDQUOTEASYNC (EMITIR APÓLICE)
        // ====================================================================

        [Fact]
        public async Task BindQuoteAsync_ShouldCreatePolicyAndUpdateQuote_OnSuccess()
        {
            // ARRANGE
            var dbName = "BindQuoteSuccessDB";
            using var dbContext = GetDbContext(dbName);
            SeedQuotationData(dbContext); // Inclui QuoteId 200
            var service = new QuotationService(dbContext);
            var originalQuoteId = 200;
            var newMediatorId = 1002;

            var request = new QuoteBindRequestDto
            {
                QuoteId = originalQuoteId,
                MediatorId = newMediatorId
            };

            // ACT
            var newPolicyId = await service.BindQuoteAsync(request);

            // ASSERT
            // 1. Verifica se um ID válido da Policy foi retornado
            Assert.True(newPolicyId > 0);

            // 2. Verifica se a Policy foi salva no DB
            var newPolicy = await dbContext.Policies.FindAsync(newPolicyId);
            Assert.NotNull(newPolicy);
            Assert.Equal(originalQuoteId, newPolicy.QuoteId);
            Assert.StartsWith("POL-", newPolicy.PolicyNumber);

            // 3. Verifica se a Quote original foi atualizada (o campo MediatorId)
            var updatedQuote = await dbContext.Quotes.FindAsync(originalQuoteId);
            Assert.NotNull(updatedQuote);
            Assert.Equal(newMediatorId, updatedQuote.MediatorId);
        }

        [Fact]
        public async Task BindQuoteAsync_ShouldThrowInvalidOperationException_WhenQuoteNotFound()
        {
            // ARRANGE
            var dbName = "BindQuoteNotFoundDB";
            using var dbContext = GetDbContext(dbName);
            SeedQuotationData(dbContext);
            var service = new QuotationService(dbContext);

            var request = new QuoteBindRequestDto { QuoteId = 999, MediatorId = 1002 };

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.BindQuoteAsync(request));
            Assert.Contains("Cotação ID 999 não encontrada.", exception.Message);
        }
    }
}