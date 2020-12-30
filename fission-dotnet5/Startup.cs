#region header

// fission-dotnet5 - Startup.cs
// 
// Created by: Alistair J R Young (avatar) at 2020/12/28 11:19 PM.

#endregion

#region using

using JetBrains.Annotations;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#endregion

namespace Fission.DotNet
{
    /// <summary>
    ///     Configure the environment container's web interface.
    /// </summary>
    public class Startup
    {
        [UsedImplicitly]
        public Startup (IConfiguration configuration) => this.Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services)
        {
            services.AddSingleton<IFunctionStore, FunctionStore> ();
            services.AddControllers ();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure ([NotNull] IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment ()) app.UseDeveloperExceptionPage ();
            app.UseRouting ();
            app.UseAuthorization ();
            app.UseEndpoints (configure: endpoints => { endpoints.MapControllers (); });
        }
    }
}
