namespace backend.Models;

public partial class StudentHistory
{
    public ulong Id { get; set; }

    public ulong? StudentId { get; set; }

    public ulong? ExamId { get; set; }

    public DateTime Time { get; set; }

    public virtual Exam? Exam { get; set; }

    public virtual Student? Student { get; set; }
}