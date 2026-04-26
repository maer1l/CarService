using System;
using System.Collections.Generic;

namespace CarService.Models;

public partial class Client
{
    public int ClientId { get; set; }

    public int? DocumentId { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public DateOnly? Birthdate { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
