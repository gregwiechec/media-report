using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Cms.Shell.UI.Rest.Capabilities;
using EPiServer.Core;
using EPiServer.Editor;
using EPiServer.ServiceLocation;

namespace Alloy.MediaReport;

[ServiceConfiguration(typeof(MediaDtoConverter))]
public class MediaDtoConverter
{
    private IContentLoader _contentLoader;
    private IContentCapability _isLocalContent;

    public MediaDtoConverter(IEnumerable<IContentCapability> capabilities, IContentLoader contentLoader)
    {
        _contentLoader = contentLoader;
        _isLocalContent = capabilities.Single(x => x.Key == "isLocalContent");
    }

    public MediaDto Convert(MediaReportDdsItem ddsItem)
    {
        if (!_contentLoader.TryGet<IContentMedia>(ddsItem.ContentLink, out var contentMedia))
        {
            return MediaDto.Empty(ddsItem.ContentLink);
        }

        var hierarchy = _contentLoader
            .GetAncestors(contentMedia.ContentLink)
            .Select(x => new KeyValuePair<string, string>(x.ContentLink.ToString(), x.Name))
            .Reverse()
            .Skip(1);

        var result = new MediaDto
        {
            ContentLink = contentMedia.ContentLink,
            Name = contentMedia.Name,
            EditUrl = PageEditing.GetEditUrlForLanguage(contentMedia.ContentLink, contentMedia.LanguageBranch()),
            Size = ddsItem.Size,
            Width = ddsItem.Width,
            Height = ddsItem.Height,
            LastModified = ddsItem.ModifiedDate == DateTime.MinValue ? "": ddsItem.ModifiedDate.ToString("yyyy-MM-dd hh:mm:ss"),
            References = ParseReferences(ddsItem.References.Split(',')),
            NumberOfReferences = ddsItem.NumberOfReferences,
            Exists = true,
            Hierarchy = hierarchy,
            MimeType = contentMedia.MimeType,
            PublicUrl = contentMedia.PublicUrl(),
            ThumbnailUrl = contentMedia.ThumbnailUrl(),
            IsLocalContent = _isLocalContent.IsCapable(contentMedia),
        };
        using var stream = contentMedia.BinaryData.OpenRead();
        result.Size = stream.Length;
        return result;
    }

    private IEnumerable<MediaReferenceDto> ParseReferences(string[] references)
    {
        foreach (var reference in references)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                continue;
            }

            if (!ContentReference.TryParse(reference, out var contentLink))
            {
                continue;
            }

            if (!_contentLoader.TryGet<IContent>(contentLink, out var content))
            {
                continue;
            }

            yield return new MediaReferenceDto
            {
                ContentLink = content.ContentLink.ToString(),
                Name = content.Name,
                EditUrl = PageEditing.GetEditUrlForLanguage(content.ContentLink, content.LanguageBranch())
            };
        }
    }
}
