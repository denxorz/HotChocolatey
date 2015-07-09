using System;

namespace HotChocolatey
{
    public class ChocoExecutionException : Exception
    {
        public ChocolateyResult Result { get; }

        public ChocoExecutionException(ChocolateyResult result)
        {
            Result = result;
        }
    }
}
