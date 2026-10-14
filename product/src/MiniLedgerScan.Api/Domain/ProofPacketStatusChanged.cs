namespace MiniLedgerScan.Api.Domain;

public sealed record ProofPacketStatusChanged(
    ProofPacketStatus PreviousStatus,
    ProofPacketStatus NewStatus,
    DateTimeOffset ChangedAt,
    string Reason
);