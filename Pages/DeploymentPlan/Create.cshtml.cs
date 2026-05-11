using ACE_Deployment_Tracking.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ACE_Deployment_Tracking.Models;

namespace ACE_Deployment_Tracking.Pages.DeploymentPlan
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

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

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            var deployment = new DeploymentSchedule
            {
                SystemName = SystemName,
                PlannedDate = DeploymentDate.HasValue ? DeploymentDate.Value : null,
                EstimatedHour = EstimatedHour,
                Status = "Planned",
                Type = Type,
                Month = Month,
                Remark = Remark,
                FromTime = FromTime,
                ToTime = ToTime,

                CreatedAt = DateTime.Now
            };

            _context.DeploymentSchedules.Add(deployment);
            _context.SaveChanges();

            return RedirectToPage("/Deployment/DeploymentSchedule");
        }
    }
}