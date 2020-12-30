#region header

// Fission.Functions - FissionRequest.cs
// 
// Created by: Alistair J R Young (avatar) at 2020/12/30 8:08 AM.

#endregion

#region using

using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using JetBrains.Annotations;

#endregion

namespace Fission.Functions
{
    [PublicAPI]
    public record FissionRequest
    {
        public Stream Body;

        [CanBeNull]
        public X509Certificate2 ClientCertificate;

        public IReadOnlyDictionary<string, IEnumerable<string>> Headers;

        public string Method;

        public string Url;

        [NotNull]
        public string BodyAsString ()
        {
            var length = (int) this.Body.Length;
            var data   = new byte[length];
            this.Body.Read (buffer: data, offset: 0, count: length);

            return Encoding.UTF8.GetString (bytes: data);
        }
    }
}
