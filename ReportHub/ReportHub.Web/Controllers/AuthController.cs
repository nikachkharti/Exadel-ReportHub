using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ReportHub.Web.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        [HttpGet("signin")]
        public async Task<IActionResult> SignIn([FromQuery] string access_token, string returnUrl = "/")
        {
            if (string.IsNullOrEmpty(access_token))
            {
                return BadRequest("Missing token");
            }

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(access_token);

            // Optionally validate token here using TokenValidationParameters

            var identity = new ClaimsIdentity(token.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Redirect(returnUrl);
        }
    }
}
