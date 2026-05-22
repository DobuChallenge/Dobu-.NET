# Sprint 1 Challenge | DOBU

## Integrantes do Grupo

| Nome | RM |
| --- | --- |
| Amandha Yumi Toyota Artulino | 563549 |
| Erick Takeshi Andrade Nakajune | 566059 |
| Giovanna Bardella Gomes | 561439 |

---

## Domínio do Projeto

A DOBU API é uma API de gerenciamento veterinário inteligente.

A aplicação centraliza o controle de usuários, pets, espécies, raças, consultas, agendamentos, prontuários, vacinas, pagamentos, lembretes, DobuCam, análises com IA e logs de erro. O projeto foi organizado em camadas seguindo uma separação próxima de Clean Architecture.

Repositório: <https://github.com/DobuChallenge/Dobu-.NET>

---

## Objetivo desta Entrega

Esta entrega corresponde à Sprint 1 de Advanced Business Development with .NET do Challenge, com foco em:

- API RESTful com ASP.NET Core Web API
- CRUD completo
- rotas parametrizadas
- retornos HTTP corretos
- persistência com Entity Framework Core
- integração com Oracle Database
- mapeamentos com Fluent API
- migrations versionadas
- contratos e implementações de repositories
- injeção de dependência
- documentação com Swagger/OpenAPI

---

## Tecnologias Utilizadas

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- Oracle Entity Framework Core Provider
- Oracle Database
- Swagger / OpenAPI

---

## Estrutura da Solução

- `Dobu.Domain`: entidades e regras centrais do domínio.
- `Dobu.Application`: contratos da aplicação, incluindo interfaces de repositories.
- `Dobu.Infrastructure`: `DobuDbContext`, Fluent API, migrations e repositories concretos.
- `Dobu.Api`: controllers, rotas REST, Swagger/OpenAPI e configuração de DI.
- `docs`: evidências da modelagem e documentação complementar.

---

## Entidades Modeladas

- Usuario
- Especie
- Raca
- Pet
- Consulta
- Agendamento
- Prontuario
- Vacina
- Pagamento
- Lembrete
- DobuCam
- AnaliseIa
- LogErro

---

## Persistência com EF Core

A persistência foi implementada na camada `Dobu.Infrastructure`, contendo:

- `DobuDbContext`
- `DbSet` para as entidades do domínio
- configurações com `IEntityTypeConfiguration`
- `ApplyConfigurationsFromAssembly`
- mapeamentos com Fluent API
- migration inicial `InitialDobu`
- repositories concretos

---

## Banco de Dados Utilizado

O SGBD utilizado neste projeto é:

**Oracle Database**

---

## Repositories

As interfaces dos repositories ficam em:

```text
DOBU/Dobu.Application/Interfaces/Repositories
```

As implementações ficam em:

```text
DOBU/Dobu.Infrastructure/Repositories
```

Repositories disponíveis:

- `IAgendamentoRepository`
- `IAnaliseIaRepository`
- `IConsultaRepository`
- `IDobuCamRepository`
- `IEspecieRepository`
- `ILembreteRepository`
- `ILogErroRepository`
- `IPagamentoRepository`
- `IPetRepository`
- `IProntuarioRepository`
- `IRacaRepository`
- `IUsuarioRepository`
- `IVacinaRepository`

Cada interface segue o mesmo padrão: `GetAll`, `GetById`, `Add`, `Update`, `Delete` e `SaveChanges`.

---

## Injeção de Dependência

O registro de dependências é feito no projeto da API por meio de:

```csharp
builder.Services.AddPersistence(builder.Configuration);
```

Esse método chama:

```csharp
services.AddInfrastructure(configuration);
```

Na Infrastructure são registrados o `DobuDbContext` com Oracle e todos os repositories no container de injeção de dependência.

---

## Connection String

A connection string deve ser configurada em `DOBU/Dobu.Api/appsettings.Development.json`.

Exemplo seguro:

