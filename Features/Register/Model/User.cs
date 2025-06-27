using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Api.Features.Register.DTO;
using Api.Infrastructure.Enums;
using Api.Utils;
using Infrastructure.Enums;

namespace Api.Features.Register.Model;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50), RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name can only contain letters and spaces")]
    public required string FirstName { get; set; }

    [Required, MaxLength(100), RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name can only contain letters and spaces")]
    public required string LastName { get; set; }

    [Required, MaxLength(100), EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; set; }

    [Required, MaxLength(15), RegularExpression(@"^\+?[1-9]\d{1,15}$", ErrorMessage = "Invalid phone number format")]
    public required string Phone { get; set; }

    [Required, MaxLength(20)]
    public required string Username { get; set; }

    [Required]
    public int Age { get; set; }

    [Required]
    public required string Password { get; set; }

    [Required]
    public Gender Gender { get; set; }

    [Required]
    public AccountStatus Status { get; set; }

    [Required]
    public AccountType AccountType { get; set; }

    [Required]
    public UserType UserType { get; set; }

    [Required]
    public bool IsVerified { get; set; }

    [MaxLength(100)]
    public string? VerificationCode { get; set; }

    public string? Preferences { get; set; }

    [Required]
    public DateTime LastLogin { get; set; }

    [MaxLength(50)]
    public string? IpAddress { get; set; }

    [MaxLength(50)]
    public string? LastIpAddress { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }



    // Factory method
    public static User Create(RegisterRequest request)
    {
        return new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Username = request.Username,
            Age = request.Age,
            Password = HashHelper.CreateHash(request.Password), // Hash the password
            Gender = request.Gender,
            Status = AccountStatus.Inactive,
            AccountType = request.AccountType,
            UserType = request.UserType,
            IsVerified = false,
            CreatedAt = DateTime.UtcNow,
            LastLogin = DateTime.UtcNow
        };
    }

    // Update methods
    public void UpdateLastLogin(string? ipAddress)
    {
        LastLogin = DateTime.UtcNow;
        LastIpAddress = ipAddress;
    }

    public void UpdateVerificationStatus(bool isVerified, string? verificationCode = null)
    {
        IsVerified = isVerified;
        VerificationCode = verificationCode;
        Status = isVerified ? AccountStatus.Active : AccountStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePreferences(object preferences)
    {
        Preferences = JsonSerializer.Serialize(preferences);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(AccountStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAccountType(AccountType accountType)
    {
        AccountType = accountType;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateUserType(UserType userType)
    {
        UserType = userType;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePersonalInfo(string firstName, string lastName, string phone, string email, string password)
    {
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Email = email;
        Password = password;
        UpdatedAt = DateTime.UtcNow;
    }
}






