using ACE_Deployment_Tracking.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACE_Deployment_Tracking.Pages.DeploymentPlan
{
    public class DeploymentStartModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeploymentStartModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int Id { get; set; }
        [BindProperty]
        public TimeSpan? ActualFromTime { get; set; }
        [BindProperty]
        public TimeSpan? ActualToTime { get; set; }
        [BindProperty]
        public decimal? ActualHour { get; set; }

        public IActionResult OnGet(int id)
        {
            var data = _context.DeploymentSchedules.Find(id);

            if (data == null)
            {
                return RedirectToPage("/Deployment/DeploymentSchedule");
            }

            Id = data.Id;
            ActualFromTime = data.FromTime;
            ActualToTime = data.ToTime; 

            return Page();
        }

        public IActionResult OnPost()
        {
            var data = _context.DeploymentSchedules.Find(Id);

            if (data == null)
            {
                return RedirectToPage("/Deployment/DeploymentSchedule");
            }

            data.ActualToTime = ActualToTime;
            data.ActualFromTime = ActualFromTime;
            data.ActualHour = ActualHour;
            data.Status = "Done";

            _context.SaveChanges();

            return RedirectToPage("/Deployment/DeploymentSchedule");
        }
    }
}
