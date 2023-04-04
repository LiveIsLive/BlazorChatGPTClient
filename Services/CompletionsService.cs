using OpenAI.GPT3.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdShineSoft.Services
{
	public class CompletionsService : BaseService
	{
		public System.Collections.ObjectModel.ObservableCollection<Models.Message> Messages { get; } = new();

		public Models.Message EditingMessage { get; protected set; } = new Models.Message { Content= "Once upon a time" };

		public string Model { get; set; } = OpenAI.GPT3.ObjectModels.Models.TextDavinciV3;

		public float? Temperature { get; set; }

		public int? ChoiceCount { get; set; }

		public float? FrequencyPenalty { get; set; }

		public int? MaxTokens { get; set; }

		public float? PresencePenalty { get; set; }

		public string? Stop { get; set; }

		public System.Collections.ObjectModel.ObservableCollection<string> StopAsList { get; } = new();

		public float? TopP { get; set; }

		public int? BestOf { get; set; }

		public bool? Echo { get; set; }

		public System.Collections.Generic.Dictionary<string,string> LogitBias { get; } = new();

		public int? LogProbs { get; set; }

		public System.Collections.ObjectModel.ObservableCollection<string> PromptAsList { get; } = new();

		public string? Suffix { get; set; }

		public CompletionsService(OpenAIService openAIService) : base(openAIService)
		{
		}

		public async Task<Models.Message[]> Send(Models.Message message)
		{
			this.Messages.Add(message);

			var completionResult = await this.OpenAIService.Completions.CreateCompletion(new OpenAI.GPT3.ObjectModels.RequestModels.CompletionCreateRequest()
			{
				Prompt = message.Content,
				Model = this.Model,
				Temperature = this.Temperature,
				User = this.User,
				N = this.ChoiceCount,
				FrequencyPenalty = this.FrequencyPenalty,
				MaxTokens = this.MaxTokens,
				PresencePenalty = this.PresencePenalty,
				Stop = this.Stop,
				StopAsList = this.StopAsList.Count == 0 ? null : this.StopAsList,
				TopP = this.TopP,

				BestOf=this.BestOf,
				Echo=this.Echo,
				LogitBias= this.LogitBias.Count == 0 ? null : this.LogitBias,
				LogProbs=this.LogProbs,
				PromptAsList= this.PromptAsList.Count == 0 ? null : this.PromptAsList,
				Suffix=null
			});

			if (completionResult.Successful)
			{
				Models.Message[] messages = completionResult.Choices.Select(c => new Models.Message(Models.Role.User, c.Text)).ToArray();
				foreach (Models.Message m in messages)
					this.Messages.Add(m);
				return messages;
			}
			else
			{
				message = new Models.Message(Models.Role.Error, $"{completionResult.Error?.Code}: {completionResult.Error?.Message}");
				this.Messages.Add(message);
				return new Models.Message[] { message };
			}
		}

		public virtual async Task<Models.Message[]> Send()
		{
			try
            {
				return await this.Send(this.EditingMessage);
            }
			finally
            {
				this.EditingMessage = new();
            }
		}
	}
}