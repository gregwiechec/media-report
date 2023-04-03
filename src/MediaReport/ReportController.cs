using EPiServer.Shell.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alloy.MediaReport
{
    [Authorize(Roles = "CmsAdmin,WebAdmins,Administrators")]
    public class ReportController : Controller
    {
        private readonly MediaDtoConverter _mediaDtoConverter;
        private readonly IMediaReportDdsRepository _mediaReportDdsRepository;
        private readonly IMediaReportItemsSumDdsRepository _mediaReportItemsSumDdsRepository;

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
                pageIndex, pageSize, fromNumberOfReferences, toNumberOfReferences, out var totalCount).ToList();
            var result = items.Select(_mediaDtoConverter.Convert).ToList();

            var mediaReportItemsSum = _mediaReportItemsSumDdsRepository.GetSum();

            return new JsonDataResult(new {items = result, filterRange = mediaReportItemsSum, totalCount});
        }
    }
}
