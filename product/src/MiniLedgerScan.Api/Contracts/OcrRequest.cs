namespace MiniLedgerScan.Api.Contracts;

public sealed record OcrRequest(
    Guid ProofPacketId,
    string ArtifactName);