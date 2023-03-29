using OpenAI.GPT3.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdShineSoft.Services
{
	public class BasicChatService : BaseService
	{
		public System.Collections.ObjectModel.ObservableCollection<Models.Message> Messages { get; } = new();

		public Models.Message EditingMessage { get; set; } = new Models.Message { Role = Models.Role.User };

		public BasicChatService(OpenAIService openAIService) : base(openAIService)
		{
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

		public async Task<Models.Message> Send()
		{
			Models.Message message = await this.Send(this.EditingMessage);
			this.EditingMessage = new Models.Message { Role = Models.Role.User };
			return message;
		}
	}
}