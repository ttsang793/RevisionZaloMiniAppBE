namespace backend.DTOs;

public class CommentReadDTO // can be change
{
    public ulong Id { get; set; }

    public ulong UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string? UserAvatar { get; set; }

    public ulong ExamId { get; set; }

    public ulong? ReplyTo { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public ICollection<CommentReadDTO> Replies { get; set; } = [];
}

public class CommentInsertDTO
{
    public ulong ExamId { get; set; }

    public ulong UserId { get; set; }

    public ulong? ReplyTo { get; set; }

    public string Content { get; set; } = null!;
}
