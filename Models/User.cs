namespace backend.Models;

public partial class User
{
    public ulong Id { get; set; }

    public string? ZaloId { get; set; }

    public string Name { get; set; } = null!;

    public string? Avatar { get; set; }

    public string Role { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Teacher? Teacher { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = [];

    public virtual ICollection<ExamAttempt> ExamAttempts { get; set; } = [];

    public virtual ICollection<PdfExamAttempt> PdfExamAttempts { get; set; } = [];

    public virtual ICollection<StudentProcess> StudentProcesses { get; set; } = [];

    public void NullifyUser()
    {
        ZaloId = null;
        Name = "Người dùng đã xóa";
        Avatar = null; //avatar là default.png
        CreatedAt = null;
        UpdatedAt = null;
    }
}
