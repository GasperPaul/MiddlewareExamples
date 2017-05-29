using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Middleware
{
    public class StartupEx1
    {
        public void Configure(IApplicationBuilder app)
        {
            // first middleware
            app.Use(async (context,next) =>
            {
                await context.Response.WriteAsync("first: before\n");
                await next();
                await context.Response.WriteAsync("first: after\n");        // DO NOT DO THIS IN PRODUCTION!
            });

            // second middleware
            app.Use(async (context,next) =>
            {
                await context.Response.WriteAsync("\tsecond: before\n");
                await next();
                await context.Response.WriteAsync("\tsecond: after\n");     // DO NOT DO THIS IN PRODUCTION!
            });

            // third middleware
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("\t\tHello World!\n");
            });
        }
    }
}
