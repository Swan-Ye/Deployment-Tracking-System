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

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }

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
        if (CurrentPage < 1)
        {
            CurrentPage = 1;
        }

        var query = _context.DeploymentSchedules.AsQueryable();

        if (!string.IsNullOrEmpty(SystemName))
        {
            query = query.Where(x => x.SystemName == SystemName);
        }

        if (FromDate.HasValue)
        {
            query = query.Where(x => x.PlannedDate >= FromDate.Value);
        }

        if (ToDate.HasValue)
        {
            query = query.Where(x => x.PlannedDate <= ToDate.Value);
        }

        int totalRecords = await query.CountAsync();

        TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

        if (TotalPages > 0 && CurrentPage > TotalPages)
        {
            CurrentPage = TotalPages;
        }

        DeploymentSchedules = await query
            .OrderByDescending(x => x.Id)
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
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