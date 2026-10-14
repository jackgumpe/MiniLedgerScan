namespace MiniLedgerScan.Api.Providers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MiniLedgerScan.Api.Configuration;
using MiniLedgerScan.Api.Contracts;

public sealed class MockBaiduOcrProvider(IOptions<MockOcrOptions> options) : IOcrProvider
{

    private readonly MockOcrOptions _options = options.Value;

    public async Task<OcrResult> ProcessAsync(
        OcrRequest request,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        Console.WriteLine("Mock Test");

        await Task.Delay(_options.DelayMilliseconds, cancellationToken);

        string text = _options.ReturnEmptyText ? string.Empty : $"Mock ocr text for '{request.ArtifactName}'.";
        
        if(_options.SimulateUnavailable == true)
        {
            throw new OcrProviderUnavailableException("503: Processing cannot finish.");
        }

        OcrResult result = new (text, "MockBaiduUnlimitedOcr");

        return result;
    }
}