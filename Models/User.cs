using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdShineSoft.Models
{
	public class User
	{
		private long? _UserId;
		public long UserId
		{
			get
			{
				if (this._UserId == null)
					this._UserId = System.DateTime.Now.Ticks;
				return this._UserId.Value;
			}
			set
			{
				this._UserId = value;
			}
		}

		[System.ComponentModel.DataAnnotations.Required(ErrorMessage = "必须输入用户名")]
		public string UserName { get; set; } = null!;

		[System.ComponentModel.DataAnnotations.Required(ErrorMessage = "必须输入密码")]
		public string Password { get; set; } = null!;

		public UserRole Role { get; set; }
	}
}