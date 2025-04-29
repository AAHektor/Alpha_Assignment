using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class SignInViewModel
{

    [Required]
    [RegularExpression("^$", ErrorMessage = "Invalid email address")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email", Prompt = "Enter email address")]
    public string Email { get; set; } = null!;

    [Required]
    [RegularExpression("^$", ErrorMessage = "Invalid password")]
    [DataType(DataType.Password)]
    [Display(Name = "Password", Prompt = "Enter password")]
    public string Password { get; set; } = null!;

    public bool ConfirmPassword { get; set; }

}
