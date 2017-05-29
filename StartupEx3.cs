using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Middleware
{
    public class StartupEx3
    {
        public void Configure(IApplicationBuilder app)
        {
            // simple map
            app.Map("/map", _app =>
            {
                _app.Run(async _c => await _c.Response.WriteAsync("Mapped path."));
            });

            // surprise! won't work here
            app.Map("/map/deeper", _app =>
            {
                _app.Run(async _c => await _c.Response.WriteAsync("Deeper map!"));
            });

            // we need to go deeper!
            app.Map("/level1", _app =>
            {
                _app.Map("/levelA", _app2 =>
                {
                    _app2.Run(async _c => await _c.Response.WriteAsync("Level A"));
                });
                _app.Map("/levelB", _app2 =>
                {
                    _app2.Run(async _c => await _c.Response.WriteAsync("Level B"));
                });
                _app.Run(async _c => await _c.Response.WriteAsync("Level 1"));
            });

            // echo using MapWhen()
            app.MapWhen(
                _c => !string.IsNullOrEmpty(_c.Request.Query["data"]),
                _app => _app.Run(async context =>
                {
                    var queryData = context.Request.Query["data"];
                    await context.Response.WriteAsync(queryData);
                })
            );

            app.Run(async context => await context.Response.WriteAsync("Hello World!"));
        }
    }
}
