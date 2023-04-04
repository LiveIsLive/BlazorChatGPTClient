﻿using System;
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

		public async Task<bool> Send(System.Collections.Generic.IEnumerable<Models.Message> messages)
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
				foreach (OpenAI.GPT3.ObjectModels.ResponseModels.EmbeddingResponse data in embeddingResult.Data)
					this.Messages.Add(new Models.Message(Models.Role.Server, $"{{Index:{data.Index},Embedding:[{string.Join(",", data.Embedding)}]}}"));
			else this.Messages.Add(new Models.Message(Models.Role.Error, $"{embeddingResult.Error?.Code}: {embeddingResult.Error?.Message}"));
			return embeddingResult.Successful;
		}

		public virtual async Task<bool> Send()
		{
			if(await this.Send(this.EditingMessages))
            {
				this.EditingMessages = new() { new Models.Message() };
				return true;
            }
			return false;
		}
	}
}