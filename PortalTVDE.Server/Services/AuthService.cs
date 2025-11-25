using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PortalTVDE.Server.Models;
using PortalTVDE.Server.Services.Interfaces;
using PortalTVDE.Shared.ModelsDTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PortalTVDE.Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, 
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                // Não revela se o usuário existe ou se a senha está errada
                return null;
            }

            // Verifica a senha do usuário
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isPasswordValid)
            {
                return null;
            }

            // Se o login for bem-sucedido, gera o token JWT
            var token = await GenerateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            return new LoginResponseDto
            {
                UserId = user.Id,
                Username = user.UserName ?? user.Email!,
                Email = user.Email!,
                Token = token,
                Roles = roles.ToList(),
                MediatorId = user.MediatorId
            };
        }

        public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            // 1. Cria o objeto ApplicationUser
            var user = new ApplicationUser
            {
                UserName = request.UserName ?? request.Email, // Usa UserName se fornecido, senão usa Email
                Email = request.Email,
              
                MediatorId = request.MediatorId
            };

            // 2. Cria o utilizador com a password
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Erro ao criar utilizador: {errors}");
            }

            // 3. Adiciona a role padrão, se necessário (ex: "Partner" ou "Driver")
             await _userManager.AddToRoleAsync(user, "Partner");

            // 4. Se o registro for bem-sucedido, gera o token de login
            var token = await GenerateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            // 5. Retorna o DTO de Resposta de Login
            return new LoginResponseDto
            {
                UserId = user.Id,
                Username = user.UserName!,
                Email = user.Email!,
                Token = token,
                Roles = roles.ToList(),
                MediatorId = user.MediatorId
            };
        }
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                // Adiciona o ID do Mediador como um Claim customizado, se existir
                new Claim("MediatorId", user.MediatorId.HasValue ? user.MediatorId.Value.ToString() : string.Empty)
            };



            // Adiciona as roles do usuário aos claims
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expiry = DateTime.Now.AddHours(int.Parse(_configuration["Jwt:ExpireHours"]!));

          
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiry,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
