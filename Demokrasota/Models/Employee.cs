using System;
using System.Collections.Generic;

namespace Demokrasota.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public int PositionId { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public DateTime? HireDate { get; set; }

    public virtual Position Position { get; set; } = null!;

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
