namespace backend.Models;

public partial class SmtpSettings
{
    public required string Server { get; set; }

    public int Port { get; set; }

    public required string Username { get; set; }

    public required string Password { get; set; }
}