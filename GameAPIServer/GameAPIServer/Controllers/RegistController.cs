using GameAPIServer.DatabaseServices.GameDb;
using GameAPIServer.Filter;
using GameAPIServer.ReqResModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GameAPIServer.DatabaseServices.GameDb.Models;

namespace GameAPIServer.Controllers;

[Route("[controller]")]
[ApiController]
[EmailFormatCheckFilter]
[NicknameFormatCheckFilter]
[PasswordFormatCheckFilter]
public class RegistController : ControllerBase
{
    readonly ILogger<RegistController> _logger;
    readonly IGameDbService _gameDbService;

    public RegistController(ILogger<RegistController> logger, IGameDbService gameDbService)
    {
        _logger = logger;
        _gameDbService = gameDbService;
    }

    [HttpPost]
    public async Task<RegistResponse> Regist(RegisteRequest request)
    {
        var response = new RegistResponse();
        await _gameDbService.InsertUserAccount( new UserAccount { 
            email = request.email,
            nickname = request.nickname,
            salt = new byte[0],
            hashed_password = new byte[0]
        });


        return response;
    }
}
