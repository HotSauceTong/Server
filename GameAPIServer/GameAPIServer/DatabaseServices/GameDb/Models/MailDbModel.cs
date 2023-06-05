using GameAPIServer.Controllers.ReqResModels;

namespace GameAPIServer.DatabaseServices.GameDb.Models;

public class MailDbModel
{
    public Int64 mail_id { get; set; }
    public Int64 user_id { get; set; }
    public Int64 collection_code { get; set; } = -1;
    public Int32 collection_count { get; set; } = -1;
    public String mail_title { get; set; }
    public String mail_body { get; set; }
    public DateTime read_date { get; set; } = new DateTime(9999, 12, 31, 23, 59, 59);
    public DateTime recieve_date { get; set; }
    public DateTime expiration_date { get; set; } = new DateTime(9999, 12, 31, 23, 59, 59);
    public Int16 is_deleted { get; set; } = 0;

    public CollectionBundle GetCollectionBundle()
    {
        return new CollectionBundle
        {
            collectionCode = collection_code,
            collectionCount = collection_count
        };
    }
}
