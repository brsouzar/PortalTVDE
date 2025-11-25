using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using PortalTVDE.Server.Controllers;
using PortalTVDE.Server.Services.Interfaces;
using PortalTVDE.Shared.ModelsDTOs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

// Garanta que este namespace esteja correto para o seu projeto de testes
namespace PortalTVDE.Server.Tests.Controllers
{
    public class MediatorsControllerTests
    {
        // Variável de Mock para o Serviço que será injetado na Controller
        private readonly Mock<IMediatorService> _mockService;
        private readonly MediatorsController _controller;

        public MediatorsControllerTests()
        {
            // Inicializa o Mock Service antes de cada teste
            _mockService = new Mock<IMediatorService>();
            // Cria a Controller injetando o objeto Mock
            _controller = new MediatorsController(_mockService.Object);
        }

        // --------------------------------------------------------------------
        // TESTES PARA GET (GetAll)
        // --------------------------------------------------------------------

        [Fact]
        public async Task GetAll_ShouldReturnOk_WithPaginatedResults()
        {
            // ARRANGE
            var expectedResponse = new PaginatedResultDto<MediatorDto>
            {
                Items = new List<MediatorDto> { new MediatorDto { Id = 1, Name = "Test Mediator" } },
                TotalCount = 1
            };
            var query = new MediatorQueryDto { Page = 1, PageSize = 10 };

            // Configura o Mock: Quando GetAllAsync é chamado com qualquer query, retorna a resposta esperada
            _mockService.Setup(svc => svc.GetAllAsync(It.IsAny<MediatorQueryDto>()))
                        .ReturnsAsync(expectedResponse);

            // ACT
            var result = await _controller.GetAll(query);

            // ASSERT
            // 1. Verifica se o tipo de retorno é StatusCode 200 (OkObjectResult)
            var okResult = Assert.IsType<OkObjectResult>(result);

            // 2. Verifica se o valor retornado é do tipo PaginatedResultDto<MediatorDto>
            var returnedValue = Assert.IsType<PaginatedResultDto<MediatorDto>>(okResult.Value);

            // 3. Verifica se os dados estão corretos
            Assert.Single(returnedValue.Items);
            Assert.Equal(1, returnedValue.TotalCount);

            // 4. Verifica se o método do serviço foi chamado
            _mockService.Verify(svc => svc.GetAllAsync(It.IsAny<MediatorQueryDto>()), Times.Once);
        }

        // --------------------------------------------------------------------
        // TESTES PARA POST (Create)
        // --------------------------------------------------------------------

        [Fact]
        public async Task Create_ShouldReturnCreated_WhenDtoIsValid()
        {
            // ARRANGE
            var createDto = new MediatorCreateDto { Name = "New Mediator", Email = "new@test.pt", Tier = Shared.Enums.MediatorTier.Bronze
                , CommissionRate = 0.10m };
            var createdMediator = new MediatorDto { Id = 5, Name = "New Mediator", Tier = Shared.Enums.MediatorTier.Bronze, CommissionRate = 0.10m };

            // Configura o Mock para retornar o DTO criado com um ID
            _mockService.Setup(svc => svc.CreateAsync(It.IsAny<MediatorCreateDto>()))
                        .ReturnsAsync(createdMediator);

            // ACT
            var result = await _controller.Create(createDto);

            // ASSERT
            // 1. Verifica se o tipo de retorno é StatusCode 201 (CreatedAtActionResult)
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);

            // 2. Verifica se o nome da ação está correto (GetAll)
            Assert.Equal(nameof(MediatorsController.GetAll), createdResult.ActionName);

            // 3. Verifica se o valor retornado contém o DTO criado
            var returnedValue = Assert.IsType<MediatorDto>(createdResult.Value);
            Assert.Equal(5, returnedValue.Id);

            // 4. Verifica se o método de criação do serviço foi chamado
            _mockService.Verify(svc => svc.CreateAsync(createDto), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // ARRANGE
            var createDto = new MediatorCreateDto { Name = "Incomplete", Tier = Shared.Enums.MediatorTier.Silver, Email = "teste@gmail.com" }; // Exemplo de DTO incompleto

            // Adiciona um erro forçado ao ModelState da Controller
            _controller.ModelState.AddModelError("Email", "O campo Email é obrigatório.");

            // ACT
            var result = await _controller.Create(createDto);

            // ASSERT
            // 1. Verifica se o tipo de retorno é StatusCode 400 (BadRequestObjectResult)
            Assert.IsType<BadRequestObjectResult>(result);

            // 2. Verifica se o método do serviço NUNCA foi chamado
            _mockService.Verify(svc => svc.CreateAsync(It.IsAny<MediatorCreateDto>()), Times.Never);
        }

        // --------------------------------------------------------------------
        // TESTES PARA PUT (Update)
        // --------------------------------------------------------------------

        [Fact]
        public async Task Update_ShouldReturnOk_WhenDtoIsValid()
        {
            // ARRANGE
            var updateDto = new MediatorUpdateDto { Id = 1, Tier = Shared.Enums.MediatorTier.Silver, CommissionRate = 0.25m };
            var updatedMediator = new MediatorDto { Id = 1, Name = "Updated Name", Tier = Shared.Enums.MediatorTier.Gold, CommissionRate = 0.25m };

            // Configura o Mock para retornar o DTO atualizado
            _mockService.Setup(svc => svc.UpdateAsync(It.IsAny<MediatorUpdateDto>()))
                        .ReturnsAsync(updatedMediator);

            // ACT
            var result = await _controller.Update(updateDto);

            // ASSERT
            // 1. Verifica se o tipo de retorno é StatusCode 200 (OkObjectResult)
            var okResult = Assert.IsType<OkObjectResult>(result);

            // 2. Verifica se o valor retornado é o DTO atualizado
            var returnedValue = Assert.IsType<MediatorDto>(okResult.Value);
            Assert.Equal(Shared.Enums.MediatorTier.Gold, returnedValue.Tier);

            // 3. Verifica se o método de atualização do serviço foi chamado
            _mockService.Verify(svc => svc.UpdateAsync(updateDto), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // ARRANGE
            var updateDto = new MediatorUpdateDto { Id = 1 };

            // Adiciona um erro forçado ao ModelState da Controller
            _controller.ModelState.AddModelError("CommissionRate", "A Taxa de Comissão é obrigatória.");

            // ACT
            var result = await _controller.Update(updateDto);

            // ASSERT
            // 1. Verifica se o tipo de retorno é StatusCode 400 (BadRequestObjectResult)
            Assert.IsType<BadRequestObjectResult>(result);

            // 2. Verifica se o método do serviço NUNCA foi chamado
            _mockService.Verify(svc => svc.UpdateAsync(It.IsAny<MediatorUpdateDto>()), Times.Never);
        }

        [Fact]
        public async Task Update_ShouldHandleKeyNotFoundException()
        {
            // ARRANGE
            var updateDto = new MediatorUpdateDto { Id = 999, Tier = Shared.Enums.MediatorTier.Bronze, CommissionRate = 0.10m };

            // Configura o Mock para lançar a exceção KeyNotFoundException (simulando que o serviço não encontrou o ID)
            _mockService.Setup(svc => svc.UpdateAsync(It.IsAny<MediatorUpdateDto>()))
                        .ThrowsAsync(new KeyNotFoundException("Mediator não encontrado."));

            // ACT & ASSERT            
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Update(updateDto));

            /*
            try {
                var updated = await _service.UpdateAsync(dto);
                return Ok(updated);
            } catch (KeyNotFoundException) {
                return NotFound();
            }
            */
        }
    }
}