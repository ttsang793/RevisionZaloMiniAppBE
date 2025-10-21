using backend.Models;

namespace backend.DTOs;

public partial class ExamAttemptGetDTO
{
    public decimal Score { get; set; }

    public ushort Duration { get; set; }

    public string? Comment { get; set; }

    public ICollection<ExamAttemptPartDTO> ExamParts { get; set; } = [];
}

public partial class ExamAttemptPartDTO
{
    public string PartTitle { get; set; } = "";

    public ICollection<ExamAttemptAnswerGetDTO> ExamAttemptAnswers { get; set; } = [];
}

public class ExamAttemptAnswerGetDTO
{
    public decimal Point { get; set; }

    public QuestionDTO Question { get; set; } = null!;

    public ICollection<string>? AnswerOrder { get; set; }

    public string? StudentAnswer { get; set; }

    public string AnswerType { get; set; } = null!;

    public string? IsCorrect { get; set; }
}

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