using System;
using System.Collections.Generic;

namespace CarService.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public int? CategoryId { get; set; }

    public int? SerialNumber { get; set; }

    public decimal? Price { get; set; }

    public int? ReleaseYear { get; set; }

    public string? Brand { get; set; }

    public string? Model { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
