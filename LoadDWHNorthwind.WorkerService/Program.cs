using LoadDWHNorthwind.Data.Context;
using LoadDWHNorthwind.Data.Interfaces;
using LoadDWHNorthwind.Data.Services;
using LoadDWHNorthwind.WorkerService;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static void Main(string[] args)
    {
       CreateHostBuilder(args).Build().Run();
    }
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) => {

            services.AddDbContextPool<NorthwindContext>(options => 
                                                      options.UseSqlServer(hostContext.Configuration.GetConnectionString("DbNorthwind")));

            services.AddDbContextPool<DWNorthwindContext>(options => 
                                                      options.UseSqlServer(hostContext.Configuration.GetConnectionString("DWNorthwind")));
 

            services.AddScoped<IDataServiceDWHNorthwind, DataServiceDWHNorthwind>();

            services.AddHostedService<Worker>();
        });
}