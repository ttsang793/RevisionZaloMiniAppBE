namespace backend.Models;

public partial class StudentReminder
{
    public ulong Id { get; set; }

    public ulong StudentId { get; set; }

    public TimeSpan? Hour { get; set; }

    public bool[] Date { get; set; } = [false, false, false, false, false, false, false];

    public bool IsActive { get; set; } = true;

    public virtual Student Student { get; set; } = null!;
}
