using Microsoft.EntityFrameworkCore;

namespace JwtWithIdentiyAuthenticatoin.Models.authModels
{
    [Owned]
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresON { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresON;
        public DateTime CreatedOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public bool IsActive => RevokedOn == null && !IsExpired;
    }
}
