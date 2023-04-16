using ColdShineSoft.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ColdShineSoft.BlazorChatGPTClient.Pages
{
	[Microsoft.AspNetCore.Authorization.AllowAnonymous]
	[IgnoreAntiforgeryToken]
	public class LoginModel : PageModel
	{
		protected readonly Services.UserService UserService;

		public LoginModel(UserService userService)
		{
			this.UserService = userService;
		}

		public async Task<IActionResult> OnPost([FromBody] ColdShineSoft.Models.User user)
		{
			string? error = this.UserService.Check(user.UserName, user.Password);
			if(error!=null)
				return new JsonResult(error);

			var claims = new List<System.Security.Claims.Claim>
			{
				new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName),
				new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role.ToString()),
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
			return new JsonResult(null);
		}
	}
}