using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GameAPIServer.Filter;

public class EmailFormatCheckFilter : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var jsonElem = (JsonElement)context.HttpContext.Items["Body"];//.GetProperty("email").GetString();
        
        var email = jsonElem.GetProperty("email").GetString();
        if (email == null)
        {
            await SetContext(context.HttpContext, ErrorCode.InvalidRequestFormat);
            return;
        }

        if (IsValidEmail(email) == false)
        {
            await SetContext(context.HttpContext, ErrorCode.InvalidEmailFormat);
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
    public bool IsValidEmail(string email)
    {
        // 이메일 정규식 패턴
        string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9]+$";

        // 정규식 패턴과 매치되는지 확인
        Regex regex = new Regex(emailPattern);
        return regex.IsMatch(email);
    }
}
