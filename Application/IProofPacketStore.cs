namespace MiniLedgerScan.Api.Application;
using MiniLedgerScan.Api.Domain;

public interface IProofPacketStore
{
    void Add(ProofPacketReview packet);
    ProofPacketReview? GetById(Guid id);
}