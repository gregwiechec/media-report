using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.Capabilities;
using EPiServer.Cms.Shell.UI.Rest.Internal;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using EPiServer.ServiceLocation;

namespace Alloy.MediaReport;

[ScheduledPlugIn(GUID = "7EFCDF8F-284B-4CCB-9C9D-98775EA018AC", DisplayName = "Media report", Description = "Builds media report",
    Restartable = true, DefaultEnabled = true, IntervalLength = 1, IntervalType = ScheduledIntervalType.Days)]
[ServiceConfiguration(IncludeServiceAccessor = false)]
public class MediaReportScheduledJob : ScheduledJobBase
{
    private bool _isStopped = false;

    private readonly IMediaReportDdsRepository _mediaReportDdsRepository;
    private readonly IMediaLoader _mediaLoader;
    private readonly IMediaReportItemsSumDdsRepository _mediaReportItemsSumDdsRepository;
    private readonly IMediaSizeResolver _mediaSizeResolver;
    private readonly IContentCapability _isLocalContent;
    private readonly ReferencedContentResolver _referencedContentResolver;
    private readonly IContentLoader _contentLoader;

    public MediaReportScheduledJob(IMediaReportDdsRepository mediaReportDdsRepository, IMediaLoader mediaLoader,
        IMediaSizeResolver mediaSizeResolver, IEnumerable<IContentCapability> capabilities,
        IContentLoader contentLoader, ReferencedContentResolver referencedContentResolver,
        IMediaReportItemsSumDdsRepository mediaReportItemsSumDdsRepository)
    {
        _mediaReportDdsRepository = mediaReportDdsRepository;
        _mediaLoader = mediaLoader;
        _mediaSizeResolver = mediaSizeResolver;
        _isLocalContent = capabilities.Single(x => x.Key == "isLocalContent");
        _contentLoader = contentLoader;
        _referencedContentResolver = referencedContentResolver;
        _mediaReportItemsSumDdsRepository = mediaReportItemsSumDdsRepository;
        IsStoppable = true;
    }

    public override string Execute()
    {
        var countProcessedItems = 0;
        _isStopped = false;

        var updatedList = new List<ContentReference>();

        var itemsSum = MediaReportItemsSum.Empty();

        // add or update all media
        var mediaList = _mediaLoader.GetAllMedia();
        foreach (var content in mediaList)
        {
            countProcessedItems++;
            if (_isStopped)
            {
                return "The job was stopped";
            }

            updatedList.Add(content.ContentLink);
            DateTime? modifiedDate = (content is IChangeTrackable changeTrackable) ? changeTrackable.Changed : null;

            var softLinks = _referencedContentResolver.GetReferenceList(content.ContentLink);
            var references = softLinks.Select(x => x.ContentLink).ToList();

            var (size, width, height, errorText) = _mediaSizeResolver.GetImageInfo(content);
            var isLocalContent = _isLocalContent.IsCapable(content);
            _mediaReportDdsRepository.CreateOrUpdate(content.ContentLink, content.Name, modifiedDate, size,
                isLocalContent, references, width, height, errorText);

            UpdateItemsSum(itemsSum, size, modifiedDate, references, errorText);

            if (countProcessedItems % 100 == 0)
            {
                OnStatusChanged($"Processing media items ({countProcessedItems})");
            }
        }
        _mediaReportItemsSumDdsRepository.SetSum(itemsSum);

        // remove media from report
        var allDdsItems = _mediaReportDdsRepository.ListAll().ToList();
        foreach (var mediaReportDdsItem in allDdsItems)
        {
            if (_isStopped)
            {
                return "The job was stopped";
            }
            if (updatedList.Contains(mediaReportDdsItem.ContentLink))
            {
                continue;
            }
            if (!_contentLoader.TryGet<IContent>(mediaReportDdsItem.ContentLink, out _))
            {
                _mediaReportDdsRepository.Delete(mediaReportDdsItem.ContentLink);
            }
        }

        return $"Job completed ({countProcessedItems} media content processed)";
    }

    private void UpdateItemsSum(MediaReportItemsSum itemsSum, long mediaSize, DateTime? modifiedDate,
        List<ContentReference> references, string errorText)
    {
        if (itemsSum.MinSize > mediaSize && mediaSize != IMediaSizeResolver.CannotReadMediaSize)
        {
            itemsSum.MinSize = mediaSize;
        }
        if (itemsSum.MaxSize < mediaSize)
        {
            itemsSum.MaxSize = mediaSize;
        }

        if (modifiedDate.HasValue)
        {
            if (itemsSum.MinModifiedDate > modifiedDate)
            {
                itemsSum.MinModifiedDate = modifiedDate.Value;
            }

            if (itemsSum.MaxModifiedDate < modifiedDate)
            {
                itemsSum.MaxModifiedDate = modifiedDate.Value;
            }
        }

        if (itemsSum.MinReferences > references.Count)
        {
            itemsSum.MinReferences = references.Count;
        }
        if (itemsSum.MaxReferences < references.Count)
        {
            itemsSum.MaxReferences = references.Count;
        }

        if (!string.IsNullOrWhiteSpace(errorText))
        {
            itemsSum.HasErrors = true;
        }
    }

    public override void Stop()
    {
        base.Stop();
        _isStopped = true;
    }
}
