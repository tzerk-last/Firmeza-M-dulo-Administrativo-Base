using Microsoft.AspNetCore.Identity;

namespace Firmeza.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string NombreCompleto { get; set; } = string.Empty;
    }
}

