using System.Drawing;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Alloy.MediaReport;

public interface IMediaSizeResolver
{
    (long size, int? width, int? height) GetImageInfo(IContentMedia media);
}

[ServiceConfiguration(typeof(IMediaSizeResolver))]
public class MediaSizeResolver : IMediaSizeResolver
{
    public (long size, int? width, int? height) GetImageInfo(IContentMedia media)
    {
        using var stream = media.BinaryData.OpenRead();
        int? height = null;
        int? width = null;
        var size = stream.Length;
        try
        {
            using var image = Image.FromStream(stream);
            height = image.Height;
            width = image.Width;
        }
        catch
        {
            // ignored
        }

        return (size, width, height);
    }
}
