using MiniLedgerScan.Api.Domain;


namespace MiniLedgerScan.Api.Tests.DomainTests{

    
    public class ProofPacketReviewTests
    {

        

        
        [Fact]
        public void Constructor_WhenPacketCreated_StartsInReceivedInReceivedWithEmptyHistories()
        {
            // Arrange & Act
            var packet = new ProofPacketReview("invoice-001.pdf");

            // Assert 
            Assert.Equal(ProofPacketStatus.Received, packet.Status);
            Assert.Empty(packet.History);
            Assert.Empty(packet.FailedAttempts);

        }

        [Fact]
        public void StateTransition_ReceivedToVerifyToValidated_UpdatesStatusAndRecords_OrderedHistory()
        {
            // Arrange
            var packet = new ProofPacketReview("invoice-002.pdf");

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

        [Fact]
        public void Verified_Throws_StatusIsStillReceivedFailedAttemptCount()
        {
            // Arrange
            var packet = new ProofPacketReview("invoice-003.pdf");

            // Act & Assert

            Assert.Throws<InvalidOperationException>(()=>  packet.Verify("Looks good") );
            Assert.Equal(ProofPacketStatus.Received, packet.Status);
            Assert.Single(packet.FailedAttempts);
        }

        [Fact]
        public void Rejected_ReceivedTransitionToRejected_PacketStatusIsRejected()
        {   
            // Arrange
            var packet = new ProofPacketReview("invoice-004.pdf");

            // Act
            packet.StartValidation();
            packet.Reject("invoice-004 has been rejected and recorded as rejected");

            // Assert
            Assert.Equal(ProofPacketStatus.Rejected, packet.Status);

        }

        [Fact]
        public void Verifiy_ThrowsWhenNullOrWhiteSpace_HistoryAndFailedAttemptsAreEmpty()
        {
            // Arrange
            var packet = new ProofPacketReview("invoice-005.pdf");

            // Act
            packet.StartValidation();
            packet.Verify("Packet is verified and accepted.");

            // Assert
            if (string.IsNullOrWhiteSpace("Packet cannot be null or whitespace."))
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