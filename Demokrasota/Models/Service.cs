using System;
using System.Collections.Generic;

namespace Demokrasota.Models;

public partial class Service
{
    public int Id { get; set; }

    public string ServiceName { get; set; } = null!;

    public string ServiceCode { get; set; } = null!;

    public string ExecutionTime { get; set; } = null!;

    public decimal AvgDeviation { get; set; }

    public string MeasurementUnit { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal PriceForRussianCosmetics { get; set; }

    public virtual ICollection<EmployeeService> EmployeeServices { get; set; } = new List<EmployeeService>();
}
