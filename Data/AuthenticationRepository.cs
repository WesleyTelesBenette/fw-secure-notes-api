using fw_secure_notes_api.Services;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace fw_secure_notes_api.Data;

public class AuthenticationRepository
{
    private readonly DatabaseContext _dbContext;

    public AuthenticationRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    //Gets
    public async Task<bool> GetPageHasPassword(string title, string pin)
    {
        var page = await _dbContext.Pages
            .FirstOrDefaultAsync(p => (p.Title == title) && (p.Pin == pin));

        return page?.Password != null;
    }


    //Posts
    public async Task<ActionResultService.Results> IsPageValide(string title, string pin, string password)
    {
        var page = await _dbContext.Pages
            .FirstOrDefaultAsync(p => (p.Title == title) && (p.Pin == pin));

        if (page != null)
        {
            var validePassword = BCrypt.Net.BCrypt.Verify(password, page.Password);

            return (validePassword)
                ? ActionResultService.Results.Created
                : ActionResultService.Results.Unauthorized;
        }

        return ActionResultService.Results.NotFound;
    }
}
