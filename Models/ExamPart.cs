using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class ExamPart
{
    public ulong Id { get; set; }

    public ulong ExamId { get; set; }

    public byte PartIndex { get; set; }

    public string PartTitle { get; set; } = null!;

    public ICollection<string> QuestionTypes { get; set; } = [];

    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = [];
}
