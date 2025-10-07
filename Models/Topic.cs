using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Topic
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public List<byte> Grades { get; set; } = [];
    public string SubjectId { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public virtual Subject Subject { get; set; } = null!;
    public bool? IsVisible { get; set; }
    public virtual ICollection<Question> Questions { get; set; } = [];

    public void TakeValuesFrom(Topic t)
    {
        Name = t.Name;
        Grades = t.Grades;
        SubjectId = t.SubjectId;
        UpdatedAt = DateTime.Now;
        Subject = t.Subject;
    }
}
