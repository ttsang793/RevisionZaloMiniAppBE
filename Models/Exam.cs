namespace backend.Models;

public partial class Exam
{
    public ulong Id { get; set; }

    public string ExamType { get; set; } = null!;

    public string DisplayType { get; set; } = null!;

    public string Title { get; set; } = null!;

    public byte Grade { get; set; }

    public ushort TimeLimit { get; set; }

    public ushort? EarlyTurnIn { get; set; }

    public bool? AllowShowScore { get; set; }

    public bool? AllowPartSwap { get; set; }

    public bool? AllowQuestionSwap { get; set; }

    public bool? AllowAnswerSwap { get; set; }

    public ulong? TeacherId { get; set; }

    public string SubjectId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }

    public byte Status { get; set; } = 0;

    public virtual ICollection<Comment> Comments { get; set; } = [];

    public virtual ICollection<ExamAttempt> ExamAttempts { get; set; } = [];

    public virtual ICollection<PdfExamCode> PdfExamCodes { get; set; } = [];

    public virtual ICollection<StudentHistory> StudentHistories { get; set; } = new List<StudentHistory>();

    public virtual Subject Subject { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}
