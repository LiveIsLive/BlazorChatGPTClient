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
			protected set
            {
				this._EditingMessages = value;
            }
		}

		public string? Model { get; set; }

		public float? Temperature { get; set; }

		public int? ChoiceCount { get; set; }

		public float? FrequencyPenalty { get; set; }

		public int? MaxTokens { get; set; }

		public float? PresencePenalty { get; set; }

		public string? Stop { get; set; }

		public System.Collections.ObjectModel.ObservableCollection<string> StopAsList { get; } = new();

		public float? TopP { get; set; }

		public ChatGPTService(OpenAI.GPT3.Managers.OpenAIService openAIService) : base(openAIService)
		{
		}

		public async Task<Models.Message[]> Send(System.Collections.Generic.IEnumerable<Models.Message> messages)
		{
			foreach (Models.Message m in messages)
				this.Messages.Add(m);

			var completionResult = await this.OpenAIService.ChatCompletion.CreateCompletion(new OpenAI.GPT3.ObjectModels.RequestModels.ChatCompletionCreateRequest
			{
				Messages = messages.Select(m => new OpenAI.GPT3.ObjectModels.RequestModels.ChatMessage(m.Role.ToString().ToLower(), m.Content)).ToArray(),
				Model = this.Model,
				Temperature = this.Temperature,
				User = this.User,
				N = this.ChoiceCount,
				FrequencyPenalty = this.FrequencyPenalty,
				MaxTokens = this.MaxTokens,
				PresencePenalty = this.PresencePenalty,
				Stop = this.Stop,
				StopAsList = this.StopAsList.Count == 0 ? null : this.StopAsList,
				TopP = this.TopP
			});

			if (completionResult.Successful)
            {
				messages = completionResult.Choices.Select(c=> new Models.Message(Models.Role.Server, c.Message.Content)).ToArray();
				foreach (Models.Message m in messages)
					this.Messages.Add(m);
				return (Models.Message[])messages;
            }
			else
			{
				Models.Message message = new Models.Message(Models.Role.Error, $"{completionResult.Error?.Code}: {completionResult.Error?.Message}");
				this.Messages.Add(message);
				return new Models.Message[] { message };
			}
		}

		public virtual async Task<Models.Message[]> Send()
		{
			try
            {
				return await this.Send(this.EditingMessages);
            }
			finally
            {
				this.EditingMessages = new(this.EditingMessages.Select(m => new Models.Message { Role = m.Role }));
            }
		}
	}
}