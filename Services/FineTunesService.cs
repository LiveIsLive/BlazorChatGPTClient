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
		public System.Collections.ObjectModel.ObservableCollection<Models.Message> Messages { get; protected set; }// = new();

		//public Models.Message EdtingMessage { get; protected set; } = new Models.Message("https://t.co/f93xEd2 Excited to share my latest blog post! ->");

		public CompletionsService CompletionsService { get; protected set; }


		public string FileId { get; set; } = null!;

		public string Prompt { get; set; } = "https://t.co/f93xEd2 Excited to share my latest blog post! ->";

		public string Model { get; set; } = OpenAI.GPT3.ObjectModels.Models.Ada;

		public int? BatchSize { get; set; }

		public System.Collections.Generic.List<string> ClassificationBetas { get; set; } = new();

		public int? ClassificationNClasses { get; set; }

		public string? ClassificationPositiveClass { get; set; }

		public bool? ComputeClassificationMetrics { get; set; }

		public float? LearningRateMultiplier { get; set; }

		public int? NEpochs { get; set; }

		public int? PromptLossWeight { get; set; }

		public string? Suffix { get; set; }

		public string? ValidationFile { get; set; }

		public System.Action MessageAdded = null!;
		protected void OnMessageAdded()
		{
			if (this.MessageAdded != null)
				this.MessageAdded();
		}

		public FineTunesService(OpenAIService openAIService) : base(openAIService)
		{
			this.CompletionsService = new CompletionsService(this.OpenAIService);
			this.Messages = this.CompletionsService.Messages;
		}

		public async Task<bool> Send(string fileId, string prompt)
		{
			var createFineTuneResponse = await this.OpenAIService.FineTunes.CreateFineTune(new OpenAI.GPT3.ObjectModels.RequestModels.FineTuneCreateRequest()
			{
				TrainingFile = fileId,
				Model = this.Model,
				BatchSize=this.BatchSize,
				ClassificationBetas = this.ClassificationBetas.Count == 0 ? null : this.ClassificationBetas,
				ClassificationNClasses = this.ClassificationNClasses,
				ClassificationPositiveClass=this.ClassificationPositiveClass,
				ComputeClassificationMetrics=this.ComputeClassificationMetrics,
				LearningRateMultiplier=this.LearningRateMultiplier,
				NEpochs=this.NEpochs,
				PromptLossWeight=this.PromptLossWeight,
				Suffix=this.Suffix,
				ValidationFile=this.ValidationFile
			});

			var listFineTuneEventsStream = await this.OpenAIService.FineTunes.ListFineTuneEvents(createFineTuneResponse.Id, true);
			using var streamReader = new StreamReader(listFineTuneEventsStream);
			this.Messages.Add(new Models.Message(Models.Role.Server, (await streamReader.ReadToEndAsync())));
			this.OnMessageAdded();

			OpenAI.GPT3.ObjectModels.ResponseModels.FineTuneResponseModels.FineTuneResponse retrieveFineTuneResponse;
			do
			{
				retrieveFineTuneResponse = await this.OpenAIService.FineTunes.RetrieveFineTune(createFineTuneResponse.Id);
				if (retrieveFineTuneResponse.Status == "succeeded" || retrieveFineTuneResponse.Status == "cancelled" || retrieveFineTuneResponse.Status == "failed")
				{
					this.Messages.Add(new Models.Message(Models.Role.Server, $"Fine-tune Status for {createFineTuneResponse.Id}: {retrieveFineTuneResponse.Status}."));
					this.OnMessageAdded();
					break;
				}

				this.Messages.Add(new Models.Message(Models.Role.Server, $"Fine-tune Status for {createFineTuneResponse.Id}: {retrieveFineTuneResponse.Status}. Wait 10 more seconds"));
				this.OnMessageAdded();
				await Task.Delay(10_000);
			} while (true);

			this.CompletionsService.EditingMessage.Content = prompt;
			this.CompletionsService.Model = retrieveFineTuneResponse.Model;
			return await this.CompletionsService.Send();
		}

		public virtual async Task<bool> Send()
		{
			if (await this.Send(this.FileId, this.Prompt))
			{
				//this.CompletionsService.EditingMessage = new();
				this.Prompt = "";
				return true;
			}
			return false;
		}
	}
}