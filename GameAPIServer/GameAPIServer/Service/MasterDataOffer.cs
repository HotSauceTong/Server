using GameAPIServer.Controllers.ReqResModels;
using GameAPIServer.Service.MasterDataModels;
using System.Text.Json;
using ZLogger;

namespace GameAPIServer.Service;

public class MasterDataOffer : IMasterDataOffer
{
    readonly ILogger<MasterDataOffer> _logger;
    readonly String _dirPath = "MasterData/";
    AttendanceRewards? _attendanceRewards;
    Dictionary<Int64, CollectionDefine> _collectionDefinesDic = new Dictionary<long, CollectionDefine>();

    public MasterDataOffer(ILogger<MasterDataOffer> logger)
    {
        _logger = logger;
    }

    public bool LoadMasterDatas()
    {
        return LoadAttendanceMasterData() && LoadCollectionDefines();
    }

    public CollectionBundle? GetAttendanceReward(Int32 stack)
    {
        if (stack < 1)
        {
            return null;
        }
        else if (_attendanceRewards.rewards.Count() < stack - 1)
        {
            return _attendanceRewards.rewards[stack - 1];
        }
        return null;
    }

    public CollectionDefine? GetCollectionDefine(Int64 collectionId)
    {
        if (_collectionDefinesDic.ContainsKey(collectionId))
        {
            return _collectionDefinesDic[collectionId];
        }
        return null;
    }

    bool LoadAttendanceMasterData()
    {
        string jsonContent = System.IO.File.ReadAllText(_dirPath + "AttendanceRewards.json");
        _attendanceRewards = JsonSerializer.Deserialize<AttendanceRewards>(jsonContent);
        if (_attendanceRewards == null)
        {
            _logger.ZLogCriticalWithPayload(new { jsonContent = jsonContent }, "LoadAttendanceMasterData Fail");
            return false;
        }
        return true;
    }

    bool LoadCollectionDefines()
    {
        string jsonContent = System.IO.File.ReadAllText(_dirPath + "CollectionDefines.json");
        var _collectionDefines = JsonSerializer.Deserialize<CollectionDefines>(jsonContent);
        if (_collectionDefines == null)
        {
            _logger.ZLogCriticalWithPayload(new { jsonContent = jsonContent }, "LoadCollectionDefines Fail");
            return false;
        }
        foreach ( var defineElem in _collectionDefines.collectionDefines ) 
        {
            _collectionDefinesDic.Add(defineElem.code, defineElem);
        }
        return true;
    }
}
