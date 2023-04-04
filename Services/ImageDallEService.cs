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

		public async Task<bool> Send(Models.Message message)
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
				foreach (OpenAI.GPT3.ObjectModels.ResponseModels.ImageResponseModel.ImageCreateResponse.ImageDataResult result in imageResult.Results)
					this.Messages.Add(new Models.Message(Models.Role.Image, result.Url));
			else this.Messages.Add(new Models.Message(Models.Role.Error, $"{imageResult.Error?.Code}: {imageResult.Error?.Message}"));
			return imageResult.Successful;

		}

		public virtual async Task<bool> Send()
		{
			if(await this.Send(this.EditingMessage))
			{
				this.EditingMessage = new();
				return true;
            }
			return false;
		}
	}
}