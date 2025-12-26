using Serilog.Context;
using System.Text;

namespace EtoileEGAPI.Middlewares;


public sealed class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestResponseLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var requestId = Guid.NewGuid().ToString();
        context.Items["RequestId"] = requestId;

        LogContext.PushProperty("RequestId", requestId);
        LogContext.PushProperty("Method", context.Request.Method);
        LogContext.PushProperty("RequestPath", context.Request.Path);

        string requestBody = await ReadRequestBody(context.Request);
        LogContext.PushProperty("RequestBody", requestBody);

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        string response = await ReadResponseBody(responseBody);
        LogContext.PushProperty("ResponseBody", response);
        LogContext.PushProperty("ResponseStatusCode", context.Response.StatusCode.ToString());

        await responseBody.CopyToAsync(originalBodyStream);
    }

    private static async Task<string> ReadRequestBody(HttpRequest request)
    {
        request.EnableBuffering();

        using var reader = new StreamReader(
            request.Body,
            Encoding.UTF8,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        return body;
    }

    private static async Task<string> ReadResponseBody(MemoryStream responseBody)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(responseBody).ReadToEndAsync();
        responseBody.Seek(0, SeekOrigin.Begin);
        return text;
    }
}
