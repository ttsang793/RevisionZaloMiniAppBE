namespace backend.Models;

public partial class Student
{
    public ulong Id { get; set; }

    public List<ulong> Favorites { get; set; } = [];

    public List<ushort> Achievements { get; set; } = [];

    public virtual ICollection<ExamAttempt> ExamAttempts { get; set; } = [];

    public virtual ICollection<Follower> Followers { get; set; } = [];

    public virtual User IdNavigation { get; set; } = null!;

    public virtual ICollection<PdfExamAttempt> PdfExamAttempts { get; set; } = [];

    public virtual ICollection<StudentHistory> StudentHistories { get; set; } = [];

    public Student() { }

    public Student(ulong id)
    {
        Id = id;
    }
}