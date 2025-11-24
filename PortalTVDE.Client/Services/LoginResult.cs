using PortalTVDE.Shared.ModelsDTOs;
namespace PortalTVDE.Client.Services
{
    public class LoginResult
    {
        public bool Successful { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public LoginResponseDto? UserInfo { get; set; }
    }
}
