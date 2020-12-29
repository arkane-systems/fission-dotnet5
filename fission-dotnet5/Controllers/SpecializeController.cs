#region using

using System;
using System.Collections.Generic;
using System.Net;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

#endregion

namespace Fission.DotNet.Controllers
{
    [Route (template: "/specialize")]
    [ApiController]
    public class SpecializeController : ControllerBase
    {
        private readonly ILogger<SpecializeController> logger;
        private readonly IFunctionStore                store;

        [NotNull]
        private static string CodePath
#if DEBUG
            => "tmp/TestFunc.cs";
#else
            => Resources.CodePath;
#endif

        public SpecializeController (ILogger<SpecializeController> logger, IFunctionStore store)
        {
            this.logger = logger;
            this.store  = store;
        }

        [HttpPost]
        [NotNull]
        public object Post ()
        {
            this.logger.LogInformation (message: "/specialize called.");

            if (System.IO.File.Exists (path: SpecializeController.CodePath))
            {
                // Load the source file.
                string source = System.IO.File.ReadAllText (path: SpecializeController.CodePath);

                // Compile the file.

                var          compiler = new FissionCompiler ();
                FunctionRef? binary   = compiler.Compile (source: source, errors: out List<string> errors);

                if (binary == null)
                {
                    string? error = string.Join (separator: Environment.NewLine, values: errors);

                    this.logger.LogError (message: error);

                    return this.StatusCode (statusCode: (int) HttpStatusCode.InternalServerError, value: error);
                }

                // Save function in cache.
                this.store.SetFunctionRef (func: binary);
            }
            else
            {
                var error = $"Unable to locate function source code at '{SpecializeController.CodePath}'.";

                this.logger.LogError (message: error);

                return this.StatusCode (statusCode: (int) HttpStatusCode.InternalServerError, value: error);
            }

            return this.Ok ();
        }
    }
}
