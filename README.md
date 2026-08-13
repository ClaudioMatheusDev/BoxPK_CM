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
