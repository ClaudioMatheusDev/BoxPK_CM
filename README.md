# BoxPK_CM

BoxPK_CM é uma aplicação full-stack para gestão de produtos, estoque, compras e fornecedores, com um backend em ASP.NET Core e um frontend em React com Vite. O projeto foi pensado para rodar de forma simples em ambiente local, com suporte a containers via Docker Compose.

## Visão geral

O sistema permite gerenciar:

- Categorias
- Coleções
- Fornecedores
- Jogos
- Produtos
- Estoque
- Movimentações de estoque
- Compras e itens de compra

A arquitetura foi organizada em camadas para separar responsabilidades entre aplicação, domínio, infraestrutura e API.

## Stack tecnológica

### Backend
- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- FluentValidation
- SQL Server

### Frontend
- React 19
- Vite
- JavaScript

### DevOps / Infraestrutura
- Docker
- Docker Compose

## Estrutura do projeto

```text
src/
  Application/      # Casos de uso, DTOs e validações
  Domain/           # Entidades e regras de negócio
  Infrastructure/   # Contexto do banco e migrations
  WebApi/           # API REST e controllers
  frontend/FrontEnd # Aplicação React/Vite
```

## Requisitos

Antes de começar, certifique-se de ter instalado:

- Docker Desktop
- Docker Compose
- Git
- .NET SDK 9 (se quiser executar a API sem containers)

## Execução rápida com Docker Compose

1. Clone o repositório:

```bash
git clone https://github.com/ClaudioMatheusDev/BoxPK_CM.git
cd BoxPK_CM
```

2. Crie um arquivo `.env` na raiz do projeto com as variáveis abaixo:

```env
SA_PASSWORD=SuaSenhaForte123!
Jwt__Key=SuaChaveJwtComPeloMenos32Caracteres
VITE_API_BASE_URL=http://localhost:5236
```

3. Suba os containers:

```bash
docker compose up -d --build
```

4. Acesse as aplicações:

- Frontend: http://localhost:5173
- API: http://localhost:5236

O Docker Compose já sobe:

- Banco de dados SQL Server
- API ASP.NET Core
- Frontend React

## Execução local da API

Se quiser rodar apenas o backend localmente:

```bash
dotnet restore src/BoxPKCM.sln
dotnet build src/BoxPKCM.sln
dotnet run --project src/WebApi/WebApi.csproj
```

Lembre-se de ajustar a connection string no arquivo de configuração da API conforme o ambiente local.

## Variáveis de ambiente

- `SA_PASSWORD`: senha do SQL Server utilizado pelo Docker Compose
- `Jwt__Key`: chave usada para assinar e validar os tokens JWT da API
- `VITE_API_BASE_URL`: URL base da API consumida pelo frontend

## Funcionalidades principais

A aplicação fornece uma base sólida para:

- Cadastro e consulta de produtos
- Controle de estoque e movimentações
- Registro de compras
- Gestão de fornecedores e categorias
- Organização de coleções e jogos

## Contribuição

Contribuições são bem-vindas. Para sugerir melhorias ou relatar problemas, abra uma issue ou envie um pull request.

## Repositório

- GitHub: https://github.com/ClaudioMatheusDev/BoxPK_CM
