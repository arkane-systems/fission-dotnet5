#region using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Runtime.Serialization.Json;

using Fission.Functions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

#endregion

namespace Fission.DotNet
{
    internal class FissionCompiler
    {
        private string packagePath;

        internal FissionCompiler () => this.packagePath = string.Empty;

        internal FunctionRef? Compile (string source, out List<string> errors)
        {
            errors = new List<string> ();

            // Parse source code.
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText (text: source);

            // Load up assembly references.
            DirectoryInfo? coreDir = Directory.GetParent (path: typeof (Enumerable).GetTypeInfo ().Assembly.Location);

            var references = new List<MetadataReference>
                             {
                                 MetadataReference
                                    .CreateFromFile (path: $"{coreDir!.FullName}{Path.DirectorySeparatorChar}mscorlib.dll"),
                                 MetadataReference.CreateFromFile (path: typeof (object).GetTypeInfo ().Assembly.Location),
                                 MetadataReference.CreateFromFile (path: typeof (IFissionFunction).GetTypeInfo ().Assembly
                                                                      .Location),
                                 MetadataReference.CreateFromFile (path: Assembly.GetEntryAssembly ()!.Location),
                                 MetadataReference.CreateFromFile (path: typeof (DataContractJsonSerializer).GetTypeInfo ()
                                                                      .Assembly.Location),
                             };

            foreach (var referencedAssembly in Assembly.GetEntryAssembly ()!.GetReferencedAssemblies ())
            {
                Assembly? assembly = Assembly.Load (assemblyRef: referencedAssembly);
                references.Add (item: MetadataReference.CreateFromFile (path: assembly.Location));
            }

            // Compile source code.
            string assemblyName = Path.GetRandomFileName ();

            CSharpCompilation compilation = CSharpCompilation.Create (
                                                                      assemblyName: assemblyName,
                                                                      syntaxTrees: new[] {syntaxTree,},
                                                                      references: references,
                                                                      options: new CSharpCompilationOptions (
                                                                       outputKind: OutputKind.DynamicallyLinkedLibrary,
                                                                       optimizationLevel: OptimizationLevel.Release));

            using var ms = new MemoryStream ();

            {
                EmitResult result = compilation.Emit (peStream: ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics
                                                             .Where (predicate: diagnostic =>
                                                                                    diagnostic.IsWarningAsError ||
                                                                                    diagnostic.Severity ==
                                                                                    DiagnosticSeverity.Error)
                                                             .ToList ();

                    foreach (Diagnostic diagnostic in failures)
                        errors.Add (item: $"{diagnostic.Id}: {diagnostic.GetMessage ()}");

                    return null;
                }

                ms.Seek (offset: 0, loc: SeekOrigin.Begin);

                Assembly assembly = AssemblyLoadContext.Default.LoadFromStream (assembly: ms);

                Type? type = assembly.GetTypes ()
                                     .FirstOrDefault (predicate: t => typeof (IFissionFunction).IsAssignableFrom (c: t));

                if (type == null)
                {
                    errors.Add (item: "FIS0001: No compatible type found during compilation.");

                    return null;
                }

                return new FunctionRef (assembly: assembly, type: type);
            }
        }
    }
}
