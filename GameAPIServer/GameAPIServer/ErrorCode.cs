namespace GameAPIServer
{
    public enum ErrorCode : Int16
    {
        None = 0,

        // Session 50 ~ 99
        SessionError = 50,
        GameDbError = 51,

        // request format 100 ~ 199
        InvalidRequestFormat = 100,
        InvalidEmailFormat = 101,
        InvalidPasswordFormat = 102,
        InvalidClientVersion = 103,

        // Regist Error
        EmailAlreadyExist = 201,
        NicknameAlreadyExist = 202,

    }
}
