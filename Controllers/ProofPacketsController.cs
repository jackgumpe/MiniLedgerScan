using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MiniLedgerScan.Api.Application;
using MiniLedgerScan.Api.Contracts;
using MiniLedgerScan.Api.Domain;


namespace MiniLedgerScan.Api.Controllers;

[ApiController]
[Route("api/proof-packets")]
public sealed class ProofPacketsController(ProofPacketService proofPacketService) : ControllerBase
{
    private readonly ProofPacketService _proofPacketService = proofPacketService;

    [HttpPost("process")]
    [ProducesResponseType(typeof(ProcessProofPacketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<ProcessProofPacketResponse>> ProcessAsync(
        [FromBody] ProcessProofPacketRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ArtifactName))
        {
            return BadRequest("Artifact name cannot be blank");
        }

        ProofPacketReview packet = new(request.ArtifactName);
        
        
        var result = await _proofPacketService.ProcessAsync(
            packet,
            cancellationToken
        );

        var response = new ProcessProofPacketResponse
        {
            Id = packet.Id,
            ArtifactName = packet.ArtifactName,
            Status = packet.Status.ToString(),
            Text = result.Text,
            ProviderName = result.ProviderName


        };

        return Ok(response);

    }

    [HttpPost]
    [ProducesResponseType(typeof(ProofPacketResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<ProofPacketResponse> Create(
        [FromBody] CreateProofPacketRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ArtifactName))
        {
            return BadRequest("ArtifactName cannot be blank.");
        }

        var packet = _proofPacketService.Create(request.ArtifactName);

        var response = new ProofPacketResponse{
            Id = packet.Id,
            ArtifactName = packet.ArtifactName,
            Status = packet.Status.ToString()
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = packet.Id},
            response
        );

    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProofPacketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ProofPacketResponse> GetById(Guid id)
    {
        var packet = _proofPacketService.GetById(id);

        if (packet is null)
        {
            return NotFound();
        }

        var response = new ProofPacketResponse
        {
            Id = packet.Id,
            ArtifactName = packet.ArtifactName,
            Status = packet.Status.ToString()
        };

        return Ok(response);

    }
    
    
}