using fw_secure_notes_api.Dtos;
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

        return (page != null) && (!string.IsNullOrEmpty(page.Password));
    }


    //Gets
    public async Task<PageThemeDto?> GetPageTheme(string title, string pin)
    {
        var theme = await _dbContext.Pages
            .FirstOrDefaultAsync(p => (p.Title == title) && (p.Pin == pin));

        return (theme == null)
            ? null
            : new() { ThemeName = theme!.Theme.ToString(), ThemeIndex = theme!.Theme };
    }

    public ICollection<string>? GetFileList(string title, string pin)
    {
        ICollection<string>? fileList = _dbContext.Files
            .Where(f => (f.Page.Title == title) && (f.Page.Pin == pin))
            .OrderBy(f => f.Title).Select(f => f.Title).ToList();

        return fileList;
    }

    public async Task<List<string>> GetPageListWithThisTitle(string title)
    {
        return (await _dbContext.Pages.Where(p => p.Title == title).Select(p => p.Pin).ToListAsync()) ?? [];
    }


    ///Posts
    public async Task<ActionResultService.Results> CreatePage(PageModel newPage)
    {
        await _dbContext.Pages.AddAsync(newPage);
        int save = await _dbContext.SaveChangesAsync();

        return (save > 0)
            ? ActionResultService.Results.Created
            : ActionResultService.Results.ServerError;
    }


    //Puts
    public async Task<ActionResultService.Results> UpdateTheme(string title, string pin, ThemePage newTheme)
    {
        var themeList = Enum.GetValues(typeof(ThemePage)).Cast<ThemePage>().ToList();

        if (!themeList.Contains(newTheme))
        {
            var page = await _dbContext.Pages
                .FirstOrDefaultAsync(p => (p.Title == title) && (p.Pin == pin));

            if (page != null)
            {
                page.Theme = newTheme;
                var save = await _dbContext.SaveChangesAsync();

                return (save > 0)
                    ? ActionResultService.Results.Update
                    : ActionResultService.Results.ServerError;
            }

            return ActionResultService.Results.NotFound;
        }

        return ActionResultService.Results.Bad;
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
