using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GameAPIServer.Filter;
using GameAPIServer.ReqResModels;
using GameAPIServer.DatabaseServices.GameDb;
using GameAPIServer.DatabaseServices.SessionDb;
using GameAPIServer.Utils;

namespace GameAPIServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [EmailFormatCheckFilter]
    public class LoginController : ControllerBase
    {
        readonly ILogger<LoginController> _logger;
        readonly IGameDbService _gameDb;
        readonly ISessionDbService _sessionDb;

        public LoginController(ILogger<LoginController> logger, IGameDbService gameDb, ISessionDbService sessionDb)
        {
            _logger = logger;
            _gameDb = gameDb;
            _sessionDb = sessionDb;
        }

        public async Task<LoginResponse> login(LoginRequest request)
        {
            LoginResponse response = new LoginResponse();
            (response.errorCode, var userAccount) = await _gameDb.GetUserAccount(request.email);
            if (response.errorCode != ErrorCode.None || userAccount == null)
            {
                return response;
            }

            if (Security.VerifyHashedPassword(request.password, userAccount.salt, userAccount.hashed_password)
                == false)
            {
                response.errorCode = ErrorCode.WrongPassword;
                return response;
            }

            String token = Security.GenerateToken();
            response.errorCode = await _sessionDb.SetSession(new SessionModel
            {
                userId = userAccount.user_id,
                email = userAccount.email,
                token = token,
            });
            if (response.errorCode != ErrorCode.None)
            {
                return response;
            }

            response.token = token;
            return response;
        }
    }
}
