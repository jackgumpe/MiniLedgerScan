namespace MiniLedgerScan.Api.Domain;

public enum ProofPacketStatus
{
    Received,
    Validating,
    Cancelled,
    Verified,
    Rejected
}