using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdShineSoft.Services
{
	public class AuthenticationStateProvider : Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider
	{
		protected readonly LoginService LoginService;

		public AuthenticationStateProvider(LoginService loginService)
		{
			this.LoginService = loginService;
			this.LoginService.LoginStateChaged = () => this.NotifyAuthenticationStateChanged(this.GetAuthenticationStateAsync());
		}

		public override Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			System.Security.Claims.ClaimsIdentity identity;
			if (this.LoginService.LoginUser == null)
				identity = new System.Security.Claims.ClaimsIdentity(new System.Security.Claims.Claim[]
				{
					new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Sid, "0"),
					new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "Anonymous"),
					new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Anonymous")
				});
			else identity=new System.Security.Claims.ClaimsIdentity(new System.Security.Claims.Claim[]
			{
				new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Sid, this.LoginService.LoginUser.UserId.ToString()),
				new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, this.LoginService.LoginUser.UserName),
				new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, this.LoginService.LoginUser.Role.ToString())
			}, "Authentication type");

			return Task.FromResult(new AuthenticationState(new System.Security.Claims.ClaimsPrincipal(identity)));
		}
	}
}
