#region using

using System;
using System.Net;

using Fission.Functions;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

#endregion

namespace Fission.DotNet.Controllers
{
    [ApiController]
    [Route (template: "/")]
    public class FunctionController : ControllerBase
    {
        private readonly ILogger<FunctionController> logger;
        private readonly IFunctionStore              store;

        public FunctionController (ILogger<FunctionController> logger, IFunctionStore store)
        {
            this.logger = logger;
            this.store  = store;
        }

        [HttpGet]
        [NotNull]
        public object Get () => this.Run ();

        [HttpPost]
        [NotNull]
        public object Post () => this.Run ();

        [HttpPut]
        [NotNull]
        public object Put () => this.Run ();

        [HttpHead]
        [NotNull]
        public object Head () => this.Run ();

        [HttpOptions]
        [NotNull]
        public object Options () => this.Run ();

        [HttpDelete]
        [NotNull]
        public object Delete () => this.Run ();

        [NotNull]
        private object Run ()
        {
            this.logger.LogInformation (message: "function invoked");

            if (this.store.Func == null)
            {
                this.logger.LogError (message: "generic container: no requests supported");

                return this.StatusCode (statusCode: (int) HttpStatusCode.InternalServerError,
                                        value: "generic container: no requests supported");
            }

            try
            {
                // Set up the context.
                var context = new FissionContext ();

                // Invoking the function.
                return this.Ok (value: this.store.Func.Invoke (context: context));
            }
            catch (Exception e)
            {
                this.logger.LogError (message: e.Message);

                return this.StatusCode (statusCode: (int) HttpStatusCode.BadRequest, value: e.Message);
            }
        }
    }
}
