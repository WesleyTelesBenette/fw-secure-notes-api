using fw_secure_notes_api.Dtos.File;
using fw_secure_notes_api.Models;
using fw_secure_notes_api.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace fw_secure_notes_api.Data;

public class FileRepository 
{
    private readonly DatabaseContext _dbContext;

    public FileRepository(DatabaseContext dbContext)
        { _dbContext = dbContext; }

    //Gets
    public async Task<FileModelDto?> GetFile(string title, string pin, ushort fileId)
    {
        var file = await _dbContext.Files
            .FirstOrDefaultAsync(f => (f.Id == fileId) && (f.Page.Title == title) && (f.Page.Pin == pin));

        if (file != null)
        {
            FileModelDto fileReturn = new()
            {
                Id = file.Id,
                Title = file.Title,
                Content = file.Content
            };

            return fileReturn;
        }

        return null;
    }

    //Posts
    public async Task<FileModelDto?> CreateFile(string title, string pin, string newFileTitle)
    {
        var page = await _dbContext.Pages
            .FirstOrDefaultAsync(p => (p.Title == title) && (p.Pin == pin));
            
        if (page != null)
        {
            FileModel newFile = new(newFileTitle, page.Id, page);
            newFile.Content.Add("");
            var fileCreated = _dbContext.Files.Add(newFile);

            var save = await _dbContext.SaveChangesAsync();

            if ((save > 0) && (fileCreated != null))
            {
                var fileModel = fileCreated.Entity;
                FileModelDto fileReturn = new()
                {
                    Id = fileModel.Id,
                    Title = fileModel.Title,
                    Content = fileModel.Content
                };

                return fileReturn;
            }
        }

        return null;
    }

    //Puts
    public async Task<ActionResultService.Results> UpdateFileTitle(string title, string pin, ushort fileId, string newTitle)
    {
        var file = await _dbContext.Files
            .FirstOrDefaultAsync(f => (f.Id == fileId) && (f.Page.Title == title) && (f.Page.Pin == pin));

        if (file != null)
        {
            if (file.Title != newTitle)
            {
                file.Title = newTitle;
                var save = await _dbContext.SaveChangesAsync();

                return (save > 0)
                    ? ActionResultService.Results.Update
                    : ActionResultService.Results.ServerError;
            }

            return ActionResultService.Results.Update;
        }

        return ActionResultService.Results.NotFound;
    }

    public async Task<ActionResultService.Results> UpdateFileAddLine(string title, string pin, ushort fileId, UpdateFileContentDto update)
    {
        var file = await _dbContext.Files
             .FirstOrDefaultAsync(f => (f.Id == fileId) && (f.Page.Title == title) && (f.Page.Pin == pin));

        if (file != null)
        {
            if (file.Content.Count == update.Index)
            {
                file.Content.Add(update.Content ?? "");
            }
            else
            {
                List<string> newFileContent = [];
                var contentFile = file.Content;

                newFileContent.AddRange(contentFile.Take(update.Index));
                newFileContent.Add(update.Content ?? "");
                newFileContent.AddRange(contentFile.TakeLast(contentFile.Count - (update.Index)));

                file.Content = newFileContent;
            }

            var save = await _dbContext.SaveChangesAsync();

            return ActionResultService.Results.Update;
        }

        return ActionResultService.Results.NotFound;
    }   

    public async Task<ActionResultService.Results> UpdateFileUpdateContent(string title, string pin, ushort fileId, UpdateFileContentDto update)
    {
        var file = await _dbContext.Files
             .FirstOrDefaultAsync(f => (f.Id == fileId) && (f.Page.Title == title) && (f.Page.Pin == pin));

        if (file != null)
        {
            if (file.Content.Count >= update.Index)
            {
                file.Content[update.Index] = update.Content ?? "";
            }
            else file.Content.Add(update.Content ?? "");

            var save = await _dbContext.SaveChangesAsync();

            return (save > 0)
                ? ActionResultService.Results.Update
                : ActionResultService.Results.ServerError;
        }

        return ActionResultService.Results.NotFound;
    }

    public async Task<ActionResultService.Results> UpdateFileRemoveLine(string title, string pin, ushort fileId, UpdateFileContentDto update)
    {
        var file = await _dbContext.Files
             .FirstOrDefaultAsync(f => (f.Id == fileId) && (f.Page.Title == title) && (f.Page.Pin == pin));

        if (file != null)
        {
            int save;

            if (file.Content.Count > 1)
            {
                var list = file.Content.ToList();
                list.RemoveAt(update.Index);

                file.Content = [.. list];
                save = await _dbContext.SaveChangesAsync();

                return (save > 0)
                    ? ActionResultService.Results.Update
                    : ActionResultService.Results.ServerError;
            }

            file.Content = [""];
            save = await _dbContext.SaveChangesAsync();

            return ActionResultService.Results.Update;

        }

        return ActionResultService.Results.NotFound;
    }


    //DELETE file
    public async Task<ActionResultService.Results> DeleteFile(string title, string pin, ushort fileId)
    {
        var file = await _dbContext.Files
            .FirstOrDefaultAsync(f => (f.Id == fileId) && (f.Page.Title == title) && (f.Page.Pin == pin));

        if (file != null)
        {
            _dbContext.Files.Remove(file);

            var save = await _dbContext.SaveChangesAsync();

            return (save > 0)
                ? ActionResultService.Results.Delete
                : ActionResultService.Results.ServerError;
        }

        return ActionResultService.Results.NotFound;
    }
}
