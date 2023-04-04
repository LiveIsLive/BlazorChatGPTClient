using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdShineSoft.Services
{
	public class EmbeddingsService : BaseService
	{
		public System.Collections.ObjectModel.ObservableCollection<Models.Message> Messages { get; } = new();

		public System.Collections.ObjectModel.ObservableCollection<Models.Message> EditingMessages { get; protected set; } = new() { { new Models.Message("The quick brown fox jumped over the lazy dog.") } };

		public string? Model { get; set; } = OpenAI.GPT3.ObjectModels.Models.TextSearchAdaDocV1;

		public string? Input { get; set; }

		public EmbeddingsService(OpenAI.GPT3.Managers.OpenAIService openAIService) : base(openAIService)
		{
		}

		public async Task<Models.Message[]> Send(System.Collections.Generic.IEnumerable<Models.Message> messages)
		{
			foreach (Models.Message m in messages)
				this.Messages.Add(m);

			var embeddingResult = await this.OpenAIService.Embeddings.CreateEmbedding(new OpenAI.GPT3.ObjectModels.RequestModels.EmbeddingCreateRequest
			{
				InputAsList = messages.Select(m=>m.Content).ToList(),
				Model = this.Model,
				Input = this.Input
			});

			if (embeddingResult.Successful)
            {
				messages = embeddingResult.Data.Select(d=> new Models.Message(Models.Role.Server, $"{{Index:{d.Index},Embedding:[{string.Join(",",d.Embedding)}]}}")).ToArray();
				foreach (Models.Message m in messages)
					this.Messages.Add(m);
				return (Models.Message[])messages;
            }
			else
			{
				Models.Message message = new Models.Message(Models.Role.Error, $"{embeddingResult.Error?.Code}: {embeddingResult.Error?.Message}");
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
				this.EditingMessages = new() { new Models.Message() };
			}
		}
	}
}