using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdShineSoft.Services
{
	public class LoginService
	{
		protected readonly UserService UserService;

		public Models.User? LoginUser { get; set; }

		public System.Action Logined { get; set; } = null!;

		public System.Action Logouted { get; set; } = null!;

		public System.Action LoginStateChaged { get; set; } = null!;


		public LoginService(UserService userService)
		{
			this.UserService = userService;
		}

		public string? Login(string userName, string password)
		{
			try
			{
				this.LoginUser = this.UserService.Check(userName, password);
				if (this.Logined != null)
					this.Logined();
				if (this.LoginStateChaged != null)
					this.LoginStateChaged();
				return null;
			}
			catch (System.Exception exception)
			{
				return exception.Message;
			}
		}

		public void Logout()
		{
			this.LoginUser = null;
			if (this.Logouted != null)
				this.Logouted();
			if (this.LoginStateChaged != null)
				this.LoginStateChaged();
		}
	}
}