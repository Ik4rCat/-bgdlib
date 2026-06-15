using SQLite;

namespace bgdlib.Models;

[Table("UserSession")]
public class UserSession
{
    [PrimaryKey]
    public int Id { get; set; } = 1;
    public bool IsGuest { get; set; } = true;
    public int UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
}
