namespace GameAPIServer.DatabaseServices.GameDb;

using GameAPIServer.DatabaseServices.GameDb.Models;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Reflection;
using ZLogger;

public class MysqlGameDbService : IGameDbService
{
    readonly ILogger<MysqlGameDbService> _logger;
    readonly QueryFactory _queryFactory;

    public MysqlGameDbService(ILogger<MysqlGameDbService> logger, IConfiguration config)
    {
        _logger = logger;
        var conString = config.GetConnectionString("Mysql_GameDb");
        var connection = new MySqlConnection(conString);
        var compiler = new MySqlCompiler();
        _queryFactory = new QueryFactory(connection, compiler);
    }
    public async Task<(ErrorCode, Int64 key)> InsertUserAccount(UserAccount userAccount)
    {
        try
        {
            var key = await _queryFactory.Query("user_accounts").InsertGetIdAsync<Int64>(new
            {
                email = userAccount.email,
                nickname = userAccount.nickname,
                salt = userAccount.salt,
                hashed_password = userAccount.hashed_password
            });
            return (ErrorCode.None, key);
        }
        catch (MySqlException ex)
        {
            if (ex.Number == 1062) //duplicated id exception
            {
                if (ex.Message.Contains("email"))
                { 
                    return (ErrorCode.EmailAlreadyExist, -1);
                }
                else
                {
                    return (ErrorCode.NicknameAlreadyExist, -1);
                }
            }
            _logger.ZLogErrorWithPayload(ex, new { email = userAccount.email }, "InsertUserAccount MysqlEXCEPTION");
            return (ErrorCode.GameDbError, -1);
        }
        catch (Exception ex)
        {
            _logger.ZLogErrorWithPayload(ex, new { email = userAccount.email }, "InsertUserAccount EXCEPTION");
            return (ErrorCode.GameDbError, -1);
        }
    }

    public async Task<(ErrorCode, UserAccount? account)> GetUserAccount(string email)
    {
        try
        {
            var account = await _queryFactory.Query("user_accounts")
                .Where(new { email = email })
                .FirstAsync<UserAccount>();
            if (account == null)
            {
                return (ErrorCode.NotExistEmail, null);
            }
            return (ErrorCode.None, account);
        }
        catch (Exception ex)
        {
            _logger.ZLogErrorWithPayload(ex, new { email = email }, "GetUserAccount EXCEPTION");
            return (ErrorCode.GameDbError, null);
        }
    }

    // For RollBack
    public async Task<ErrorCode> DeleteUserAccount(Int64 userId)
    {
        try
        {
            await _queryFactory.Query("user_accounts")
                .Where( new { user_id = userId } )
                .DeleteAsync();
            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            _logger.ZLogErrorWithPayload(ex, new { userId = userId }, "DeleteUserAccount EXCEPTION");
            return ErrorCode.GameDbError;
        }
    }

    public async Task<(ErrorCode, Int64 key)> InsertUserAttendance(UserAttendance userAttendence)
    {
        try
        {
            var key = await _queryFactory.Query("user_attendences").InsertGetIdAsync<Int64>(new
            {
                user_id = userAttendence.user_id,
                consecutive_login_count = userAttendence.consecutive_login_count,
                last_login_date = userAttendence.last_login_date
            });
            return (ErrorCode.None, key);
        }
        catch (MySqlException ex)
        {
            if (ex.Number == 1062) //duplicated id exception
            {
                _logger.ZLogErrorWithPayload(ex, new { userId = userAttendence.user_id }, "InsertUserAttendence user_id duplicated");
                return (ErrorCode.GameDbError, -1);
            }
            _logger.ZLogErrorWithPayload(ex, new { userId = userAttendence.user_id }, "InsertUserAttendence MysqlEXCEPTION");
            return (ErrorCode.GameDbError, -1);
        }
        catch (Exception ex)
        {
            _logger.ZLogErrorWithPayload(ex, new { userId = userAttendence.user_id }, "InsertUserAttendence EXCEPTION");
            return (ErrorCode.GameDbError, -1);
        }

    }
    public async Task<(ErrorCode, UserAttendance? attendance)> GetUesrAttendence(Int64 userId)
    {
        try
        {
            var userAttendance = await _queryFactory.Query("user_attendences")
                .Where("user_id", userId)
                .FirstAsync<UserAttendance>();
            if (userAttendance == null)
            {
                return (ErrorCode.GameDbError, null);
            }
            return (ErrorCode.None, userAttendance);
        }
        catch (Exception ex)
        {
            _logger.ZLogErrorWithPayload(ex, new { userId = userId }, "GetUserAttendence EXCEPTION");
            return (ErrorCode.GameDbError, null);
        }
    }

    public async Task<ErrorCode> UpdateUserAttendance(Int64 userId, UserAttendance userAttendence)
    {
        try
        {
            await _queryFactory.Query("user_attendences")
                .Where("user_id", userId)
                .UpdateAsync(new
                {
                    consecutive_login_count = userAttendence.consecutive_login_count,
                    last_login_date = userAttendence.last_login_date
                });
            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            _logger.ZLogErrorWithPayload(ex, new { userId = userId }, "UpdateUserAttendance EXCEPTION");
            return ErrorCode.GameDbError;
        }
    }
}
