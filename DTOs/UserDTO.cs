namespace backend.DTOs;

public class UserDTO
{
    public ulong? Id { get; set; }

    public string? ZaloId { get; set; }

    public string Name { get; set; } = null!;

    public string Avatar { get; set; } = "/avatar/default.jpg";

    public string Email { get; set; } = null!;
}

public class StudentDTO : UserDTO
{
    public byte Grade { get; set; }
}

public class TeacherDTO : UserDTO
{

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