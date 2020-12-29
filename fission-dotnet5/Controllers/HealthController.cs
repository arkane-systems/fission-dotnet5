#region using

using JetBrains.Annotations;

using Microsoft.AspNetCore.Mvc;

#endregion

namespace Fission.DotNet.Controllers
{
    [Route (template: "/healthz")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [NotNull]
        public object Get () => this.Ok ();
    }
}
