namespace MiniLedgerScan.Api.Providers;

using System.Threading;
using System.Threading.Tasks;

using MiniLedgerScan.Api.Contracts;

public interface IOcrProvider
{
    Task<OcrResult> ProcessAsync(
        OcrRequest request,
        CancellationToken cancellationToken
    );
}