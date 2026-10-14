

using Microsoft.Extensions.Options;

namespace MiniLedgerScan.Api.Configuration;

public sealed class MockOcrOptionsValidator : IValidateOptions<MockOcrOptions>
{

    public ValidateOptionsResult Validate(string? name, MockOcrOptions options)
    {

        if (options.DelayMilliseconds < 0 || options.DelayMilliseconds > 5000)
        {
            return ValidateOptionsResult.Fail($"{nameof(options.DelayMilliseconds)} must be between 0 and 5000. Current value: {options.DelayMilliseconds}");
        }

        return ValidateOptionsResult.Success;
    }

}

    
