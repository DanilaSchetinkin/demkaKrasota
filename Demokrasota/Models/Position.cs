using System;
using System.Collections.Generic;

namespace Demokrasota.Models;

public partial class Position
{
    public int Id { get; set; }

    public string Role { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
