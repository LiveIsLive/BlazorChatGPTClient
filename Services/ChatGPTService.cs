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
					this._EditingMessages.Add(new Models.Message(Models.MessageRole.System,"You are a helpful assistant."));
					this._EditingMessages.Add(new Models.Message(Models.MessageRole.User, "Who won the world series in 2020?"));
					this._EditingMessages.Add(new Models.Message(Models.MessageRole.Assistant, "The Los Angeles Dodgers won the World Series in 2020."));
					this._EditingMessages.Add(new Models.Message(Models.MessageRole.User, "Where was it played?"));
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

		public async Task<bool> Send(System.Collections.Generic.IEnumerable<Models.Message> messages)
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
				foreach (OpenAI.GPT3.ObjectModels.SharedModels.ChatChoiceResponse choice in completionResult.Choices)
					this.Messages.Add(new Models.Message(Models.MessageRole.Server, choice.Message.Content));
			else this.Messages.Add(new Models.Message(Models.MessageRole.Error, $"{completionResult.Error?.Code}: {completionResult.Error?.Message}"));
			return completionResult.Successful;
		}

		public virtual async Task<bool> Send()
		{
			if(await this.Send(this.EditingMessages))
			{
				this.EditingMessages = new(this.EditingMessages.Select(m => new Models.Message { Role = m.Role }));
				return true;
            }
			return false;
		}
	}
}