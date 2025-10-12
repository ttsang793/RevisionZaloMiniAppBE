using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class ExamPart
{
    public ulong Id { get; set; }

    public ulong ExamId { get; set; }

    public byte PartIndex { get; set; }

    public ICollection<decimal> Points { get; set; } = [];

    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = [];
}
