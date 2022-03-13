using CDC.Reader.Application;
using CDC.Reader.Application.Extension;
using CDC.Reader.Infrastructure.Context;
using CDC.Reader.Infrastructure.Context.Interface;
using CDC.Reader.Infrastructure.Repository;
using CDC.Reader.Infrastructure.Repository.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CDC.Reader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var connString = hostContext.Configuration.GetConnectionString("CDCMirror");
                    services.AddHostedService<Worker.Worker>();
                    services.AddTransient<ISqlExtension, SqlExtension>();
                    services.AddTransient<IEmployeeRepository, EmployeeRepository>();
                    services.AddTransient<ICDCProcessLogRepository, CDCProcessLogRepository>();
                    services.AddTransient<IContextFactory, ContextFactory>();
                });
    }
}
