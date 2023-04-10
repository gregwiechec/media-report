using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Alloy.MediaReport.ScheduledJob;

/// <summary>
/// Root page used when indexing media files
/// </summary>
public interface IMediaHierarchyRootResolver
{
    ContentReference GetRoot();
}

[ServiceConfiguration(typeof(IMediaHierarchyRootResolver))]
internal class MediaHierarchyRootResolver: IMediaHierarchyRootResolver
{
    public ContentReference GetRoot() => ContentReference.RootPage;
}
