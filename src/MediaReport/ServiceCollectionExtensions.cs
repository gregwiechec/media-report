using EPiServer.Shell.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace MediaReport
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediaReport(this IServiceCollection services)
        {
            services.Configure<ProtectedModuleOptions>(
                pm =>
                {
                    if (!pm.Items.Any(i =>
                        i.Name.Equals("MediaReport", StringComparison.OrdinalIgnoreCase)))
                    {
                        pm.Items.Add(new ModuleDetails { Name = "MediaReport", Assemblies = { typeof(ServiceCollectionExtensions).Assembly.GetName().Name }  });
                    }
                });
            return services;
        }
    }
}
