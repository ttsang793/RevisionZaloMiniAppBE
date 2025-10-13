namespace backend.Models;

public partial class Subject
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public ICollection<byte> Grades { get; set; } = [];
    public bool QuestionTN { get; set; }
    public bool QuestionDS { get; set; }
    public bool QuestionTLN { get; set; }
    public bool QuestionDVCT { get; set; }
    public bool QuestionTL { get; set; }
    public bool QuestionSX { get; set; }
    public bool IsVisible { get; set; } = true;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }    
    public virtual ICollection<Exam> Exams { get; set; } = [];
    public virtual ICollection<Question> Questions { get; set; } = [];
    public virtual ICollection<StudentProcess> StudentProcesses { get; set; } = [];
    public virtual ICollection<Teacher> Teachers { get; set; } = [];
    public virtual ICollection<Topic> Topics { get; set; } = [];

    public void TakeValuesFrom(Subject s)
    {
        Name = s.Name;
        Grades = s.Grades;
        QuestionTN = s.QuestionTN;
        QuestionDS = s.QuestionDS;
        QuestionTLN = s.QuestionTLN;
        QuestionDVCT = s.QuestionDVCT;
        QuestionTL = s.QuestionTL;
        QuestionSX = s.QuestionSX;
        UpdatedAt = DateTime.Now;
    }
}
