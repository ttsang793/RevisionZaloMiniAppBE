using System.Linq.Expressions;

namespace backend.Models;

public partial class Student
{
    public ulong Id { get; set; }

    public ICollection<ulong> Favorites { get; set; } = [];

    public ICollection<ushort> Achievements { get; set; } = [];

    public byte Grade { get; set; }

    public ICollection<DateTime> Attendance = [];

    public ushort Streak { get; set; } = 0;

    public virtual ICollection<ExamAttempt> ExamAttempts { get; set; } = [];

    public virtual ICollection<Follower> Followers { get; set; } = [];

    public virtual User IdNavigation { get; set; } = null!;

    public virtual ICollection<PdfExamAttempt> PdfExamAttempts { get; set; } = [];

    public virtual ICollection<StudentHistory> StudentHistories { get; set; } = [];

    public Student() { }

    public Student(ulong id, byte grade)
    {
        Id = id;
        Grade = grade;
        Attendance = [DateTime.Now];
        Streak = 1;
    }
}