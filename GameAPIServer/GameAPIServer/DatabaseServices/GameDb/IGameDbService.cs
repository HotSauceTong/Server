using GameAPIServer.Controllers.ReqResModels;
using GameAPIServer.DatabaseServices.GameDb.Models;

namespace GameAPIServer.DatabaseServices.GameDb;

public interface IGameDbService
{
    Task<(ErrorCode, Int64 key)> InsertUserAccount(UserAccount userAccount);
    Task<(ErrorCode, UserAccount? account)> GetUserAccount(string email);
    Task<(ErrorCode, Int64 key)> InsertUserAttendance(UserAttendance userAttendence);
    Task<(ErrorCode, UserAttendance? attendance)> GetUesrAttendence(Int64 userId);
    Task<ErrorCode> UpdateUserAttendance(Int64 userId, UserAttendance userAttendence);
    Task<ErrorCode> DeleteUserAccount(Int64 userId);
    Task<(ErrorCode, Int64 mailId)> SendMailToUser(Int64 userId, MailDbModel mail);
    Task<(ErrorCode, List<MailDbModel>?)> GetUserMails(Int64 userId);
    Task<(ErrorCode, MailDbModel?)> GetUserMail(Int64 userId, Int64 mailId);
    Task<ErrorCode> ReadUserMails(Int64 userId, Int64 mailId, DateTime dateTime);
    Task<ErrorCode> UpdateUserMailCollection(Int64 userId, Int64 mailId, CollectionBundle? collections);
    Task<ErrorCode> GiveCollectionsToUser(Int64 userId, CollectionBundle collections);
    Task<ErrorCode> DeleteAllRecvedMails(Int64 userId);
    Task<(ErrorCode, List<CollectionBundle>?)> GetAllUserCollectionList(Int64 userId);
    Task<(ErrorCode, List<CollectionBundle>?)> GetUserCurrencyList(Int64 userId);
    Task<(ErrorCode, List<CollectionBundle>?)> GetUserCardList(Int64 userId);
}
