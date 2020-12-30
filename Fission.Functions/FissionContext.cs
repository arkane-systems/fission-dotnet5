#region header

// /*
//  * Fission.Functions - FissionContext.cs
//  *
//  * Created by: avatar at 2020/12/28 11:30 PM.
//  *
//  */

#endregion

#region using

#endregion

#region using

using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using JetBrains.Annotations;

#endregion

namespace Fission.Functions
{
    [PublicAPI]
    public record FissionContext
    {
        public IReadOnlyDictionary<string, string> Arguments;

        public FissionLogger Logger;

        public FissionRequest Request;
    }

    [PublicAPI]
    public record FissionRequest
    {
        [CanBeNull]
        public X509Certificate2 ClientCertificate;

        public string Method;

        public string Url;
    }

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
