using System;
using System.Collections.Generic;

namespace CitizenAppeals.Server.Model;

public partial class Executor
{
    public int Id { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public virtual ICollection<Appeal> Appeals { get; } = new List<Appeal>();
}
