using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Alloy.MediaReport;

/// <summary>
/// Allow to filter media content loaded in scheduled job
/// By default external providers are not indexed
/// </summary>
public interface IMediaLoaderFilter
{
    bool ShouldLoadChildren(IContent content);
}

[ServiceConfiguration(typeof(IMediaLoaderFilter))]
internal class MediaLoaderFilter: IMediaLoaderFilter
{
    IContentProviderManager _providerManager;

    public MediaLoaderFilter(IContentProviderManager providerManager)
    {
        _providerManager = providerManager;
    }

    public bool ShouldLoadChildren(IContent content) => content.ContentLink.ProviderName == null && !_providerManager.ProviderMap.IsEntryPoint(content.ContentLink);
}
