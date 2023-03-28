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
        private IMediaReportItemsSumDdsRepository _mediaReportItemsSumDdsRepository;

        public ReportController(MediaDtoConverter mediaDtoConverter,
            IMediaReportDdsRepository mediaReportDdsRepository,
            IMediaReportItemsSumDdsRepository mediaReportItemsSumDdsRepository)
        {
            _mediaDtoConverter = mediaDtoConverter;
            _mediaReportDdsRepository = mediaReportDdsRepository;
            _mediaReportItemsSumDdsRepository = mediaReportItemsSumDdsRepository;
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

            var mediaReportItemsSum = _mediaReportItemsSumDdsRepository.GetSum();

            return new JsonDataResult(new {items = result, filterRange = mediaReportItemsSum, totalCount});
        }
    }
}
//TODO: implement filters
//TODO: add paging
