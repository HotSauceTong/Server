namespace GameAPIServer.DatabaseServices.GameDb.Models;

public class UserCollection
{
    public Int64 collection_id { get; set; }
    public Int64 user_id { get; set; }
    public Int64 collection_code { get; set; }
    public Int32 collection_count { get; set; }
}
