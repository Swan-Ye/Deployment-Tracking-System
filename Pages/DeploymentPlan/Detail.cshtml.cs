using ACE_Deployment_Tracking.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACE_Deployment_Tracking.Pages.DeploymentPlan
{
    public class DetailModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        public string? SystemName { get; set; }

        [BindProperty]
        public DateTime? DeploymentDate { get; set; }

        [BindProperty]
        public TimeSpan? FromTime { get; set; }

        [BindProperty]
        public TimeSpan? ToTime { get; set; }

        [BindProperty]
        public decimal? EstimatedHour { get; set; }

        [BindProperty]
        public string? Month { get; set; }

        [BindProperty]
        public string? Type { get; set; }

        [BindProperty]
        public string? Remark { get; set; }

        [BindProperty]
        public string? Status { get; set; }

        public IActionResult OnGet(int id)
        {
            var data = _context.DeploymentSchedules.Find(id);

            if (data == null)
            {
                return RedirectToPage("/Deployment/DeploymentSchedule");
            }

            Id = data.Id;
            SystemName = data.SystemName;
            DeploymentDate = data.PlannedDate;
            EstimatedHour = data.EstimatedHour;
            Month = data.Month;
            Type = data.Type;
            Remark = data.Remark;
            Status = data.Status;
            FromTime = data.FromTime;
            ToTime = data.ToTime;

            return Page();
        }
    }
}