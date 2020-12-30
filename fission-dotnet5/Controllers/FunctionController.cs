#region header

// fission-dotnet5 - FunctionController.cs
// 
// Created by: Alistair J R Young (avatar) at 2020/12/28 11:19 PM.

#endregion

#region using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

using Fission.Functions;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

#endregion

namespace Fission.DotNet.Controllers
{
    [ApiController]
    [Route (template: "/")]
    public class FunctionController : ControllerBase
    {
        private readonly ILogger<IFissionFunction>   funcLogger;
        private readonly ILogger<FunctionController> logger;
        private readonly IFunctionStore              store;

        public FunctionController (ILogger<FunctionController> logger, ILogger<IFissionFunction> funcLogger, IFunctionStore store)
        {
            this.logger     = logger;
            this.funcLogger = funcLogger;
            this.store      = store;
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
                FissionContext? context = this.BuildContext ();

                // Invoking the function.
                return this.Ok (value: this.store.Func.Invoke (context: context));
            }
            catch (Exception e)
            {
                this.logger.LogError (message: e.Message);

                return this.StatusCode (statusCode: (int) HttpStatusCode.BadRequest, value: e.Message);
            }
        }

        private FissionContext BuildContext (string packagePath = "")
        {
            // Logger
            var fl = new FissionLogger
                     {
                         WriteInfo     = (format, objects) => this.funcLogger.LogInformation (message: format, args: objects),
                         WriteWarning  = (format, objects) => this.funcLogger.LogWarning (message: format, args: objects),
                         WriteError    = (format, objects) => this.funcLogger.LogError (message: format, args: objects),
                         WriteCritical = (format, objects) => this.funcLogger.LogCritical (message: format, args: objects),
                     };

            // Arguments
            IQueryCollection qs = this.Request.Query;

            var args = new Dictionary<string, string> ();

            foreach (var k in qs.Keys) args[key: k] = qs[key: k];

            // Headers
            var headers = new Dictionary<string, IEnumerable<string>> ();

            foreach (KeyValuePair<string, StringValues> kv in this.Request.Headers)
                headers.Add (key: kv.Key, value: kv.Value);

            // Request
            var fr = new FissionRequest
                     {
                         Body              = this.Request.Body,
                         ClientCertificate = this.Request.HttpContext.Connection.ClientCertificate,
                         Headers           = new ReadOnlyDictionary<string, IEnumerable<string>> (dictionary: headers),
                         Method            = this.Request.Method,
                         Url               = this.Request.GetEncodedUrl (),
                     };

            // Context
            var fc = new FissionContext
                     {
                         Logger      = fl,
                         Arguments   = new ReadOnlyDictionary<string, string> (dictionary: args),
                         Request     = fr,
                         PackagePath = packagePath,
                     };

            return fc;
        }
    }
}
