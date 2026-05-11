using ACE_Deployment_Tracking.Data;
using ACE_Deployment_Tracking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACE_Deployment_Tracking.Pages.DeploymentPlan
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DeploymentSchedule DeploymentSchedule { get; set; } = new();

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
            FromTime = data.FromTime;
            ToTime = data.ToTime;

            return Page();
        }

        public IActionResult OnPost()
        {
            var data = _context.DeploymentSchedules.Find(Id);

            if (data == null)
            {
                return RedirectToPage("/Deployment/DeploymentSchedule");
            }

            data.SystemName = SystemName;
            data.PlannedDate = DeploymentDate;
            data.EstimatedHour = EstimatedHour;
            data.Month = Month;
            data.Type = Type;
            data.Remark = Remark;
            data.FromTime = FromTime;
            data.ToTime = ToTime;

            data.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return RedirectToPage("/Deployment/DeploymentSchedule");
        }
    }
}