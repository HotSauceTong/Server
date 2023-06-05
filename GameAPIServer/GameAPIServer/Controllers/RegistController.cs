using GameAPIServer.DatabaseServices.GameDb;
using GameAPIServer.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GameAPIServer.DatabaseServices.GameDb.Models;
using GameAPIServer.Utils;
using GameAPIServer.Controllers.ReqResModels;

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

        var (salt, hashedPassword) = Security.GetSaltAndHashedPassword(request.password);
        (response.errorCode, var userAccountKey) = await _gameDbService.InsertUserAccount( new UserAccount { 
            email = request.email,
            nickname = request.nickname,
            salt = salt,
            hashed_password = hashedPassword
        });
        if (response.errorCode != ErrorCode.None)
        {
            return response;
        }
        (response.errorCode, var userAttendenceKey) = await _gameDbService.InsertUserAttendance(new UserAttendance
        {
            user_id = userAccountKey,
            attendences_stack = 0,
            last_login_date = new DateTime(1000, 1, 1, 0, 0, 0),
            reward_version = ""
        });
        // TODO: 기능 추가에 따른, 기본데이터 추가 로직 실시.
        return response;
    }
}
