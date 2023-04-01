using EPiServer.Core;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;

namespace Alloy.MediaReport;

public interface IMediaReportDdsRepository
{
    void Delete(ContentReference contentLink);

    void CreateOrUpdate(ContentReference contentLink, DateTime? modifiedDate, long size, bool isLocalContent,
        IEnumerable<ContentReference> references, int? height, int? width);

    IEnumerable<MediaReportDdsItem> ListAll();

    IEnumerable<MediaReportDdsItem> Search(int? sizeFrom, int? sizeTo, bool? isLocalContent, int? pageIndex,
        int? pageSize, int? fromNumberOfReferences, int? toNumberOfReferences, out int totalCount);
}

[ServiceConfiguration(typeof(IMediaReportDdsRepository))]
public class MediaReportDdsRepository : IMediaReportDdsRepository
{
    private readonly DynamicDataStoreFactory _dataStoreFactory;

    public MediaReportDdsRepository(DynamicDataStoreFactory dataStoreFactory)
    {
        _dataStoreFactory = dataStoreFactory;
    }

    public IEnumerable<MediaReportDdsItem> ListAll()
    {
        var store = GetStore();
        return store.LoadAll<MediaReportDdsItem>();
    }

    public IEnumerable<MediaReportDdsItem> Search(int? sizeFrom, int? sizeTo, bool? isLocalContent, int? pageIndex,
        int? pageSize, int? fromNumberOfReferences, int? toNumberOfReferences, out int totalCount)
    {
        var store = GetStore();
        IQueryable<MediaReportDdsItem> items = store.Items<MediaReportDdsItem>();
        if (sizeFrom.HasValue)
        {
            items = items.Where(x => x.Size >= sizeFrom);
        }

        if (sizeTo.HasValue)
        {
            items = items.Where(x => x.Size <= sizeTo);
        }

        if (isLocalContent.HasValue)
        {
            if (isLocalContent.Value)
            {
                items = items.Where(x => x.IsLocalContent);
            }
            else
            {
                items = items.Where(x => !x.IsLocalContent);
            }
            
        }

        if (fromNumberOfReferences.HasValue)
        {
            items = items.Where(x => x.NumberOfReferences >= fromNumberOfReferences);
        }

        if (toNumberOfReferences.HasValue)
        {
            items = items.Where(x => x.NumberOfReferences <= toNumberOfReferences);
        }

        totalCount = items.Count();

        if (pageSize.HasValue && pageIndex.HasValue)
        {
            items = items
                .Skip(pageIndex.Value * pageSize.Value)
                .Take(pageSize.Value);
        }

        return items.ToList();
    }

    public void Delete(ContentReference contentLink)
    {
        var store = GetStore();
        var item = store.Items<MediaReportDdsItem>().FirstOrDefault(x => x.ContentLink == contentLink);
        if (item == null)
        {
            throw new InvalidOperationException($"Cannot find item for {contentLink}");
        }
        store.Delete(item);
    }

    public void CreateOrUpdate(ContentReference contentLink, DateTime? modifiedDate, long size, bool isLocalContent,
        IEnumerable<ContentReference> references, int? height, int? width)
    {
        var store = GetStore();

        var item = store.Items<MediaReportDdsItem>().FirstOrDefault(x => x.ContentLink == contentLink);

        if (item == null)
        {
            item = new MediaReportDdsItem
            {
                ContentLink = contentLink,
            };
        }

        var referencesList = references.ToList();

        item.ModifiedDate = modifiedDate ?? DateTime.MinValue;
        item.Height = height ?? -1;
        item.Width = width ?? -1;
        item.Size = size;
        item.IsLocalContent = isLocalContent;
        item.References = string.Join(",", referencesList.Select(x => x.ToString()));
        item.NumberOfReferences = referencesList.Count;

        store.Save(item);
    }

    private DynamicDataStore GetStore()
    {
        return _dataStoreFactory.GetStore(typeof(MediaReportDdsItem)) ?? _dataStoreFactory.CreateStore(typeof(MediaReportDdsItem));
    }
}
