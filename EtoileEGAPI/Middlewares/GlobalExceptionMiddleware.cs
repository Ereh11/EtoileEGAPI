using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net;

namespace EtoileEGAPI.Middlewares;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        IWebHostEnvironment env)
    {
        _next = next;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleAsync(context, exception);
        }
    }

    private async Task HandleAsync(
        HttpContext context,
        Exception exception)
    {
        var problem = CreateProblemDetails(context, exception);

        Log.Error(exception,
            "Unhandled exception occurred. TraceId: {TraceId}",
            context.TraceIdentifier);

        context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problem);
    }

    private ProblemDetails CreateProblemDetails(
        HttpContext context,
        Exception exception)
    {
        var statusCode = exception switch
        {
            HttpRequestException httpEx when httpEx.InnerException is TaskCanceledException
                => HttpStatusCode.GatewayTimeout,

            HttpRequestException
                => HttpStatusCode.BadGateway,

            InvalidCastException or FormatException
                => HttpStatusCode.BadRequest,

            DbUpdateException
                => HttpStatusCode.Conflict,

            SqlException
                => HttpStatusCode.InternalServerError,

            _ => HttpStatusCode.InternalServerError
        };

        var title = statusCode switch
        {
            HttpStatusCode.BadRequest => "Invalid request data",
            HttpStatusCode.Conflict => "Data conflict occurred",
            HttpStatusCode.BadGateway => "External service error",
            HttpStatusCode.GatewayTimeout => "External service timeout",
            _ => "Unexpected server error"
        };

        return new ProblemDetails
        {
            Title = title,
            Status = (int)statusCode,
            Detail = _env.IsDevelopment()
                ? exception.Message
                : "Something went wrong, please try again later.",
            Instance = context.Request.Path,
            Extensions =
            {
                ["traceId"] = context.TraceIdentifier
            }
        };
    }
}
