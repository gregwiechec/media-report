using System;
using System.Linq;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;

namespace AlloyMvcTemplates.Business.Plugins;

public class MediaReportItemsSum
{
    public long MinSize { get; set; }

    public long MaxSize { get; set; }

    public DateTime MinModifiedDate { get; set; }

    public DateTime MaxModifiedDate { get; set; }

    public int MinReferences { get; set; }

    public int MaxReferences { get; set; }

    public static MediaReportItemsSum Empty()
    {
        return new MediaReportItemsSum
        {
            MinSize = int.MaxValue,
            MaxSize = 0,
            MinModifiedDate = DateTime.MaxValue,
            MaxModifiedDate = DateTime.MinValue,
            MinReferences = 0,
            MaxReferences = 0
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
        };
    }

    private DynamicDataStore GetStore()
    {
        return _dataStoreFactory.GetStore(typeof(MediaReportItemsSumDds)) ?? _dataStoreFactory.CreateStore(typeof(MediaReportItemsSumDds));
    }
}
