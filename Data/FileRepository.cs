using fw_secure_notes_api.Dtos;
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
    public async Task<FileModelDto?> GetFile(string title, string pin, int fileIndex)
    {
        var fileList = await _dbContext.Files
            .Where(f => (f.Page.Title == title) && (f.Page.Pin == pin))
            .OrderBy(f => f.Title)
            .Select(f => new FileModelDto
            {
                Title = f.Title,
                Content = f.Content
            })
            .ToListAsync();

        if ((fileList.Count > 0) && (fileIndex >= 0) && (fileList.Count > fileIndex))
        {
            var fileObject = fileList[fileIndex];
            return fileObject;
        }

        return null;
    }

    public async Task<int> GetFileId(string title, string pin, int fileIndex)
    {
        var fileList = await _dbContext.Files
            .Where(f => (f.Page.Title == title) && (f.Page.Pin == pin))
            .OrderBy(f => f.Title).Select(f => f.Id).ToListAsync();

        if ((fileList.Count > 0) && (fileIndex >= 0) && (fileList.Count > fileIndex))
        {
            var fileObject = fileList[fileIndex];
            return fileObject;
        }

        return -1;
    }


    //Posts
    public async Task<ActionResultService.Results> CreateFile(string title, string pin, string newFileTitle)
    {
        var page = await _dbContext.Pages
            .FirstOrDefaultAsync(p => (p.Title == title) && (p.Pin == pin));
            
        if (page != null)
        {
            FileModel newFile = new(newFileTitle, page.Id, page);
            _dbContext.Files.Add(newFile);

            var save = await _dbContext.SaveChangesAsync();

            return (save > 0)
                ? ActionResultService.Results.Created
                : ActionResultService.Results.ServerError;
        }

        return ActionResultService.Results.NotFound;
    }


    //Puts
    public async Task<ActionResultService.Results> UpdateFileTitle(string title, string pin, int fileIndex, string newTitle)
    {
        var fileId = await GetFileId(title, pin, fileIndex);

        if (fileId != -1)
        {
            var file = await _dbContext.Files
                .FirstOrDefaultAsync(f => f.Id == fileId);

            file!.Title = newTitle;

            var save = await _dbContext.SaveChangesAsync();

            return (save > 0)
                ? ActionResultService.Results.Update
                : ActionResultService.Results.ServerError;
        }

        return ActionResultService.Results.NotFound;
    }

    public async Task<ActionResultService.Results> UpdateFileContent(string title, string pin, int fileIndex, Dictionary<int, string?> updateContent)
    {
        var fileId = await GetFileId(title, pin, fileIndex);

        if ((fileId != -1))
        {
            var file = await _dbContext.Files
           .FirstOrDefaultAsync(f => f.Id == fileId);

            int indexDeleteCount = 0;

            foreach (var newLineContent in updateContent)
            {
                if (file?.Content.ElementAtOrDefault(newLineContent.Key - indexDeleteCount) == null)
                {
                    if (newLineContent.Value != null)
                    {
                        file?.Content.Add(newLineContent.Value);
                    }

                    continue;
                }

                if (newLineContent.Value == null)
                {
                    var list = file!.Content.ToList();
                    list.RemoveAt(newLineContent.Key - indexDeleteCount);

                    file!.Content = [.. list];
                    indexDeleteCount++;
                    continue;
                }

                file!.Content[newLineContent.Key - indexDeleteCount] = newLineContent.Value;
            }

            var save = await _dbContext.SaveChangesAsync();

            return (save > 0)
                ? ActionResultService.Results.Update
                : ActionResultService.Results.ServerError;
        }

        return ActionResultService.Results.NotFound;
    }


    //DELETE file
    public async Task<ActionResultService.Results> DeleteFile(string title, string pin, int fileIndex)
    {
        var fileId = await GetFileId(title, pin, fileIndex);

        var file = await _dbContext.Files
                .FirstOrDefaultAsync(f => f.Id == fileId);

        if ((fileId != -1) && (file != null))
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
