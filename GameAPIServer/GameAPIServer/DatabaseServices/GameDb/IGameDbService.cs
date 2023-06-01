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
}
