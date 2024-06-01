using fw_secure_notes_api.Data;

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
            string pin   = "";
            var pages    = await _page.GetPageListWithThisTitle(title);
            var pagePins = pages.Select(p => p.Pin).ToList();
            int quant    = pages.Count;

            for (int a = 0; true; a++)
            {
                pin = "";

                for (int c = 1; c <= 3; c++)
                    pin = string.Concat(pin, caracters[(((quant+c+a)*c))%caracters.Length]);
                
                if (!pagePins.Contains(pin))
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
