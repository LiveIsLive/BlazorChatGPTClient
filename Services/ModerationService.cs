﻿using OpenAI.GPT3.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdShineSoft.Services
{
	public class ModerationService : BaseService
	{
		public System.Collections.ObjectModel.ObservableCollection<Models.Message> Messages { get; } = new();

		public Models.Message Message { get; protected set; } = new Models.Message(Models.Role.Assistant, "I want to kill them.");

		public string? Model { get; set; }

		public System.Collections.Generic.List<string> InputAsList { get; set; } = new();

		public ModerationService(OpenAIService openAIService) : base(openAIService)
		{
		}

		public async Task<bool> Send(Models.Message message)
		{
			this.Messages.Add(message);

			var moderationResponse = await this.OpenAIService.Moderation.CreateModeration(new OpenAI.GPT3.ObjectModels.RequestModels.CreateModerationRequest
			{
				Input = this.Message.Content,
				Model = this.Model,
				InputAsList = this.InputAsList.Count == 0 ? null : this.InputAsList
			});

			if (moderationResponse.Successful)
				foreach (OpenAI.GPT3.ObjectModels.ResponseModels.Result result in moderationResponse.Results)
					this.Messages.Add(new Models.Message(Models.Role.Server, System.Text.Json.JsonSerializer.Serialize(result)));
			else this.Messages.Add(new Models.Message(Models.Role.Error, $"{moderationResponse.Error?.Code}: {moderationResponse.Error?.Message}"));
			return moderationResponse.Successful;
		}

		public virtual async Task<bool> Send()
		{
			if(await this.Send(this.Message))
            {
				this.Message = new();
				return true;
            }
			return false;
		}
	}
}