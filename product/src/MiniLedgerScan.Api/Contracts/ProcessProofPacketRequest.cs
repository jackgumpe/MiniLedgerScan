namespace MiniLedgerScan.Api.Contracts;

public sealed record ProcessProofPacketRequest
{
    public required string ArtifactName {get; init;}
}