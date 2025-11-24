using Microsoft.AspNetCore.Mvc;
using PortalTVDE.Server.Services.Interfaces;
using PortalTVDE.Shared.ModelsDTOs;

namespace PortalTVDE.Server.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _service;

        public VehiclesController(IVehicleService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedVehiclesResponse>> GetAll([FromQuery] VehicleQueryDto query)
        {
            return Ok(await _service.GetVehiclesAsync(query));
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<VehicleDto>>> GetAllSimple()
        {
            var vehicles = await _service.GetAllSimpleVehiclesAsync();
            return Ok(vehicles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleDto>> GetById(int id)
        {
            var vehicle = await _service.GetByIdAsync(id);
            return vehicle is null ? NotFound() : Ok(vehicle);
        }

        [HttpPost]
        public async Task<ActionResult> Create(VehicleCreateDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return Created($"/api/vehicles/{id}", new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, VehicleCreateDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
