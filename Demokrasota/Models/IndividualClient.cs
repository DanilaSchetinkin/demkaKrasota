using System;
using System.Collections.Generic;

namespace Demokrasota.Models;

public partial class IndividualClient
{
    public string FullName { get; set; } = null!;

    public string ClientCode { get; set; } = null!;

    public string PassportData { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string Address { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
}
