Alloy.MediaReport

Installation
============


In order to start using MediaReport you need to add it explicitly to your site.
Please add the following statement to your Startup.cs

public class Startup
{
    ...
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddMediaReport();
        ...
    }
    ...
}

Setting custom root for media
=============================

[ModuleDependency(typeof(InitializationModule))]
public class CustomMediaReportInitialization : IConfigurableModule
{
    public void ConfigureContainer(ServiceConfigurationContext context)
    {
        context.ConfigurationComplete += (o, e) =>
        {
            //Register custom implementations that should be used in favour of the default implementations
            context.Services.AddTransient<IMediaHierarchyRootResolver, CustomMediaHierarchyRootResolver>();
        };
    }
}

public class CustomMediaHierarchyRootResolver : IMediaHierarchyRootResolver
{
    public ContentReference GetRoot()
    {
        return SiteDefinition.Current.SiteAssetsRoot;
    }
}


Registering custom media report scheduled job filter that allows to not iterate children
========================================================================================
[ModuleDependency(typeof(InitializationModule))]
public class CustomMediaReportInitialization : IConfigurableModule
{
    public void ConfigureContainer(ServiceConfigurationContext context)
    {
        context.ConfigurationComplete += (o, e) =>
        {
            context.Services.AddTransient<IMediaLoaderFilter, CustomMediaLoaderFilter>();
        };
    }
}

public class CustomMediaLoaderFilter : IMediaLoaderFilter
{
    public bool ShouldLoadChildren(IContent content)
    {
        return content.ContentLink.ProviderName != "videos";
    }
}
