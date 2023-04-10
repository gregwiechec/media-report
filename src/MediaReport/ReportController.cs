using EPiServer.Shell.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alloy.MediaReport;

[Authorize(Roles = "CmsAdmin,WebAdmins,Administrators")]
public class ReportController : Controller
{
    private readonly MediaDtoConverter _mediaDtoConverter;
    private readonly IMediaReportDdsRepository _mediaReportDdsRepository;
    private readonly IMediaReportItemsSumDdsRepository _mediaReportItemsSumDdsRepository;
    private readonly ISettingsResolver _settingsResolver;

    public ReportController(MediaDtoConverter mediaDtoConverter,
        IMediaReportDdsRepository mediaReportDdsRepository,
        IMediaReportItemsSumDdsRepository mediaReportItemsSumDdsRepository, ISettingsResolver settingsResolver)
    {
        _mediaDtoConverter = mediaDtoConverter;
        _mediaReportDdsRepository = mediaReportDdsRepository;
        _mediaReportItemsSumDdsRepository = mediaReportItemsSumDdsRepository;
        _settingsResolver = settingsResolver;
    }

    public IActionResult Index()
    {
        return View(_settingsResolver.Get());
    }

    public JsonResult GetMedia(int? sizeFrom, int? sizeTo, bool? isLocalContent, bool? showErrors, int? pageIndex, int? pageSize,
        int? fromNumberOfReferences, int? toNumberOfReferences, string sortBy, string sortOrder)
    {
        var items = _mediaReportDdsRepository.Search(sizeFrom, sizeTo, isLocalContent, showErrors,
            pageIndex, pageSize, fromNumberOfReferences, toNumberOfReferences, sortBy, sortOrder, out var totalCount).ToList();
        var result = items.Select(_mediaDtoConverter.Convert).ToList();

        var mediaReportItemsSum = _mediaReportItemsSumDdsRepository.GetSum();

        return new JsonDataResult(new {items = result, filterRange = mediaReportItemsSum, totalCount});
    }
}
