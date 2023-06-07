namespace GameAPIServer.Controllers.ReqResModels;

public class AllCollecionListUpRequest : BaseRequest
{
}

public class CurrencyListUpRequest : BaseRequest
{
}

public class CardListUpRequest : BaseRequest
{
}

public class CurrencyListUpResponse : BaseResponse
{
    public List<CollectionBundle>? currencyList { get; set; }
}

public class AllCollectionListUpResponse : BaseResponse
{
    public List<CollectionBundle>? collectionList { get; set; }
}

public class CardListUpResponse : BaseResponse
{
    public List<CollectionBundle>? cardList { get; set; }
}

