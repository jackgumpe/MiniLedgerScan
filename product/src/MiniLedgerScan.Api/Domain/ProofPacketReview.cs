namespace MiniLedgerScan.Api.Domain;

public sealed class ProofPacketReview
{
    private readonly List<ProofPacketStatusChanged> _history = []; // (See changes* (2))
    private readonly List<ProofPacketTransitionAttempt> _failedAttempts = []; // (See changes* (2))

    
    public Guid Id {get; init;} 

    public string ArtifactName {get; init;} = "";

    public ProofPacketStatus Status {get; private set;}

    public IReadOnlyList<ProofPacketStatusChanged> History => _history; 
    public IReadOnlyList<ProofPacketTransitionAttempt> FailedAttempts => _failedAttempts;

    public ProofPacketReview(string artifactName)
    {
        if (string.IsNullOrWhiteSpace(artifactName))
        {
            throw new ArgumentException("Artifact name is required.", nameof(artifactName));
        }

        Id = Guid.NewGuid();
        ArtifactName = artifactName;
        Status = ProofPacketStatus.Received;
    }

    public void StartValidation()
    {
        TransitionTo(ProofPacketStatus.Validating, "Proof Packet validation started.");

    }

    public void Verify(string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason); // (See changes* (1))
        TransitionTo(ProofPacketStatus.Verified, reason);
    }

    public void Reject(string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason); // (See changes* (1))
        TransitionTo(ProofPacketStatus.Rejected, reason);
    }

    private void TransitionTo(ProofPacketStatus nextStatus, string reason)
    {
        if (!CanTransitionTo(nextStatus))
        {
            string errorMessage = $"Cannot transition from {Status} to {nextStatus}";

            _failedAttempts.Add(new ProofPacketTransitionAttempt(
                CurrentStatus: Status,
                RequestedStatus: nextStatus,
                AttemptedAt: DateTimeOffset.UtcNow,
                FailureMessage: errorMessage
            ));

            throw new InvalidOperationException(errorMessage);
        }

        var currentStatus = Status;
        Status = nextStatus;

        _history.Add(new ProofPacketStatusChanged(
            PreviousStatus: currentStatus,
            NewStatus: nextStatus,
            ChangedAt: DateTimeOffset.UtcNow,
            Reason: reason
        ));
    }

    private bool CanTransitionTo(ProofPacketStatus nextStatus)
    {
        return (Status, nextStatus) switch
        {
            (ProofPacketStatus.Received, ProofPacketStatus.Validating) => true,
            (ProofPacketStatus.Validating, ProofPacketStatus.Verified) => true,
            (ProofPacketStatus.Validating, ProofPacketStatus.Rejected) => true,

            _ => false

        };
    }

}