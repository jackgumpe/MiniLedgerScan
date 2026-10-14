using System.Collections.Concurrent;
using MiniLedgerScan.Api.Application;
using MiniLedgerScan.Api.Domain;

namespace MiniLedgerScan.Api.Infrastructure;

public sealed class InMemoryProofPacketStore : IProofPacketStore
{

    private readonly ConcurrentDictionary<Guid,ProofPacketReview> _packets = new();

    public void Add(ProofPacketReview packet)
    {
        ArgumentNullException.ThrowIfNull(packet);

        bool added = _packets.TryAdd(packet.Id, packet);

        if (!added)
        {
            throw new InvalidOperationException(
                $"A proof packet with ID '{packet.Id}' already exists."
            );
        }
    }

    // Other code calls this method to retrieve a stored packet.
    public ProofPacketReview? GetById(Guid id)
    {
        bool found = _packets.TryGetValue(id, out var packet);

        if (found)
        {
            return packet;
        }

        return null;
    }

    

}
