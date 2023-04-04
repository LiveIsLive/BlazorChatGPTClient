using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdShineSoft.Models
{
	public class Message
	{
		public Role Role { get; set; } = Models.Role.User;

		[System.ComponentModel.DataAnnotations.Required(ErrorMessage = "必须输入内容")]
		public string Content { get; set; } = null!;

		public System.DateTime Time { get; set; } = System.DateTime.Now;

		public Message()
		{
		}

		public Message(Role role)
		{
			this.Role = role;
		}

		public Message(string content)
		{
			this.Content = content;
		}

		public Message(Role role, string content)
		{
			this.Role = role;
			this.Content = content;
		}
	}
}