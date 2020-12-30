#region header

// Fission.Functions - FissionContext.cs
// 
// Created by: Alistair J R Young (avatar) at 2020/12/28 11:30 PM.

#endregion

#region using

using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

using JetBrains.Annotations;

#endregion

namespace Fission.Functions
{
    [PublicAPI]
    public record FissionContext
    {
        public IReadOnlyDictionary<string, string> Arguments;

        public FissionLogger Logger;

        public string PackagePath;

        public FissionRequest Request;

        [CanBeNull]
        public T GetSettings<T> (string relativePath)
        {
            string filePath = Path.Combine (path1: this.PackagePath, path2: relativePath);
            string json     = File.ReadAllText (path: filePath);

            return JsonSerializer.Deserialize<T> (json: json);
        }
    }

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
