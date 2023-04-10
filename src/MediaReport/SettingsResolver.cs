using Alloy.MediaReport.ScheduledJob;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Shell;

namespace Alloy.MediaReport;

public interface ISettingsResolver
{
    public ReportSettings Get();
}

[ServiceConfiguration(typeof(ISettingsResolver))]
public class SettingsResolver: ISettingsResolver
{
    private readonly IScheduledJobRepository _scheduledJobRepository;

    private static readonly object _lock = new object();
    private static Guid? _jobId = null;

    public SettingsResolver(IScheduledJobRepository scheduledJobRepository)
    {
        _scheduledJobRepository = scheduledJobRepository;
    }

    public ReportSettings Get()
    {
        return new ReportSettings
        {
            MediaReportScheduledJobUrl = Paths.ToResource("EPiServer.Cms.UI.Admin",
                "default#/ScheduledJobs/detailScheduledJob/" + GetMediaReportScheduledJobId())
        };
    }

    private Guid GetMediaReportScheduledJobId()
    {
        if (_jobId.HasValue)
        {
            return _jobId.Value;
        }

        lock (_lock)
        {
            if (_jobId.HasValue)
            {
                return _jobId.Value;
            }

            var job = _scheduledJobRepository.List()
                .FirstOrDefault(x => x.TypeName == typeof(MediaReportScheduledJob).FullName);
            if (job != null)
            {
                _jobId = job.ID;
                return _jobId.Value;
            }
        }

        return Guid.Empty;
    }
}

public class ReportSettings
{
    public string MediaReportScheduledJobUrl { get; set; }
}
