using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Middleware
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<StartupEx5>()
                .Build();

            host.Run();
        }
    }
}
