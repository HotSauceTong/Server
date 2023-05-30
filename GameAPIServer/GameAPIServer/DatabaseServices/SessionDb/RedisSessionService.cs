namespace GameAPIServer.DatabaseServices.SessionDb;
using CloudStructures;
using CloudStructures.Structures;

public class RedisSessionService : ISessionService
{
    readonly ILogger<RedisSessionService> _logger;
    readonly RedisConnection _redisConnection;

    RedisSessionService(ILogger<RedisSessionService> logger, IConfiguration config)
    {
        _logger = logger;
        var _connectionString = config.GetConnectionString("Redis_Session");
        RedisConfig redisConfig = new CloudStructures.RedisConfig("test", _connectionString);
        _redisConnection = new RedisConnection(redisConfig);
    }
    public async Task<ErrorCode> SetSession(SessionModel session)
    {
        //try
        //{
        //    var key = GenerateSessionKey(session.email);
        //    var redisString = new RedisString<SessionModel>(_redisConnection, key, TimeSpan.FromHours(1));
        //    if (await redisString.SetAsync(session, TimeSpan.FromHours(1)) == true)
        //    {
        //        return ErrorCode.None;
        //    }
        //    else
        //    {
        //        return ErrorCode.SessionError;
        //    }
        //}
        throw new NotImplementedException();
    }

    public Task<ErrorCode> DeleteSession(string email)
    {
        throw new NotImplementedException();
    }

    public Task<(ErrorCode, SessionModel?)> GetSession(string email)
    {
        throw new NotImplementedException();
    }


    String GenerateSessionKey(String email)
    {
        return $"{email}Session";
    }
}
