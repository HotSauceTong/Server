namespace GameAPIServer.ReqResModels;

public class AttendanceRequest : BaseRequest
{
}

public class AttendanceResponse : BaseResponse
{
    public Int16 attendanceStack { get; set; }
    public DateTime lastLoginDate { get; set; }
}
