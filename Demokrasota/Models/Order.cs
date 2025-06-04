using System;
using System.Collections.Generic;

namespace Demokrasota.Models;

public partial class Order
{
    public int Id { get; set; }

    public string OrderNumber { get; set; } = null!;

    public DateOnly CreationDate { get; set; }

    public string ClientCode { get; set; } = null!;

    public string Services { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateOnly? ClosingDate { get; set; }

    public int EmployeeId { get; set; }

    public string ExecutionTime { get; set; } = null!;

    public virtual Client ClientCodeNavigation { get; set; } = null!;
}
