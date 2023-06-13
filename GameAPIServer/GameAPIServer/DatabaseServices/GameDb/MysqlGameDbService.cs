namespace GameAPIServer.DatabaseServices.GameDb;

using GameAPIServer.Controllers.ReqResModels;
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
                .FirstOrDefaultAsync<UserAccount>();
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
                attendences_stack = userAttendence.attendences_stack,
                last_login_date = userAttendence.last_login_date,
                reward_version = userAttendence.reward_version
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
                .FirstOrDefaultAsync<UserAttendance>();
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
                    attendences_stack = userAttendence.attendences_stack,
                    last_login_date = userAttendence.last_login_date,
                    reward_version = userAttendence.reward_version
                });
            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            _logger.ZLogErrorWithPayload(ex, new { userId = userId }, "UpdateUserAttendance EXCEPTION");
            return ErrorCode.GameDbError;
        }
    }

    public async Task<(ErrorCode, Int64 mailId)> SendMailToUser(Int64 userId, MailDbModel mail)
    {
        try
        {
            var mailId = await _queryFactory.Query("mailbox")
                .InsertGetIdAsync<Int64>(mail);
            return (ErrorCode.None, mailId);
        }
        catch (Exception ex)
        {
            _logger.ZLogErrorWithPayload(ex, new { userId = userId }, "SendMailToUser EXCEPTION");
            return (ErrorCode.GameDbError, -1);
        }
    }

    public async Task<(ErrorCode, List<MailDbModel>?)> GetUserMails(Int64 userId)
    {
        try
        {
            var mailLsit = await _queryFactory.Query("mailbox")
                .Where("user_id", userId)
                .Where("expiration_date", ">", DateTime.Now)
                .Where("is_deleted", false)
                .GetAsync<MailDbModel>();
            if (mailLsit == null)
            {
                return (ErrorCode.GameDbError, null);
            }
            return (ErrorCode.None, mailLsit.ToList());
        }
        catch (Exception ex)
        {
            _logger.ZLogErrorWithPayload(ex, new { userId = userId }, "GetUserMails EXCEPTION");
            return (ErrorCode.GameDbError, null);
        }
    }

    public async Task<(ErrorCode, MailDbModel?)> GetUserMail(Int64 userId, Int64 mailId)
    {
        try
        {
            var mail = await _queryFactory.Query("mailbox")
                .Where("user_id", userId)
                .Where("mail_id", mailId)
                .Where("is_deleted", false)
                .FirstOrDefaultAsync<MailDbModel>();
            if (mail == null)
            {
                return (ErrorCode.NotExistEmail, null);
            }
            else if (mail.expiration_date < DateTime.Now)
            {
                return (ErrorCode.ExpiredMail, null);
            }
            return (ErrorCode.None, mail);
        }
        catch (Exception ex)
        {
            _logger.ZLogErrorWithPayload(ex, new { userId = userId, mailId = mailId }, "GetUserMails EXCEPTION");
            return (ErrorCode.GameDbError, null);
        }
    }
    public async Task<ErrorCode> ReadUserMails(Int64 userId, Int64 mailId, DateTime dateTime)
    {
        try
        {
            await _queryFactory.Query("mailbox")
                .Where("user_id", userId)
                .Where("mail_id", mailId)
                .Where("is_deleted", false)
                .UpdateAsync(new
                {
                    read_date = dateTime
                });
            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            _logger.ZLogErrorWithPayload(ex, new { userId = userId, mailId = mailId }, "GetUserMails EXCEPTION");
            return ErrorCode.GameDbError;
        }
    }

    public async Task<ErrorCode> UpdateUserMailCollection(Int64 userId, Int64 mailId, CollectionBundle? collection)
    {
        try
        {
            Int64 collectionCode = collection == null ? -1 : collection.collectionCode;
            Int32 collectionCount = collection == null ? -1 : collection.collectionCount;
            await _queryFactory.Query("mailbox")
                .Where("user_id", userId)
                .Where("mail_id", mailId)
                .Where("is_deleted", false)
                .UpdateAsync(new
                {
                    collection_code = collectionCode,
                    collection_count = collectionCount
                });
            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            _logger.ZLogErrorWithPayload(ex, new { userId = userId, mailId = mailId }, "UpdateUserMailCollection EXCEPTION");
            return ErrorCode.GameDbError;
        }
    }

    public async Task<ErrorCode> GiveCollectionsToUser(Int64 userId, CollectionBundle collections)
    {
        try
        {
            var affectedRowNum = await _queryFactory.StatementAsync(CollectionCountAccumulate("user_collections", userId, collections));
            // 쿼리가 없는 경우
            if (affectedRowNum == 0)
            {
                await _queryFactory.Query("user_collections")
                    .InsertAsync( new {
                        user_id = userId,
                        collection_code = collections.collectionCode,
                        collection_count = collections.collectionCount
                    });
            }
            return ErrorCode.None;
        }
        catch (MySqlException ex)
        {
            // 오버플로우인 경우 : number = 1264
            if (ex.Number == 1264)
            {
                _logger.ZLogWarningWithPayload(ex, new { userId = userId, collections = collections }, "GiveCollectionsToUser EXCEPTION");
                return ErrorCode.OverflowCollectionCount;
            }
            _logger.ZLogErrorWithPayload(ex, new { userId = userId, collections = collections }, "GiveCollectionsToUser EXCEPTION");
            return ErrorCode.GameDbError;
        }
        catch (Exception ex)
        {
            _logger.ZLogErrorWithPayload(ex, new { userId = userId, collections = collections }, "GiveCollectionsToUser EXCEPTION");
            return ErrorCode.GameDbError;
        }
    }

    public async Task<ErrorCode> DeleteAllRecvedMails(Int64 userId)
    {
        try
        {
            await _queryFactory.Query("mailbox")
                .Where("user_id", userId)
                .Where(q => q.Where("collection_code", -1).OrWhere("collection_count", -1))
                .UpdateAsync(new { is_deleted = true });
            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            
            _logger.ZLogErrorWithPayload(ex, new { userId = userId }, "DeleteAllRecvedMails EXCEPTION");
            return ErrorCode.GameDbError;
        }
    }

    public async Task<(ErrorCode, List<CollectionBundle>?)> GetAllUserCollectionList(Int64 userId)
    {
        try
        {
            List<CollectionBundle>? collectionBundles = null;
            var collections = await _queryFactory.Query("user_collections")
                .Where("user_id", userId)
                .GetAsync<UserCollection>();
            if (collections.Count() != 0)
            {
                collectionBundles = new List<CollectionBundle>();
            }
            foreach (var collecElem in collections)
            {
                collectionBundles.Add(collecElem.GetCollectionBundle());
            }
            return (ErrorCode.None, collectionBundles);
        }
        catch (Exception ex)
        {
            _logger.ZLogErrorWithPayload(ex, new { userId = userId }, "GetAllUserCollectionList EXCEPTION");
            return (ErrorCode.GameDbError, null);
        }
    }

    public async Task<(ErrorCode, List<CollectionBundle>?)> GetUserCurrencyList(Int64 userId)
    {
        try
        {
            List<CollectionBundle>? currencyBundles = null;
            var collections = await _queryFactory.Query("user_collections")
                .Where("user_id", userId)
                .Where(q => q.Where("collection_code", 0).OrWhere("collection_code", 1))
                .GetAsync<UserCollection>();
            if (collections.Count() != 0)
            {
                currencyBundles = new List<CollectionBundle>();
            }
            foreach (var collecElem in collections)
            {
                currencyBundles.Add(collecElem.GetCollectionBundle());
            }
            return (ErrorCode.None, currencyBundles);
        }
        catch (Exception ex)
        {
            _logger.ZLogErrorWithPayload(ex, new { userId = userId }, "GetUserCurrencyList EXCEPTION");
            return (ErrorCode.GameDbError, null);
        }
    }

    public async Task<(ErrorCode, List<CollectionBundle>?)> GetUserCardList(Int64 userId)
    {
        try
        {
            List<CollectionBundle>? cardBundles = null;
            var collections = await _queryFactory.Query("user_collections")
                .Where("user_id", userId)
                .WhereNot(q => q.Where("collection_code", 0).OrWhere("collection_code", 1))
                .GetAsync<UserCollection>();
            if (collections.Count() != 0)
            {
                cardBundles = new List<CollectionBundle>();
            }
            foreach (var collecElem in collections)
            {
                cardBundles.Add(collecElem.GetCollectionBundle());
            }
            return (ErrorCode.None, cardBundles);
        }
        catch (Exception ex)
        {
            _logger.ZLogErrorWithPayload(ex, new { userId = userId }, "GetUserCardList EXCEPTION");
            return (ErrorCode.GameDbError, null);
        }
    }

    String CollectionCountAccumulate(String tableName, Int64 userId, CollectionBundle collections)
    {
        //String Query = "INSERT INTO " + tableName + " (user_id, collection_code, collection_count) VALUES ";
        //for (int i = 0; i < collections.Count; i++)
        //{
        //    Query += "(" + userId + ", " + collections[i].collectionCode + ", " + collections[i].collectionCount + ")";
        //    if (i != collections.Count - 1)
        //    {
        //        Query += ", ";
        //    }
        //}
        String Query = "UPDATE " + tableName 
            + " SET collection_count = collection_count + " + collections.collectionCount.ToString() 
            + " WHERE user_id = " + userId.ToString() 
            + " AND collection_code = " + collections.collectionCode.ToString();
        return Query;
    }
}
