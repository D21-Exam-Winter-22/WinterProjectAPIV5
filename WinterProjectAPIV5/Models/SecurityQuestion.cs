using System;
using System.Collections.Generic;

namespace WinterProjectAPIV5.Models;

public partial class SecurityQuestion
{
    public int QuestionId { get; set; }

    public string? Question { get; set; }

    public virtual ICollection<ShareUser> ShareUsers { get; } = new List<ShareUser>();
}
