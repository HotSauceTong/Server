using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GameAPIServer.Filter;

public class NicknameFormatCheckFilter : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var jsonElem = (JsonElement)context.HttpContext.Items["Body"];

        var nickname = jsonElem.GetProperty("nickname").GetString();
        if (nickname == null)
        {
            await SetContext(context.HttpContext, ErrorCode.InvalidRequestFormat);
            return;
        }

        if (IsValidNickname(nickname) == false)
        {
            await SetContext(context.HttpContext, ErrorCode.InvalidNicknameFormat);
            return;
        }

        await next();
    }

    async Task SetContext(HttpContext context, ErrorCode errorCode)
    {
        context.Response.ContentType = "application/json";
        var responseContent = new { errorCode = errorCode };
        await context.Response.WriteAsJsonAsync(responseContent);
    }

    bool IsValidNickname(String nickname)
    {
        if (nickname.Length < 3 || nickname.Length > 12)
        {
            return false;
        }
        string nicknamePattern = @"^\S+$";
        Regex regex = new Regex(nicknamePattern);
        return regex.IsMatch(nickname);
    }
}
