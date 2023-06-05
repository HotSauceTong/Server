using GameAPIServer.DatabaseServices.GameDb;
using GameAPIServer.DatabaseServices.GameDb.Models;
using GameAPIServer.DatabaseServices.SessionDb;
using GameAPIServer.Filter;
using GameAPIServer.ReqResModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace GameAPIServer.Controllers;

[Route("[controller]")]
[ApiController]
[EmailFormatCheckFilter]
[ServiceFilter(typeof(SessionCheckAndGetFilter))]
public class MailController : ControllerBase
{
    readonly ILogger<MailController> _logger;
    readonly IGameDbService _gameDbService;

    public MailController(IHttpContextAccessor httpContextAccessor, ILogger<MailController> logger, IGameDbService gameDbService)
    {
        _logger = logger;
        _gameDbService = gameDbService;
    }

    // 메일 리스트 받기
    public async Task<MailListResponse> mailList(MailListRequest request)
    {
        var session = (SessionModel?)HttpContext.Items["Session"];
        var response = new MailListResponse();

        (response.errorCode, var mailList) = await _gameDbService.GetUserMails(session.userId);
        if (response.errorCode != ErrorCode.None)
        {
            return response;
        }
        else if (mailList == null)
        {
            response.errorCode = ErrorCode.GameDbError;
            return response;
        }
        response.mailList = GenerateMailListElement(mailList);
        return response;
    }
    // 특정 메일 읽기
    public async Task<MailReadResponse> MailRead(MailReadRequest request)
    {
        var session = (SessionModel?)HttpContext.Items["Session"];
        MailReadResponse response = new MailReadResponse();
        (response.errorCode, var mailDbModel) = await _gameDbService.GetUserMail(session.userId, request.mailId);
        if (response.errorCode != ErrorCode.None)
        {
            return response;
        }
        else if (mailDbModel == null)
        {
            response.errorCode = ErrorCode.GameDbError;
            return response;
        }
        var now = DateTime.Now;
        response.errorCode = await _gameDbService.ReadUserMails(session.userId, request.mailId, now);
        if (response.errorCode != ErrorCode.None)
        {
            return response;
        }

        response.mail = new Mail()
        {
            mailId = mailDbModel.mail_id,
            collectionCode = mailDbModel.collection_code,
            collectionCount = mailDbModel.collection_count,
            mailTitle = mailDbModel.mail_title,
            mailBody = mailDbModel.mail_body,
            readDate = now,
            receiveDate = mailDbModel.recieve_date,
            expirationDate = mailDbModel.expiration_date
        };
        return response;
    }

    // 특정 메일 아이템 받기
    public async Task<MailItemReceiveResponse> MailItemReceive(MailItemReceiveRequest request)
    {
        var session = (SessionModel?)HttpContext.Items["Session"];
        MailItemReceiveResponse response = new MailItemReceiveResponse();
        // 메일 읽어오기
        (response.errorCode, var mailModel) = await _gameDbService.GetUserMail(session.userId, request.mailId);
        if (response.errorCode != ErrorCode.None)
        {
            return response;
        }
        
        // 메일에 아이템 삭제하기
        response.errorCode = await _gameDbService.UpdateUserMailCollection(session.userId, request.mailId, null);
        if (response.errorCode != ErrorCode.None)
        {
            return response;
        }
        // 아이템 지급하기
        response.errorCode = await _gameDbService.GiveCollectionsToUser(session.userId, new List<CollectionBundle>() { mailModel.GetCollectionBundle() });
        if (response.errorCode != ErrorCode.None)
        {
            await _gameDbService.UpdateUserMailCollection(session.userId, request.mailId, mailModel.GetCollectionBundle());
            return response;
        }
        response.collectionBundle = mailModel.GetCollectionBundle();
        return response;
    }

    List<MailListElement> GenerateMailListElement(List<MailDbModel> mailModelList)
    {
        var mailListResponse = new List<MailListElement>();
        foreach(var mailElem in mailModelList)
        {
            mailListResponse.Add(new MailListElement()
            {
                mailId = mailElem.mail_id,
                collectionCode = mailElem.collection_code,
                collectionCount = mailElem.collection_count,
                mailTitle = mailElem.mail_title,
                readDate = mailElem.read_date,
                expirationDate = mailElem.expiration_date
            });
        }
        return mailListResponse;
    }

}
