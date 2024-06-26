﻿using fw_secure_notes_api.Attributes;
using fw_secure_notes_api.Data;
using fw_secure_notes_api.Dtos;
using fw_secure_notes_api.Filters;
using fw_secure_notes_api.Models;
using fw_secure_notes_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fw_secure_notes_api.Controllers;

[ApiController]
[Route("[controller]")]
public class PageController : Controller
{
    private readonly PageRepository _page;
    private readonly GeneratePinService _gnPin;
    private readonly ActionResultService _result;

    public PageController(PageRepository page, GeneratePinService gnPin, ActionResultService result)
    {
        _page = page;
        _gnPin = gnPin;
        _result = result;
    }

    [HttpGet("exist")]
    [ServiceFilter(typeof(ParmatersValidateActionFilter))]
    [AllowAnonymous]
    public async Task<IActionResult> GetPageExist([FromRoute] string title, [FromRoute] string pin)
    {
        try
        {
            var isExist = await _page.IsPageExist(title, pin);

            return (isExist)
                ? _result.GetActionAuto(ActionResultService.Results.Get)
                : _result.GetActionAuto(ActionResultService.Results.NotFound);
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpGet("theme")]
    [ServiceFilter(typeof(ParmatersValidateActionFilter))]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> GetPageTheme([FromRoute] string title, [FromRoute] string pin)
    {
        try
        {
            var theme = await _page.GetPageTheme(title, pin);

            return (theme != null)
                ? _result.GetAction(ActionResultService.Results.Get, content: theme)
                : _result.GetAction(ActionResultService.Results.Get, content: 0);
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpGet("files")]
    [ServiceFilter(typeof(ParmatersValidateActionFilter))]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public IActionResult GetPageFileList([FromRoute] string title, [FromRoute] string pin)
    {
        try
        {
            var fileList = _page.GetFileList(title, pin);

            return (fileList != null)
                ? _result.GetAction(ActionResultService.Results.Get, content: fileList)
                : _result.GetActionAuto(ActionResultService.Results.NoContent, "Page.Files");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpPost]
    [NoParameters]
    [AllowAnonymous]
    public async Task<IActionResult> CreatePage([FromBody] CreatePageDto newPage)
    {
        try
        {
            PageModel newPageModel = new()
            {
                Title = newPage.Title,
                Pin = await _gnPin.Generate(newPage.Title),
                Password = BCrypt.Net.BCrypt.HashPassword(newPage.Password)
            };

            if (string.IsNullOrEmpty(newPageModel.Pin))
                throw new Exception("Falha ao gerar PIN.");

            var page = await _page.CreatePage(newPageModel);

            return _result.GetActionAuto(page, "Page", new { newPage.Title, newPageModel.Pin });
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpPut("theme")]
    [ServiceFilter(typeof(ParmatersValidateActionFilter))]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> ChangePageTheme
        ([FromRoute] string title, [FromRoute] string pin, [FromBody] UpdatePageThemeDto newTheme)
    {
        try
        {
            var result = await _page.UpdateTheme(title, pin, newTheme.Theme);

            return _result.GetActionAuto(result, "Page.Theme");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpPut("password")]
    [ServiceFilter(typeof(ParmatersValidateActionFilter))]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> ChangePagePassword
        ([FromRoute] string title, [FromRoute] string pin, [FromBody] UpdatePagePasswordDto updatePassword)
    {
        try
        {
            var result = await _page.UpdatePassword(title, pin, updatePassword.OldPassword, updatePassword.NewPassword);

            return _result.GetActionAuto(result, "Page.Password");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpDelete]
    [ServiceFilter(typeof(ParmatersValidateActionFilter))]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> DeletePage
        ([FromRoute] string title, [FromRoute] string pin)
    {
        try
        {
            var result = await _page.DeletePage(title, pin);

            return _result.GetActionAuto(result, "Page");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }
}
