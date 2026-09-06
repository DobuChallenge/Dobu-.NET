# Sprint 4 Dobu API Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Evoluir a API Dobu com os requisitos funcionais da Sprint 4 e entregar integralmente monitoramento, observabilidade, testes AAA e documentação exigidos na Sprint 3.

**Architecture:** Manter a solução em camadas Domain, Application, Infrastructure e Api, adicionando DTOs/serviços para não expor entidades diretamente. O banco será configurável: SQLite por padrão para desenvolvimento e testes locais, Oracle preservado por configuração de produção; autenticação será JWT com hash de senha.

**Tech Stack:** .NET 9, ASP.NET Core Web API, EF Core, SQLite, Oracle EF Core, JWT Bearer, Serilog, OpenTelemetry, xUnit, Moq, WebApplicationFactory.

**Spec:** `C:\Users\yumizxs\.codex\attachments\21788b0b-0331-417c-932e-44845ee681d5\pasted-text.txt`

## Global Constraints

- Health checks devem verificar a API, o banco de dados e a disponibilidade de serviço externo.
- Logging estruturado deve incluir níveis Information, Warning e Error, correlação de requisições e saída em console/arquivo.
- Tracing distribuído e métricas devem expor tempo de resposta e taxa de erros.
- Testes unitários e de integração devem usar xUnit e padrão AAA.
- Testes de integração devem usar `WebApplicationFactory` e validar autenticação, sucesso e erros.
- O README deve documentar health checks, monitoramento, testes e funcionalidades novas.

### Task 1: Preparar dependências e persistência configurável

**Files:**
- Modify: `DOBU/Dobu.Api/Dobu.Api.csproj`, `DOBU/Dobu.Infrastructure/Dobu.Infrastructure.csproj`
- Modify: `DOBU/Dobu.Api/Program.cs`, `DOBU/Dobu.Api/appsettings.json`, `DOBU/Dobu.Api/appsettings.Development.json`
- Modify: `DOBU/Dobu.Infrastructure/Persistence/DobuDbContext.cs`

- [ ] Adicionar pacotes de SQLite, JWT, Serilog, OpenTelemetry e health checks.
- [ ] Configurar SQLite como padrão e Oracle quando `Database:Provider` for `Oracle`.
- [ ] Garantir que o `DbContext` possa ser criado pela API e pela fábrica de integração.
- [ ] Executar `dotnet build DOBU/Dobu.sln` e corrigir erros de dependência.

### Task 2: Autenticação e autorização

**Files:**
- Create: `DOBU/Dobu.Application/DTOs/AuthDtos.cs`
- Create: `DOBU/Dobu.Application/Services/IAuthService.cs`, `DOBU/Dobu.Application/Services/AuthService.cs`
- Create: `DOBU/Dobu.Api/Controllers/AuthController.cs`
- Modify: `DOBU/Dobu.Api/Program.cs`

- [ ] Escrever testes unitários AAA para registro, login, senha inválida e e-mail duplicado.
- [ ] Verificar os testes falhando por ausência do serviço.
- [ ] Implementar hash seguro de senha, emissão de JWT e claims de usuário/tipo.
- [ ] Proteger endpoints por `[Authorize]` e aplicar regras para responsável/veterinário/admin.
- [ ] Executar testes unitários e confirmar aprovação.

### Task 3: Pets, agendamentos e informações de cuidado

**Files:**
- Create: `DOBU/Dobu.Domain/Entities/InformacaoCuidado.cs`
- Create: `DOBU/Dobu.Application/DTOs/PetDtos.cs`, `DOBU/Dobu.Application/DTOs/AgendamentoDtos.cs`, `DOBU/Dobu.Application/DTOs/InformacaoCuidadoDtos.cs`
- Create: `DOBU/Dobu.Api/Controllers/AgendamentosController.cs`, `DOBU/Dobu.Api/Controllers/InformacoesCuidadoController.cs`
- Modify: `DOBU/Dobu.Api/Controllers/PetsController.cs`, `DOBU/Dobu.Infrastructure/Persistence/DobuDbContext.cs`, `DOBU/Dobu.Infrastructure/Persistence/Configurations/DobuConfiguration.cs`

