using Microsoft.EntityFrameworkCore;
using PortalTVDE.Server.Data;
using PortalTVDE.Server.Models;
using PortalTVDE.Server.Services.Interfaces;
using PortalTVDE.Shared.ModelsDTOs;

namespace PortalTVDE.Server.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly ApplicationDbContext _db;

        public PolicyService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<PolicyDto>> GetAllAsync()
        {
            return await _db.Policies
                .Select(p => new PolicyDto
                {
                    Id = p.Id,
                    PolicyNumber = p.PolicyNumber,
                    EffectiveFrom = p.EffectiveFrom,
                    EffectiveTo = p.EffectiveTo,
                    TotalPremium = p.TotalPremium,
                    Commission = p.Commission
                })
                .ToListAsync();
        }

        public async Task<Policy?> GetPolicyAsync(int policyId)
        {
            return await _db.Policies
                .Include(p => p.Quote)
                .FirstOrDefaultAsync(p => p.Id == policyId);
        }
    }
}
