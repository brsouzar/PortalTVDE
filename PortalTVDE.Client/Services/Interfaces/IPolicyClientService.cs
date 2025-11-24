using PortalTVDE.Shared.ModelsDTOs;

namespace PortalTVDE.Client.Services.Interfaces
{
    public interface IPolicyClientService
    {
        Task<List<PolicyDto>> GetAllPoliciesAsync();

        Task<PolicyDto?> GetPolicyByIdAsync(int id);

       
    }
}
