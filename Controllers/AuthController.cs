using JwtWithIdentiyAuthenticatoin.Dto.authDto;
using JwtWithIdentiyAuthenticatoin.Models.authModels;
using JwtWithIdentiyAuthenticatoin.services;
using Microsoft.AspNetCore.Mvc;

namespace JwtWithIdentiyAuthenticatoin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthServices authServices) : ControllerBase
    {
        [HttpPost("Registered")]
        public async Task <IActionResult> RegistrationAsync(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await authServices.RegisterAysnc(model);

            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginModel LogModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await authServices.LoginAsync(LogModel);
            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }
            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshTokenInCookie(result.RefreshToken,result.RefreshTokenExpiration);
            }

            return Ok(result);
        }

        [HttpPost("AddRole")]
        public async Task<IActionResult>AddRoleAsync(RoleModel rolemodel)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await authServices.AddRoleAsync(rolemodel);
            if(string.IsNullOrEmpty(result))
            {
                return BadRequest(result);
            }

            return Ok(rolemodel);
        }

        [HttpGet("RfreshToken")]
        public async Task<IActionResult> GetRefreshToken()
        {
            var RefreshToken = Request.Cookies["refreshToken"];
            var result = await authServices.RefreshTokenAsync(RefreshToken);
            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }
            SetRefreshTokenInCookie(result.RefreshToken,result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpPost("RevokeToken")]
        public async Task<IActionResult> RevokeTokenAsync(RevokeTokenModel Model)
        {
            var token = Model.Token ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("token Is Required");
            }
            var result = await authServices.RevokeTokenAsync(token);
            if (!result)
            {
                return BadRequest(" Token is InValid");
            }
            return Ok();
        }

        private  void SetRefreshTokenInCookie(string refreshToken,  DateTime expires) 
        {
            var CookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };
            Response.Cookies.Append("refreshToken",refreshToken, CookieOptions);
        }
    }
}
