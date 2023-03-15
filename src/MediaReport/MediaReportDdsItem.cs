using System;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace AlloyMvcTemplates.Business.Plugins;

[EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
public class MediaReportDdsItem : IDynamicData
{
    public Identity Id { get; set; }

    [EPiServerDataIndex]
    public ContentReference ContentLink { get; set; }

    public long Size { get; set; }

    public bool IsLocalContent { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public string References { get; set; }

    public int NumberOfReferences { get; set; }

    public DateTime ModifiedDate { get; set; }
}
