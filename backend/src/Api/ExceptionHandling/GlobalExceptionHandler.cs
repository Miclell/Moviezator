using Application.Common.Exceptions;
using Core.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.ExceptionHandling;

public partial class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            DomainException e => (StatusCodes.Status400BadRequest, e.Message),
            NotFoundException e => (StatusCodes.Status404NotFound, e.Message),
            ConflictException e => (StatusCodes.Status409Conflict, e.Message),
            ValidationException e => (StatusCodes.Status400BadRequest, e.Message),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error")
        };

        if (statusCode >= StatusCodes.Status500InternalServerError)
            LogUnhandledExceptionMappedToStatusCodeStatuscode(logger, statusCode);
        else
            LogHandledExceptionMappedToStatusCodeStatuscode(logger, statusCode);

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = title
        }, cancellationToken);

        return true;
    }

    [LoggerMessage(LogLevel.Error, "Unhandled exception mapped to status code {statusCode}")]
    static partial void LogUnhandledExceptionMappedToStatusCodeStatuscode(ILogger<GlobalExceptionHandler> logger,
        int statusCode);

    [LoggerMessage(LogLevel.Warning, "Handled exception mapped to status code {statusCode}")]
    static partial void LogHandledExceptionMappedToStatusCodeStatuscode(ILogger<GlobalExceptionHandler> logger,
        int statusCode);
}
