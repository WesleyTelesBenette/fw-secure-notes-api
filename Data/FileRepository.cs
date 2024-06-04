using fw_secure_notes_api.Models;
using Microsoft.EntityFrameworkCore;

namespace fw_secure_notes_api.Data;

public class FileRepository 
{
    private readonly DatabaseContext _dbContext;

    public FileRepository(DatabaseContext dbContext)
        { _dbContext = dbContext; }

    //Gets
    public async Task<FileModel?> GetFile(string title, string pin, int fileIndex)
    {
        var fileList = await _dbContext.Files
            .Where(f => (f.Page.Title == title) && (f.Page.Pin == pin)).ToListAsync();

        var file = fileList?[fileIndex] ?? null;

        return file;
    }

    public async Task<string[]?> GetFileContent(string title, string pin, int fileIndex)
    {
        var file = await GetFile(title, pin, fileIndex);

        return (file != null)
            ? file.Content
            : [];
    }


    //Posts
    public async Task<bool> CreateFile(string title, string pin, string newFileTitle)
    {
        try
        {
            var page = await _dbContext.Pages
                .FirstOrDefaultAsync(p => (p.Title == title) && (p.Pin == pin));
            
            if (page != null)
            {
                FileModel newFile = new(newFileTitle, page.Id, page);
                _dbContext.Files.Add(newFile);

                var save = await _dbContext.SaveChangesAsync();

                return ((save) > 0);
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    //Puts
    public async Task<bool> UpdateFileTitle(string title, string pin, int fileIndex, string newTitle)
    {
        try
        {
            var file = await GetFile(title, pin, fileIndex);

            if (file != null)
            {
                file.Title = newTitle;
                return ((await _dbContext.SaveChangesAsync()) > 0);
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateFileContent(string title, string pin, int fileIndex, Dictionary<int, string> updateContent)
    {
        try
        {
            var file = await GetFile(title, pin, fileIndex);
            
            if (file != null)
            {
                foreach (var newLineContent in updateContent)
                {
                    if (updateContent.Count >= newLineContent.Key)
                        file.Content[newLineContent.Key] = newLineContent.Value;
                }

                var save = await _dbContext.SaveChangesAsync();

                return (save > 0);
            }

            return false;
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message, "Erro ao atualizar conteúdo do arquivo...");
            return false;
        }
    }

    //DELETE file
    public async Task<bool> DeleteFile(string title, string pin, int fileIndex)
    {
        try
        {
            var file = await GetFile(title, pin, fileIndex);
            var page = await _dbContext.Pages.FirstOrDefaultAsync(p => (p.Title == title) && (p.Pin == pin));

            if ((file != null) && (page != null))
            {
                _dbContext.Files.Remove(file);
                page.Files.Remove(file);

                return ((await _dbContext.SaveChangesAsync()) > 0);
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}
