using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Teacher
{
    public ulong Id { get; set; }

    public string Role { get; set; } = null!;

    public string? SubjectId { get; set; }

    public ICollection<byte> Grades { get; set; } = [];

    public string? Introduction { get; set; }

    public virtual ICollection<Exam> ExamApprovedByNavigations { get; set; } = new List<Exam>();

    public virtual ICollection<Exam> ExamTeachers { get; set; } = new List<Exam>();

    public virtual User IdNavigation { get; set; } = null!;

    public virtual Subject? Subject { get; set; }
}
