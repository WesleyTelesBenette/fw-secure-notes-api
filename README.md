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
- Altere o CORS na linha 19 do arquivo "Program.cs". Troque "https://wesleytelesbenette.github.io" pelo endereço do seu servidor (provavelmente "http://localhost").

```bash
# Compile e rode a aplicação
dotnet build
dotnet run
```

## 🏬 Padrão de Arquitetura
A arquitetura talvez seja uma das mais simples e utilizadas globalmente, o famoso: ***MVC***.

![](https://github.com/WesleyTelesBenette/my-sources-for-docs/blob/main/fw-secure-notes/mvc.svg)

Por ser uma API simples, mas não tão pequena a ponto de a definir como um microsserviço, foi preferível adotar esse modelo de implementação. O fato do sistema se resumir ao gerenciamento de uma base de dados, e precisar de várias rotas e modelos, faz com que o padrão pareça fornecer a medida certa de complexidade que o projeto precisa.

## 📡 Comunicação com a API

### Endereço da API
- https://fw-secure-notes-api.onrender.com

### Rotas para Requisição
As rotas com 🔒 precisam de autenticação JWT, e as ☑️ não tem nenhum controle de acesso além do próprio CORS.

#### Authentication
- ☑️ **GET**: "/Authentication/{title}/{pin}/password" - Retorna se uma página tem senha (bool).
- 🔒 **GET**: "/Authentication/{title}/{pin}/validate" - Retorna se o token está autenticado.
- ☑️ **POST**: "/Authentication/{title}/{pin}" - Criar um Token JWT, com o corpo de [LoginDto](Dtos/General/LoginDto.cs).

#### File
- 🔒 **GET**: "/File/{title}/{pin}/{fileId}" - Retorna um objeto [FileModelDto](Dtos/File/FileModelDto.cs).
- 🔒 **POST**: "/File/{title}/{pin}" - Criar um Arquivo, com corpo de [CreateFileDto](Dtos/File/CreateFileDto.cs).
- 🔒 **PUT**: "/File/{title}/{pin}/{fileId}/title" - Atualiza o título de um Arquivo, com corpo de [UpdateFileTitleDto](Dtos/File/UpdateFileTitleDto.cs).
- 🔒 **PUT**: "/File/{title}/{pin}/{fileId}/content" - Atualiza o conteúdo de um Arquivo, com corpo de [UpdateFileContentDto](Dtos/File/UpdateFileContentDto.cs).
- 🔒 **DELETE**: "/File/{title}/{pin}/{fileId}" - Exclui um Arquivo.

#### Page
- ☑️ **GET**: "/Page/{title}/{pin}/exist" - Retorna se a Página existe (bool).
- 🔒 **GET**: "/Page/{title}/{pin}/files" - Retorna uma lista com os arquivos da Página (array de [FileModelDto](Dtos/File/FileModelDto.cs)).
- 🔒 **GET**: "/Page/{title}/{pin}/theme" - Retorna o tema de cores atual da Página (int).
- ☑️ **POST**: "/Page" - Cria uma Página, com corpo de [CreatePageDto](Dtos/Page/CreatePageDto.cs).
- 🔒 **PUT**: "/Page/{title}/{pin}/password" - Atualiza a senha da Página, com corpo de [UpdatePagePasswordDto](Dtos/Page/UpdatePagePasswordDto.cs).
- 🔒 **PUT**: "/Page/{title}/{pin}/theme" - Atualiza o tema de cores da Página, com corpo de [UpdatePageThemeDto](Dtos/Page/UpdatePageThemeDto.cs).
- 🔒 **DELETE**: "/Page/{title}/{pin}" - Exclui uma Página.

