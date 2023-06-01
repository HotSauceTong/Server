using GameAPIServer.DatabaseServices.GameDb;
using GameAPIServer.ReqResModels;
using GameAPIServer.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GameAPIServer.DatabaseServices.SessionDb;
using GameAPIServer.DatabaseServices.GameDb.Models;

namespace GameAPIServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [EmailFormatCheckFilter]
    [ServiceFilter(typeof(SessionCheckAndGetFilter))]
    public class AttendanceController : ControllerBase
    {
        readonly ILogger<AttendanceController> _logger;
        readonly IGameDbService _gameDbService;
        public AttendanceController(IHttpContextAccessor httpContextAccessor, ILogger<AttendanceController> logger, IGameDbService gameDbService)
        {
            _logger = logger;
            _gameDbService = gameDbService;
        }

        public async Task<AttendanceResponse> Attendance(AttendanceRequest request) 
        {
            var response = new AttendanceResponse();
            var session = (SessionModel?)HttpContext.Items["Session"];
            (response.errorCode, var userAttendance) = await _gameDbService.GetUesrAttendence(session.userId);
            if (response.errorCode != ErrorCode.None)
            {
                return response;
            }
            if (renewalAttendance(userAttendance) == true)
            {
                response.errorCode = await _gameDbService.UpdateUserAttendance(session.userId, userAttendance);
                if (response.errorCode != ErrorCode.None)
                {
                    return response;
                }
            }
            response.consecutiveLoginCount = userAttendance.consecutive_login_count;
            response.lastLoginDate = userAttendance.last_login_date;
            return response;
        }

        // c#에서 클래스, 배열등을 전달하면 Call by reference로 전달된다.
        bool renewalAttendance(UserAttendance userAttendance)
        {
            // 하루 걸른 경우
            if (userAttendance.last_login_date < DateTime.Today.AddDays(-1).AddHours(6))
            {
                userAttendance.consecutive_login_count = 1;
                userAttendance.last_login_date = DateTime.Now;
            }
            // 어제 출석하고 오늘은 출석 안한 경우
            else if (userAttendance.last_login_date < DateTime.Today.AddHours(6))
            {
                userAttendance.consecutive_login_count++;
                userAttendance.last_login_date = DateTime.Now;
            }
            else
            {
                userAttendance.last_login_date = DateTime.Now;
                return false;
            }
            return true;
        }
    }
}
