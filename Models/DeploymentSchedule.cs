using System.ComponentModel.DataAnnotations;

namespace ACE_Deployment_Tracking.Models
{
    public class DeploymentSchedule
    {
        public int Id { get; set; }

        [Required]
        public string SystemName { get; set; }

        [Required]
        public DateTime? PlannedDate { get; set; }

        [Required]
        public decimal? EstimatedHour { get; set; }

        public decimal? ActualHour { get; set; }

        public string? Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? Type { get; set; }

        public string? Month { get; set; }

        public string? Remark { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public TimeSpan? FromTime { get; set; }

        public TimeSpan? ToTime { get; set; }
    }
}
