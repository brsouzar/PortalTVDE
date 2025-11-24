using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalTVDE.Server.Services.Interfaces;
using PortalTVDE.Shared.ModelsDTOs;


[ApiController]
[Route("api/[controller]")]
public class MediatorsController : ControllerBase
{
    private readonly IMediatorService _service;

    public MediatorsController(IMediatorService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] MediatorQueryDto query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(MediatorCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [HttpPut]
    public async Task<IActionResult> Update(MediatorUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _service.UpdateAsync(dto);
        return Ok(updated);
    }

   
}
