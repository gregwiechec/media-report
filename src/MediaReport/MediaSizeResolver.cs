using System.Drawing;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Alloy.MediaReport;

public interface IMediaSizeResolver
{
    (long size, int? width, int? height, string errorText) GetImageInfo(IContentMedia media);
}

[ServiceConfiguration(typeof(IMediaSizeResolver))]
public class MediaSizeResolver : IMediaSizeResolver
{
    public (long size, int? width, int? height, string errorText) GetImageInfo(IContentMedia media)
    {
        try
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

            return (size, width, height, "");
        }
        catch (DirectoryNotFoundException)
        {
            return (-1, null, null, "Blob directory not found");
        }
        catch (FileNotFoundException)
        {
            return (-1, null, null, "File not found");
        }
        catch (Exception)
        {
            return (-1, null, null, "Unknown exception when opening the file");
        }
    }
}
