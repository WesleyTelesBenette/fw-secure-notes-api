using Microsoft.EntityFrameworkCore;

namespace fw_secure_notes_api.Data;

public class PageRepository
{
    private readonly DatabaseContext _dbContext;

    public PageRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsPageValid(string title, string pin, string password)
    {
        var page = await _dbContext.Pages
            .FirstOrDefaultAsync(p => (p.Title == title) && (p.Pin == pin));
        
        return (page != null) && BCrypt.Net.BCrypt.Verify(password, page.Password);
    }
}
 