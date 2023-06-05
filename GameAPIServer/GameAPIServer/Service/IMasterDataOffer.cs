using GameAPIServer.Controllers.ReqResModels;
using GameAPIServer.Service.MasterDataModels;

namespace GameAPIServer.Service
{
    public interface IMasterDataOffer
    {
        bool LoadMasterDatas();
        CollectionBundle? GetAttendanceReward(Int32 stack);
        CollectionDefine? GetCollectionDefine(Int64 collectionId);
    }
}
