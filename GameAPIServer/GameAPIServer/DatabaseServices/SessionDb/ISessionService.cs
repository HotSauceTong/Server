namespace GameAPIServer.DatabaseServices.SessionDb;

public interface ISessionService
{
    Task<ErrorCode> SetSession(SessionModel session);
    Task<(ErrorCode, SessionModel?)> GetSession(String email);
    Task<ErrorCode> DeleteSession(String email);
}
