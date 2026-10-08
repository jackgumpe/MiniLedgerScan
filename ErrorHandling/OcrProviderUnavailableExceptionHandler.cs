namespace MiniLedgerScan.Api.ErrorHandling;
using Microsoft.AspNetCore.Diagnostics;
using MiniLedgerScan.Api.Providers;


public class OcrProviderUnavailableExceptionHandler : IExceptionHandler
{

       public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if(exception is not OcrProviderUnavailableException)
        {
            return false;
        }

        IResult response = Results.Problem(
            statusCode: StatusCodes.Status503ServiceUnavailable,
            title: "OCR provider unavailable",
            detail: "OCR processing is temporarily unavailable. Please try again later."
        );

        await response.ExecuteAsync(httpContext);
        return true;
    }

}