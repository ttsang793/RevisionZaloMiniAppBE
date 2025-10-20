namespace backend.DTOs;

public class ExamAttemptDTO
{
    public ulong ExamId { get; set; }

    public ulong StudentId { get; set; }

    public decimal? Score { get; set; }

    public DateTime StartedAt { get; set; }

    public bool IsPractice { get; set; } = false;

    public ICollection<ulong> PartOrder { get; set; } = [];

    public ICollection<ExamAttemptAnswerDTO> ExamAttemptAnswers { get; set; } = [];
}

public class ExamAttemptAnswerDTO
{
    public ulong ExamQuestionId { get; set; }

    public ICollection<string>? AnswerOrder { get; set; }

    public string? StudentAnswer { get; set; }

    public string AnswerType { get; set; } = null!;

    public string? IsCorrect { get; set; }
}