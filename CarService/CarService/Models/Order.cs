using System;
using System.Collections.Generic;

namespace CarService.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? ProductId { get; set; }

    public Guid MasterId { get; set; }

    public int ClientId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public decimal? Price { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Product? Product { get; set; }
}
