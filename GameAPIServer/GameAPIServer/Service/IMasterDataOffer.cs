using GameAPIServer.Controllers.ReqResModels;
using GameAPIServer.Service.MasterDataModels;

namespace GameAPIServer.Service
{
    public interface IMasterDataOffer
    {
        bool LoadMasterDatas();
        CollectionBundle? GetAttendanceReward(Int32 stack);
        String GetAttendanceRewardVersion();
        Int32 GetAttendenceMaxCount();

        CollectionDefine? GetCollectionDefine(Int64 collectionId);
    }
}
