using backend.Models;

namespace backend.DTOs;

public partial class ExamAttemptStatDTO
{
    public decimal? MaxTotalPoint { get; set; }

    public ushort? Duration { get; set; }

    public int Count { get; set; } = 0;
}

public partial class ExamAttemptGetDTO
{
    public ulong? Id { get; set; }

    public string? ExamTitle { get; set; }

    public decimal TotalPoint { get; set; }

    public DateTime SubmittedAt { get; set; }

    public DateTime? MarkedAt { get; set; }

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

    public ICollection<string> StudentAnswer { get; set; } = [];

    public ICollection<sbyte> Correct { get; set; } = [];
}

public class ExamAttemptDTO
{
    public ulong? Id { get; set; }

    public ulong? ExamId { get; set; }

    public ulong? StudentId { get; set; }

    public decimal TotalPoint { get; set; }

    public DateTime? StartedAt { get; set; }

    public bool? IsPractice { get; set; } = false;

    public ICollection<ulong>? PartOrder { get; set; } = [];

    public ICollection<ExamAttemptAnswerDTO> ExamAttemptAnswers { get; set; } = [];

    public string? Comment { get; set; }
}

public class ExamAttemptAnswerDTO
{
    public ulong? ExamQuestionId { get; set; }

    public ICollection<string>? AnswerOrder { get; set; }

    public ICollection<string>? StudentAnswer { get; set; } = [];

    public ICollection<sbyte> Correct { get; set; } = [];

    public decimal Point { get; set; }
}

public partial class PdfExamAttemptDTO
{
    public ulong ExamId { get; set; }

    public ulong StudentId { get; set; }

    public decimal TotalPoint { get; set; }

    public DateTime StartedAt { get; set; }

    public ulong PdfExamCodeId { get; set; }

    public ICollection<string> StudentAnswer { get; set; } = [];

    public ICollection<decimal> PointBoard { get; set; } = [];
}