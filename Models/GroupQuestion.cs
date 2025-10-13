namespace backend.Models;

public partial class GroupQuestion
{
    public ulong Id { get; set; }

    public string? PassageTitle { get; set; }

    public string? PassageContent { get; set; }

    public string? PassageAuthor { get; set; }

    public ICollection<ulong> Questions { get; set; } = [];

    public virtual Question IdNavigation { get; set; } = null!;
}