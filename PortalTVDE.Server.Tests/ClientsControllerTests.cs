using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using PortalTVDE.Server.Controllers;
using PortalTVDE.Server.Services.Interfaces;
using PortalTVDE.Shared.ModelsDTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ClientsControllerTests
{
    [Fact]
    public async Task GetById_ShouldReturnOk_WhenClientExists()
    {
        // ARRANGE
        var mockService = new Mock<IClientService>();
        var expectedClient = new ClientDtoWithId { Id = 1, Name = "Alice" };

        // 1. Configura o Mock: Quando GetByIdAsync(1) é chamado, retorne 'expectedClient'
        mockService.Setup(svc => svc.GetByIdAsync(1))
                   .ReturnsAsync(expectedClient);

        var controller = new ClientsController(mockService.Object);

        // ACT
        var result = await controller.GetById(1);

        // ASSERT
        // 1. Verifica se o tipo de retorno é StatusCode 200 (OkObjectResult)
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        // 2. Verifica se o valor retornado é o DTO esperado
        var returnedClient = Assert.IsType<ClientDtoWithId>(okResult.Value);
        Assert.Equal(1, returnedClient.Id);

        // 3. Opcional: Verifica se o método do serviço foi chamado exatamente uma vez
        mockService.Verify(svc => svc.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenClientDoesNotExist()
    {
        // ARRANGE
        var mockService = new Mock<IClientService>();

        // 1. Configura o Mock: Quando GetByIdAsync(999) é chamado, retorne NULL
        mockService.Setup(svc => svc.GetByIdAsync(999))
                   .ReturnsAsync((ClientDtoWithId?)null); // Retorna null para simular não encontrado

        var controller = new ClientsController(mockService.Object);

        // ACT
        var result = await controller.GetById(999);

        // ASSERT
        // 1. Verifica se o tipo de retorno é StatusCode 404 (NotFoundResult)
        Assert.IsType<NotFoundResult>(result.Result);

        // 2. Verifica se o método do serviço foi chamado
        mockService.Verify(svc => svc.GetByIdAsync(999), Times.Once);
    }

    // --- Exemplo de teste para POST/Create ---

    [Fact]
    public async Task Create_ShouldReturnCreated_WithNewId()
    {
        // ARRANGE
        var mockService = new Mock<IClientService>();
        var newClientDto = new ClientDto { Name = "New Client", Email = "new@client.com" };
        const int expectedId = 5; // Simula que o serviço retornará o ID 5

        // Configura o Mock para retornar um ID após a criação
        mockService.Setup(svc => svc.CreateAsync(It.IsAny<ClientDto>()))
                   .ReturnsAsync(expectedId);

        var controller = new ClientsController(mockService.Object);

        // ACT
        var result = await controller.Create(newClientDto);

        // ASSERT
        // 1. Verifica se o tipo de retorno é StatusCode 201 (CreatedAtActionResult)
        var createdResult = Assert.IsType<CreatedResult>(result);
        Assert.Equal(201, createdResult.StatusCode);

        // 2. Verifica se o local do recurso criado está correto
        Assert.Equal($"/api/clients/{expectedId}", createdResult.Location);

        // 3. Verifica se o método de criação no serviço foi chamado com qualquer DTO
        mockService.Verify(svc => svc.CreateAsync(It.IsAny<ClientDto>()), Times.Once);
    }
}