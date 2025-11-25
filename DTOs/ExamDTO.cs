namespace backend.DTOs;

public partial class ExamReadDTO
{
    public ulong? Id { get; set; }

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

    public byte Status { get; set; } = 1;

    public decimal? TotalPoint { get; set; }

    public string? TeacherName { get; set; }

    public string? TeacherAvatar { get; set; }

    public string? SubjectName { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }

    public DateTime? MarkedAt { get; set; }
}

public partial class ExamInsertDTO
{
    public ulong? Id { get; set; }

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

    public byte Status { get; set; } = 1;
}

public partial class ExamPartDTO
{
    public ulong Id { get; set; }

    public string? PartTitle { get; set; }

    public ICollection<string> QuestionTypes { get; set; } = [];

    public ICollection<ExamQuestionDTO> ExamQuestions { get; set; } = [];
}

public partial class ExamQuestionDTO
{
    public ulong Id { get; set; }

    public int OrderIndex { get; set; }

    public decimal Point { get; set; }

    public QuestionDTO Question { get; set; } = null!;
}


public partial class ExamQuestionsInsertDTO
{
    public ulong ExamId { get; set; }

    public byte ExamStatus { get; set; }

    public ICollection<string> PartTitles { get; set; } = [];

    public ICollection<string[]> QuestionTypes { get; set; } = [];

    public ICollection<ExamQuestionInsertItemDTO> ExamQuestions { get; set; } = [];
}

public class ExamQuestionInsertItemDTO
{
    public string PartTitle { get; set; } = null!;

    public ulong QuestionId { get; set; }

    public byte OrderIndex { get; set; }

    public decimal Point { get; set; }
}