- [ ] Escrever testes unitários AAA para regras de criação/atualização e transições de status.
- [ ] Verificar os testes falhando antes da implementação.
- [ ] Implementar CRUD autenticado de pets, agendamentos e informações de cuidado com DTOs e validação.
- [ ] Restringir acesso aos dados do responsável autenticado e permitir gestão administrativa/veterinária conforme papel.
- [ ] Executar testes unitários e testes de compilação.

### Task 4: Health checks, logging, tracing e métricas

**Files:**
- Create: `DOBU/Dobu.Api/Health/ExternalServiceHealthCheck.cs`
- Create: `DOBU/Dobu.Api/Middleware/CorrelationIdMiddleware.cs`
- Create: `DOBU/Dobu.Api/Observability/ObservabilityExtensions.cs`
- Modify: `DOBU/Dobu.Api/Program.cs`, `DOBU/Dobu.Api/appsettings.json`

- [ ] Escrever teste de integração AAA para `/health` com status saudável e `/health/ready` incluindo banco/serviço externo.
- [ ] Verificar o teste falhando antes da configuração.
- [ ] Configurar `AddHealthChecks`, banco e verificação HTTP externa sem bloquear a inicialização.
- [ ] Configurar Serilog com console, arquivo diário, níveis e `CorrelationId`.
- [ ] Configurar OpenTelemetry ASP.NET Core/HttpClient/EF Core, exportador console e métricas Prometheus em `/metrics`.
- [ ] Executar os testes e verificar respostas e instrumentação.

### Task 5: Projetos de testes e fixtures

**Files:**
- Create: `DOBU/tests/Dobu.UnitTests/Dobu.UnitTests.csproj`
- Create: `DOBU/tests/Dobu.IntegrationTests/Dobu.IntegrationTests.csproj`
- Create: `DOBU/tests/Dobu.IntegrationTests/CustomWebApplicationFactory.cs`
- Create: testes unitários e de integração organizados por camada
- Modify: `DOBU/Dobu.sln`

- [ ] Organizar testes com nomes `MetodoTestado_Cenario_ResultadoEsperado`.
- [ ] Usar `WebApplicationFactory` e collection fixture para compartilhar servidor/banco de integração.
- [ ] Cobrir autenticação, endpoints protegidos, respostas de sucesso, validação e erros HTTP.
- [ ] Executar `dotnet test DOBU/Dobu.sln --collect:"XPlat Code Coverage"`.

### Task 6: README e verificação final

**Files:**
- Modify: `README.md`
- Modify: `DOBU/Dobu.Api/Properties/launchSettings.json` if required

- [ ] Documentar configuração SQLite/Oracle, JWT, endpoints funcionais, `/health`, `/health/ready` e `/metrics`.
- [ ] Documentar logging estruturado, correlação, tracing e arquivos gerados.
- [ ] Documentar execução da API e `dotnet test` com cobertura.
- [ ] Executar build, testes e smoke tests HTTP.
- [ ] Conferir `git diff` e garantir que todos os requisitos da Sprint 3 aparecem explicitamente no README.

## Self-Review Checklist

- Health checks: API, banco e serviço externo cobertos na Task 4.
- Logging: níveis, correlação e console/arquivo cobertos na Task 4.
- Tracing/métricas: OpenTelemetry, tempo de resposta e erros cobertos na Task 4.
- Unitários: xUnit, AAA e mocking cobertos nas Tasks 2 e 3.
- Integração: `WebApplicationFactory`, autenticação, sucesso e erros cobertos na Task 5.
- Organização: projetos Unit/Integration, fixtures e nomenclatura cobertos na Task 5.
- README: endpoints, monitoramento, testes e funcionalidades cobertos na Task 6.
