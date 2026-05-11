using System.ComponentModel.DataAnnotations;

namespace ACE_Deployment_Tracking.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}