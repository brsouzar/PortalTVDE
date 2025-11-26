using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PortalTVDE.Server.Data;
using PortalTVDE.Server.Models;
using PortalTVDE.Server.Services;
using PortalTVDE.Server.Services.Interfaces;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    // 1. Usar SQL Server
    options.UseSqlServer(connectionString,
        // 2. Definir Opções Específicas do SQL Server (Retry)
        sqlServerOptions =>
        {
            sqlServerOptions.EnableRetryOnFailure();
        }
    ));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Configurações de senha, lockout, etc. (Opcional, mas recomendado)
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:7131");
        builder.WithMethods("GET", "POST", "PUT", "DELETE");
        builder.WithHeaders("Content-Type", "Accept", "Authorization");
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options =>
          {
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuer = true,
                  ValidateAudience = true,
                  ValidateLifetime = true,
                  ValidateIssuerSigningKey = true,
                  ValidIssuer = builder.Configuration["Jwt:Issuer"],
                  ValidAudience = builder.Configuration["Jwt:Audience"],
                  IssuerSigningKey = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
              };
              options.Events = new JwtBearerEvents
              {
                  OnChallenge = context =>
                  {
                      // Evita o comportamento padrão do JWT que é redirecionar ou retornar 401 puro
                      context.HandleResponse();

                      // Configura a resposta como JSON
                      context.Response.StatusCode = 401;
                      context.Response.ContentType = "application/json";

                      // Retorna uma mensagem de erro JSON clara
                      var responseBody = "{\"message\":\"Token de autenticação inválido ou ausente.\",\"status\":401}";
                      return context.Response.WriteAsync(responseBody);
                  },
                  OnForbidden = context =>
                  {
                      // Para falhas de autorização (403 Forbidden)
                      context.Response.StatusCode = 403;
                      context.Response.ContentType = "application/json";
                      var responseBody = "{\"message\":\"Você não tem permissão para acessar este recurso.\",\"status\":403}";
                      return context.Response.WriteAsync(responseBody);
                  }
              };
          });



builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("admin"));
    options.AddPolicy("MediatorPolicy", policy => policy.RequireRole("Mediator"));
});

builder.Services.AddScoped<IQuotationService, QuotationService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMediatorService, MediatorService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

//builder.Services.AddScoped<UserSetupService>();

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddHealthChecks();

// NOVO BLOCO DE CONFIGURAÇÃO DO SWAGGER
builder.Services.AddSwaggerGen(options =>
{
    // 1. Definição do Esquema de Segurança
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer", // Define o esquema de autenticação como Bearer
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Token Access"
    }); 
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
                });


});
// FIM DO NOVO BLOCO


var app = builder.Build();


//await RunUserSetupAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
   
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}


app.UseHttpsRedirection();

// Adicionar ProblemDetails para tratamento de erros HTTP
app.UseStatusCodePages();

// Servir o Blazor WebAssembly (Client)
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("MyPolicy");

app.UseStatusCodePages(async context =>
{
    // Verifica se a requisição é para a API (que espera JSON)
    if (context.HttpContext.Request.Path.StartsWithSegments("/api"))
    {
        context.HttpContext.Response.ContentType = "text/plain";
        await context.HttpContext.Response.WriteAsync($"Status Code: {context.HttpContext.Response.StatusCode}");
    }
    // Para todas as outras requisições (do Blazor), o código de status será tratado normalmente.
});
app.MapHealthChecks("/health");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); 
app.MapRazorPages();

// Fallback para o Blazor Client
app.MapFallbackToFile("index.html");

app.Run();





static async Task RunUserSetupAsync(WebApplication app)
{
    // Cria um escopo de serviço, pois o UserSetupService é Scoped (tem dependências Scoped, como o UserManager)
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>(); // Para logar o resultado no console

        try
        {
            // Resolve o serviço necessário dentro do escopo
            var userSetupService = services.GetRequiredService<UserSetupService>();

            // --- VARIÁVEIS CONFIGURADAS ---
            const string targetEmail = "mediator@mds.pt";
            const string correctPassword = "Passw0rd#";
            // ------------------------------------

            logger.LogInformation("Iniciando correção do hash de senha para o usuário: {Email}", targetEmail);

            // Chama o método de correção no seu serviço
            var success = await userSetupService.CorrectUserPasswordHashAsync(targetEmail, correctPassword);

            if (success)
            {
                logger.LogInformation("Hash de senha corrigido com sucesso para {Email}. Lembre-se de remover este código!", targetEmail);
            }
            else
            {
                logger.LogWarning("Falha na correção do hash para {Email}. Verifique se o e-mail existe no banco de dados.", targetEmail);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro fatal durante a execução da correção de dados do usuário.");
        }
    }

    
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Lista de roles que você precisa
        string[] roleNames = { "admin", "Mediator", "Partner" };

        foreach (var roleName in roleNames)
        {
            // Verifica se o papel já existe
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                // Se não existe, cria o novo papel
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }


}

