using Microsoft.AspNetCore.Mvc;
using PortalTVDE.Shared.ModelsDTOs;
using PortalTVDE.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace PortalTVDE.Server.Controllers
{
  

    [ApiController]
    [Route("api/clients")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _service;

        public ClientsController(IClientService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedClientsResponse>> GetAll([FromQuery] ClientQueryDto query)
        {
            var result = await _service.GetClientsAsync(query);
            return Ok(result);
        }

        // GET /api/clients/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDtoWithId>> GetById(int id)
        {
            var client = await _service.GetByIdAsync(id);

            if (client == null)
                return NotFound();

            return Ok(client);
        }

        // POST /api/clients
        [HttpPost]
        public async Task<ActionResult> Create(ClientDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return Created($"/api/clients/{id}", new { id });
        }

        // PUT /api/clients/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ClientDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        // DELETE /api/clients/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
