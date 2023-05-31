namespace GameAPIServer.DatabaseServices.SessionDb;
using CloudStructures;
using CloudStructures.Structures;
using ZLogger;

public class RedisSessionDbService : ISessionDbService
{
    readonly ILogger<RedisSessionDbService> _logger;
    readonly RedisConnection _redisConnection;

    public RedisSessionDbService(ILogger<RedisSessionDbService> logger, IConfiguration config)
    {
        _logger = logger;
        var _connectionString = config.GetConnectionString("Redis_Session");
        RedisConfig redisConfig = new CloudStructures.RedisConfig("test", _connectionString);
        _redisConnection = new RedisConnection(redisConfig);
    }
    public async Task<ErrorCode> SetSession(SessionModel session)
    {
        try
        {
            var key = GenerateSessionKey(session.email);
            var redisString = new RedisString<SessionModel>(_redisConnection, key, TimeSpan.FromHours(1));
            if (await redisString.SetAsync(session, TimeSpan.FromHours(1)) == true)
            {
                return ErrorCode.None;
            }
            else
            {
                return ErrorCode.SessionError;
            }
        }
        catch (Exception ex)
        {
            _logger.ZLogErrorWithPayload(ex, new { Session = session }, "SetSession EXCEPTION"); }
        return ErrorCode.SessionError;
    }

    public async Task<ErrorCode> DeleteSession(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<(ErrorCode, SessionModel?)> GetSession(string email)
    {
        try
        {
            var key = GenerateSessionKey(email);
            var redisString = new RedisString<SessionModel>(_redisConnection, key, null);
            var redisResult = await redisString.GetAsync();
            if (redisResult.HasValue == false)
            {
                return (ErrorCode.ExpiredSession, null);
            }
            return (ErrorCode.None, redisResult.Value);
        }
        catch (Exception ex)
        {
            _logger.ZLogErrorWithPayload(ex, new { Email = email }, "GetSession EXCEPTION");
            return (ErrorCode.SessionError, null);
        }
    }

    String GenerateSessionKey(String email)
    {
        return $"{email}Session";
    }
}

