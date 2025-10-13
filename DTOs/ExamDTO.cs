using backend.Models;

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

    public ulong? ApprovedBy { get; set; }

    public byte State { get; set; } = 1;

    public string? TeacherName { get; set; }

    public string? SubjectName { get; set; }
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

    public ulong? ApprovedBy { get; set; }

    public byte State { get; set; } = 1;
}

public partial class ExamQuestionsInsertDTO
{
    public ulong? ExamId { get; set; }

    public ICollection<string> PartTitles { get; set; } = [];

    public ICollection<ExamQuestion> ExamQuestions { get; set; } = [];
}