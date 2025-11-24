using Microsoft.AspNetCore.Mvc;
using PortalTVDE.Server.Models;
using PortalTVDE.Server.Services.Interfaces;
using PortalTVDE.Shared.ModelsDTOs;

[ApiController]
[Route("api/[controller]")]
public class PoliciesController : ControllerBase
{
    private readonly IPolicyService _policyService;

    public PoliciesController(IPolicyService policyService)
    {
        _policyService = policyService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Policy>>> GetAll()
    {
        var policies = await _policyService.GetAllAsync();
        return Ok(policies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PolicyDto>> GetPolicy(int id)
    {
        var policy = await _policyService.GetPolicyAsync(id);

        if (policy == null)
            return NotFound();

        var dto = new PolicyDto
        {
            Id = policy.Id,
            PolicyNumber = policy.PolicyNumber,
            EffectiveFrom = policy.EffectiveFrom,
            EffectiveTo = policy.EffectiveTo,
            TotalPremium = policy.TotalPremium,
            Commission = policy.Commission
        };

        return Ok(dto);
    }
}
