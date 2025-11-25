# PortalTVDE
Mediação “Auto TVDE Lite” 

 Portal de mediação para cotar e emitir uma apólice simples “Auto TVDE Lite”
   
   #Tecnologias aplicadas:
   
     .NET 9 SDK 

      API: ASP.NET Core Web API (Swagger, ProblemDetails, HealthChecks) 

      UI: Blazor WebAssembly Hosted (servida pelo ASP.NET Core Server) 

      Dados: SQL Server local; EF Core Code-First + Migrations 

      Auth: JWT (roles Admin, Mediator) 

      Testes: xUnit 
	  
	  
	  Descrição das Telas.
	 
	 


# FUNCIONALIDADES DA TELA DE LOGIN/REGISTRO (PortalTVDE)

Este documento descreve as funcionalidades e o fluxo de trabalho da tela de autenticação (Login e Registro) do Portal TVDE, 
conforme implementado na Controller `AuthController` e no serviço `AuthService`.

---

## 1. Módulos e Interface

A tela de autenticação (`/login`) possui dois modos principais, alternáveis por botões: **Entrar (Login)** e **Registar Novo (Registro)**.

### Campos de Entrada:
* **Email** e **Password** (Obrigatórios em ambos os modos).
* **Nome de Utilizador (Opcional)** (Apenas no modo Registro).
* **ID do Mediador (Obrigatório)** (Apenas no modo Registro), que liga o novo usuário a um Mediador existente.

---

## 2. Funcionalidade de ENTRAR (Login)

### Fluxo de Trabalho:
1.  O Cliente envia **Email** e **Password** para o endpoint da API: `POST api/auth/login`.
2.  O serviço **`AuthService.LoginAsync`** verifica:
    * Se o usuário com o email fornecido existe no banco de dados.
    * Se a senha fornecida está correta (usando `_userManager.CheckPasswordAsync`).
    * **Em caso de falha (usuário não encontrado ou senha incorreta):** Retorna `401 Unauthorized` com a mensagem genérica: "Credenciais inválidas." (Não revela qual campo está errado, por segurança).
3.  **Em caso de sucesso na autenticação:**
    * O servidor verifica as **Roles (permissões)** do usuário.
    * **Apenas** usuários com a role **"Admin"** ou **"Mediator"** são autorizados a prosseguir no Portal.
    * Se o usuário autentica, mas não tem uma dessas roles, o servidor retorna **`403 Forbid`** (acesso negado ao Portal).
4.  **Login Válido (Admin/Mediator):**
    * O servidor gera um **Token JWT** contendo Claims (incluindo `UserId`, `Email`, `Roles` e `MediatorId`).
    * O servidor retorna **`200 OK`** com o Token e dados do usuário (`LoginResponseDto`).
5.  **No Cliente (`ClientAuthService.Login`):** O token JWT é armazenado no **Session Storage** e o estado de autenticação (AuthenticationStateProvider) é atualizado, redirecionando o usuário para a página `/quotes`.

---

## 3. Funcionalidade de REGISTRAR NOVO (Registro)

Este endpoint é **público** (`[AllowAnonymous]`) e permite que novos usuários criem contas.

### Fluxo de Trabalho:
1.  O Cliente envia **Email**, **Password**, **UserName** (Opcional) e **MediatorId** (Obrigatório) para o endpoint da API: `POST api/auth/register`.
2.  O serviço **`AuthService.RegisterAsync`** realiza:
    * Cria um novo objeto `ApplicationUser` e tenta criá-lo no Identity (usando `_userManager.CreateAsync`).
    * O novo usuário é associado ao `MediatorId` fornecido.
    * O usuário recebe a role padrão **"Partner"** (presumivelmente um parceiro ou motorista, que não é Admin nem Mediador).
3.  **Em caso de falha no servidor (ex: Email já em uso ou violação das regras de senha do Identity):**
    * O servidor captura o erro e retorna **`400 Bad Request`** com a mensagem detalhada do erro.
4.  **Em caso de sucesso no registro:**
    * O servidor gera um **Token JWT** para o novo usuário (realizando um login automático).
    * O servidor retorna **`200 OK`** com o Token e dados do novo usuário (`LoginResponseDto`).
5.  **No Cliente (`ClientAuthService.Register`):** O novo token é armazenado no **Session Storage** e o usuário é logado automaticamente e redirecionado para a página `/quotes`.

---

## 4. Geração e Uso do Token JWT

O serviço **`AuthService`** contém o método privado `GenerateJwtToken` que:
* Cria uma lista de Claims, incluindo o **ID do Mediador** (`MediatorId`), **ID do Usuário**, **Email** e todas as **Roles** do usuário.
* Assina o token usando a chave secreta configurada (`Jwt:Key`) e define a validade (Expira).
* Este token é o mecanismo de segurança que permite ao usuário acessar outras Controllers protegidas com `[Authorize]`.

---

## 5. Logout

### Fluxo de Trabalho:
1.  O Cliente chama o método **`ClientAuthService.Logout`**.
2.  O token JWT é **removido** do **Session Storage**.
3.  O estado de autenticação é resetado (`MarkUserAsLoggedOut`), e o `HttpClient` tem seu cabeçalho de autorização removido, encerrando a sessão.
