#region header

// fission-dotnet5 - FissionCompiler.cs
// 
// Created by: Alistair J R Young (avatar) at 2020/12/29 9:08 AM.

#endregion

#region using

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Runtime.Serialization.Json;

using Fission.DotNet.Properties;
using Fission.Functions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

#endregion

namespace Fission.DotNet
{
    /// <summary>
    ///     The compiler which builds Fission functions from source text, when a builder container is not used.
    /// </summary>
    internal class FissionCompiler
    {
        /// <summary>
        ///     Compile C# source text (implementing <see cref="IFissionFunction" />) to an assembly stored in memory.
        /// </summary>
        /// <param name="source">The source code to compile.</param>
        /// <param name="errors">On exit, a list of compilation errors.</param>
        /// <returns>A <see cref="FunctionRef" /> referencing the compiled Fission function.</returns>

        // ReSharper disable once MemberCanBeMadeStatic.Global
        [SuppressMessage (category: "Performance",
                          checkId: "CA1822:Mark members as static",
                          Justification = "Instance members are expected in later iterations. -- AJRY 2020/12/30")]
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
                                 MetadataReference.CreateFromFile (path: typeof (IFissionFunction).GetTypeInfo ()
                                                                      .Assembly
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
                    errors.Add (item: Resources.FissionCompiler_Compile_NoEntrypoint);

                    return null;
                }

                return new FunctionRef (assembly: assembly, type: type);
            }
        }
    }
}
