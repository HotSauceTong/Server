using System.Reflection.Metadata;
using System.Text.Json;

namespace GameAPIServer.MiddleWare
{
    public class BodyParsingMiddleware
    {
        private readonly RequestDelegate _next;

        public BodyParsingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();
            // 요청 본문을 JSON으로 파싱하여 HttpContext.Items["Body"]에 저장합니다.
            string requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            /*READ: 따라서 using (JsonDocument document = JsonDocument.Parse(requestBody))를 사용하여 JsonDocument 인스턴스를 생성하고, using 문의 범위를 벗어나면 document 변수가 소멸되면서 JsonDocument가 자동으로 해제됩니다. 이를 통해 메모리 누수를 방지하고 자원을 효율적으로 관리할 수 있습니다.*/
            using (var doc = JsonDocument.Parse(requestBody))
            {
                /*READ: 복제를 사용하는 이유는 다음과 같습니다. JsonDocument는 JsonElement에 대한 읽기 전용 뷰를 제공하며, JsonElement 자체가 변경 가능한 속성을 가지지 않습니다. 따라서 JsonElement를 복제하면 원본 JsonDocument의 수명과 관계없이 복제된 JsonElement를 안전하게 사용할 수 있습니다. 원본 JsonDocument가 해제되더라도 복제된 JsonElement는 그대로 유효하게 남아있을 것입니다.*/
                context.Items["Body"] = doc.RootElement.Clone();
            }
            context.Request.Body.Position = 0;
            await _next(context);
        }
    }
}
