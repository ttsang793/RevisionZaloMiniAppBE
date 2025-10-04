using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Topic
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public List<int> Classes { get; set; } = [];
    public string SubjectId { get; set; } = null!;
    public bool IsVisible { get; set; } = true;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public virtual Subject Subject { get; set; } = null!;

    public void TakeValuesFrom(Topic t)
    {
        Name = t.Name;
        Classes = t.Classes;
        SubjectId = t.SubjectId;
        UpdateAt = DateTime.Now;
        Subject = t.Subject;
    }
}
