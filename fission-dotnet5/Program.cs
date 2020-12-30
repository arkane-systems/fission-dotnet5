#region header

// fission-dotnet5 - Program.cs
// 
// Created by: Alistair J R Young (avatar) at 2020/12/28 11:19 PM.

#endregion

#region using

using Fission.DotNet;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

#endregion

// Entry point.

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
