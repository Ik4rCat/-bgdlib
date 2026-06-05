using SQLite;

namespace bgdlib.Models;

[Table("UserSession")]
public class UserSession
{
    [PrimaryKey]
    public int Id { get; set; } = 1; // всегда одна запись

    public bool IsGuest { get; set; } = true;
    public string FirebaseUid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string IdToken { get; set; } = string.Empty;
    public DateTime TokenExpiresAt { get; set; }
}
