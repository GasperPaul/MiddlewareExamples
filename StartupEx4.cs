using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Middleware
{
    public class EchoMiddleware
    {
        private readonly RequestDelegate next;

        public EchoMiddleware(RequestDelegate _next)
        {
            next = _next;
        }

        public Task Invoke(HttpContext context)
        {
            var data = context.Request.Query["data"];
            if (!string.IsNullOrEmpty(data))
                return context.Response.WriteAsync(data);
            else
                return this.next(context);
        }
    }

    public class StartupEx4
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<EchoMiddleware>();

            app.Run(async context => await context.Response.WriteAsync("Hello World!"));
        }
    }
}
