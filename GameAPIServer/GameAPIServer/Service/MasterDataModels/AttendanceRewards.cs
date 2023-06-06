using GameAPIServer.Controllers.ReqResModels;

namespace GameAPIServer.Service.MasterDataModels;

public class AttendanceRewards
{
    public String version { get; set; }
    public List<CollectionBundle> rewards { get; set; }
}
