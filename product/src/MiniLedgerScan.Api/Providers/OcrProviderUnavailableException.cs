namespace MiniLedgerScan.Api.Providers;

public sealed class OcrProviderUnavailableException(string message) : Exception(message)
{
}