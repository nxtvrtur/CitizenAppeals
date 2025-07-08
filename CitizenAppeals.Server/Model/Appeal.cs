using System;
using System.Collections.Generic;

namespace CitizenAppeals.Server.Model;

public partial class Appeal
{
    public int Id { get; set; }

    public string AppealNumber { get; set; } = null!;

    public DateTime AppealDate { get; set; }

    public int? CitizenId { get; set; }

    public int ViolationType { get; set; }

    public string? Result { get; set; } = null!;

    public string? AppealLink { get; set; }
    public virtual Citizen? Citizen { get; set; }

    public virtual ICollection<Executor> Executors { get; } = new List<Executor>();
}
