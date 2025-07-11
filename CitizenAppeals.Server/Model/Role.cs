﻿using System;
using System.Collections.Generic;

namespace CitizenAppeals.Server.Model;

public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; } = new List<User>();
}
