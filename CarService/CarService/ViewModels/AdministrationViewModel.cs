using CarService.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;

namespace CarService.ViewModels
{
    public class AdministrationViewModel
    {
        public List<IdentityRole> roles { get; set; }
        public List<ApplicationUser> users { get; set; }
        public Dictionary<string, string> userRoles { get; set; }
    }
}
