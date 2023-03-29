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

		public BaseService(OpenAI.GPT3.Managers.OpenAIService openAIService)
		{
			this.OpenAIService = openAIService;
		}
	}
}