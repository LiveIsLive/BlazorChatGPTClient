using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdShineSoft.Services
{
	public abstract class BaseService
	{
		protected readonly OpenAI.GPT3.Managers.OpenAIService OpenAIService;

		public string User { get; set; } = null!;

		private static Models.MessageRole[] _ChatRoles = null!;
		public Models.MessageRole[] ChatRoles
		{
			get
			{
				if (_ChatRoles == null)
					_ChatRoles = new Models.MessageRole[] { Models.MessageRole.System, Models.MessageRole.Assistant, Models.MessageRole.User };
				return _ChatRoles;
			}
		}

		private static System.Collections.Generic.KeyValuePair<string, string>[] _ChatModels = null!;
		public System.Collections.Generic.KeyValuePair<string, string>[] ChatModels
		{
			get
			{
				if (_ChatModels == null)
					_ChatModels = typeof(OpenAI.GPT3.ObjectModels.Models).GetProperties().Select(p => new KeyValuePair<string, string>(p.Name, p.GetValue(null)?.ToString()!)).ToArray();
				return _ChatModels;
			}
		}

		public BaseService(OpenAI.GPT3.Managers.OpenAIService openAIService)
		{
			this.OpenAIService = openAIService;
		}

	}
}