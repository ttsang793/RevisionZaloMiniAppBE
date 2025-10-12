using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Student
{
    public ulong Id { get; set; }

    public string RealName { get; set; } = null!;

    public virtual ICollection<ExamAttempt> ExamAttempts { get; set; } = new List<ExamAttempt>();

    public virtual User IdNavigation { get; set; } = null!;

    public virtual ICollection<PdfExamAttempt> PdfExamAttempts { get; set; } = new List<PdfExamAttempt>();

    public virtual ICollection<StudentProcess> StudentProcesses { get; set; } = new List<StudentProcess>();
}
