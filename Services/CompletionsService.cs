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
		public System.Collections.Generic.KeyValuePair<string, string>[] Sizes { get; } = typeof(OpenAI.GPT3.ObjectModels.StaticValues.ImageStatics.Size).GetProperties().Select(p => new KeyValuePair<string, string>(p.Name, p.GetValue(null)?.ToString()!)).ToArray();

		public int? ImageCount { get; set; }

		public string? Size { get; set; }

		public System.Collections.ObjectModel.ObservableCollection<Models.Message> Messages { get; } = new();

		public Models.Message EditingMessage { get; protected set; } = new Models.Message { Content= "Once upon a time" };

		public CompletionsService(OpenAIService openAIService) : base(openAIService)
		{
		}

		public async Task<Models.Message> Send(Models.Message message)
		{
			this.Messages.Add(message);

			var completionResult = await this.OpenAIService.Completions.CreateCompletion(new OpenAI.GPT3.ObjectModels.RequestModels.CompletionCreateRequest()
			{
				Prompt = message.Content,
				Model = OpenAI.GPT3.ObjectModels.Models.TextDavinciV3
			});

			if (completionResult.Successful)
				message = new Models.Message(Models.Role.User, completionResult.Choices.First().Text);
			else message = new Models.Message(Models.Role.Error, $"{completionResult.Error?.Code}: {completionResult.Error?.Message}");
			this.Messages.Add(message);
			return message;
		}

		public virtual async Task<Models.Message> Send()
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