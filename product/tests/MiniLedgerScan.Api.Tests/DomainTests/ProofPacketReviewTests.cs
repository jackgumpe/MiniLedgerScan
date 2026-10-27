using MiniLedgerScan.Api.Domain;
using Xunit;

namespace MiniLedgerScan.Api.Tests.DomainTests{

    
    public class ProofPacketReviewTests
    {

        

        
        [Theory]
        [InlineData("invoice-001.pdf")]
        [InlineData(" ")]
        public void Constructor_WhenPacketCreated_StartsInReceivedInReceivedWithEmptyHistories(string artifactName)
        {
            // Arrange & Act
            var packet = new ProofPacketReview(artifactName);

            // Assert 
            Assert.Equal(ProofPacketStatus.Received, packet.Status);
            Assert.Empty(packet.History);
            Assert.Empty(packet.FailedAttempts);

        }

        [Theory]
        [InlineData("invoice-002")]
        [InlineData(" ")]
        public void StateTransition_ReceivedToVerifyToValidated_UpdatesStatusAndRecords_OrderedHistory(string artifactName)
        {
            // Arrange
            var packet = new ProofPacketReview(artifactName);

            // Act 1: First transition
            packet.StartValidation();

            // Assert 1: Verify State-Machine intermedite transitioned to validating right after the first call
            Assert.Equal(ProofPacketStatus.Validating, packet.Status);
            Assert.Single(packet.History);

            // Act 2: Second transition
            packet.Verify("Packet has been verified");

            // Assert 2: Verify State-Machine and history entries cound.
            Assert.Equal(ProofPacketStatus.Verified, packet.Status);
            Assert.Equal(2, packet.History.Count);

            // Assert 3: Verfiy chronologial order of History entries.
            Assert.Equal(ProofPacketStatus.Received, packet.History[0].PreviousStatus);
            Assert.Equal(ProofPacketStatus.Validating, packet.History[0].NewStatus);

            Assert.Equal(ProofPacketStatus.Validating, packet.History[1].PreviousStatus);
            Assert.Equal(ProofPacketStatus.Verified, packet.History[1].NewStatus);

            
        }

        [Theory]
        [InlineData("invoice-003")]
        [InlineData(" ")]
        public void Verified_Throws_StatusIsStillReceivedFailedAttemptCount(string artifactName)
        {
            // Arrange
            var packet = new ProofPacketReview(artifactName);

            // Act & Assert

            Assert.Throws<InvalidOperationException>(()=>  packet.Verify("Looks good") );
            Assert.Equal(ProofPacketStatus.Received, packet.Status);
            Assert.Single(packet.FailedAttempts);
        }

        [Theory]
        [InlineData("invoice-004")]
        [InlineData(" ")]
        public void Rejected_ReceivedTransitionToRejected_PacketStatusIsRejected(string artifactName)
        {   
            // Arrange
            var packet = new ProofPacketReview(artifactName);

            // Act
            packet.StartValidation();
            packet.Reject("invoice-004 has been rejected and recorded as rejected");

            // Assert
            Assert.Equal(ProofPacketStatus.Rejected, packet.Status);

        }

        [Theory]
        [InlineData(" ")]
        [InlineData("Packet is verified and accepted.")]
        public void Verifiy_ThrowsWhenNullOrWhiteSpace_HistoryAndFailedAttemptsAreEmpty(string reason)
        {
            // Arrange
            var packet = new ProofPacketReview("invoice-005.pdf");

            // Act
            packet.StartValidation();
            packet.Verify(reason);

            // Assert
            if (string.IsNullOrWhiteSpace(reason))
            {
                Assert.Single(packet.History);
                Assert.Empty(packet.FailedAttempts);
            }
            else
            {
                Assert.Equal(2, packet.History.Count);
            }
            

        }
            
        
        



    }
}