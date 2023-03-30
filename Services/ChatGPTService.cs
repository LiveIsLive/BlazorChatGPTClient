using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdShineSoft.Services
{
	public class ChatGPTService : BaseService
	{
		public System.Collections.ObjectModel.ObservableCollection<Models.Message> Messages { get; } = new();

		private System.Collections.ObjectModel.ObservableCollection<Models.Message> _EditingMessages = null!;
		public System.Collections.ObjectModel.ObservableCollection<Models.Message> EditingMessages
		{
			get
			{
				if(this._EditingMessages==null)
				{
					this._EditingMessages = new();
					this._EditingMessages.Add(new Models.Message(Models.Role.System,"You are a helpful assistant."));
					this._EditingMessages.Add(new Models.Message(Models.Role.User, "Who won the world series in 2020?"));
					this._EditingMessages.Add(new Models.Message(Models.Role.Assistant, "The Los Angeles Dodgers won the World Series in 2020."));
					this._EditingMessages.Add(new Models.Message(Models.Role.User, "Where was it played?"));
				}
				return this._EditingMessages;
			}
		}

		public string? Model { get; set; }

		public float? Temperature { get; set; }

		public ChatGPTService(OpenAI.GPT3.Managers.OpenAIService openAIService) : base(openAIService)
		{
		}

		public async Task<Models.Message> Send(System.Collections.Generic.IEnumerable<Models.Message> messages)
		{
			foreach (Models.Message m in messages)
				this.Messages.Add(m);

			var completionResult = await this.OpenAIService.ChatCompletion.CreateCompletion(new OpenAI.GPT3.ObjectModels.RequestModels.ChatCompletionCreateRequest
			{
				Messages = messages.Select(m=> new OpenAI.GPT3.ObjectModels.RequestModels.ChatMessage(m.Role.ToString().ToLower(),m.Content)).ToArray(),
				Model = this.Model,
				Temperature = this.Temperature,
				User = this.User
			});
			Models.Message message;
			if (completionResult.Successful)
				message = new Models.Message(Models.Role.Server, completionResult.Choices.First().Message.Content);
			else
			{
				message = new Models.Message(Models.Role.Server, $"{completionResult.Error?.Code}: {completionResult.Error?.Message}");
			}

			this.Messages.Add(message);
			return message;
		}

		public virtual async Task<Models.Message> Send()
		{
			return await this.Send(this.EditingMessages);
		}
	}
}