#region header

// Fission.Functions - FissionLogger.cs
// 
// Created by: Alistair J R Young (avatar) at 2020/12/30 8:08 AM.

#endregion

#region using

using JetBrains.Annotations;

#endregion

namespace Fission.Functions
{
    public delegate void FissionWriteLog (string format, params object[] args);

    [PublicAPI]
    public record FissionLogger
    {
        public FissionWriteLog WriteCritical;

        public FissionWriteLog WriteError;

        public FissionWriteLog WriteInfo;

        public FissionWriteLog WriteWarning;
    }
}
