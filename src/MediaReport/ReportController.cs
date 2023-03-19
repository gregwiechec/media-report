using EPiServer.Shell.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlloyMvcTemplates.Business.Plugins
{
    [Authorize(Roles = "CmsAdmin,WebAdmins,Administrators")]
    public class ReportController : Controller
    {
        private MediaDtoConverter _mediaDtoConverter;
        private IMediaReportDdsRepository _mediaReportDdsRepository;

        public ReportController(MediaDtoConverter mediaDtoConverter,
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
