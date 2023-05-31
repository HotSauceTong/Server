namespace GameAPIServer.DatabaseServices.SessionDb;

public interface ISessionDbService
{
    Task<ErrorCode> SetSession(SessionModel session);
    Task<(ErrorCode, SessionModel?)> GetSession(String email);
    Task<ErrorCode> DeleteSession(String email);
}
