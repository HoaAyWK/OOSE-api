using System.ComponentModel.DataAnnotations.Schema;

namespace OpenRequest.Core.Entities;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; } = null!;
    public string JwtId { get; set; } = null!;
    public bool IsUsed { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime ExpiryDate { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
}