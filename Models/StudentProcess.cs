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

    public ICollection<ulong> FavoriteExams { get; set; } = [];

    public virtual User Student { get; set; } = null!;

    public virtual Subject? Subject { get; set; }
}
