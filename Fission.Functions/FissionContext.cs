#region header

// Fission.Functions - FissionContext.cs
// 
// Created by: Alistair J R Young (avatar) at 2020/12/28 11:30 PM.

#endregion

#region using

using System.Collections.Generic;
using System.IO;
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
}
