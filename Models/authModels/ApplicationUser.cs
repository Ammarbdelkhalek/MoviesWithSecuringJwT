using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace JwtWithIdentiyAuthenticatoin.Models.authModels
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(255)]
        public string FirstName { get; set; }
        [Required, MaxLength(255)]
        public string LastName { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; }

    }
}
