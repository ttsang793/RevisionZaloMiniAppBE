namespace backend.DTOs;

public class StudentDTO : UserDTO
{
    public byte Grade { get; set; }

    public bool? AllowSaveHistory { get; set; }
}

public class StudentHistoryReadDTO(DateOnly date)
{
    public DateOnly Date { get; set; } = date;

    public List<ExamReadDTO> Exams { get; set; } = [];
}

public class StudentReminderDTO
{
    public TimeSpan? Hour { get; set; }

    public bool[] Date { get; set; } = [false, false, false, false, false, false, false];

    public bool IsActive { get; set; } = true;
}