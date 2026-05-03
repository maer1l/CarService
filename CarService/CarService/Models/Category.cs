using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarService.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Категрия не может быть без названия!")]
    [StringLength(40)]
    public string? CategoryName { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
