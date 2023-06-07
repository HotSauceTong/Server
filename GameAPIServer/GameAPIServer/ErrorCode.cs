namespace GameAPIServer
{
    public enum ErrorCode : Int16
    {
        None = 0,
        // Auth Error 10 ~ 49
        InvalidToken = 10,
        InvalidEmail = 11,
        // Db Error 50 ~ 99
        SessionError = 50,
        GameDbError = 51,
        MasterDataError = 52,

        // request format Error 100 ~ 199
        InvalidRequestFormat = 100,
        InvalidEmailFormat = 101,
        InvalidPasswordFormat = 102,
        InvalidClientVersion = 103, //  TODO: 버전검사 위치를 다른 영역으로 옮기기
        InvalidNicknameFormat = 104,

        // Regist Error 200 ~ 299
        EmailAlreadyExist = 201,
        NicknameAlreadyExist = 202,
         
        //Login Error 300 ~ 399
        NotExistEmail = 301,
        WrongPassword = 302,
        ExpiredSession = 303,

        //Mail Error 400 ~ 499
        NotExistMail = 401,
        ExpiredEmail = 402,
        
        //Attendance Error 500 ~ 599
        AlreadyAttendance = 501,
        MaxRewardStackReached = 502,
    }
}
