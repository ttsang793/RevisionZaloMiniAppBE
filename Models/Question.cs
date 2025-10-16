namespace backend.Models;

public partial class Question
{
    public ulong Id { get; set; }

    public string? Title { get; set; }

    public byte? Grade { get; set; }

    public string Type { get; set; } = null!;

    public byte? Difficulty { get; set; }

    public string? TopicId { get; set; } = null!;

    public string SubjectId { get; set; } = null!;

    public string? Explanation { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = [];

    public virtual GroupQuestion? GroupQuestion { get; set; }

    public virtual ManualResponseQuestion? ManualResponseQuestion { get; set; }

    public virtual MultipleChoiceQuestion? MultipleChoiceQuestion { get; set; }

    public virtual ShortAnswerQuestion? ShortAnswerQuestion { get; set; }

    public virtual SortingQuestion? SortingQuestion { get; set; }

    public virtual Subject? Subject { get; set; }

    public virtual Topic? Topic { get; set; }

    public virtual TrueFalseQuestion? TrueFalseQuestion { get; set; }

    public virtual TrueFalseTHPTQuestion? TrueFalseTHPTQuestion { get; set; }
}

public abstract class TypeQuestion
{
    public ulong Id { get; set; }

    public virtual Question IdNavigation { get; set; } = null!;
}

public partial class MultipleChoiceQuestion : TypeQuestion
{

    public string CorrectAnswer { get; set; } = null!;

    public string WrongAnswer1 { get; set; } = null!;

    public string WrongAnswer2 { get; set; } = null!;

    public string WrongAnswer3 { get; set; } = null!;
}

public partial class TrueFalseQuestion : TypeQuestion
{
    public bool? AnswerKey { get; set; }
}

public partial class ShortAnswerQuestion : TypeQuestion
{
    public string AnswerKey { get; set; } = null!;
}

public partial class ManualResponseQuestion : TypeQuestion
{
    public ICollection<string> AnswerKeys { get; set; } = [];

    public bool? AllowTakePhoto { get; set; }

    public bool? AllowEnter { get; set; }

    public bool? MarkAsWrong { get; set; }
}


public partial class SortingQuestion : TypeQuestion
{
    public ICollection<string> CorrectOrder { get; set; } = [];
}

public partial class GroupQuestion : TypeQuestion
{
    public string? PassageTitle { get; set; }

    public string? PassageContent { get; set; }

    public string? PassageAuthor { get; set; }

    public ICollection<ulong> Questions { get; set; } = [];
}

public partial class TrueFalseTHPTQuestion : TypeQuestion
{
    public string? PassageTitle { get; set; }

    public string? PassageContent { get; set; }

    public string? PassageAuthor { get; set; }

    public ICollection<string> Statements { get; set; } = [];

    public ICollection<bool> AnswerKeys { get; set; } = [];
}