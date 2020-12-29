#region using

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

#endregion

namespace Fission.DotNet
{
    public static class Program
    {
        public static int Main (string[] args)
        {
            // Build minimal web host.
            IWebHost host = new WebHostBuilder ()
                           .ConfigureLogging (configureLogging: log => log.AddConsole ())
                           .UseKestrel ()
                           .UseUrls ("http://*:8888")
                           .UseStartup<Startup> ()
                           .Build ();

            host.Run ();

            // Return success.
            return 0;
        }
    }
}
