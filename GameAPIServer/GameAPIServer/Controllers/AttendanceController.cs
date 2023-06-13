using GameAPIServer.DatabaseServices.GameDb;
using GameAPIServer.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GameAPIServer.DatabaseServices.SessionDb;
using GameAPIServer.DatabaseServices.GameDb.Models;
using GameAPIServer.Controllers.ReqResModels;
using GameAPIServer.Service;

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
        readonly IMasterDataOffer _masterDataOffer;
        public AttendanceController(IMasterDataOffer masterDataOffer, ILogger<AttendanceController> logger, IGameDbService gameDbService)
        {
            _logger = logger;
            _gameDbService = gameDbService;
            _masterDataOffer = masterDataOffer;
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

            UserAttendance oldUserAttendance = userAttendance;
            var attendanceRenewalErrorCode = renewalAttendance(userAttendance);
            response.errorCode = await _gameDbService.UpdateUserAttendance(session.userId, oldUserAttendance);
            if (response.errorCode != ErrorCode.None)
            {
                return response;
            }

            if (attendanceRenewalErrorCode == ErrorCode.None)
            {
                var rewardBundle = _masterDataOffer.GetAttendanceReward(userAttendance.attendences_stack);
                if (rewardBundle == null)
                {
                    response.errorCode = await _gameDbService.UpdateUserAttendance(session.userId, oldUserAttendance);
                    response.errorCode = ErrorCode.MasterDataError;
                    return response;
                }

                (response.errorCode, var mailID) = await _gameDbService.SendMailToUser(session.userId,
                    new MailDbModel {
                        user_id = session.userId,
                        collection_code = rewardBundle.collectionCode,
                        collection_count = rewardBundle.collectionCount,
                        mail_title = $"{userAttendance.attendences_stack}일 차 로그인 보상 입니다.",
                        mail_body = $"{userAttendance.attendences_stack}일 차 로그인 보상!",
                        sender = "SYSTEM",
                        recieve_date = DateTime.Now,
                        expiration_date = DateTime.Now.AddDays(15) // TODO: appsetting같은데 넣기?
                        }
                    );
                if (response.errorCode != ErrorCode.None)
                {
                    response.errorCode = await _gameDbService.UpdateUserAttendance(session.userId, userAttendance);
                    return response;
                }
            }
            response.errorCode = attendanceRenewalErrorCode;
            response.attendanceStack = userAttendance.attendences_stack;
            response.lastLoginDate = oldUserAttendance.last_login_date;
            return response;
        }

        // c#에서 클래스, 배열등을 전달하면 Call by reference로 전달된다.
        ErrorCode renewalAttendance(UserAttendance userAttendance)
        {
            ErrorCode rt = ErrorCode.None;
            // 버전이 바뀐 경우
            if (userAttendance.reward_version.Equals(_masterDataOffer.GetAttendanceRewardVersion()) == false) // TODO: 마스터 데이터 확립시 변경
            {
                userAttendance.attendences_stack = 1;
                userAttendance.reward_version = _masterDataOffer.GetAttendanceRewardVersion();
            }
            // 어제 출석하고 오늘은 출석 안한 경우
            //else if (userAttendance.last_login_date < DateTime.Today.AddHours(6))// for test
            else if (userAttendance.attendences_stack < _masterDataOffer.GetAttendenceMaxCount())
            {
                userAttendance.attendences_stack++;
            }
            else
            {
                userAttendance.attendences_stack = 1; // for test
                //rt = ErrorCode.MaxRewardStackReached;//for test
            }
            //else //for test
            //{
            //    rt = ErrorCode.AlreadyAttendance;
            //}
            userAttendance.last_login_date = DateTime.Now;
            return rt;
        }

        
    }
}
