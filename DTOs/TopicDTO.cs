namespace backend.DTOs;

public class TopicDTO
{
    public string? Id { get; set; }
    public string Name { get; set; } = null!;
    public List<byte> Grades { get; set; } = [];
    public string SubjectId { get; set; } = null!;
}
