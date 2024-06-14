using fw_secure_notes_api.Dtos.General;
using Microsoft.AspNetCore.Mvc;

namespace fw_secure_notes_api.Services;

public class ActionResultService
{
    public enum Results
    {
        Get,
        Created,
        Update,
        Delete,
        NoContent,
        NotFound,
        Bad,
        Unauthorized,
        ServerError
    }

    public IActionResult GetAction(Results result)
    {
        return result switch
        {
            Results.Get          => new OkResult(),
            Results.Created      => new CreatedResult(),
            Results.Update       => new OkResult(),
            Results.Delete       => new OkResult(),
            Results.NotFound     => new NotFoundResult(),
            Results.NoContent    => new OkResult(),
            Results.Bad          => new BadRequestResult(),
            Results.Unauthorized => new UnauthorizedResult(),
            Results.ServerError  => new StatusCodeResult(500),
            _ => new StatusCodeResult(500),
        };
    }
    public IActionResult GetAction(Results result, string? message = null, object? content = null)
    {
        return result switch
        {
            Results.Get          => new OkObjectResult(ResponseObject(message, 200, content)),
            Results.Created      => new CreatedResult("", ResponseObject(message, 201, content)),
            Results.Update       => new OkObjectResult(ResponseObject(message, 200, content)),
            Results.Delete       => new OkObjectResult(ResponseObject(message, 200, content)),
            Results.NoContent    => new OkObjectResult(ResponseObject(message, 200)),
            Results.NotFound     => new NotFoundObjectResult(ResponseObject(message, 404)),
            Results.Bad          => new BadRequestObjectResult(ResponseObject(message, 400)),
            Results.Unauthorized => new UnauthorizedObjectResult(ResponseObject(message, 401)),
            Results.ServerError  => new ObjectResult(ResponseObject(message, 500)) { StatusCode = 500 },
            _ => new ObjectResult(ResponseObject(message, 500)) { StatusCode = 500 },
        };
    }

    public IActionResult GetActionAuto(Results result, string featur = "", object? content = null)
    {
        if (featur != "")
            featur = $" '{featur}'";

        return result switch
        {
            Results.Get          => new OkObjectResult(ResponseObject($"O Recurso{featur} foi acessado com sucesso.", 200, content)),
            Results.Created      => new CreatedResult("", ResponseObject($"O Recurso{featur} foi criado com sucesso.", 201, content)),
            Results.Update       => new OkObjectResult(ResponseObject($"O Recurso{featur} foi atualizado com sucesso!", 200, content)),
            Results.Delete       => new OkObjectResult(ResponseObject($"O Recurso{featur} foi excluído com sucesso!", 200)),
            Results.NoContent    => new OkObjectResult(ResponseObject($"O Recurso{featur} foi acessado, mas está vazio...", 200)),
            Results.NotFound     => new NotFoundObjectResult(ResponseObject($"O Recurso{featur} não foi encontrado...", 404)),
            Results.Bad          => new BadRequestObjectResult(ResponseObject
                ($"Falha ao acessar o recurso{featur}.\n", 400, "É possível que sua requisição não esteja no formato certo...")), 
            Results.Unauthorized => new UnauthorizedObjectResult(ResponseObject
                ($"Você não tem autorização para acessar o recurso{featur}!", 401)),
            Results.ServerError  => new ObjectResult(ResponseObject("Erro interno do servidor!", 500)) { StatusCode = 500 },
            _ => new ObjectResult(ResponseObject("Um erro inesperado ocorreu no servidor...", 500)) { StatusCode = 500 }
        };
    }

    private static ResultDto ResponseObject(string? message = null, int code = 200, object? content = null)
    {
        return new() { Message = message, StatusCode = code, Content = content };
    }
}
