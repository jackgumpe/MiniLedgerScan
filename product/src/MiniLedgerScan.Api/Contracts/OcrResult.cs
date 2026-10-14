namespace MiniLedgerScan.Api.Contracts;

public sealed record OcrResult(
    string Text,
    string ProviderName);