using Xunit;
using Microsoft.EntityFrameworkCore;
using PortalTVDE.Server.Data;
using PortalTVDE.Server.Models;
using PortalTVDE.Server.Services;
using PortalTVDE.Shared.ModelsDTOs;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace PortalTVDE.Server.Tests.Services
{
    public class VehicleServiceTests
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
        private void SeedVehicles(ApplicationDbContext dbContext)
        {
            // O RowVersion é um placeholder para satisfazer a restrição NOT NULL do In-Memory
            var placeholderRowVersion = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };

            dbContext.Vehicles.AddRange(new List<Vehicle>
            {
                new Vehicle
                {
                    Id = 1,
                    LicensePlate = "AA-01-BB",
                    Make = "Toyota",
                    Model = "Prius",
                    Year = 2018,
                    PowerKW = 90,
                    Usage = "TVDE",
                    RowVersion = placeholderRowVersion
                },
                new Vehicle
                {
                    Id = 2,
                    LicensePlate = "BC-02-DE",
                    Make = "Tesla",
                    Model = "Model 3",
                    Year = 2022,
                    PowerKW = 150,
                    Usage = "TVDE",
                    RowVersion = placeholderRowVersion
                },
                new Vehicle
                {
                    Id = 3,
                    LicensePlate = "FG-03-HI",
                    Make = "Toyota",
                    Model = "Yaris",
                    Year = 2015,
                    PowerKW = 70,
                    Usage = "TVDE",
                    RowVersion = placeholderRowVersion
                }
            });
            dbContext.SaveChanges();
        }

        // ====================================================================
        // TESTES PARA GETVEHICLESASYNC (FILTROS E PAGINAÇÃO)
        // ====================================================================

        [Fact]
        public async Task GetVehiclesAsync_ShouldFilterByLicensePlate()
        {
            // ARRANGE
            var dbName = "VehicleFilterByPlateDB";
            using var dbContext = GetDbContext(dbName);
            SeedVehicles(dbContext);
            var service = new VehicleService(dbContext);

            var query = new VehicleQueryDto { LicensePlate = "BC-02", Page = 1, PageSize = 10 };

            // ACT
            var result = await service.GetVehiclesAsync(query);

            // ASSERT
            Assert.Equal(1, result.TotalCount);
            Assert.Equal("BC-02-DE", result.Items.First().LicensePlate);
        }

        [Fact]
        public async Task GetVehiclesAsync_ShouldFilterByMake()
        {
            // ARRANGE
            var dbName = "VehicleFilterByMakeDB";
            using var dbContext = GetDbContext(dbName);
            SeedVehicles(dbContext);
            var service = new VehicleService(dbContext);

            var query = new VehicleQueryDto { Make = "Toyota", Page = 1, PageSize = 10 };

            // ACT
            var result = await service.GetVehiclesAsync(query);

            // ASSERT
            Assert.Equal(2, result.TotalCount);
            Assert.True(result.Items.All(v => v.Make == "Toyota"));
        }

        // ====================================================================
        // TESTES PARA GETBYIDASYNC
        // ====================================================================

        [Fact]
        public async Task GetByIdAsync_ShouldReturnVehicle_WhenIdExists()
        {
            // ARRANGE
            var dbName = "VehicleGetByIdDB";
            using var dbContext = GetDbContext(dbName);
            SeedVehicles(dbContext);
            var service = new VehicleService(dbContext);

            // ACT
            var result = await service.GetByIdAsync(2);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(2, result!.Id);
            Assert.Equal("Tesla", result.Make);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenIdDoesNotExist()
        {
            // ARRANGE
            var dbName = "VehicleGetByIdNotFoundDB";
            using var dbContext = GetDbContext(dbName);
            var service = new VehicleService(dbContext);
            // Nenhum dado adicionado

            // ACT
            var result = await service.GetByIdAsync(999);

            // ASSERT
            Assert.Null(result);
        }

        // ====================================================================
        // TESTES PARA CREATEASYNC
        // ====================================================================

        [Fact]
        public async Task CreateAsync_ShouldAddVehicleAndReturnId()
        {
            // ARRANGE
            var dbName = "VehicleCreateTest";
            using var dbContext = GetDbContext(dbName);
            var service = new VehicleService(dbContext);

            var newVehicleDto = new VehicleCreateDto
            {
                LicensePlate = "ZZ-99-ZZ",
                Make = "Honda",
                Model = "Civic",
                Year = 2024,
                PowerKW = 100,
                Usage = "TVDE"
            };

            // ACT
            var newId = await service.CreateAsync(newVehicleDto);

            // ASSERT
            // 1. Verifica se um ID válido foi retornado (gerado pelo DB)
            Assert.True(newId > 0);

            // 2. Verifica se o item foi realmente salvo no banco de dados
            var savedVehicle = await dbContext.Vehicles.FindAsync(newId);
            Assert.NotNull(savedVehicle);
            Assert.Equal("ZZ-99-ZZ", savedVehicle.LicensePlate);
            Assert.Equal("TVDE", savedVehicle.Usage);

            // 3. Verifica se os campos da BaseEntity foram preenchidos
            Assert.NotEqual(default(System.DateTime), savedVehicle.CreatedAt);
        }

        // ====================================================================
        // TESTES PARA UPDATEASYNC 🚀
        // ====================================================================

        [Fact]
        public async Task UpdateAsync_ShouldUpdateVehicleDetails_WhenIdExists()
        {
            // ARRANGE
            var dbName = "VehicleUpdateTest";
            using var dbContext = GetDbContext(dbName);
            SeedVehicles(dbContext); // Popula com dados (Id=1, 2, 3)
            var service = new VehicleService(dbContext);
            var idToUpdate = 1;

            var updatedDto = new VehicleCreateDto
            {
                LicensePlate = "ZZ-88-XX", // Novo valor
                Make = "Toyota",
                Model = "Corolla",         // Novo valor
                Year = 2020,               // Novo valor
                PowerKW = 110,             // Novo valor
                Usage = "Own"              // Novo valor
            };

            // ACT
            await service.UpdateAsync(idToUpdate, updatedDto);

            // ASSERT
            // 1. Recupera o veículo atualizado diretamente do DB
            var updatedVehicle = await dbContext.Vehicles.FindAsync(idToUpdate);

            // 2. Verifica se os campos foram atualizados
            Assert.NotNull(updatedVehicle);
            Assert.Equal("ZZ-88-XX", updatedVehicle!.LicensePlate);
            Assert.Equal("Corolla", updatedVehicle.Model);
            Assert.Equal(2020, updatedVehicle.Year);
            Assert.Equal(110, updatedVehicle.PowerKW);
            Assert.Equal("Own", updatedVehicle.Usage);

            // 3. Verifica se o RowVersion foi atualizado (no DB real, seria diferente)
            // No In-Memory, basta verificar que não é nulo.
            Assert.NotNull(updatedVehicle.RowVersion);
        }

        [Fact]
        public async Task UpdateAsync_ShouldDoNothing_WhenIdDoesNotExist()
        {
            // ARRANGE
            var dbName = "VehicleUpdateNotFoundTest";
            using var dbContext = GetDbContext(dbName);
            SeedVehicles(dbContext);
            var service = new VehicleService(dbContext);
            var idToUpdate = 999;
            var initialCount = await dbContext.Vehicles.CountAsync();

            var updatedDto = new VehicleCreateDto
            {
                LicensePlate = "XX-99-XX",
                Make = "Dummy",
                Model = "Dummy",
                Year = 2000,
                PowerKW = 10,
                Usage = "TVDE"
            };

            // ACT
            await service.UpdateAsync(idToUpdate, updatedDto);

            // ASSERT
            // 1. Verifica se a contagem de veículos permaneceu a mesma
            var finalCount = await dbContext.Vehicles.CountAsync();
            Assert.Equal(initialCount, finalCount);

            // 2. Tenta encontrar o veículo (deve ser null)
            var vehicle = await dbContext.Vehicles.FindAsync(idToUpdate);
            Assert.Null(vehicle);
        }


        // ====================================================================
        // TESTES PARA DELETEASYNC 
        // ====================================================================

        [Fact]
        public async Task DeleteAsync_ShouldRemoveVehicle_WhenIdExists()
        {
            // ARRANGE
            var dbName = "VehicleDeleteTest";
            using var dbContext = GetDbContext(dbName);
            SeedVehicles(dbContext); // Popula com 3 veículos
            var service = new VehicleService(dbContext);
            var idToDelete = 1;
            var initialCount = await dbContext.Vehicles.CountAsync();

            // ACT
            await service.DeleteAsync(idToDelete);

            // ASSERT
            // 1. Verifica se a contagem diminuiu
            var finalCount = await dbContext.Vehicles.CountAsync();
            Assert.Equal(initialCount - 1, finalCount);

            // 2. Tenta encontrar o veículo deletado
            var deletedVehicle = await dbContext.Vehicles.FindAsync(idToDelete);
            Assert.Null(deletedVehicle);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDoNothing_WhenIdDoesNotExist()
        {
            // ARRANGE
            var dbName = "VehicleDeleteNotFoundTest";
            using var dbContext = GetDbContext(dbName);
            SeedVehicles(dbContext); // Popula com 3 veículos
            var service = new VehicleService(dbContext);
            var idToDelete = 999;
            var initialCount = await dbContext.Vehicles.CountAsync();

            // ACT
            await service.DeleteAsync(idToDelete);

            // ASSERT
            // A contagem deve permanecer a mesma
            var finalCount = await dbContext.Vehicles.CountAsync();
            Assert.Equal(initialCount, finalCount);
        }
    }
}