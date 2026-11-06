# BoxPK_CM

BoxPK_CM é uma aplicação full-stack para gestão de uma loja/coleção de cartas e produtos TCG. O projeto combina uma API REST em ASP.NET Core com um frontend em React, oferecendo cadastro, consulta e manutenção de produtos, jogos, coleções, categorias, fornecedores, estoque e compras.

## Principais funcionalidades

- Autenticação de usuários com ASP.NET Core Identity, JWT e refresh token
- Cadastro e gerenciamento de categorias
- Cadastro e gerenciamento de fornecedores
- Cadastro e gerenciamento de jogos TCG
- Cadastro e gerenciamento de coleções
- Cadastro e gerenciamento de produtos
- Controle de estoque
- Registro de movimentações de estoque
- Registro de compras e itens de compra
- Frontend administrativo em React consumindo a API
- Banco SQL Server com migrations do Entity Framework Core
- Ambiente local com Docker Compose

## Stack

### Backend

- .NET 9
- ASP.NET Core Web API
- ASP.NET Core Identity
- JWT Bearer Authentication
- Entity Framework Core
- FluentValidation
- SQL Server

### Frontend

- React 19
- Vite
- JavaScript
- Oxlint

### Infraestrutura

- Docker
- Docker Compose
- SQL Server 2022

## Estrutura do projeto

```text
.
|-- docker-compose.yml
|-- README.md
`-- src
    |-- BoxPKCM.sln
    |-- Application
    |   |-- Dtos
    |   `-- Users
    |-- Domain
    |   |-- Entities
    |   `-- Enums
    |-- Infrastructure
    |   |-- Data
    |   `-- Migrations
    |-- WebApi
    |   |-- Controllers
    |   |-- Service
    |   |-- Program.cs
    |   `-- Dockerfile
    `-- frontend
        `-- FrontEnd
            |-- src
            |-- package.json
            `-- Dockerfile
```

## Requisitos

Para executar com Docker:

- Docker Desktop
- Docker Compose
- Git

Para executar localmente sem containers:

- .NET SDK 9
- Node.js 22 ou superior
- SQL Server ou SQL Server LocalDB
- EF Core CLI, caso queira criar/aplicar migrations manualmente

## Configuração de ambiente

Crie um arquivo `.env` na raiz do projeto:

```env
SA_PASSWORD=SuaSenhaForte123!
Jwt__Key=SuaChaveJwtComPeloMenos32Caracteres
VITE_API_BASE_URL=http://localhost:5236
```

Variáveis usadas pelo projeto:

- `SA_PASSWORD`: senha do usuário `sa` do SQL Server no Docker Compose
- `Jwt__Key`: chave usada para assinar os tokens JWT
- `VITE_API_BASE_URL`: URL base da API usada pelo frontend

Há um modelo em `.env.example`. Variáveis opcionais:

- `ALLOW_REGISTRATION` (`Auth__AllowRegistration`): `false` desativa o endpoint de cadastro
- `CORS_ORIGIN` (`Cors__AllowedOrigins__N`): origens permitidas pelo CORS
- `Https__Redirect`: `true` ativa o redirecionamento HTTPS da API
- `ASPNETCORE_ENVIRONMENT`: ambiente da API (padrão `Development` no compose)

A chave `Jwt__Key` precisa ter pelo menos 32 caracteres, caso contrário a API não inicia.

> Importante: não versionar valores reais de `.env`, senhas ou chaves JWT.

## Executando com Docker Compose

Na raiz do projeto, execute:

```bash
docker compose up -d --build
```

Serviços iniciados:

- SQL Server: `localhost:1433`
- API: `http://localhost:5236`
- Frontend: `http://localhost:5173`

O container da API usa a variável `APPLY_MIGRATIONS=true`, então as migrations do Entity Framework Core são aplicadas automaticamente ao iniciar.

Para parar os containers:

```bash
docker compose down
```

Para parar e remover também o volume do banco:

```bash
docker compose down -v
```

## Seeds (admin padrão e dados de demonstração)

A API pode criar, na inicialização, um usuário administrador padrão e popular a plataforma com dados de demonstração de uma loja de cartas Pokémon. Os seeds são idempotentes: rodar de novo não duplica nada.

| Variável (`.env`) | Configuração da API | Padrão | Descrição |
| --- | --- | --- | --- |
| `SEED_DATABASE` | `Seed__Enabled` | `false` | Liga os seeds |
| `SEED_ADMIN_EMAIL` | `Seed__AdminEmail` | `admin@boxtcg.local` | E-mail/usuário do administrador |
| `SEED_ADMIN_PASSWORD` | `Seed__AdminPassword` | — | Senha do administrador (obrigatória com os seeds ligados) |
| `SEED_DEMO_DATA` | `Seed__DemoData` | `true` | Popula o tenant do administrador com os dados de demonstração |

O que é criado:

