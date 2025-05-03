using System.ComponentModel.DataAnnotations;

namespace Presentation.Models
{
    public class SignUpViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[a-zA-ZåäöÅÄÖ\s\-']+$", ErrorMessage = "Full name can only contain letters, spaces, hyphens and apostrophes.")]
        [Display(Name = "Full Name", Prompt = "Enter your full name")]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email", Prompt = "Enter email address")]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$", ErrorMessage = "Password must contain uppercase, lowercase, a number, and a special character")]
        [Display(Name = "Password", Prompt = "Enter password")]
        public string Password { get; set; } = null!;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password", Prompt = "Confirm password")]
        public string ConfirmPassword { get; set; } = null!;

        [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms.")]
        public bool Terms { get; set; }
    }
}