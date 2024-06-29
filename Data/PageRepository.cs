using fw_secure_notes_api.Dtos.File;
using fw_secure_notes_api.Models;
using fw_secure_notes_api.Services;
using Microsoft.EntityFrameworkCore;

namespace fw_secure_notes_api.Data;

public class PageRepository
{
    private readonly DatabaseContext _dbContext;

    public PageRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    //Verifies
    public async Task<bool> IsPageExist(string title, string pin)
    {
        var page = await _dbContext
            .Pages.FirstOrDefaultAsync(p => (p.Title == title) && (p.Pin == pin));

        return (page != null) && (!string.IsNullOrEmpty(page.Title)) && (!string.IsNullOrEmpty(page.Pin));
    }

    public async Task<bool> IsPageValid(string title, string pin, string password)
    {
        var page = await _dbContext.Pages
            .FirstOrDefaultAsync(p => (p.Title == title) && (p.Pin == pin));

        return (page != null) && BCrypt.Net.BCrypt.Verify(password, page.Password);
    }

    public async Task<bool> IsPageHasPassword(string title, string pin)
    {
        var page = await _dbContext.Pages
            .FirstOrDefaultAsync(p =>
                (p.Title == title) &&
                (p.Pin == pin));

        return (page != null) && (BCrypt.Net.BCrypt.Verify("", page.Password) == false);
    }


    //Gets
    public async Task<int?> GetPageTheme(string title, string pin)
    {
        var theme = await _dbContext.Pages
            .FirstOrDefaultAsync(p => (p.Title == title) && (p.Pin == pin));

        return theme?.Theme;
    }

    public ICollection<FileModelDto>? GetFileList(string title, string pin)
    {
        ICollection<FileModelDto>? fileList = _dbContext.Files
            .Where(f => (f.Page.Title == title) && (f.Page.Pin == pin))
            .OrderBy(f => f.Title).Select(f => new FileModelDto { Id = f.Id, Title = f.Title }).ToList();

        return fileList;
    }

    public async Task<List<string>> GetPageListWithThisTitle(string title)
    {
        return (await _dbContext.Pages.Where(p => p.Title == title).Select(p => p.Pin).ToListAsync()) ?? [];
    }


    ///Posts
    public async Task<ActionResultService.Results> CreatePage(PageModel newPage)
    {
        newPage.Files.Add(new(
            "🥳 Manual de Boas-Vindas",
            """
            # ✨ Seja Bem-vindo(a) ao FW Secure Notes
            ---
            Utilizando este App você é capaz de anotar todas suas ideias, sonhos e objetivos. Talvez planejar sua vida? Plano de Carreira? Projetos? Tarefas?
            Aqui você tem **liberdade** para criar o que quiser, de uma forma **hyper simplificada**, mas que seja bonita de visualizar.
            # Como usar o FW Secure Notes? 🤔
            ---
            Aqui vai uma explicação resumida.
            - **Arquivos**: Você pode gerenciar seus arquivos (criar, acessar ou excluir).
            -- Cada arquivo, possui um **título** e uma **área de escrita** onde você inserir suas anotações efetivamente.
            - **Barra de Ferramentas**: Nela você pode encontrar opções como: Modo visualização, ajuda, copiar link da página, configurações e deslogar da página, respectivamente.
            -- Copiar o Link da página é a forma mais eficiente de acessar ela depois, para não precisar decorar seu PIN.
            -- O PIN pode ser visualizado nas configurações, mas também no próprio título da página (logo após o nome que você deu à página, tem um PIN de 3 caracteres).
            -- Nas configurações você pode realizar mais algumas ações, como: escolher o tema de cor da página, trocar a senha, deletar a página.
            Ps. Você pode ter uma explicação mais detalhada no [Repositório do Projeto](https://github.com/WesleyTelesBenette/fw-secure-notes).
            # Estilização 🎨
            ---
            ## Formatar texto
            Você pode formatar seus textos com caracteres especiais, deixando o texto em negrito, itálico etc.
            - *Texto em Itálico*
            - **Texto em Negrito**
            - ***Texto em Itálico e Negrito***
            - ~Texto Tachado~
            - _Texto Sublinhado_
            ## Adicionar Links
            Você pode criar um link, definindo um título e um endereço.
            - [Receitas de Abobrinha](https://www.terra.com.br/vida-e-estilo/degusta/receitas/5-receitas-com-abobrinha-para-fugir-do-convencional-nas-refeicoes,f9bba1fb767aa0c53ff9626b7ecd83595g4npc2z.html).
            ## Criar Títulos
            Colocando algumas dessas 3 variações no começo da linha, você conseguir criar um título bem legal 😎
            - # Títulos
            - ## de diferentes
            - ### tamanhos
            ## Criar Listas
            Listas de itens, com 3 níveis de subitens.
            - Lista
            -- Subitem 1
            --- Subitem 2
            ---- Subitem 3
            ## Criar Linhas de Divisão
            Você pode criar uma linha de divisão usando três hifens (---):
            ---
            """));
        await _dbContext.Pages.AddAsync(newPage);

        int save = await _dbContext.SaveChangesAsync();

        return (save > 0)
            ? ActionResultService.Results.Created
            : ActionResultService.Results.ServerError;
    }


    //Puts
    public async Task<ActionResultService.Results> UpdateTheme(string title, string pin, int newTheme)
    {
        var page = await _dbContext.Pages
            .FirstOrDefaultAsync(p => (p.Title == title) && (p.Pin == pin));

        if (page != null)
        {
            if (page.Theme != newTheme)
            {
                page.Theme = newTheme;
                var save = await _dbContext.SaveChangesAsync();

                return (save > 0)
                    ? ActionResultService.Results.Update
                    : ActionResultService.Results.ServerError;
            }

            return ActionResultService.Results.Update;
        }

        return ActionResultService.Results.NotFound;
    }

    public async Task<ActionResultService.Results> UpdatePassword(string title, string pin, string oldPassword, string newPassword)
    {
        var pageValid = await IsPageValid(title, pin, oldPassword);

        if (pageValid)
        {
            var page = await _dbContext.Pages
                .FirstOrDefaultAsync(p => (p.Title == title) && (p.Pin == pin));

            if (page != null)
            {
                page.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                var save = await _dbContext.SaveChangesAsync();

                return (save > 0)
                    ? ActionResultService.Results.Update
                    : ActionResultService.Results.ServerError;
            }

            return ActionResultService.Results.NotFound;
        }

        return ActionResultService.Results.Unauthorized;
    }


    //Deletes
    public async Task<ActionResultService.Results> DeletePage(string title, string pin)
    {
        var page = await _dbContext.Pages
            .FirstOrDefaultAsync(p => (p.Title == title) && (p.Pin == pin));

        if (page != null)
        {
            _dbContext.Remove(page);
            var save = await _dbContext.SaveChangesAsync();

            return (save > 0)
                ? ActionResultService.Results.Delete
                : ActionResultService.Results.ServerError;
        }
        return ActionResultService.Results.NotFound;
    }
}
