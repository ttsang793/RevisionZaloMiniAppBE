namespace backend.Models;

public partial class ExamAttempt
{
    public ulong Id { get; set; }

    public ulong ExamId { get; set; }

    public ulong StudentId { get; set; }

    public decimal? TotalPoint { get; set; }

    public DateTime StartedAt { get; set; }

    public ushort Duration { get; set; }

    public DateTime SubmittedAt { get; set; }

    public DateTime? MarkedAt { get; set; }

    public string? Comment { get; set; }

    public ICollection<ulong> PartOrder { get; set; } = [];

    public virtual Exam Exam { get; set; } = null!;

    public virtual ICollection<ExamAttemptAnswer> ExamAttemptAnswers { get; set; } = [];

    public virtual Student Student { get; set; } = null!;
}
