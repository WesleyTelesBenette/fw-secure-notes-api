# API do FW Secure Notes

Um sistema Backend, responsável por intermediar a comunicação entre a aplicação [FW Secure Notes](https://github.com/WesleyTelesBenette/fw-secure-notes) e seu respectivo banco de dados.

## 🍃 Tecnologias utilizadas
- **ASP.NET** (C#): v8.0
- **PostgreSQL**: v15.1.1.64

## 💻 Como executar o Projeto localmente?
### Pré-requisitos
- Tenha o [.NET](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0) instalado, na versão 8 obviamente.

### Execução
```bash
# Clone o repositório
git clone https://github.com/WesleyTelesBenette/fw-secure-notes-api.git
```
```bash
# Acesse a pasta do projeto
cd fw-secure-notes-api
```
```bash
# Instale as dependências
dotnet restore
```
```bash
# Compile e rode a aplicação
dotnet build
dotnet run
```

## 🏬 Padrão de Arquitetura
A arquitetura talvez seja uma das mais simples e utilizadas globalmente, o famoso: ***MVC***.

Por ser uma API simples, mas não tão pequena a ponto de a definir como um microsserviço, foi preferível adotar esse modelo de implementação. O fato do sistema se resumir ao gerenciamento de uma base de dados, e precisar de várias rotas e modelos, faz com que o padrão parecça fornecer a medida certa de complexidade que o projeto precisa.

## 📡 Comunicação com a API

### Endereço da API
- https://fw-secure-notes-api.onrender.com

### Rotas para Requisição
As rotas com 🔒 precisam de autenticação JWT, e as ☑️ não tem nenhum controle de acesso além do próprio CORS.

#### Authentication
- ☑️ **GET**: "/Authentication/{title}/{pin}/password"
- ☑️ **GET**: "/Authentication/{title}/{pin}/validate"
- ☑️ **POST**: "/Authentication/{title}/{pin}"

#### File
- 🔒 **GET**: "/File/{title}/{pin}/{fileId}"
- 🔒 **POST**: "/File/{title}/{pin}"
- 🔒 **PUT**: "/File/{title}/{pin}/{fileId}/title"
- 🔒 **PUT**: "/File/{title}/{pin}/{fileId}/content"
- 🔒 **DELETE**: "/File/{title}/{pin}/{fileId}"

#### Page
- ☑️ **GET**: "/Page/{title}/{pin}/exist"
- 🔒 **GET**: "/Page/{title}/{pin}/files"
- 🔒 **GET**: "/Page/{title}/{pin}/theme"
- ☑️ **POST**: "/Page"
- 🔒 **PUT**: "/Page/{title}/{pin}/password"
- 🔒 **PUT**: "/Page/{title}/{pin}/theme"
- 🔒 **DELETE**: "/Page/{title}/{pin}"

