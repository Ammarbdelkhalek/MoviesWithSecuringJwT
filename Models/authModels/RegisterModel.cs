using System.ComponentModel.DataAnnotations;

namespace JwtWithIdentiyAuthenticatoin.Models.authModels
{
    public class RegisterModel
    {
        [Required, StringLength(255)]
        public string FirstName { get; set; }
        [Required, StringLength(255)]
        public string LastName { get; set; }
        [Required, StringLength(255)]
        public string UserName { get; set; }
        [Required, StringLength(255)]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
