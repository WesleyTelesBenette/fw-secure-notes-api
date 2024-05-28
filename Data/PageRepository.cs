using fw_secure_notes_api.Models;
using Microsoft.EntityFrameworkCore;

namespace fw_secure_notes_api.Data;

public class PageRepository
{
    private readonly DatabaseContext _dbContext;

    public PageRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

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

        return (page != null) && string.IsNullOrEmpty(page.Password);
    }

    public async Task<ICollection<FileModel>?> GetFileList(string title, string pin)
    {
        var page = await _dbContext.Pages
            .FirstOrDefaultAsync(p => (p.Title == title) && (p.Pin == pin));

        return (page?.Files) ?? null;
    }

    public async Task<ICollection<PageModel>> GetPageListWithThisTitle(string title)
    {
        return (await _dbContext.Pages.Where(p => p.Title == title).ToListAsync());
    }

    public async Task<bool> CreatePage(PageModel newPage)
    {
        try
        {
            await _dbContext.Pages.AddAsync(newPage);
            int page = await _dbContext.SaveChangesAsync();

            return (page > 0);
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateTheme(string title, string pin, ThemePage newTheme)
    {
        try
        {
            var page = await _dbContext.Pages
                .FirstOrDefaultAsync(p => (p.Title == title) && (p.Pin == pin));

            if (page == null)
                return false;

            page.Theme = newTheme;
            await _dbContext.SaveChangesAsync();
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeletePage(string title, string pin, string password)
    {
        try
        {
            var page = await _dbContext.Pages
                .FirstOrDefaultAsync(p =>
                (p.Title == title)
                && (p.Pin == pin)
                && (p.Password == password));

            if (page == null)
                return false;

            _dbContext.Remove(page);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }
}