- Usuário administrador (e-mail confirmado) no papel `Admin`, com tenant próprio
- Jogo **Pokémon Estampas Ilustradas** e 14 coleções (Escarlate e Violeta até Rivais Predestinados, além de Origem Perdida inativa)
- 8 categorias (Carta Avulsa, Booster, Elite Trainer Box, Booster Box, Coleção Especial, Deck, Acessórios e Carta Graduada desabilitada)
- 5 fornecedores fictícios (domínios `example.com`)
- 29 cartas avulsas (Charizard ex, Umbreon ex, Pikachu ex, Iono...) com imagem do pokemontcg.io e 11 produtos selados/acessórios
- 5 compras (confirmadas, pendente e cancelada) com itens
- Estoque e movimentações derivados das compras confirmadas e de vendas, com o status de cada produto coerente com o saldo

Os dados de demonstração ficam no tenant do administrador, portanto só aparecem ao entrar com essa conta. Em `Production` apenas o administrador é criado; os dados de demonstração são ignorados.

Para rodar localmente sem Docker:

```bash
Seed__Enabled=true Seed__AdminPassword='TroqueEstaSenha@123' dotnet run --project src/WebApi/WebApi.csproj
```

> Troque a senha do administrador padrão e não use os valores de exemplo em ambientes reais.

## Executando a API localmente

Restaure e compile a solução:

```bash
dotnet restore src/BoxPKCM.sln
dotnet build src/BoxPKCM.sln
```

Configure a connection string em `src/WebApi/appsettings.json` ou use variável de ambiente:

```bash
ConnectionStrings__DefaultConnection="Server=(localdb)\MinhaInstancia;Database=BoxPKCM;Trusted_Connection=True;TrustServerCertificate=True;"
Jwt__Key="SuaChaveJwtComPeloMenos32Caracteres"
```

Execute a API:

```bash
dotnet run --project src/WebApi/WebApi.csproj
```

## Executando o frontend localmente

Acesse a pasta do frontend:

```bash
cd src/frontend/FrontEnd
```

Instale as dependências:

```bash
npm install
```

Execute em modo desenvolvimento:

```bash
npm run dev
```

Scripts disponíveis:

- `npm run dev`: inicia o Vite em desenvolvimento
- `npm run build`: gera build de produção
- `npm run preview`: serve o build localmente
- `npm run lint`: executa o Oxlint

## Endpoints principais da API

Base URL em Docker: `http://localhost:5236`

### Autenticação

- `POST /api/Auth/register`
- `POST /api/Auth/login`
- `POST /api/Auth/refresh`
- `POST /api/Auth/logout`

### Recursos

Os recursos abaixo seguem o padrão REST com `POST`, `GET`, `GET /{id}`, `PUT /{id}` e `DELETE /{id}`:

- `/api/categoria`
- `/api/fornecedor`
- `/api/jogo`
- `/api/colecao`
- `/api/produto`
- `/api/estoque`
- `/api/movimentacoestoque`
- `/api/compra`
- `/api/itemcompra`

### Comportamento da API

- **Multi-tenant:** cada usuário cadastrado recebe um `TenantId` (claim no JWT). Todas as entidades de negócio têm filtro global por tenant, então um usuário só enxerga e altera os próprios dados.
- **Erros:** respostas no formato ProblemDetails (`title` com a mensagem): `404` não encontrado, `400` regra de negócio, `409` conflito/concorrência, `500` erro interno.
- **Criação:** `POST` devolve `201` com `{ "id": ... }`; `PUT`/`DELETE` devolvem `204`.
- **Paginação:** as listagens aceitam `?page=1&pageSize=100` (padrão: 500 itens, máximo 1000).
- **Login:** bloqueio de 15 min após 5 senhas incorretas e rate limit de 20 req/min por IP nos endpoints de autenticação.
- **Compras:** o `ValorTotal` é sempre a soma dos itens. Ao mudar o status de `Pendente` para `Confirmada`, a API gera uma entrada no estoque para cada item. Compras confirmadas/canceladas não podem mais ser alteradas, e confirmadas não podem ser excluídas.
- **Estoque:** movimentações validam saldo e usam controle de concorrência (`RowVersion`); excluir uma movimentação que deixaria o saldo negativo é recusado.

## Testes

```bash
dotnet test src/BoxPKCM.sln
```

Os testes (`src/BoxPKCM.Tests`) usam SQLite em memória com o modelo real e cobrem estoque, compras e isolamento por tenant. O workflow `.github/workflows/ci.yml` roda build, testes, lint e build do frontend.

## Produção

```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build
```

Serve o frontend como build estático via nginx (`Dockerfile.prod`) e roda a API em `Production`. Defina `CORS_ORIGIN` com a URL pública do frontend.

## Banco de dados e migrations

As migrations ficam em:

```text
src/Infrastructure/Migrations
```

Para aplicar migrations manualmente em ambiente local:

```bash
dotnet ef database update --project src/Infrastructure/Infrastructure.csproj --startup-project src/WebApi/WebApi.csproj
```

Para criar uma nova migration:

```bash
dotnet ef migrations add NomeDaMigration --project src/Infrastructure/Infrastructure.csproj --startup-project src/WebApi/WebApi.csproj
```

## Comandos úteis

Build da solução:

```bash
dotnet build src/BoxPKCM.sln
```

Build do frontend:

```bash
cd src/frontend/FrontEnd
npm run build
```

Ver logs dos containers:

```bash
docker compose logs -f
```

Recriar ambiente Docker:

```bash
docker compose down -v
docker compose up -d --build
```

## Repositório

GitHub: https://github.com/ClaudioMatheusDev/BoxPK_CM
