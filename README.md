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
	  Banco de Dados: Sql Sever 2025
	  
	  
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


# FUNCIONALIDADES DO SERVIÇO CLIENTE DE COTAÇÕES (`QuoteClientService`) 🤝

Este documento descreve as funcionalidades de comunicação do lado do **Cliente (Blazor WebAssembly)** com o servidor (API) para o módulo de Cotações (Quotes). Este serviço atua como uma ponte que envia dados do usuário para o *backend* e trata as respostas, incluindo erros.

---

## 1. Módulos e Componentes Envolvidos

| Componente | Função |
| :--- | :--- |
| **`QuoteClientService`** | Serviço injetável no Blazor (Client) que usa o `HttpClient` (autenticado) para fazer chamadas à API. |
| **`HttpClient _http`** | A instância injetada do cliente HTTP (configurada para incluir o token de autorização). |
| **`QuotePriceRequestDto`** | DTO de dados de entrada para calcular o preço. |
| **`QuotePricedDto`** | DTO de dados de saída (resultado do cálculo do preço). |
| **`QuoteBindRequestDto`** | DTO de dados de entrada para emitir a apólice. |
| **`BindResultDto`** | DTO de dados de saída (ID da nova apólice). |

---

## 2. Funcionalidade: CALCULAR PREÇO (`PriceQuoteAsync`)

Esta função envia os dados de risco fornecidos pelo Mediador ao servidor para precificação.

### Fluxo de Comunicação:

1.  **Requisição:** Envia uma requisição **`POST`** com o objeto `QuotePriceRequestDto` (contendo ClientId, VehicleId, etc.) serializado como JSON para o endpoint: **`api/quotes/price`**.
2.  **Tratamento de Sucesso (200 OK):**
    * Se a resposta for bem-sucedida, o conteúdo é desserializado para o objeto **`QuotePricedDto`** (contendo o prêmio total e o detalhamento) e retornado.
3.  **Tratamento de Falha (4xx/5xx):**
    * Se o servidor retornar um código de falha (ex: `400 Bad Request` devido a uma regra de subscrição não atendida), o serviço tenta ler a **mensagem de erro** do corpo da resposta (via `ErrorResponse`).
    * É lançada uma **`ApplicationException`** no Cliente, que permite que a UI exiba a mensagem de erro específica do negócio ("Cliente deve ter pelo menos 18 anos", por exemplo).

---

## 3. Funcionalidade: EMITIR APÓLICE (`BindQuoteAsync`)

Esta função é usada para converter uma cotação precificada em uma apólice de seguro ativa.

### Fluxo de Comunicação:

1.  **Requisição:** Envia uma requisição **`POST`** com o objeto `QuoteBindRequestDto` (contendo `QuoteId` e `MediatorId`) para o endpoint específico da cotação: **`api/quotes/{QuoteId}/bind`**.
2.  **Tratamento de Sucesso (200 OK):**
    * Se a emissão for bem-sucedida, o servidor retorna o **`BindResultDto`**, que contém o **`PolicyId`** da nova apólice.
3.  **Tratamento de Falha (4xx/5xx):**
    * Se houver falha na emissão (ex: cotação não encontrada, erro de servidor), o serviço lê a mensagem de erro do `ErrorResponse`.
    * É lançada uma **`ApplicationException`** com a mensagem do servidor, permitindo que o Mediador veja o motivo da falha na UI.

---

## 4. Funcionalidade: HISTÓRICO DE COTAÇÕES (`GetQuotesByClientAsync`)

Esta função permite que a aplicação do Cliente recupere o histórico de cotações para um cliente específico.

### Fluxo de Comunicação:

1.  **Requisição:** Envia uma requisição **`GET`** para o endpoint que inclui o ID do cliente: **`api/quotes/client/{clientId}`**.
2.  **Tratamento de Sucesso (200 OK):**
    * Se a busca for bem-sucedida, o conteúdo é desserializado para uma **lista de `QuoteDto`** (DTOs de cotações simplificadas) e retornado.
3.  **Tratamento de Falha (4xx/5xx):**
    * Se houver falha na busca, é lançada uma **`ApplicationException`** com a mensagem de erro do servidor.
