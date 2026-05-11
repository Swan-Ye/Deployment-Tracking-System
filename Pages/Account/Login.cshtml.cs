using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ACE_Deployment_Tracking.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using ACE_Deployment_Tracking.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace ACE_Deployment_Tracking.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _db;
        public LoginModel(AppDbContext db) => _db = db;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required, EmailAddress]
            public string Email { get; set; } = null!;

            [Required]
            public string Password { get; set; } = null!;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == Input.Email);
            if (user == null || !PasswordHasher.Verify(Input.Password, user.PasswordHash))
            {
                ErrorMessage = "Invalid credentials.";
                return Page();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToPage("/Index");
        }
    }
}