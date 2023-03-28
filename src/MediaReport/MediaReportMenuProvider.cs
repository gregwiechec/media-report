using EPiServer.Shell;
using EPiServer.Shell.Navigation;

namespace Alloy.MediaReport;

[MenuProvider]
public class MediaReportMenuProvider : IMenuProvider
{
    public IEnumerable<MenuItem> GetMenuItems()
    {
        var url = Paths.ToResource(typeof(MediaReportMenuProvider), "Report/Index");

        var urlMenuItem1 = new UrlMenuItem("Media report", MenuPaths.Global + "/cms/admin/mediareport", url)
        {
            IsAvailable = context => true,
            SortIndex = 100,
        };

        return new List<MenuItem>
        {
            urlMenuItem1
        };
    }
}
