using fw_secure_notes_api.Data;
using System.Text.RegularExpressions;

namespace fw_secure_notes_api.Services;

public class GeneratePinService
{
    private readonly PageRepository _page;
    private readonly string caracters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-";

    public GeneratePinService(PageRepository page)
    {
        _page = page;
    }

    public async Task<string> Generate(string title)
    {
        try
        {
            var random = new Random();
            string pin   = "";
            var pagePins = await _page.GetPageListWithThisTitle(title);
            int quant    = pagePins.Count;

            for (int a = 0; true; a++)
            {
                pin = "";

                for (int c = 1; c <= 3; c++)
                    pin += caracters[random.Next(caracters.Length)];
                
                if ((Regex.IsMatch(pin, @"^[a-zA-Z0-9\-]+$")) && (!pagePins.Contains(pin)))
                    break;
            }
            
            return pin;
        }
        catch
        {
            //Invalid format
            return "####";
        }
    }
}
