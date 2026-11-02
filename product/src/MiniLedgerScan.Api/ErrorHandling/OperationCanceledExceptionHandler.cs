using Microsoft.AspNetCore.Diagnostics;

namespace MiniLedgerScan.Api.ErrorHandling;

public sealed class OperationCanceledExceptionHandler(
    ILogger<OperationCanceledExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not OperationCanceledException)
        {
            return false;                       // not mine — let the next handler try
        }

        // Case 1: the client went away.
        if (httpContext.RequestAborted.IsCancellationRequested)
        {
            logger.LogInformation(
                "Request aborted by client: {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);

            return true;                        // handled; nothing to write
        }

        // Case 2: cancelled on our side.
        logger.LogWarning(
            exception,
            "Request cancelled server-side: {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path);

        await Results.Problem(
                statusCode: StatusCodes.Status504GatewayTimeout,
                title: "Upstream operation timed out",
                detail: "The OCR provider did not respond in time. The operation was aborted.")
            .ExecuteAsync(httpContext);

        return true;
    }
}

