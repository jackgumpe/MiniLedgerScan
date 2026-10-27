
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Diagnostics;

public enum ManifestStatus { Queued, Inspecting, Cleared, Quarantined }

namespace MiniLedgerScanTest.Api.DomainTests
{
    
    public enum ManifestStatus { Queued, Inspecting, Cleared, Quarantined }

public sealed record ManifestTransition(
    ManifestStatus From, 
    ManifestStatus To, 
    DateTimeOffset OccurredAt, 
    string? Note = null);

public sealed record FailedTransitionAttempt(
    ManifestStatus CurrentStatus, 
    ManifestStatus AttemptedStatus, 
    DateTimeOffset AttemptedAt, 
    string FailureMessage);

public sealed class ShippingManifest
{
        private readonly List<ManifestTransition> _history = [];
        private readonly List<FailedTransitionAttempt> _failedAttempts = [];

        public Guid Id { get; } = Guid.NewGuid();
        public string CargoCode { get; }
        public ManifestStatus Status { get; private set; } = ManifestStatus.Queued;

        public IReadOnlyList<ManifestTransition> History => _history;
        public IReadOnlyList<FailedTransitionAttempt> FailedAttempts => _failedAttempts;

        public ShippingManifest(string cargoCode)
        {
            if (string.IsNullOrWhiteSpace(cargoCode))
                throw new ArgumentException("Cargo code cannot be blank.", nameof(cargoCode));

            CargoCode = cargoCode;
        }

        public void BeginInspection()
        {
            if (Status != ManifestStatus.Queued)
            {
                RecordFailureAndThrow(ManifestStatus.Inspecting, "Can only begin inspection from Queued.");
            }

            TransitionTo(ManifestStatus.Inspecting);
        }

        public void Clear(string clearanceNote)
        {
            // 1. Parameter Guard fires FIRST (before checking state or touching lists)
            if (string.IsNullOrWhiteSpace(clearanceNote))
                throw new ArgumentException("Clearance note is required.", nameof(clearanceNote));

            // 2. State Invariant Check fires SECOND
            if (Status != ManifestStatus.Inspecting)
            {
                RecordFailureAndThrow(ManifestStatus.Cleared, "Manifest must be Inspecting before it can be Cleared.");
            }

            // 3. Legal Transition mutates state and appends to History
            TransitionTo(ManifestStatus.Cleared, clearanceNote);
        }

        public void Quarantine(string hazardReason)
        {
            if (string.IsNullOrWhiteSpace(hazardReason))
                throw new ArgumentException("Hazard reason is required.", nameof(hazardReason));

            if (Status != ManifestStatus.Inspecting)
            {
                RecordFailureAndThrow(ManifestStatus.Quarantined, "Manifest must be Inspecting to be Quarantined.");
            }

            TransitionTo(ManifestStatus.Quarantined, hazardReason);
        }

        private void TransitionTo(ManifestStatus nextStatus, string? note = null)
        {
            var previous = Status;
            Status = nextStatus;
            _history.Add(new ManifestTransition(previous, nextStatus, DateTimeOffset.UtcNow, note));
        }

        private void RecordFailureAndThrow(ManifestStatus attempted, string message)
        {
            _failedAttempts.Add(new FailedTransitionAttempt(Status, attempted, DateTimeOffset.UtcNow, message));
            throw new InvalidOperationException(message);
        }

        [Fact]
        public void Constructor_WhenCreated_StartsInQueuedWithEmptyHistories()
        {
            // Arrange & Act
            var manifest = new ShippingManifest("CARGO-101");

            // Assert (Three distinct checks)
            Assert.Equal(ManifestStatus.Queued, manifest.Status);
            Assert.Empty(manifest.History);
            Assert.Empty(manifest.FailedAttempts);

        }

        [Fact]
        public void LegalLifecycle_QueuedToInspectingToCleared_UpdatesStatusAndRecords_OrderedHistory()
        {
            
        

        // Arrange
        var manifest = new ShippingManifest("CARGO-102");

        // Act 1: First transition
        manifest.BeginInspection();

        // Assert 1: Verfiy intermediate state right agter the first call
        Assert.Equal(ManifestStatus.Inspecting, manifest.Status);
        Assert.Single(manifest.History);

        // Act: Second transition
        manifest.Clear("Passed X-ray scan");

        // Assert 2: Verify final state and ordered history entries
        Assert.Equal(ManifestStatus.Cleared, manifest.Status);
        Assert.Equal(2, manifest.History.Count);

        // Verify chronological order using index [0] and index [1]
        Assert.Equal(ManifestStatus.Queued, manifest.History[0].From);
        Assert.Equal(ManifestStatus.Inspecting, manifest.History[0].To);

        Assert.Equal(ManifestStatus.Inspecting, manifest.History[1].From);
        Assert.Equal(ManifestStatus.Cleared, manifest.History[1].To);

        }

    }   


}
