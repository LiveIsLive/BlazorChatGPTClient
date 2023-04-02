using OpenAI.GPT3.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdShineSoft.Services
{
	public class ImageDallEService : BaseService
	{
		public System.Collections.Generic.KeyValuePair<string, string>[] Sizes { get; } = typeof(OpenAI.GPT3.ObjectModels.StaticValues.ImageStatics.Size).GetProperties().Select(p => new KeyValuePair<string, string>(p.Name, p.GetValue(null)?.ToString()!)).ToArray();

		public int? ImageCount { get; set; }

		public string? Size { get; set; }

		public System.Collections.ObjectModel.ObservableCollection<Models.Message> Messages { get; } = new();

		public Models.Message EditingMessage { get; protected set; } = new();

		public ImageDallEService(OpenAIService openAIService) : base(openAIService)
		{
		}

		public async Task<Models.Message[]> Send(Models.Message message)
		{
			this.Messages.Add(message);

			var imageResult = await this.OpenAIService.Image.CreateImage(new OpenAI.GPT3.ObjectModels.RequestModels.ImageCreateRequest
			{
				Prompt = message.Content,
				N = this.ImageCount,
				Size = this.Size,
				ResponseFormat = OpenAI.GPT3.ObjectModels.StaticValues.ImageStatics.ResponseFormat.Url,
				User = this.User
			});

			if (imageResult.Successful)
			{
				Models.Message[] messages = imageResult.Results.Select(r => new Models.Message(Models.Role.Image, r.Url)).ToArray();
				foreach (Models.Message m in messages)
					this.Messages.Add(m);
				return messages;
			}
			else
			{
				message = new Models.Message(Models.Role.Error, $"{imageResult.Error?.Code}: {imageResult.Error?.Message}");
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