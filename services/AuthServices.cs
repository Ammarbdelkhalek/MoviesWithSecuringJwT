using JwtWithIdentiyAuthenticatoin.helper;
using JwtWithIdentiyAuthenticatoin.Models.authModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JwtWithIdentiyAuthenticatoin.services
{
    public class AuthServices(UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager,IOptions<jwtOptions> options) : IAuthServices
    { 
        public async Task<AuthModel> LoginAsync(LoginModel model)
        {
            var authmodel = new AuthModel();
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null|| !await userManager.CheckPasswordAsync(user,model.Password))
            {
                return new AuthModel { Message = "Email Or Password invalid" };
            }
            var token = await GenerateToken(user);
            await userManager.GetRolesAsync(user);
            authmodel.Message = "You'r logged in sucessfuly";
            authmodel.Token = token;
            authmodel.Username = user.UserName;
            authmodel.Email = user.Email;
            authmodel.Roles = new List<string> { "User" };
            authmodel.IsAuthenticated = true;

            if (user.RefreshTokens.Any(x => x.IsActive))
            {
                var ActiveRefreshToken = user.RefreshTokens.FirstOrDefault(x => x.IsActive);
                authmodel.RefreshToken = ActiveRefreshToken.Token;
                authmodel.RefreshTokenExpiration = ActiveRefreshToken.ExpiresON;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authmodel.RefreshToken = refreshToken.Token;
                authmodel.RefreshTokenExpiration = refreshToken.ExpiresON;
                user.RefreshTokens.Add(refreshToken);
                await userManager.UpdateAsync(user);
            }
            return  authmodel;
        }
        public async  Task<AuthModel> RegisterAysnc(RegisterModel model)
        {
            if(await userManager.FindByEmailAsync(model.Email) is not null)
            {
                return new AuthModel { Message = "Email is already Exist" };
            }
            if(await userManager.FindByNameAsync(model.UserName) is not null)
            {
                return new AuthModel { Message = "UserName Is ALready Exist" };
            }
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if(!result.Succeeded)
            {
                var error = string.Empty;
                foreach(var err in result.Errors)
                {
                    error += err.Description;
                }
                return new AuthModel { Message = error };
            }
            var Token = await GenerateToken(user);
            await userManager.AddToRoleAsync(user, "User");
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await userManager.UpdateAsync(user);

            return new AuthModel
            {
                Message = "Registred successfuly",
                Email = user.Email,
                //ExpirationDate = DateTime.Now.AddDays(options.Value.LifeTime),
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Username = user.UserName,
                Token = Token,
                RefreshTokenExpiration = refreshToken.ExpiresON,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<string> GenerateToken(ApplicationUser user)
        {
            var userClaims = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SigninKey));
            var securityCredintial = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var securitToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(options.Value.LifeTime),
                 issuer: options.Value.issuer,
                 audience: options.Value.Audienc,
                 signingCredentials:securityCredintial
                );
            var tokenHandler = new JwtSecurityTokenHandler().WriteToken(securitToken);
            return tokenHandler;
        }

        public async Task<string> AddRoleAsync(RoleModel rolemodel)
        {
            var user = await userManager.FindByIdAsync(rolemodel.UserId);
            if (user is null|| !await roleManager.RoleExistsAsync(rolemodel.RoleName))
            {
                return "user or Rolle not exist ";

            }
            if(await userManager.IsInRoleAsync(user, rolemodel.RoleName))
            {
                return " the role is already exist ";
            }

            var result = await userManager.AddToRoleAsync(user, rolemodel.RoleName);

            return  result.Succeeded ? string.Empty : "something went wrong";
        }

        public async Task<AuthModel> RefreshTokenAsync(string Token)
        {
             var authmodel = new AuthModel();
            var user =  userManager.Users.FirstOrDefault(x => x.RefreshTokens.Any(x => x.Token == Token));
            if (user is null)
            {
                authmodel.Message = "Token Is Invalid";
                authmodel.IsAuthenticated = false;
                return  authmodel;
            }
            var RefreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == Token);
            if (!RefreshToken.IsActive)
            {
                authmodel.Message = "Token Is Inactive";
                authmodel.IsAuthenticated = false;
                return authmodel;
            }
            RefreshToken.RevokedOn = DateTime.UtcNow;
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await userManager.UpdateAsync(user);

            authmodel.IsAuthenticated = true;
            authmodel.Token = await GenerateToken(user);
            authmodel.Email = user.Email;
            authmodel.Username = user.UserName;
            var roles = await userManager.GetRolesAsync(user);
            authmodel.Roles = roles.ToList();
            authmodel.RefreshToken = newRefreshToken.Token;
            authmodel.RefreshTokenExpiration = newRefreshToken.ExpiresON;
            return authmodel;
        }
        public async Task<bool> RevokeTokenAsync(string Token)
        {
            var user =  userManager.Users.FirstOrDefault(x => x.RefreshTokens.Any(x => x.Token == Token));
            if(user == null)
            {
                return false;
            }
            var refreshToken = user.RefreshTokens.Single(x => x.Token == Token);
            if(!refreshToken.IsActive)
            {
                return false;
            }
            refreshToken.RevokedOn = DateTime.UtcNow;
            return true;
         
        }

        private RefreshToken GenerateRefreshToken( )
        {
            var randomNum = new byte[32];
            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(randomNum);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNum),
                ExpiresON = DateTime.UtcNow.AddMinutes(1),
                CreatedOn = DateTime.UtcNow,
            };

        } 
    }
}



    

