using Xunit;
using Microsoft.EntityFrameworkCore;
using PortalTVDE.Server.Data;
using PortalTVDE.Server.Models;
using PortalTVDE.Server.Services;
using PortalTVDE.Shared.ModelsDTOs;


namespace PortalTVDE.Server.Tests.Services
{
    // Define a classe de testes
    public class MediatorServiceTests
    {
        
        // Método auxiliar para configurar e popular o DbContext em memória
        private ApplicationDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new ApplicationDbContext(options);

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

           
            var mediators = new List<Mediator>
            {
                // Tier Bronze (1)
                new Mediator { Id = 1, Name = "Alpha Mediator", 
                    Email = "alpha@tvde.pt", 
                    Tier = Shared.Enums.MediatorTier.Bronze, 
                    CommissionRate = 0.10m,
                    IsDeleted = false,
                    RowVersion = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 }},
                // Tier Silver (2)
                new Mediator { Id = 2, Name = "Beta Broker",
                    Email = "beta@tvde.pt", 
                    Tier = Shared.Enums.MediatorTier.Silver, 
                    CommissionRate = 0.15m, 
                    IsDeleted = false,
                    RowVersion = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 } },
                // Tier Bronze (1)
                new Mediator { Id = 3, Name = "Gamma Agent", 
                    Email = "gamma@tvde.pt", 
                    Tier = Shared.Enums.MediatorTier.Bronze,
                    IsDeleted = false,
                    CommissionRate = 0.12m,
                    RowVersion = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 }},
                // Tier Gold (3)
                new Mediator { Id = 4, Name = "Delta Broker", 
                    Email = "delta@tvde.pt", 
                    Tier = Shared.Enums.MediatorTier.Gold, 
                    CommissionRate = 0.20m,
                    IsDeleted = false,
                    RowVersion = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 }}
            };

            dbContext.Mediators.AddRange(mediators);
            dbContext.SaveChanges();

            return dbContext;
        }


       
        [Fact]
        public async Task GetAllAsync_ShouldFilterByTier_Bronze()
        {
            // ARRANGE
            var dbName = "MediatorFilterByTierTest_Bronze";
            using var dbContext = GetInMemoryDbContext(dbName);
            var service = new MediatorService(dbContext);

            // Consulta para buscar Tier BRONZE (valor 1)
            var query = new MediatorQueryDto { Tier = Shared.Enums.MediatorTier.Bronze, Page = 1, PageSize = 10 };

            // ACT
            var result = await service.GetAllAsync(query);

            // ASSERT
            // Esperamos 2 resultados (Alpha, Gamma)
            Assert.Equal(2, result.TotalCount);
            Assert.True(result.Items.All(m => m.Tier == Shared.Enums.MediatorTier.Bronze));
        }

        [Fact]
        public async Task GetAllAsync_ShouldFilterByTier_Gold()
        {
            // ARRANGE
            var dbName = "MediatorFilterByTierTest_Gold";
            using var dbContext = GetInMemoryDbContext(dbName);
            var service = new MediatorService(dbContext);

            // Consulta para buscar Tier GOLD (valor 3)
            var query = new MediatorQueryDto { Tier = Shared.Enums.MediatorTier.Gold, Page = 1, PageSize = 10 };

            // ACT
            var result = await service.GetAllAsync(query);

            // ASSERT
            // Esperamos 1 resultado (Delta Broker)
            Assert.Equal(1, result.TotalCount);
            Assert.True(result.Items.All(m => m.Tier == Shared.Enums.MediatorTier.Gold));
        }


        [Fact]
        public async Task GetAllAsync_ShouldHandleCombinedFilters()
        {
            // ARRANGE
            var dbName = "MediatorCombinedFilterTest";
            using var dbContext = GetInMemoryDbContext(dbName);
            var service = new MediatorService(dbContext);

            // Consulta: Nome contém 'Agent' E Tier BRONZE (valor 1)
            var query = new MediatorQueryDto { Name = "Agent", Tier = Shared.Enums.MediatorTier.Bronze, Page = 1, PageSize = 10 };

            // ACT
            var result = await service.GetAllAsync(query);

            // ASSERT
            // Esperamos 1 resultado (Gamma Agent)
            Assert.Equal(1, result.TotalCount);
            Assert.Equal("Gamma Agent", result.Items.First().Name);
        }

        // ====================================================================
        // TESTES PARA CreateAsync
        // ====================================================================

        [Fact]
        public async Task CreateAsync_ShouldAddMediatorAndReturnDto()
        {
            // ARRANGE
            var dbName = "MediatorCreateTest";
            using var dbContext = GetInMemoryDbContext(dbName);
            var service = new MediatorService(dbContext);

            var newMediatorDto = new MediatorCreateDto
            {
                Name = "New Guy",
                Email = "new@test.pt",
                Tier = Shared.Enums.MediatorTier.Gold, 
                CommissionRate = 0.30m,               
            };

            // ACT
            var resultDto = await service.CreateAsync(newMediatorDto);

            // ASSERT
            Assert.NotNull(resultDto);
            // ... (outros asserts) ...

            // 2. Verifica se o item foi realmente salvo no banco de dados com o Tier correto
            var savedMediator = await dbContext.Mediators.FindAsync(resultDto.Id);
            Assert.NotNull(savedMediator);
            Assert.Equal(Shared.Enums.MediatorTier.Gold, savedMediator.Tier); 
        }

        // ====================================================================
        // TESTES PARA UpdateAsync
        // ====================================================================

        [Fact]
        public async Task UpdateAsync_ShouldModifyMediatorAndReturnUpdatedDto()
        {
            // ARRANGE
            var dbName = "MediatorUpdateTest";
            using var dbContext = GetInMemoryDbContext(dbName);
            var service = new MediatorService(dbContext);

            // DTO para atualizar o Mediador ID 1 (Bronze) para Gold
            var updateDto = new MediatorUpdateDto
            {
                Id = 1,
                Tier = Shared.Enums.MediatorTier.Gold,
                CommissionRate = 0.50m
            };

            // ACT
            var resultDto = await service.UpdateAsync(updateDto);

            // ASSERT
            // 1. Verifica se o DTO de retorno reflete a atualização
            Assert.NotNull(resultDto);
            Assert.Equal(1, resultDto.Id);
            Assert.Equal(Shared.Enums.MediatorTier.Gold, resultDto.Tier); // **AJUSTADO**
            Assert.Equal(0.50m, resultDto.CommissionRate);

            // 2. Verifica se a atualização persistiu no banco de dados
            var updatedMediator = await dbContext.Mediators.FindAsync(1);
            Assert.Equal(Shared.Enums.MediatorTier.Gold, updatedMediator!.Tier); // **AJUSTADO**
            Assert.Equal(0.50m, updatedMediator.CommissionRate);
        }

        
    }
}