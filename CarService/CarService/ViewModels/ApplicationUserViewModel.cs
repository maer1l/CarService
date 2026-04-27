using CarService.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace CarService.ViewModels
{
    public class ApplicationUserViewModel
    {
        public ApplicationUser user { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }
}
