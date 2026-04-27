using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarService.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [PersonalData]
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Пожалуйста, введите имя!")] // используется валидация средствами js, чтобы не было перезагрузки страницы
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Имя не должно превышать 30 символов!")]
    //[Remote("ValidateTitle", "Books", HttpMethod = "POST", ErrorMessage = "Title should contain letter 'a'")]
    [Column(TypeName = "varchar(30)")]
    public string FirstName { get; set; }

    [PersonalData]
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Пожалуйста, введите фамилию!")] // используется валидация средствами js, чтобы не было перезагрузки страницы
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Фамилия не должна превышать 30 символов!")]
    [Column(TypeName = "varchar(30)")]
    public string LastName { get; set; }

    [PersonalData]
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Пожалуйста, введите возраст!")] // используется валидация средствами js, чтобы не было перезагрузки страницы
    [Range(18, 90, ErrorMessage = "Возраст должен быть от 18 до 90 лет!")]
    [Column(TypeName = "int")]
    public int Age { get; set; }

    [PersonalData]
    [Column(TypeName = "int")]
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Пожалуйста, укажите номер паспорта!")]
    [Range(100000, 999999, ErrorMessage = "Номер паспорта содержит 6 цифр!")]
    public int DocumentId { get; set; }
}

