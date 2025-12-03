namespace backend.DTOs;

public class EmailDTO
{
    public required string ToEmail { get; set; }

    public string? Subject { get; set; }

    public string? Body { get; set; }
}
