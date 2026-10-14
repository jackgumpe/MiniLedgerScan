namespace MiniLedgerScan.Api.Contracts;

public sealed record ProofPacketResponse
{
    public required Guid Id {get; init;}
    public required string ArtifactName {get; init;}
    public required string Status {get; init;}
}