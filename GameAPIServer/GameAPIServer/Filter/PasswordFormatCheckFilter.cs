using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GameAPIServer.Filter;

public class PasswordFormatCheckFilter : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var jsonElem = (JsonElement)context.HttpContext.Items["Body"];

        var password = jsonElem.GetProperty("password").GetString();
        if (password == null)
        {
            await SetContext(context.HttpContext, ErrorCode.InvalidRequestFormat);
            return;
        }

        if (IsValidPassword(password) == false)
        {
            await SetContext(context.HttpContext, ErrorCode.InvalidPasswordFormat);
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

    bool IsValidPassword(String password)
    {
        if (password.Length < 6 || password.Length > 12)
        {
            return false;
        }
        string nicknamePattern = @"^\S+$";
        Regex regex = new Regex(nicknamePattern);
        return regex.IsMatch(password);
    }
}
