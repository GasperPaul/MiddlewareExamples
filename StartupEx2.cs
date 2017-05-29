using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Middleware
{
    public class StartupEx2
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Use(async (context,next) =>
            {
                var queryData = context.Request.Query["data"];
                if (!string.IsNullOrEmpty(queryData))
                    await context.Response.WriteAsync(queryData);
                else
                    await next();
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
