namespace MiniLedgerScan.Api.Contracts;

public sealed record CreateProofPacketRequest
{
    public required string ArtifactName {get; init;}
}