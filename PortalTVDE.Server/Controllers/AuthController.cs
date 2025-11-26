
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalTVDE.Server.Services.Interfaces;
using PortalTVDE.Shared.ModelsDTOs;

namespace PortalTVDE.Server.Controllers
{
    //[Authorize(Roles ="admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint para autenticar um usuário e retornar um token JWT.
        /// Suporta usuários com as roles Admin ou Mediator.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.LoginAsync(request);

            if (response == null)
            {
                _logger.LogWarning("Tentativa de login falhou para o email: {Email}", request.Email);
                return Unauthorized(new { Message = "Credenciais inválidas. Verifique o email e a senha." });
            }

            // Verifica se o usuário tem a role de Admin ou Mediator (os únicos autorizados a usar o Portal)
            if (!response.Roles.Any(r => r.Equals("admin", StringComparison.OrdinalIgnoreCase) ||
                                         r.Equals("Mediator", StringComparison.OrdinalIgnoreCase) ||
                                          r.Equals("Partner", StringComparison.OrdinalIgnoreCase)))
            {
                // Usuário autenticado, mas não autorizado a usar o Portal
                _logger.LogWarning("Usuário {Email} logou mas não possui roles de Admin ou Mediator.", request.Email);
                return Forbid();
            }

            return Ok(response);
        }

        /// <summary>
        /// Endpoint para registrar um novo usuário e ligá-lo a um Mediator existente.
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous] // Permite acesso a este endpoint sem autenticação prévia
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Chama o serviço de autenticação para criar o usuário e gerar o token.
                // Aqui é onde o MediatorId é usado para ligar o usuário.
                var response = await _authService.RegisterAsync(request);

                _logger.LogInformation("Novo usuário registrado com sucesso: {Email}", request.Email);

                // Retorna a resposta de login (incluindo o Token e MediatorId)
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                // Captura erros específicos da criação de usuário (ex: Email já em uso, MediatorId inválido)
                _logger.LogError(ex, "Erro durante o registro do usuário: {Message}", ex.Message);
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado durante o registro.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = "Ocorreu um erro interno no servidor durante o registro." });
            }
        }
    }
}
