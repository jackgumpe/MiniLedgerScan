namespace MiniLedgerScan.Api.Contracts;

public sealed record ProcessProofPacketResponse
{
    public required Guid Id {get; init;}    
    public required string ArtifactName {get; init;}

    public required string Status {get; init;}

    public required string Text {get; init;}

    public required string ProviderName {get; init;}
}