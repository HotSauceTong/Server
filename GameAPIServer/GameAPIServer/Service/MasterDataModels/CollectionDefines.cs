namespace GameAPIServer.Service.MasterDataModels;

public class CollectionDefine
{
    public Int64 code { get; set; }
    public Int16 attribute { get; set; }
    public String name { get; set; }
    public String text { get; set; }
    public Int32 attack { get; set; }
    public Int32 healthPoint { get; set; }
    public Int16 cost { get; set; }
    public String description { get; set; }
}

public class CollectionDefines
{
    public List<CollectionDefine> collectionDefines { get; set; }
}
