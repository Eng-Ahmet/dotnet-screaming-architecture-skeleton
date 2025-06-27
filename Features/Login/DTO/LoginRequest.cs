using System.ComponentModel.DataAnnotations;

namespace Features.Login.DTO;

public class LoginRequest
{
    [Required(ErrorMessage = "Email is required."), StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters."),
     EmailAddress(ErrorMessage = "Invalid email format.")]
    public required string Email { get; set; }

    [Required, MaxLength(100), RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
        ErrorMessage = "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, and one number.")]
    public required string Password { get; set; }


}