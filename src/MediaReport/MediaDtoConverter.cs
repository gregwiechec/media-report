﻿using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Cms.Shell.UI.Rest.Capabilities;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Editor;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using EPiServer.Web;

namespace Alloy.MediaReport;

[ServiceConfiguration(typeof(MediaDtoConverter))]
public class MediaDtoConverter
{
    private readonly IContentLoader _contentLoader;
    private readonly IContentCapability _isLocalContent;
    private readonly IContentTypeRepository _contentTypeLoader;
    private readonly IUrlResolver _urlResolver;
    private readonly ITemplateResolver _templateResolver;
    private readonly List<(int id, string name)> _contentTypes = new List<(int id, string name)>();

    public MediaDtoConverter(IEnumerable<IContentCapability> capabilities,
        IContentLoader contentLoader,
        IContentTypeRepository contentTypeLoader, IUrlResolver urlResolver, ITemplateResolver templateResolver)
    {
        _contentLoader = contentLoader;
        _contentTypeLoader = contentTypeLoader;
        _urlResolver = urlResolver;
        _templateResolver = templateResolver;
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
            ContentTypeName = GetContentTypeName(contentMedia.ContentTypeID),
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
            ThumbnailUrl = contentMedia.ThumbnailUrl(_urlResolver, _templateResolver),
            IsLocalContent = _isLocalContent.IsCapable(contentMedia),
            ErrorText = ddsItem.ErrorText
        };
        return result;
    }


    private string GetContentTypeName(int contentTypeId)
    {
        if (!_contentTypes.Any())
        {
            _contentTypes.AddRange(_contentTypeLoader.List().Select(x => (x.ID, x.LocalizedName)).ToList());
        }

        return _contentTypes.FirstOrDefault(x => x.id == contentTypeId).name;
    }

    private IEnumerable<MediaReferenceDto> ParseReferences(IEnumerable<string> references)
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
