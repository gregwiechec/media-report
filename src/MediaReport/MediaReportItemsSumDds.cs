using EPiServer.Data;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;

namespace Alloy.MediaReport;

public class MediaReportItemsSum
{
    public long MinSize { get; set; }

    public long MaxSize { get; set; }

    public DateTime MinModifiedDate { get; set; }

    public DateTime MaxModifiedDate { get; set; }

    public int MinReferences { get; set; }

    public int MaxReferences { get; set; }

    public bool HasErrors { get; set; }

    public static MediaReportItemsSum Empty()
    {
        return new MediaReportItemsSum
        {
            MinSize = int.MaxValue,
            MaxSize = 0,
            MinModifiedDate = DateTime.MaxValue,
            MaxModifiedDate = DateTime.MinValue,
            MinReferences = 0,
            MaxReferences = 0,
            HasErrors = false
        };
    }
}

[EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
public class MediaReportItemsSumDds : MediaReportItemsSum, IDynamicData
{
    public Identity Id { get; set; }
}

public interface IMediaReportItemsSumDdsRepository
{
    void SetSum(MediaReportItemsSum itemsSum);

    MediaReportItemsSum GetSum();
}

[ServiceConfiguration(typeof(IMediaReportItemsSumDdsRepository))]
class MediaReportItemsSumDdsRepository : IMediaReportItemsSumDdsRepository
{
    private readonly DynamicDataStoreFactory _dataStoreFactory;

    public MediaReportItemsSumDdsRepository(DynamicDataStoreFactory dataStoreFactory)
    {
        _dataStoreFactory = dataStoreFactory;
    }

    public void SetSum(MediaReportItemsSum itemsSum)
    {
        var store = GetStore();
        var item = store.Items<MediaReportItemsSumDds>().FirstOrDefault();
        if (item == null)
        {
            item = new MediaReportItemsSumDds();
        }

        item.MinSize = itemsSum.MinSize;
        item.MaxSize = itemsSum.MaxSize;
        item.MinModifiedDate = itemsSum.MinModifiedDate;
        item.MaxModifiedDate = itemsSum.MaxModifiedDate;
        item.MinReferences = itemsSum.MinReferences;
        item.MaxReferences = itemsSum.MaxReferences;
        item.HasErrors = itemsSum.HasErrors;

        store.Save(item);
    }

    public MediaReportItemsSum GetSum()
    {
        var store = GetStore();

        var item = store.Items<MediaReportItemsSumDds>().FirstOrDefault();

        if (item == null)
        {
            return MediaReportItemsSum.Empty();
        }

        return new MediaReportItemsSum
        {
            MinSize = item.MinSize,
            MaxSize = item.MaxSize,
            MinModifiedDate = item.MinModifiedDate,
            MaxModifiedDate = item.MaxModifiedDate,
            MinReferences = item.MinReferences,
            MaxReferences = item.MaxReferences,
            HasErrors = item.HasErrors,
        };
    }

    private DynamicDataStore GetStore()
    {
        return _dataStoreFactory.GetStore(typeof(MediaReportItemsSumDds)) ?? _dataStoreFactory.CreateStore(typeof(MediaReportItemsSumDds));
    }
}
