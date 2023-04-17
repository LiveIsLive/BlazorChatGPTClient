using OpenAI.GPT3.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdShineSoft.Services
{
	public class BasicChatService : ChatGPTService
	{

		public Models.Message EditingMessage
		{
			get
			{
				return this.EditingMessages[0];
			}
			set
			{
				this.EditingMessages.Clear();
				this.EditingMessages.Add(value);
			}
		}

		public BasicChatService(OpenAIService openAIService) : base(openAIService)
		{
			this.EditingMessage = new Models.Message { Role = Models.MessageRole.User };
		}

		public override async Task<bool> Send()
		{
			if (await base.Send())
            {
				this.EditingMessage = new Models.Message { Role = Models.MessageRole.User };
				return true;
            }
			this.EditingMessage = new Models.Message(this.EditingMessage);
			return false;
		}
	}
}