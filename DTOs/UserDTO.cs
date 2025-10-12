namespace backend.DTOs;

public class UserDTO
{
    public ulong? Id { get; set; }
    public string? Name { get; set; }
}

public class TeacherDTO : UserDTO
{
    public string Role { get; set; } = "teacher";

    public string? SubjectId { get; set; }

    public ICollection<byte> Grades { get; set; } = [];

    public string? Introduction { get; set; }
}
