namespace backend.Models;

public partial class ExamAttemptAnswer
{
    public ulong Id { get; set; }

    public ulong ExamAttemptId { get; set; }

    public ulong ExamQuestionId { get; set; }

    public ICollection<string>? AnswerOrder { get; set; }

    public ICollection<string> StudentAnswer { get; set; } = [];

    public ICollection<byte> Correct { get; set; } = [];

    public decimal? Point { get; set; }

    public virtual ExamAttempt ExamAttempt { get; set; } = null!;

    public virtual ExamQuestion ExamQuestion { get; set; } = null!;
}