using System.ComponentModel.DataAnnotations;

namespace MiniLedgerScan.Api.Configuration;

public sealed class MockOcrOptions
{

    public const string SectionName = "MockOcr";
    public int DelayMilliseconds {get; set;} = 250;
    public bool ReturnEmptyText {get; set;} = false;
    public bool SimulateUnavailable {get; set;} = false;

}