using System.Collections.Generic;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace AlloyMvcTemplates.Business.Plugins;

/// <summary>
/// Load all IContentMedia from CMS
/// </summary>
public interface IMediaLoader
{
    IEnumerable<IContentMedia> GetAllMedia();
}

[ServiceConfiguration(typeof(IMediaLoader))]
public class MediaLoader : IMediaLoader
{
    private readonly IContentLoader _contentLoader;

    public MediaLoader(IContentLoader contentLoader)
    {
        _contentLoader = contentLoader;
    }

    public IEnumerable<IContentMedia> GetAllMedia()
    {
        return LoadChildren(ContentReference.RootPage);
    }

    private IEnumerable<IContentMedia> LoadChildren(ContentReference parentPageId)
    {
        var children = _contentLoader.GetChildren<IContent>(parentPageId);
        foreach (var content in children)
        {
            if (content is IContentMedia media)
            {
                yield return media;
            }

            var descendants = LoadChildren(content.ContentLink);
            foreach (var descendant in descendants)
            {
                yield return descendant;
            }
        }
    }
}
