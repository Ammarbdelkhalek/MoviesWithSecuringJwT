using System.ComponentModel.DataAnnotations;

namespace JwtWithIdentiyAuthenticatoin.Models.authModels
{
    public class RoleModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
}
