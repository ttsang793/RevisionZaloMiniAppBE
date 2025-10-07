using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Question
{
    public ulong Id { get; set; }

    public string Title { get; set; } = null!;

    public byte? Grade { get; set; }

    public byte? Type { get; set; }

    public byte? Difficulty { get; set; }

    public string? TopicId { get; set; }

    public string? SubjectId { get; set; }

    public string? Explaination { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ConstructedResponseQuestion? ConstructedResponseQuestion { get; set; }

    public virtual FillInTheBlankQuestion? FillInTheBlankQuestion { get; set; }

    public virtual GroupQuestion? GroupQuestion { get; set; }

    public virtual MultipleChoiceQuestion? MultipleChoiceQuestion { get; set; }

    public virtual ShortAnswerQuestion? ShortAnswerQuestion { get; set; }

    public virtual SortingQuestion? SortingQuestion { get; set; }

    public virtual Subject? Subject { get; set; }

    public virtual Topic? Topic { get; set; }

    public virtual TrueFalseQuestion? TrueFalseQuestion { get; set; }
}

public partial class MultipleChoiceQuestion
{
    public ulong Id { get; set; }

    public string AnswerA { get; set; } = null!;

    public string AnswerB { get; set; } = null!;

    public string AnswerC { get; set; } = null!;

    public string AnswerD { get; set; } = null!;

    public string AnswerKey { get; set; } = null!;

    public virtual Question IdNavigation { get; set; } = null!;
}

public partial class TrueFalseQuestion
{
    public ulong Id { get; set; }

    public bool? AnswerKey { get; set; }

    public virtual Question IdNavigation { get; set; } = null!;
}

public partial class ShortAnswerQuestion
{
    public ulong Id { get; set; }

    public decimal AnswerKey { get; set; }

    public virtual Question IdNavigation { get; set; } = null!;
}

public partial class FillInTheBlankQuestion
{
    public ulong Id { get; set; }

    public ICollection<string> AnswerKeys { get; set; } = [];

    public bool? MarkAsWrong { get; set; }

    public virtual Question IdNavigation { get; set; } = null!;
}


public partial class ConstructedResponseQuestion
{
    public ulong Id { get; set; }

    public ICollection<string> AnswerKeys { get; set; } = [];

    public bool? AllowTakePhoto { get; set; }

    public bool? AllowEnter { get; set; }

    public bool? MarkAsWrong { get; set; }

    public virtual Question IdNavigation { get; set; } = null!;
}


public partial class SortingQuestion
{
    public ulong Id { get; set; }

    public ICollection<string> CorrectOrder { get; set; } = [];

    public virtual Question IdNavigation { get; set; } = null!;
}