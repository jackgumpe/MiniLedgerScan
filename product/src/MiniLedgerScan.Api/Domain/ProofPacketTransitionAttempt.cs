namespace MiniLedgerScan.Api.Domain;

public sealed record ProofPacketTransitionAttempt(
    ProofPacketStatus CurrentStatus,
    ProofPacketStatus RequestedStatus,
    DateTimeOffset AttemptedAt,
    string FailureMessage
);
