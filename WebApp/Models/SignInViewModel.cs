using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class SignInViewModel
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email", Prompt = "Enter email address")]
    public string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password", Prompt = "Enter password")]
    public string Password { get; set; } = null!;

    public bool RememberMe { get; set; }
}
