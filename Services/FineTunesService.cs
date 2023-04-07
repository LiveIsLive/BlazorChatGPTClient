using OpenAI.GPT3.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdShineSoft.Services
{
	public class FineTunesService : BaseService
	{
		public System.Collections.ObjectModel.ObservableCollection<Models.Message> Messages { get; } = new();

		public Models.Message EdtingMessage { get; protected set; } = new Models.Message("https://t.co/f93xEd2 Excited to share my latest blog post! ->");


		public string FileId { get; set; } = null!;

		public string Model { get; set; } = OpenAI.GPT3.ObjectModels.Models.Ada;

		public FineTunesService(OpenAIService openAIService) : base(openAIService)
		{
		}

		public async Task<bool> Send(string fileId, Models.Message promptMessage)
		{
			var createFineTuneResponse = await this.OpenAIService.FineTunes.CreateFineTune(new OpenAI.GPT3.ObjectModels.RequestModels.FineTuneCreateRequest()
			{
				TrainingFile = fileId,
				Model = this.Model
			});

			var listFineTuneEventsStream = await this.OpenAIService.FineTunes.ListFineTuneEvents(createFineTuneResponse.Id, true);
			using var streamReader = new StreamReader(listFineTuneEventsStream);
			this.Messages.Add(new Models.Message(Models.Role.Server, (await streamReader.ReadToEndAsync())));


			OpenAI.GPT3.ObjectModels.ResponseModels.FineTuneResponseModels.FineTuneResponse retrieveFineTuneResponse;
			do
			{
				retrieveFineTuneResponse = await this.OpenAIService.FineTunes.RetrieveFineTune(createFineTuneResponse.Id);
				if (retrieveFineTuneResponse.Status == "succeeded" || retrieveFineTuneResponse.Status == "cancelled" || retrieveFineTuneResponse.Status == "failed")
				{
					this.Messages.Add(new Models.Message(Models.Role.Server, $"Fine-tune Status for {createFineTuneResponse.Id}: {retrieveFineTuneResponse.Status}."));
					break;
				}

				this.Messages.Add(new Models.Message(Models.Role.Error, $"Fine-tune Status for {createFineTuneResponse.Id}: {retrieveFineTuneResponse.Status}. Wait 10 more seconds"));
				await Task.Delay(10_000);
			} while (true);

			do
			{
				this.Messages.Add(promptMessage);
				var completionResult = await this.OpenAIService.Completions.CreateCompletion(new OpenAI.GPT3.ObjectModels.RequestModels.CompletionCreateRequest()
				{
					MaxTokens = 1,
					Prompt = promptMessage.Content,
					Model = retrieveFineTuneResponse.FineTunedModel,
					LogProbs = 2
				});
				int error = 0;
				if (completionResult.Successful)
				{
					foreach (OpenAI.GPT3.ObjectModels.SharedModels.ChoiceResponse choice in completionResult.Choices)
						this.Messages.Add(new Models.Message(Models.Role.Server, choice.Text));
					return true;
				}
				else
				{
					this.Messages.Add(new Models.Message(Models.Role.Error, $"{completionResult.Error?.Code}: {completionResult.Error?.Message}"));
					await Task.Delay(10_000);
					if (error++ > 10)
						return false;
				}
			} while (true);
		}

		public virtual async Task<bool> Send()
		{
			if (await this.Send(this.FileId, this.EdtingMessage))
			{
				this.EdtingMessage = new();
				return true;
			}
			return false;
		}
	}
}