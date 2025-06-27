using System.ComponentModel.DataAnnotations;
using Api.Infrastructure.Enums;
using Infrastructure.Enums;

namespace Api.Features.Register.DTO;

public class RegisterRequest
{
    [Required, MaxLength(50)]
    public string FirstName { get; set; } = default!;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = default!;

    [Required, MaxLength(100), EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = default!;

    [Required, MaxLength(15), RegularExpression(@"^\+?[1-9]\d{1,15}$", ErrorMessage = "Invalid phone number format")]
    public string Phone { get; set; } = default!;

    [Required, MaxLength(20)]
    public string Username { get; set; } = default!;

    [Required]
    public int Age { get; set; }

    [Required, MaxLength(100), RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
        ErrorMessage = "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, and one number.")]
    public required string Password { get; set; }

    [Required]
    public Gender Gender { get; set; }

    [Required]
    public AccountType AccountType { get; set; }

    [Required]
    public UserType UserType { get; set; }
}



