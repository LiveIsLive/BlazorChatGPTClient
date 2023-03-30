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

		private static Models.Role[] _ChatGPTRoles = null!;
		public Models.Role[] ChatGPTRoles
		{
			get
			{
				if (_ChatGPTRoles == null)
					_ChatGPTRoles = System.Enum.GetValues<Models.Role>().Where(r => r != Models.Role.Server && r != Models.Role.Error).ToArray();
				return _ChatGPTRoles;
			}
		}

		public BaseService(OpenAI.GPT3.Managers.OpenAIService openAIService)
		{
			this.OpenAIService = openAIService;
		}
	}
}