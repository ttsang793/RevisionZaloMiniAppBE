namespace backend.Models;

public partial class Follower
{
    public ulong Id { get; set; }

    public ulong StudentId { get; set; }

    public ulong TeacherId { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}