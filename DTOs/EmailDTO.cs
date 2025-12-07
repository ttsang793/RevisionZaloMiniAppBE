namespace backend.DTOs;

public class EmailDTO
{
    public required ulong ToId { get; set; }

    public required string TextValue { get; set; }

    public string? ToEmail { get; set; }

    public string? Subject { get; set; }

    public string? Body { get; set; }
}
