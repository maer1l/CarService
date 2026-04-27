using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarService.Models;

public partial class Product
{
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Пожалуйста, выберите категорию!")]
    public int? CategoryId { get; set; }

    [Required(ErrorMessage = "Пожалуйста, введите серийный номер!")]
    [Range(100000, 999999, ErrorMessage = "Серийный номер должен быть вида: 111111")]
    public int? SerialNumber { get; set; }

    [Required(ErrorMessage = "Пожалуйста, введите цену!")]
    [Range(5, 10000, ErrorMessage = "Цена должна быть в диапазоне от 5 до 10000")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Пожалуйста, введите год выпуска товара!")]
    [Range(1970, 2026, ErrorMessage = "Год выпуска товара должен быть в диапазоне от 1970 до 2026")]
    public int? ReleaseYear { get; set; }

    [Required(ErrorMessage = "Товар не может быть без бренда!")]
    public string? Brand { get; set; }

    [Required(ErrorMessage = "Товар не может быть безымянным!")]
    public string? Model { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
