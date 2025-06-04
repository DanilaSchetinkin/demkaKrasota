using System;
using System.Collections.Generic;

namespace Demokrasota.Models;

public partial class Client
{
    public int Id { get; set; }

    public string? Type { get; set; }

    public string? ClientCodeIndivid { get; set; }

    public string? ClientCodeLegal { get; set; }

    public string ClientCode { get; set; } = null!;

    public virtual IndividualClient? ClientCodeIndiv { get; set; }

    public virtual LegalClient? ClientCodeLegalNavigation { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
