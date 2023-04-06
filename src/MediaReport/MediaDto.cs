using EPiServer.Core;

namespace Alloy.MediaReport;

public class MediaReferenceDto
{
    public string ContentLink { get; set; } = "";
    public string Name { get; set; } = "";
    public string EditUrl { get; set; } = "";
}

public class MediaDto
{
    public string Name { get; set; } = "";
    public string EditUrl { get; set; } = "";

    public IEnumerable<KeyValuePair<string, string>> Hierarchy { get; set; } =
        Enumerable.Empty<KeyValuePair<string, string>>();
    public string MimeType { get; set; } = "";
    public long Size { get; set; }
    public ContentReference ContentLink { get; set; } = ContentReference.EmptyReference;
    public string ContentTypeName { get; set; } = "";
    public string PublicUrl { get; set; } = "";
    public string ThumbnailUrl { get; set; } = "";
    public bool IsLocalContent { get; set; }
    public int NumberOfReferences { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string LastModified { get; set; } = "";
    public IEnumerable<MediaReferenceDto> References { get; set; } = Enumerable.Empty<MediaReferenceDto>();
    public string ErrorText { get; set; }

    // When true, then content exists in DDS and in CMS repository
    public bool Exists { get; set; }

    public static MediaDto Empty(ContentReference contentLink)
    {
        return new MediaDto
        {
            Name = "-",
            Hierarchy = Enumerable.Empty<KeyValuePair<string, string>>(),
            MimeType = "",
            Size = 0,
            ContentLink = contentLink,
            PublicUrl = "",
            ThumbnailUrl = "",
            IsLocalContent = false,
            References = Enumerable.Empty<MediaReferenceDto>(),
            NumberOfReferences = 0,
            Exists = false,
            Height = 0,
            Width = 0,
            EditUrl = "",
            LastModified = ""
        };
    }
}
