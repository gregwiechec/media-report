using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Alloy.MediaReport;

/// <summary>
/// Load all IContentMedia from CMS
/// </summary>
public interface IMediaLoader
{
    IEnumerable<IContentMedia> GetAllMedia(ContentReference rootPage);
}

[ServiceConfiguration(typeof(IMediaLoader))]
internal class MediaLoader : IMediaLoader
{
    private readonly IContentLoader _contentLoader;
    private IMediaLoaderFilter _mediaLoaderFilter;

    public MediaLoader(IContentLoader contentLoader, IMediaLoaderFilter mediaLoaderFilter)
    {
        _contentLoader = contentLoader;
        _mediaLoaderFilter = mediaLoaderFilter;
    }

    public IEnumerable<IContentMedia> GetAllMedia(ContentReference rootPage) => LoadChildren(rootPage);

    private IEnumerable<IContentMedia> LoadChildren(ContentReference parentPageId)
    {
        var children = _contentLoader.GetChildren<IContent>(parentPageId);
        foreach (var content in children)
        {
            if (content is IContentMedia media)
            {
                yield return media;
            }

            if (!_mediaLoaderFilter.ShouldLoadChildren(content))
            {
                continue;
            }

            var descendants = LoadChildren(content.ContentLink);
            foreach (var descendant in descendants)
            {
                yield return descendant;
            }
        }
    }
}
