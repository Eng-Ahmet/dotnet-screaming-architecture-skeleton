using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Api.Features.Register.Model;

namespace Api.Features.Login.Model;

public class LoginAttempt
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [Required]
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;

    [Required, MaxLength(100)]
    public string IpAddress { get; set; } = string.Empty;

    public bool IsSuccessful { get; set; }

    [MaxLength(250)]
    public string? FailureReason { get; set; }
}
