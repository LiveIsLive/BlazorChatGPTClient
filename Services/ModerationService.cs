using OpenAI.GPT3.Managers;
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

		public Models.Message EditingMessage { get; protected set; } = new Models.Message(Models.MessageRole.Assistant, "I want to kill them.");

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
				Input = this.EditingMessage.Content,
				Model = this.Model,
				InputAsList = this.InputAsList.Count == 0 ? null : this.InputAsList
			});

			if (moderationResponse.Successful)
			{
				if (moderationResponse.Results.Count == 0)
				{
					this.Messages.Add(new Models.Message(Models.MessageRole.Error, "服务器没有返回数据"));
					return false;
				}
				foreach (OpenAI.GPT3.ObjectModels.ResponseModels.Result result in moderationResponse.Results)
					this.Messages.Add(new Models.Message(Models.MessageRole.Server, System.Text.Json.JsonSerializer.Serialize(result)));
				return true;
			}
			else
			{
				this.Messages.Add(new Models.Message(Models.MessageRole.Error, $"{moderationResponse.Error?.Code}: {moderationResponse.Error?.Message}"));
				return false;
			}
		}

		public virtual async Task<bool> Send()
		{
			if(await this.Send(this.EditingMessage))
            {
				this.EditingMessage = new();
				return true;
            }
			this.EditingMessage = new Models.Message(this.EditingMessage);
			return false;
		}
	}
}