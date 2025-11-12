namespace backend.Models;

public partial class Subject
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public ICollection<byte> Grades { get; set; } = [];
    public bool QuestionMC { get; set; }
    public bool QuestionTF { get; set; }
    public bool QuestionSA { get; set; }
    public bool QuestionGF { get; set; }
    public bool QuestionST { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }    
    public bool IsVisible { get; set; } = true;
    public virtual ICollection<Exam> Exams { get; set; } = [];
    public virtual ICollection<Teacher> Teachers { get; set; } = [];
    public virtual ICollection<Topic> Topics { get; set; } = [];

    public void TakeValuesFrom(Subject s)
    {
        Name = s.Name;
        Grades = s.Grades;
        QuestionMC = s.QuestionMC;
        QuestionTF = s.QuestionTF;
        QuestionSA = s.QuestionSA;
        QuestionGF = s.QuestionGF;
        QuestionST = s.QuestionST;
    }
}
