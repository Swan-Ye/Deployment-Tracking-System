using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ACE_Deployment_Tracking.Data;
using ACE_Deployment_Tracking.Models;
using ACE_Deployment_Tracking.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ACE_Deployment_Tracking.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _db;
        public RegisterModel(AppDbContext db) => _db = db;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required, EmailAddress]
            public string Email { get; set; } = null!;

            [Required]
            [MinLength(8)]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$",
            ErrorMessage = "Password must contain uppercase, lowercase, number and special character.")]
            public string Password { get; set; } = null!;

            [Required]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; } = null!;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var exists = await _db.Users.AnyAsync(u => u.Email == Input.Email);
            if (exists)
            {
                ModelState.AddModelError(string.Empty, "Email already registered.");
                return Page();
            }

            var user = new User
            {
                Email = Input.Email,
                PasswordHash = PasswordHasher.Hash(Input.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return RedirectToPage("/Account/Login");
        }
    }
}