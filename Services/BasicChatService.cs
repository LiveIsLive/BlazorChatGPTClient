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

		public Models.Message EditingMessage { get; set; } = new();
		//{
		//	get
		//	{
		//		return this.EditingMessages[0];
		//	}
		//	set
		//	{
		//		this.EditingMessages.Clear();
		//		this.EditingMessages.Add(value);
		//	}
		//}

		public BasicChatService(OpenAIService openAIService) : base(openAIService)
		{
		}

		public async Task<bool> Send()
		{
			this.EditingMessages = new System.Collections.ObjectModel.ObservableCollection<Models.Message>(this.Messages.Where(m => m.Role == Models.MessageRole.User || m.Role == Models.MessageRole.Server).Select(m => new Models.Message(m.Role == Models.MessageRole.User ? m.Role : Models.MessageRole.Assistant, m.Content)));
			this.EditingMessages.Add(this.EditingMessage);
			this.Messages.Add(this.EditingMessage);
			if (await base.Send(false))
            {
				this.EditingMessage = new();
				return true;
            }
			this.EditingMessage = new Models.Message(this.EditingMessage);
			return false;
		}
	}
}