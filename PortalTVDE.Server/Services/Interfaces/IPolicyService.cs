using PortalTVDE.Server.Models;
using PortalTVDE.Shared.ModelsDTOs;

namespace PortalTVDE.Server.Services.Interfaces
{
    public interface IPolicyService
    {
        Task<List<PolicyDto>> GetAllAsync();
        Task<Policy?> GetPolicyAsync(int policyId);
    }
}
