using GameAPIServer.DatabaseServices.SessionDb;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace GameAPIServer.Filter;

public class SessionCheckAndGetFilter : ActionFilterAttribute
{
    readonly ILogger<SessionCheckAndGetFilter> _logger;
    readonly ISessionDbService _sessionDb;

    public SessionCheckAndGetFilter(ILogger<SessionCheckAndGetFilter> logger, ISessionDbService sessionDb)
    {
        _logger = logger;
        _sessionDb = sessionDb;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        JsonElement jsonElem = (JsonElement)context.HttpContext.Items["Body"];
        String? email = jsonElem.GetProperty("email").GetString();
        String? token = jsonElem.GetProperty("token").GetString();
        if (email == null || token == null)
        {
            // TODO: eamil 없음 이런식의 구체적인 에러코드 작성.
            // 이상한 요청이나 로그 남기는게 좋을듯 -> 정상적인 클라가 아닐 가능성 높음.
            await SetContext(context.HttpContext, ErrorCode.InvalidRequestFormat);
            return;
        }

        var (errorCode, session) = await _sessionDb.GetSession(email);
        if (errorCode == ErrorCode.ExpiredSession)
        {
            await SetContext(context.HttpContext, errorCode);
            return;
        }
        else if (errorCode != ErrorCode.None || session == null)
        {
            await SetContext(context.HttpContext, ErrorCode.SessionError);
            return;
        }

        if (session.token.Equals(token) == false)
        {
            await SetContext(context.HttpContext, ErrorCode.InvalidToken);
            return;
        }

        context.HttpContext.Items["Session"] = session;
        await next();
    }

    async Task SetContext(HttpContext context, ErrorCode errorCode)
    {
        context.Response.ContentType = "application/json";
        var responseContent = new { errorCode = errorCode };
        await context.Response.WriteAsJsonAsync(responseContent);
    }
}
