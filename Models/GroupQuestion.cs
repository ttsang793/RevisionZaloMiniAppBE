namespace backend.Models;

public partial class GroupQuestion : TypeQuestion
{
    public string? PassageTitle { get; set; }

    public string? PassageContent { get; set; }

    public string? PassageAuthor { get; set; }

    public ICollection<ulong> Questions { get; set; } = [];
}