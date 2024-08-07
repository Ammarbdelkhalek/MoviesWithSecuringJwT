using JwtWithIdentiyAuthenticatoin.Models.authModels;

namespace JwtWithIdentiyAuthenticatoin.services
{
    public interface IAuthServices
    {
      Task<AuthModel> RegisterAysnc(RegisterModel model);
      Task<AuthModel> LoginAsync(LoginModel model);
      Task<string> GenerateToken(ApplicationUser user);
      Task<string> AddRoleAsync(RoleModel rolemodel);
      Task <AuthModel> RefreshTokenAsync(string Token);
      Task <bool> RevokeTokenAsync(string Token);
        
    }
}
