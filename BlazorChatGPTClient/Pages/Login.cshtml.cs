using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ColdShineSoft.BlazorChatGPTClient.Pages
{
	[Microsoft.AspNetCore.Authorization.AllowAnonymous]
	[IgnoreAntiforgeryToken]
	public class LoginModel : PageModel
	{
		public async Task<IActionResult> OnPost(string userName,string password)
		{
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, userName),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Administrator"),
            };
            var claimsIdentity = new System.Security.Claims.ClaimsIdentity(claims, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
			{
                IsPersistent = true,
                RedirectUri = this.Request.Host.Value
            };
            await HttpContext.SignInAsync(
				Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
                new System.Security.Claims.ClaimsPrincipal(claimsIdentity),
                authProperties);
            return this.Content("Test");
		}
	}
}