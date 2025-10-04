using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Subject
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public ICollection<int> Classes { get; set; } = [];
    public bool QuestionTN { get; set; }
    public bool QuestionDS { get; set; }
    public bool QuestionTLN { get; set; }
    public bool QuestionDVCT { get; set; }
    public bool QuestionTL { get; set; }
    public bool QuestionSX { get; set; }
    public bool IsVisible { get; set; } = true;
    public DateTime? CreatedAt { get; set; } = null!;
    public DateTime? UpdateAt { get; set; } = null!;
    public virtual ICollection<Topic> Topics { get; set; } = [];

    public void TakeValuesFrom(Subject s)
    {
        Name = s.Name;
        Classes = s.Classes;
        QuestionTN = s.QuestionTN;
        QuestionDS = s.QuestionDS;
        QuestionTLN = s.QuestionTLN;
        QuestionDVCT = s.QuestionDVCT;
        QuestionTL = s.QuestionTL;
        QuestionSX = s.QuestionSX;
        UpdateAt = DateTime.Now;
    }
}