```json
{
  "ConnectionStrings": {
    "DobuOracle": "Data Source=oracle.fiap.com.br:1521/orcl;User ID=<USUARIO>;Password=<SENHA>;"
  }
}
```

---

## Documentação OpenAPI

O Swagger/OpenAPI está configurado na API.

Com a aplicação em execução, acesse:

```text
http://localhost:5070/swagger
```

Pela interface é possível visualizar os endpoints, conferir parâmetros e testar requisições.

---

## Principais Rotas

### Usuários

| Método | Rota |
| --- | --- |
| GET | `/api/usuarios` |
| GET | `/api/usuarios/{id}` |
| GET | `/api/usuarios/tipo/{tipoUsuario}` |
| GET | `/api/usuarios/email/{email}` |
| POST | `/api/usuarios` |
| PUT | `/api/usuarios/{id}` |
| DELETE | `/api/usuarios/{id}` |

### Pets

| Método | Rota |
| --- | --- |
| GET | `/api/pets` |
| GET | `/api/pets/{id}` |
| GET | `/api/pets/responsavel/{responsavelId}` |
| GET | `/api/pets/raca/{racaId}` |
| GET | `/api/pets/nome/{nome}` |
| POST | `/api/pets` |
| PUT | `/api/pets/{id}` |
| DELETE | `/api/pets/{id}` |

### Consultas

| Método | Rota |
| --- | --- |
| GET | `/api/consultas` |
| GET | `/api/consultas/{id}` |
| GET | `/api/consultas/pet/{petId}` |
| GET | `/api/consultas/veterinario/{veterinarioId}` |
| GET | `/api/consultas/periodo?inicio=2026-05-01&fim=2026-05-31` |
| POST | `/api/consultas` |
| PUT | `/api/consultas/{id}` |
| DELETE | `/api/consultas/{id}` |

### Outras Rotas CRUD

Também existem controllers REST para:

- `/api/agendamentos`
- `/api/analises-ia`
- `/api/dobucams`
- `/api/especies`
- `/api/lembretes`
- `/api/logs-erro`
- `/api/pagamentos`
- `/api/prontuarios`
- `/api/racas`
- `/api/vacinas`

---

## Retornos HTTP Utilizados

| Código | Descrição |
| --- | --- |
| 200 OK | Operação realizada com sucesso |
| 201 Created | Recurso criado com sucesso |
| 204 NoContent | Recurso removido com sucesso |
| 400 BadRequest | Dados inválidos |
| 404 NotFound | Recurso não encontrado |

---

## Migrations

A solução contém migration versionada para criação inicial do schema do banco.

Gerar uma nova migration:

```bash
dotnet ef migrations add NomeDaMigration --project DOBU/Dobu.Infrastructure --startup-project DOBU/Dobu.Api
```

Aplicar migrations:

```bash
dotnet ef database update --project DOBU/Dobu.Infrastructure --startup-project DOBU/Dobu.Api
```

---

## Evidências

As evidências complementares ficam na pasta `/docs`, incluindo:

- modelo relacional
- MER
- esquema físico

---

## Como Executar o Projeto

1. Restaurar os pacotes:

```bash
dotnet restore DOBU/Dobu.sln
```

2. Compilar a solução:

```bash
dotnet build DOBU/Dobu.sln
```

3. Aplicar a migration no banco:

```bash
dotnet ef database update --project DOBU/Dobu.Infrastructure --startup-project DOBU/Dobu.Api
```

4. Executar a API:

```bash
dotnet run --project DOBU/Dobu.Api
```

5. Abrir o Swagger:

```text
http://localhost:5070/swagger
```

---

## Observações Finais

O projeto foi estruturado respeitando a separação por camadas: domínio em `Dobu.Domain`, contratos em `Dobu.Application`, persistência em `Dobu.Infrastructure` e exposição REST em `Dobu.Api`.

Os controllers mantêm as rotas REST da entrega, enquanto os repositories ficam disponíveis para evolução da camada de aplicação e para atender ao requisito de contratos e implementações de persistência.
