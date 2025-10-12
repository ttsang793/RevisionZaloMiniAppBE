using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class User
{
    public ulong Id { get; set; }

    public string? ZaloId { get; set; }

    public string Name { get; set; } = null!;

    public string? Avatar { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Student? Student { get; set; }

    public virtual Teacher? Teacher { get; set; }
}
