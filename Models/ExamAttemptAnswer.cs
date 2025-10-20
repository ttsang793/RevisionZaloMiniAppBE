namespace backend.Models;

public partial class ExamAttemptAnswer
{
    public ulong Id { get; set; }

    public ulong ExamAttemptId { get; set; }

    public ulong ExamQuestionId { get; set; }

    public ICollection<string>? AnswerOrder { get; set; }

    public string? StudentAnswer { get; set; }

    public string AnswerType { get; set; } = null!;

    public string? IsCorrect { get; set; }

    public virtual ExamAttempt ExamAttempt { get; set; } = null!;

    public virtual ExamQuestion ExamQuestion { get; set; } = null!;
}