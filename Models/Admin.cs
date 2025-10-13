namespace backend.Models;

public partial class Admin
{
    public ulong Id { get; set; }

    public string? Username { get; set; }

    public string Password { get; set; } = null!;

    public virtual User IdNavigation { get; set; } = null!;
}
