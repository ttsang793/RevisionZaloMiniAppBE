namespace backend.DTOs;

public class UserDTO
{
    public ulong? Id { get; set; }

    public string? ZaloId { get; set; }

    public string? Name { get; set; }

    public string? Avatar { get; set; }
}

public class TeacherDTO : UserDTO
{
    public string Role { get; set; } = "teacher";

    public string? SubjectId { get; set; }

    public ICollection<byte> Grades { get; set; } = [];

    public string? Introduction { get; set; }
}

public class AdminDTO : UserDTO
{
    public string Password { get; set; } = null!;
}

public class AdminErrorDTO
{
    public string? IdError { get; set; }

    public string? PasswordError { get; set; }
}