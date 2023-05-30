using GameAPIServer.DatabaseServices.SessionDb;
using System.Text;
using System.Text.Json;
using ZLogger;

namespace GameAPIServer.MiddleWare;

public class AuthCheckMiddleware
{
    readonly RequestDelegate _next;
    readonly ILogger<AuthCheckMiddleware> _logger;
    readonly ISessionService _session;

    public AuthCheckMiddleware(RequestDelegate next, ISessionService sesion, ILogger<AuthCheckMiddleware> logger)
    {
        _next = next;
        _session = sesion;
        _logger = logger;
    }
    public async Task Invoke(HttpContext context)
    {
        String path = context.Request.Path;
        if (!(path.StartsWith("/Regist") || path.StartsWith("/Login")))
        {
            context.Request.EnableBuffering();
            using (var streamReader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                var requestBody = await streamReader.ReadToEndAsync(); // 요청 본문을 문자열로 읽어옵니다.
                                                                       // 요청 body에 id, token이 있는지 확인하고, 있는 경우 찾아서 반환.
                
                var (userId, token) = await CheckBodyFormAndGetIdToken(context, requestBody);

                if (userId == null || token == null)
                {
                    return;
                }
                if (await CheckTokenAndSetSessionDataInContext(context, userId, token) == false)
                {
                    return;
                }
            }
            context.Request.Body.Position = 0;
        }
        await _next(context);
        //응답 로직
    }

    private async Task<(String?, String?)> CheckBodyFormAndGetIdToken(HttpContext context, String? body)
    {
        if (String.IsNullOrEmpty(body))
        {
            _logger.ZLogWarningWithPayload(new { Path = context.Request.Path }, "Http Body NULLorEMPTY");
            await SetContext(context, 400, ErrorCode.InvalidRequestFormat);
            return (null, null);
        }
        try
        {
            var doc = JsonDocument.Parse(body);
            if (doc == null)
            {
                _logger.ZLogWarningWithPayload(new { Path = context.Request.Path }, "Http Body UNPARSINGABLE");
                await SetContext(context, 400, ErrorCode.InvalidRequestFormat);
                return (null, null);
            }
            else if (doc.RootElement.TryGetProperty("userEmail", out var id))
            {
                if (doc.RootElement.TryGetProperty("token", out var token))
                {
                    return (id.GetString(), token.GetString());
                }
            }
            return (null, null);
        }
        catch (FormatException ex)
        {
            _logger.ZLogWarningWithPayload(new { Path = context.Request.Path, Exception = ex }, "Http Body FormatEXCEPTION");
            await SetContext(context, 400, ErrorCode.InvalidRequestFormat);
            return (null, null);
        }
        catch
        {
            _logger.ZLogErrorWithPayload(new { Path = context.Request.Path }, "Http SERVERERROR");
            await SetContext(context, 500, ErrorCode.InvalidRequestFormat);
            return (null, null);
        }
    }

    private async Task<bool> CheckTokenAndSetSessionDataInContext(HttpContext context, String userId, String inputToken)
    {
        var (errorCode, userInfo) = await _session.GetSession(userId);
        if (errorCode != ErrorCode.None || userInfo == null)
        {
            _logger.ZLogErrorWithPayload(new { Path = context.Request.Path, errorCode = errorCode }, "Session FAIL");
            await SetContext(context, 400, errorCode);
            return false;
        }
        if (userInfo.token != inputToken)
        {
            _logger.ZLogInformationWithPayload(new { Path = context.Request.Path, userId = userId }, "Token Check FAIL");
            await SetContext(context, 400, ErrorCode.InvalidRequestFormat);
            return false;
        }
        context.Items["Session"] = userInfo;
        return true;
    }

    async Task SetContext(HttpContext context, Int32 statusCode, ErrorCode errorCode)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var responseContent = new { errorCode = errorCode };
        await context.Response.WriteAsJsonAsync(responseContent);
    }

}
