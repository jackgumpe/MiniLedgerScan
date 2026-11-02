namespace MiniLedgerScan.Api.Application;

using System;
using System.Threading;
using System.Threading.Tasks;
using MiniLedgerScan.Api.Providers;
using MiniLedgerScan.Api.Domain;
using MiniLedgerScan.Api.Contracts;
public sealed class ProofPacketService(IOcrProvider ocrProvider, IProofPacketStore ProofPacketStore)
{

    private readonly IOcrProvider _ocrProvider = ocrProvider;
    
    private readonly IProofPacketStore _proofPacketStore = ProofPacketStore;


    public ProofPacketReview Create(string artifactName)
        {
            ProofPacketReview packet = new(artifactName);

            _proofPacketStore.Add(packet);

            return packet;
        }

    public ProofPacketReview? GetById(Guid id)
    {
        ProofPacketReview? retrieved = _proofPacketStore.GetById(id);

        return retrieved;
    }


    public async Task<OcrResult> ProcessAsync(
        ProofPacketReview packet,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        OcrRequest request = new(
            packet.Id, 
            packet.ArtifactName
            );

        packet.StartValidation();

     
        try
        {
            var result = await _ocrProvider.ProcessAsync(
            request,
            cancellationToken);

            if (string.IsNullOrWhiteSpace(result.Text))
            {
                packet.Reject("OCR text was empty.");
            }
            else
            {
                packet.Verify("OCR text passed validation.");
            }

            
            return result;

        }
        catch (OperationCanceledException)
        {
            packet.Cancel("Operation has been interrupted.");
            throw;
        }

        

        
    }

}


// If cancellation occurs during the provider’s delay, 
// what state will the packet be left in, and why?
//
// I would imagine it would be in stuck in StartValidation() because the cancellation token fires during 250ms delay
// after StartValidation had already been executed,
