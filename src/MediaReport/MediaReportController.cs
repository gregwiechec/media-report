using System.Collections.Generic;
using System.Linq;
using EPiServer.Shell.Navigation;
using EPiServer.Shell.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlloyMvcTemplates.Business.Plugins
{
    [MenuProvider]
    public class MediaReportMenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
            var urlMenuItem1 = new UrlMenuItem("Media report", MenuPaths.Global + "/cms/admin/mediareport",
                "/MediaReport/Index")
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

    [Authorize(Roles = "CmsAdmin,WebAdmins,Administrators")]
    public class MediaReportController : Controller
    {
        private MediaDtoConverter _mediaDtoConverter;
        private IMediaReportDdsRepository _mediaReportDdsRepository;

        public MediaReportController(MediaDtoConverter mediaDtoConverter,
            IMediaReportDdsRepository mediaReportDdsRepository)
        {
            _mediaDtoConverter = mediaDtoConverter;
            _mediaReportDdsRepository = mediaReportDdsRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetMedia(int? sizeFrom, int? sizeTo, bool? isLocalContent, int? pageIndex, int? pageSize,
            int? fromNumberOfReferences, int? toNumberOfReferences)
        {
            var items = _mediaReportDdsRepository.Search(sizeFrom, sizeTo, isLocalContent,
                pageIndex, pageSize, fromNumberOfReferences, toNumberOfReferences, out int totalCount).ToList();
            var result = items.Select(_mediaDtoConverter.Convert).ToList();
            return new JsonDataResult(new { items = result, totalCount = totalCount });
        }
    }
}
//TODO: send items sum to client
//TODO: implement filters
//TODO: add paging
