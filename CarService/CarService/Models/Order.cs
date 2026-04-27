using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarService.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? ProductId { get; set; }

    public string MasterId { get; set; }

    public int ClientId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [Required(ErrorMessage = "Пожалуйста, введите цену!")]
    [Range(5, 10000, ErrorMessage = "Цена должна быть в диапазоне от 5 до 10000")]
    public decimal Price { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Product? Product { get; set; }
}
