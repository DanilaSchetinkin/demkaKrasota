using System;
using System.Collections.Generic;

namespace Demokrasota.Models;

public partial class EmployeeService
{
    public int EmployeeId { get; set; }

    public int ServiceId { get; set; }

    public virtual Service Service { get; set; } = null!;
}
