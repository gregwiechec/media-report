# media-report
Extension for Optimizely that shows media report

## Introduction

The media report displays a list with useful statistics about the files and allows to sort and filter the results.

In the list you can find the following information:

* **Name and thumbnail** – thumbnail with preview in tooltip and link the edit mode
* **Last modified date** – shows when the content was last modified
* **Path** – path to content in folder structure
* **Size and dimensions** – size of the content media. When content is an image, then also the dimensions are displayed
* **Is local content** – shows checked icon when content media is stored in “For this page” folder
* **List of refrenced content** -list of all contents that have links to the media

In addition, the report has a few filters:
* **Size** – allow to find very big content media files
* **References** – allow to find content media with no references or very popular media
* **Local items** – allow to find content media stored in “For this page” folder

### How to initialize the report

The report does not display data coming directly from ContentRepository. File size, number of references and other statistics cannot be calculated quickly. Therefore, the report data is stored and read from the DDS, which is written to in Media Report ScheduledJob.
It should be noted that the data displayed in the report are not always up to date and depends on how often the scheduled job is run.

### Setting root

By default, when building a report, a content media search starts from Root. This behavior can be changed by implementing IMediaHierarchyRootResolver service.

```c#
[ModuleDependency(typeof(InitializationModule))]
public class CustomMediaReportInitialization : IConfigurableModule
{
    public void ConfigureContainer(ServiceConfigurationContext context)
    {
        context.ConfigurationComplete += (o, e) =>
        {
            context.Services.AddTransient<IMediaHierarchyRootResolver, CustomMediaHierarchyRootResolver>();
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
```

### Setting media filter

Not all files should be saved in DDS. For example, if there is an external media provider registered in the media tree, then we should not search it. By implementing the IMediaLoaderFilter interface, we can define how the media should be filtered.

```c#
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
 
    public void Initialize(InitializationEngine context) { }
 
    public void Uninitialize(InitializationEngine context) { }
 
    public void Preload(string[] parameters){}
}
 
public class CustomMediaLoaderFilter : IMediaLoaderFilter
{
    public bool ShouldLoadChildren(IContent content)
    {
        return content.ContentLink.ProviderName != "videos";
    }
}
```
## Develop

* setup 
* build
* buildclient
* iisexpress
