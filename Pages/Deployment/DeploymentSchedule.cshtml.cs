using ACE_Deployment_Tracking.Data;
using ACE_Deployment_Tracking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class DeploymentScheduleModel : PageModel
{
    private readonly AppDbContext _context;

    public DeploymentScheduleModel(AppDbContext context)
    {
        _context = context;
    }

    public List<DeploymentSchedule> DeploymentSchedules { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SystemName { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    public List<string> SystemNames { get; set; } = new()
    {
        "ACE IS - Contract & Project Management System",
        "ACE IS - Integrated Accounting Systems",
        "ACE IS - Human Resource Management Systems",
        "Bitrix 24 CRM System",
        "Backlog System",
        "ACE Issue Tracking System"
    };

    public async Task OnGetAsync()
    {
        var query = _context.DeploymentSchedules.AsQueryable();
        if (!string.IsNullOrEmpty(SystemName))
        {
            query = query.Where(x => x.SystemName == SystemName);
        }

        if (!string.IsNullOrEmpty(Status))
        {
            query = query.Where(x => x.Status == Status);
        }

        if (FromDate.HasValue)
        {
            query = query.Where(x => x.PlannedDate >= FromDate.Value);
        }

        if (ToDate.HasValue)
        {
            query = query.Where(x => x.PlannedDate <= ToDate.Value);
        }

        DeploymentSchedules = await query
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var schedule = await _context.DeploymentSchedules.FindAsync(id);

        if (schedule != null)
        {
            schedule.Status = "Canceled";

            _context.DeploymentSchedules.Update(schedule);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}