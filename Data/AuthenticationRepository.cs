using fw_secure_notes_api.Services;
using Microsoft.EntityFrameworkCore;

namespace fw_secure_notes_api.Data;

public class AuthenticationRepository
{
    private readonly DatabaseContext _dbContext;

    public AuthenticationRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    //Gets
    public async Task<ActionResultService.Results> GetPageConfig(string title, string pin)
    {
        var page = await _dbContext.Pages
            .FirstOrDefaultAsync(p => (p.Title == title) && (p.Pin == pin));

        if (page != null)
        {
            return (BCrypt.Net.BCrypt.Verify("", page.Password))
                ? ActionResultService.Results.NoContent
                : ActionResultService.Results.Get;
        }

        return ActionResultService.Results.NotFound;
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
