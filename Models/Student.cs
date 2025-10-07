using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Student
{
    public ulong Id { get; set; }

    public string RealName { get; set; } = null!;

    public virtual User IdNavigation { get; set; } = null!;
}
