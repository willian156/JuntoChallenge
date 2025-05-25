# JuntoChallenge API

API desenvolvida como parte do desafio técnico do processo seletivo da **Junto Seguros**.  
O objetivo é construir uma aplicação back-end robusta para gerenciamento de usuários, com autenticação JWT, criptografia de senhas, logs e operações CRUD seguras e eficientes.

---

## ✅ Tecnologias e pacotes utilizados

- **.NET 8**
- **Entity Framework Core** (`Microsoft.EntityFrameworkCore`)
- **BCrypt.Net-Next** (para hash de senhas)
- **DotNetEnv** (leitura de variáveis de ambiente via `.env`)
- **JWT Bearer Authentication** (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- **FluentAssertions**, **FakeItEasy** e **xUnit** (para testes)
- **Swashbuckle.AspNetCore** (Swagger/OpenAPI)
- **Docker + Docker Compose** (containerização)

---

## ⚙️ Requisitos para rodar o projeto

- [.NET SDK 8.0+](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/) (recomendado para subir ambiente)
- SQL Server
- (Opcional) Visual Studio / VS Code

---

## ▶️ Como rodar o projeto
### 1. Clonar o repositório
```bash
git clone https://github.com/seu-usuario/JuntoChallenge.git
cd JuntoChallenge
```

### 2. Definir variáveis de ambiente no arquivo .env

```bash
CONNECTION_STRING=Server=db,1433;Database=Junto.db;User Id=sa;Password=SuaSenha@123;Encrypt=False;TrustServerCertificate=True;
JWT_KEY=sua-chave-secreta-jwt
JWT_ISSUER=JuntoChallengeAPI
JWT_AUDIENCE=JuntoChallengeUser
JWT_EXPIRE_MINUTES=60
```
### 3. Subir via Docker Compose
```bash
docker-compose up --build -d
```
---
### 🔄 Atualizar o banco de dados com EF Core

Para aplicar as migrations no banco de dados, use o seguinte comando no terminal ou console do gerenciador de pacotes:
```bash
dotnet ef database update --project ./JuntoChallenge.Infrastructure/JuntoChallenge.Infrastructure.csproj --startup-project ./JuntoChallenge.API/JuntoChallenge.API.csproj
```
>⚠️ Certifique-se de que o banco esteja acessível e que a connection string esteja configurada corretamente.

---

## 📌 Endpoints de Usuário
### 🔐 Autenticação
- `POST /api/Login`
  Autentica o usuário e retorna o token JWT.
- `POST /api/Users`
  Cria um novo usuário com `username`, `email`, e `password`

---

### 👤 CRUD de Usuário (necessário token JWT)
- `GET /api/Users`
  Lista paginada de usuários. Parâmetros: `pageNumber`, `pageSize`.
- `GET /api/Users/{id}`
  Retorna os dados de um usuário por ID.
- `PUT /api/Users/{id}`
  Atualiza os dados (exceto senha) de um usuário existente.
- `DELETE /api/Users/{id}`
  Deleta logicamente um usuário (define `IsDeleted = true`).

---

### 📝 Observações
Deixei os arquivos .env para exemplificar de como ele está arquivado no sistema e, também, para a análise das chaves contidas no documento, 
mas o arquivo pode ser facilmente eliminado após a inclusão do nome do mesmo no .gitignore.
Também deixei os endpoints de criar um usuário (`POST /api/Users`) e login (`POST /api/Login`) sem autenticação, pois foi o método que escolhi para que o usuário possa obter a chave jwt facilmente.

---

Feito com muito café e códigos por Willian Beck Ribeiro Simões

---
