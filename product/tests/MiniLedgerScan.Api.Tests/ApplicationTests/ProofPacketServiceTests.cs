using Microsoft.Extensions.Options;
using MiniLedgerScan.Api.Domain;
using MiniLedgerScan.Api.Application;
using MiniLedgerScan.Api.Configuration;
using MiniLedgerScan.Api.Providers;
using MiniLedgerScan.Api.Infrastructure;


namespace MiniLedgerScan.Api.Tests.ApplicationTests
{
    public class ProofPacketServiceTests
    {

        private static ProofPacketService BuildPacketService(int delayMS) => new(
            new MockBaiduOcrProvider(Options.Create(
                new MockOcrOptions{DelayMilliseconds = delayMS, SimulateUnavailable = false})),
                 new InMemoryProofPacketStore());

        [Fact]
        public async Task ProcessAsync_WhenCancelledDuringProviderCall_LeavesPacketInCancelled()
        {
            // Arrange
            ProofPacketReview packet = new("invoice-001.pdf");
            ProofPacketService proofPacketService = BuildPacketService(delayMS: 5000);

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(50);

            // Act
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => proofPacketService.ProcessAsync(packet, cts.Token)
            );
            
            // Assert
            Assert.Equal(ProofPacketStatus.Cancelled, packet.Status);
            Assert.Equal(2, packet.History.Count);
            Assert.Equal(ProofPacketStatus.Received, packet.History[0].PreviousStatus);
            Assert.Equal(ProofPacketStatus.Validating, packet.History[0].NewStatus);
            Assert.Equal(ProofPacketStatus.Validating, packet.History[1].PreviousStatus);
            Assert.Equal(ProofPacketStatus.Cancelled, packet.History[1].NewStatus);
            Assert.Empty(packet.FailedAttempts);
            
        }

        [Fact]
        public async Task ProcessAsync_WhenTokenCancelledBeforeProviderCall_ThrowsBeforeValidating()
        {
            // Arrange
            var packet = new ProofPacketReview("invoice-002.pdf");
            var packetService = BuildPacketService(delayMS: 5000);

            using var cts = new CancellationTokenSource();

            cts.Cancel();

            // Act
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => packetService.ProcessAsync(packet, cts.Token)
            );

            // Assert
            Assert.Equal(ProofPacketStatus.Received, packet.Status);
            Assert.Empty(packet.History);
            Assert.Empty(packet.FailedAttempts);

        }

        [Fact]
        public void Cancel_WhenPacketAlreadyVerified_ThrowsAndsRecordsFailedAttempt()
        {
            // Arrange
            var packet = new ProofPacketReview("invoice-003.pdf");
            
            packet.StartValidation();
            packet.Verify("Packet has been successfully validated.");

            // Act
            Assert.Throws<InvalidOperationException>(
                () => packet.Cancel("Cancelled too late")
            );

            // Assert
            Assert.Equal(ProofPacketStatus.Verified, packet.Status);
            Assert.Single(packet.FailedAttempts);
            Assert.Equal(2, packet.History.Count);

        }
        


    }
}