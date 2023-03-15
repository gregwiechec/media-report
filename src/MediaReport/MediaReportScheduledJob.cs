using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.Capabilities;
using EPiServer.Cms.Shell.UI.Rest.Internal;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using EPiServer.ServiceLocation;

namespace AlloyMvcTemplates.Business.Plugins;

[ScheduledPlugIn(GUID = "7EFCDF8F-284B-4CCB-9C9D-98775EA018AC", DisplayName = "Media report", Restartable = true, DefaultEnabled = true,
    IntervalLength = 1, IntervalType = ScheduledIntervalType.Days)]
[ServiceConfiguration(IncludeServiceAccessor = false)]
public class MediaReportScheduledJob : ScheduledJobBase
{
    private bool _isStopped = false;

    private IMediaReportDdsRepository _mediaReportDdsRepository;
    private IMediaLoader _mediaLoader;
    private IMediaReportItemsSumDdsRepository _mediaReportItemsSumDdsRepository;
    private IMediaSizeResolver _mediaSizeResolver;
    private IContentCapability _isLocalContent;
    private readonly ReferencedContentResolver _referencedContentResolver;
    private IContentLoader _contentLoader;

    public MediaReportScheduledJob(IMediaReportDdsRepository mediaReportDdsRepository, IMediaLoader mediaLoader,
        IMediaSizeResolver mediaSizeResolver, IContentCapability isLocalContent,
        IContentLoader contentLoader, ReferencedContentResolver referencedContentResolver,
        IMediaReportItemsSumDdsRepository mediaReportItemsSumDdsRepository)
    {
        _mediaReportDdsRepository = mediaReportDdsRepository;
        _mediaLoader = mediaLoader;
        _mediaSizeResolver = mediaSizeResolver;
        _isLocalContent = isLocalContent;
        _contentLoader = contentLoader;
        _referencedContentResolver = referencedContentResolver;
        _mediaReportItemsSumDdsRepository = mediaReportItemsSumDdsRepository;
        IsStoppable = true;
    }

    public override string Execute()
    {
        _isStopped = false;

        var updatedList = new List<ContentReference>();

        var itemsSum = MediaReportItemsSum.Empty();

        // add or update all media
        var mediaList = _mediaLoader.GetAllMedia();
        foreach (var content in mediaList)
        {
            if (_isStopped)
            {
                return "The job was stopped";
            }

            updatedList.Add(content.ContentLink);
            DateTime? modifiedDate = (content is IChangeTrackable changeTrackable) ? changeTrackable.Changed : null;

            var softLinks = _referencedContentResolver.GetReferenceList(content.ContentLink);
            var references = softLinks.Select(x => x.ContentLink).ToList();

            var mediaSize = _mediaSizeResolver.GetImageInfo(content);
            _mediaReportDdsRepository.CreateOrUpdate(content.ContentLink, modifiedDate, mediaSize.size,
                _isLocalContent.IsCapable(content), references, mediaSize.width, mediaSize.height);

            UpdateItemsSum(itemsSum, mediaSize.size, modifiedDate, references);
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

        //TODO: media report add statuses
        return "Job completed";
    }

    private void UpdateItemsSum(MediaReportItemsSum itemsSum, long mediaSize, DateTime? modifiedDate, List<ContentReference> references)
    {
        if (itemsSum.MinSize > mediaSize)
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
    }

    public override void Stop()
    {
        base.Stop();
        _isStopped = true;
    }
}
