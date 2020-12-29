#region using

using Fission.DotNet;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

#endregion

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
