using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class GroupQuestion
{
    public ulong Id { get; set; }

    public List<ulong> Questions { get; set; } = [];

    public virtual Question IdNavigation { get; set; } = null!;
}
