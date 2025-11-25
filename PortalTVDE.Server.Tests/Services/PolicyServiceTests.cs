using Xunit;
using Microsoft.EntityFrameworkCore;
using PortalTVDE.Server.Data;
using PortalTVDE.Server.Models;
using PortalTVDE.Server.Services;
using PortalTVDE.Shared.ModelsDTOs;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;


namespace PortalTVDE.Server.Tests.Services
{
    public class PolicyServiceTests
    {
        // Método auxiliar para criar um DbContext em memória
        private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new ApplicationDbContext(options);
            dbContext.Database.EnsureDeleted(); // Limpa a DB antes de cada teste
            dbContext.Database.EnsureCreated(); // Cria o esquema
            return dbContext;
        }

        // --- DADOS BASE DE TESTE ---
        private void SeedPolicies(ApplicationDbContext dbContext)
        {
            var placeholderRowVersion = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };

            // Adiciona entidades Quote para suportar as políticas (se houver FK)
            var quote1 = new Quote { Id = 1, Number = "QUO-001", RowVersion = placeholderRowVersion };
            var quote2 = new Quote { Id = 2, Number = "QUO-002", RowVersion = placeholderRowVersion };
            dbContext.Quotes.AddRange(quote1, quote2);
            dbContext.SaveChanges();


            dbContext.Policies.AddRange(new List<Policy>
            {
                new Policy
                {
                    Id = 101,
                    PolicyNumber = "POL-2025-001",
                    EffectiveFrom = new DateTime(2025, 1, 1),
                    EffectiveTo = new DateTime(2025, 12, 31),
                    TotalPremium = 1200.50m,
                    Commission = 120.00m,
                    QuoteId = 1,
                    Quote = quote1, // Associa a entidade Quote
                    RowVersion = placeholderRowVersion
                },
                new Policy
                {
                    Id = 102,
                    PolicyNumber = "POL-2025-002",
                    EffectiveFrom = new DateTime(2025, 3, 1),
                    EffectiveTo = new DateTime(2026, 2, 28),
                    TotalPremium = 800.00m,
                    Commission = 80.00m,
                    QuoteId = 2,
                    Quote = quote2,
                    RowVersion = placeholderRowVersion
                },
                new Policy
                {
                    Id = 103,
                    PolicyNumber = "POL-2025-003",
                    EffectiveFrom = new DateTime(2025, 6, 15),
                    EffectiveTo = new DateTime(2026, 6, 14),
                    TotalPremium = 2500.75m,
                    Commission = 250.00m,
                    QuoteId = 1,
                    RowVersion = placeholderRowVersion
                }
            });
            dbContext.SaveChanges();
        }

        // ====================================================================
        // TESTES PARA GETALLASYNC
        // ====================================================================

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllPoliciesAsDto()
        {
            // ARRANGE
            var dbName = "PolicyGetAllDB";
            using var dbContext = GetDbContext(dbName);
            SeedPolicies(dbContext);
            var service = new PolicyService(dbContext);
            var expectedCount = 3;

            // ACT
            var result = await service.GetAllAsync();

            // ASSERT
            // 1. Verifica se o número total de políticas é o esperado
            Assert.Equal(expectedCount, result.Count);

            // 2. Verifica se a conversão para DTO foi correta (testando um campo)
            var policy102 = result.First(p => p.Id == 102);
            Assert.Equal("POL-2025-002", policy102.PolicyNumber);
            Assert.Equal(800.00m, policy102.TotalPremium);
            Assert.Equal(new DateTime(2025, 3, 1), policy102.EffectiveFrom);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoPoliciesExist()
        {
            // ARRANGE
            var dbName = "PolicyGetAllEmptyDB";
            using var dbContext = GetDbContext(dbName);
            // Nenhum dado adicionado
            var service = new PolicyService(dbContext);

            // ACT
            var result = await service.GetAllAsync();

            // ASSERT
            Assert.Empty(result);
        }

        // ====================================================================
        // TESTES PARA GETPOLICYASYNC (Com Include)
        // ====================================================================

        [Fact]
        public async Task GetPolicyAsync_ShouldReturnPolicyWithQuoteIncluded_WhenIdExists()
        {
            // ARRANGE
            var dbName = "PolicyGetWithIncludeDB";
            using var dbContext = GetDbContext(dbName);
            SeedPolicies(dbContext);
            var service = new PolicyService(dbContext);
            var policyId = 101;

            // ACT
            var result = await service.GetPolicyAsync(policyId);

            // ASSERT
            // 1. Verifica se a política foi encontrada
            Assert.NotNull(result);
            Assert.Equal(policyId, result!.Id);

            // 2. Verifica se a entidade aninhada (Quote) foi incluída (o ponto principal do teste)
            Assert.NotNull(result.Quote);
            Assert.Equal(1, result.Quote.Id);
            Assert.Equal("QUO-001", result.Quote.Number);
        }

        [Fact]
        public async Task GetPolicyAsync_ShouldReturnNull_WhenIdDoesNotExist()
        {
            // ARRANGE
            var dbName = "PolicyGetNotFoundDB";
            using var dbContext = GetDbContext(dbName);
            SeedPolicies(dbContext);
            var service = new PolicyService(dbContext);

            // ACT
            var result = await service.GetPolicyAsync(999);

            // ASSERT
            Assert.Null(result);
        }
    }
}