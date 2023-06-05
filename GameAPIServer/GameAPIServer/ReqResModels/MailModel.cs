namespace GameAPIServer.ReqResModels;

public class MailListElement
{
    public Int64 mailId { get; set; }
    public Int64 collectionCode { get; set; } = -1;
    public Int32 collectionCount { get; set; } = -1;
    public String mailTitle { get; set; }
    public DateTime readDate { get; set; } = new DateTime(9999, 12, 31, 23, 59, 59);
    public DateTime expirationDate { get; set; } = new DateTime(9999, 12, 31, 23, 59, 59);
}

public class Mail
{
    public Int64 mailId { get; set; }
    public Int64 collectionCode { get; set; } = -1;
    public Int32 collectionCount { get; set; } = -1;
    public String mailTitle { get; set; }
    public String mailBody { get; set; }
    public DateTime readDate { get; set; } = new DateTime(9999, 12, 31, 23, 59, 59);
    public DateTime receiveDate { get; set; }
    public DateTime expirationDate { get; set; } = new DateTime(9999, 12, 31, 23, 59, 59);
}

public class CollectionBundle
{
    public Int64 collectionCode { get; set; }
    public Int32 collectionCount { get; set; }
}


public class MailListRequest : BaseRequest
{
}

public class MailReadRequest : BaseRequest
{
    public Int64 mailId { get; set; }
}

public class MailItemReceiveRequest : BaseRequest
{
    public Int64 mailId { get; set; }
}

public class MailListResponse : BaseResponse
{
    public List<MailListElement>? mailList { get; set; }
}

public class MailReadResponse : BaseResponse
{
    public Mail? mail { get; set; }
}

public class MailItemReceiveResponse : BaseResponse
{
    public CollectionBundle collectionBundle { get; set; }
}