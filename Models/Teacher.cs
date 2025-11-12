namespace backend.Models;

public partial class Teacher
{
    public ulong Id { get; set; }

    public string? SubjectId { get; set; }

    public ICollection<byte> Grades { get; set; } = [];

    public string? Introduction { get; set; }

    public virtual ICollection<Exam> Exams { get; set; } = [];

    public virtual ICollection<Follower> Followers { get; set; } = [];

    public virtual User IdNavigation { get; set; } = null!;

    public virtual Subject? Subject { get; set; }

    public void NullifyTeacher()
    {
        SubjectId = null;
        Grades = [];
        Introduction = null;
    }
}
