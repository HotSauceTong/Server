using GameAPIServer.DatabaseServices.GameDb.Models;

namespace GameAPIServer.DatabaseServices.GameDb;

public interface IGameDbService
{
    Task<ErrorCode> InsertUserAccount(UserAccount userAccount);
    Task<ErrorCode> DeleteUserAccount(Int64 userId);
    Task<ErrorCode> InsertUserAttendence(UserAttendence userAttendence);
}
