using System.Collections.Generic;

namespace HotChocolatey
{
    public struct ChocolateyResult
    {
        public List<string> Output { get; }
        public int ExitCode { get; }
        public string Arguments { get; }

        public bool Succeeded => ExitCode == 0;

        public ChocolateyResult(List<string> output, int exitCode, string arguments) 
        {
            Output = output;
            ExitCode = exitCode;
            Arguments = arguments;
        }

        public void ThrowIfNotSucceeded()
        {
            if (!Succeeded)
            {
                throw new ChocoExecutionException(this);
            }
        }
    }
}
