using Xunit;
using Microsoft.EntityFrameworkCore;
using PortalTVDE.Server.Data;
using PortalTVDE.Server.Models;
using PortalTVDE.Server.Services;
using PortalTVDE.Shared.ModelsDTOs;

public class ClientServiceTests
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

    // --- TESTES PARA GETCLIENTSASYNC (FILTROS) ---

    [Fact]
    public async Task GetClientsAsync_ShouldFilterByName()
    {
        // ARRANGE
        var dbName = "FilterByNameDB";
        using var dbContext = GetDbContext(dbName);

        // Popula o banco de dados em memória
        dbContext.Clients.AddRange(new List<Clientt> // Assumindo que seu modelo é Clientt
        {
            new Clientt { Id = 1, Name = "Alice Smith", Email = "alice@test.com", 
                NIF = "123456789", BirthDate = new DateTime(1995, 7, 15),
                RowVersion = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 }
         },
            new Clientt { Id = 2, Name = "Bob Johnson", Email = "bob@test.com",
                NIF = "987654321", BirthDate = new DateTime(1985, 5, 15),
                RowVersion = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 }
       },
            new Clientt { Id = 3, Name = "Carl Smith", Email = "carl@test.com", 
                NIF = "111222333" , BirthDate = new DateTime(1990, 1, 1),
                RowVersion = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 }
        },
        });
        await dbContext.SaveChangesAsync();

        var service = new ClientService(dbContext);
        var query = new ClientQueryDto { Name = "Smith", Email = "", Page = 1, PageSize = 10 };

        // ACT
        var result = await service.GetClientsAsync(query);

        // ASSERT
        // Deve encontrar 2 clientes com 'Smith' no nome
        Assert.Equal(2, result.TotalCount);
        Assert.True(result.Items.All(c => c.Name.Contains("Smith")));
    }

    // --- TESTES PARA GETBYIDASYNC ---

    [Fact]
    public async Task GetByIdAsync_ShouldReturnClient_WhenIdExists()
    {
        // ARRANGE
        var dbName = "GetByIdDB";
        using var dbContext = GetDbContext(dbName);

        // Adiciona um cliente que esperamos encontrar
        var expectedClient = new Clientt { Id = 10, Name = "Test User", Email = "test@user.com", NIF = "100000000", BirthDate = new DateTime(2000, 10, 20), RowVersion = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 } };
        dbContext.Clients.Add(expectedClient);
        await dbContext.SaveChangesAsync();

        var service = new ClientService(dbContext);

        // ACT
        var result = await service.GetByIdAsync(10);

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("Test User", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenIdDoesNotExist()
    {
        // ARRANGE
        var dbName = "GetByIdNotFoundDB";
        using var dbContext = GetDbContext(dbName);
        // Nenhum dado adicionado

        var service = new ClientService(dbContext);

        // ACT
        var result = await service.GetByIdAsync(999);

        // ASSERT
        Assert.Null(result); // Esperamos null
    }

    // Você deve criar testes para CreateAsync, UpdateAsync, DeleteAsync, e os outros filtros (Email, NIF, Paginação)
}