using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using Microsoft.Extensions.DependencyInjection;
using Alloy.MediaReport.ScheduledJob;
using EPiServer.Core;
using Alloy.MediaReport;

namespace AlloyTemplates.Business.Initialization
{
    [ModuleDependency(typeof(InitializationModule))]
    public class CustomMediaReportInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.ConfigurationComplete += (o, e) =>
            {
                //context.Services.AddTransient<IMediaHierarchyRootResolver, CustomMediaHierarchyRootResolver>();
                //context.Services.AddTransient<IMediaLoaderFilter, CustomMediaLoaderFilter>();
            };
        }

        public void Initialize(InitializationEngine context) { }

        public void Uninitialize(InitializationEngine context) { }

        public void Preload(string[] parameters){}
    }

    public class CustomMediaHierarchyRootResolver : IMediaHierarchyRootResolver
    {
        public ContentReference GetRoot()
        {
            return SiteDefinition.Current.SiteAssetsRoot;
        }
    }

    public class CustomMediaLoaderFilter : IMediaLoaderFilter
    {
        public bool ShouldLoadChildren(IContent content)
        {
            return content.ContentLink.ProviderName != "videos";
        }
    }
}
