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
			this.EditingMessage = new Models.Message { Role = Models.Role.User };
		}

		public async Task<Models.Message> Send(Models.Message message)
		{
			this.Messages.Add(message);
			var completionResult = await this.OpenAIService.ChatCompletion.CreateCompletion(new OpenAI.GPT3.ObjectModels.RequestModels.ChatCompletionCreateRequest
			{
				Messages = new OpenAI.GPT3.ObjectModels.RequestModels.ChatMessage[]
				{
					new OpenAI.GPT3.ObjectModels.RequestModels.ChatMessage(message.Role.ToString().ToLower(),message.Content)
				}
			});
			if (completionResult.Successful)
				message = new Models.Message(Models.Role.Server, completionResult.Choices.First().Message.Content);
			else
			{
				message = new Models.Message(Models.Role.Server, $"{completionResult.Error?.Code}: {completionResult.Error?.Message}");
			}

			this.Messages.Add(message);
			return message;
		}

		public override async Task<Models.Message> Send()
		{
			Models.Message message = await base.Send();
			this.EditingMessage = new Models.Message { Role = Models.Role.User };
			return message;
		}
	}
}