using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class StudentProcess
{
    public ulong Id { get; set; }

    public ulong StudentId { get; set; }

    public string? SubjectId { get; set; }

    public decimal? AvgScore { get; set; }

    public decimal? CorrectRate { get; set; }

    public uint? StreakDays { get; set; }

    public DateTime? LastActivity { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual Subject? Subject { get; set; }
}
