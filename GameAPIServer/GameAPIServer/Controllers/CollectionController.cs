using GameAPIServer.Controllers.ReqResModels;
using GameAPIServer.DatabaseServices.GameDb;
using GameAPIServer.DatabaseServices.SessionDb;
using GameAPIServer.Filter;
using GameAPIServer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameAPIServer.Controllers;

[Route("[controller]")]
[ApiController]
[EmailFormatCheckFilter]
[ServiceFilter(typeof(SessionCheckAndGetFilter))]
public class CollectionController : ControllerBase
{
    readonly ILogger<CollectionController> _logger;
    readonly IGameDbService _gameDbService;
    readonly IMasterDataOffer _masterDataOffer;

    public CollectionController(ILogger<CollectionController> logger, IGameDbService gameDbService, IMasterDataOffer masterDataOffer)
    {
        _logger = logger;
        _gameDbService = gameDbService;
        _masterDataOffer = masterDataOffer;
    }

    [HttpPost("AllCollectionListUp")]
    public async Task<AllCollectionListUpResponse> AllCollectionListUp(AllCollecionListUpRequest request)
    {
        var response = new AllCollectionListUpResponse();
        var session = (SessionModel?)HttpContext.Items["Session"];

        (response.errorCode, var allCollectionList) = await _gameDbService.GetAllUserCollectionList(session.userId);
        if (response.errorCode != ErrorCode.None)
        {
            return response;
        }

        response.collectionList = allCollectionList;
        return response;
    }

    [HttpPost("CurrencyListUp")]
    public async Task<CurrencyListUpResponse> CurrencyListUp(CurrencyListUpRequest request)
    {
        var response = new CurrencyListUpResponse();
        var session = (SessionModel?)HttpContext.Items["Session"];

        (response.errorCode, var currencyList) = await _gameDbService.GetUserCurrencyList(session.userId);
        if (response.errorCode != ErrorCode.None)
        {
            return response;
        }

        response.currencyList = currencyList;
        return response;
    }

    [HttpPost("CardListUp")]
    public async Task<CardListUpResponse> CardListUp(CardListUpResponse request)
    {
        var response = new CardListUpResponse();
        var session = (SessionModel?)HttpContext.Items["Session"];

        (response.errorCode, var cardList) = await _gameDbService.GetUserCardList(session.userId);
        if (response.errorCode != ErrorCode.None)
        {
            return response;
        }

        response.cardList = cardList;
        return response;
    }
}
