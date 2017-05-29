using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Middleware
{
    public abstract class MiddlewareBase
    {
        protected readonly RequestDelegate next;

        public MiddlewareBase(RequestDelegate _next) { next = _next; }

        public abstract Task Invoke(HttpContext context);
    }


    public class RedirectMiddleware : MiddlewareBase
    {
        public RedirectMiddleware(RequestDelegate _next) : base(_next) { }

        public async override Task Invoke(HttpContext context)
        {
            var redirectData = context.Request.Headers["redirect"].ToString();
            if (!string.IsNullOrEmpty(redirectData))
                context.Response.Redirect(redirectData, permanent: true);
            else await this.next(context);
        }
    }

    public class CookiesMiddleware : MiddlewareBase
    {
        private readonly CookiesMiddlewareOptions options;
        public CookiesMiddleware(RequestDelegate _next,
            IOptions<CookiesMiddlewareOptions> _options) : base(_next)
        {
            options = _options.Value;
        }

        public async override Task Invoke(HttpContext context)
        {
            context.Response.OnStarting(SetCookies, context);
            await this.next(context);
        }

        private Task SetCookies(object arg)
        {
            var context = (HttpContext)arg;
            context.Response.Cookies.Append(options.CookieName, options.CookieValue,
                new CookieOptions { Expires = System.DateTime.Now.AddHours(1), HttpOnly = true });
            return Task.FromResult(0);
        }
    }

    public class CookiesMiddlewareOptions
    {
        public string CookieValue { get; set; }
        public string CookieName { get; set; }
    }

    public class StartupEx5
    {
        private IConfigurationRoot Configuration { get; set; }

        public StartupEx5(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                 .SetBasePath(env.ContentRootPath)
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<CookiesMiddlewareOptions>(options => Configuration.GetSection("CookiesMiddlewareSection").Bind(options));
        }

        public void Configure(IApplicationBuilder app)
        {
            // modules
            app.UseMiddleware<RedirectMiddleware>();
            app.UseMiddleware<CookiesMiddleware>();

            // handlers
            app.MapWhen(
                _c => _c.Request.Path.ToString().EndsWith(".jpg"),
                _app =>
                {
                    // _app.UseMiddleware<>() is fine too
                    _app.Run(async _c => await _c.Response.WriteAsync(".jpg handler"));
                });

            app.Run(async context => await context.Response.WriteAsync("Some other handler"));
        }
    }
}