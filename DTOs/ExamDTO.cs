using System;
using System.Collections.Generic;

namespace backend.DTOs;

public partial class ExamReadDTO
{
    public ulong Id { get; set; }

    public byte ExamType { get; set; }

    public string DisplayType { get; set; } = null!;

    public string Title { get; set; } = null!;

    public byte Grade { get; set; }

    public ushort TimeLimit { get; set; }

    public ushort? EarlyTurnIn { get; set; }

    public ICollection<string>? PartTitle { get; set; } = [];

    public bool? AllowShowScore { get; set; }

    public bool? AllowPartSwap { get; set; }

    public bool? AllowQuestionSwap { get; set; }

    public bool? AllowAnswerSwap { get; set; }

    public ulong? TeacherId { get; set; }

    public string? TeacherName { get; set; }

    public string? SubjectId { get; set; }

    public string? SubjectName { get; set; }

    public ushort? State { get; set; }
}

public partial class ExamInsertDTO
{
    public ulong? Id { get; set; }

    public byte ExamType { get; set; }

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

    public string? SubjectId { get; set; }

    public ulong? ApprovedBy { get; set; }

    public ushort? State { get; set; }
}

/*
public partial class ExamQuestionDTO
{
    public ICollection<string> PartTitle { get; set; } = null!;

    public ICollection
}*/