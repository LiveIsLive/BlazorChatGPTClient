using OpenAI.GPT3.Managers;

namespace ColdShineSoft.Services
{
	public class WhisperService : BaseService
	{
		private static System.Collections.Generic.KeyValuePair<string, string>[] _ResponseFormats = null!;
		public System.Collections.Generic.KeyValuePair<string, string>[] ResponseFormats
		{
			get
			{
				if (_ResponseFormats == null)
					_ResponseFormats = typeof(OpenAI.GPT3.ObjectModels.StaticValues.AudioStatics.ResponseFormat).GetProperties().Select(p => new KeyValuePair<string, string>(p.Name, p.GetValue(null)?.ToString()!)).ToArray();
				return _ResponseFormats;
			}
		}

		public System.Collections.ObjectModel.ObservableCollection<Models.Message> Messages { get; } = new();

		public string FileName { get; set; } = null!;

		public System.IO.Stream FileStream { get; set; } = null!;

		public string Model { get; set; } = OpenAI.GPT3.ObjectModels.Models.WhisperV1;

		public string ResponseFormat { get; set; } = OpenAI.GPT3.ObjectModels.StaticValues.AudioStatics.ResponseFormat.VerboseJson;

		public string? Language { get; set; }

		public string? Prompt { get; set; }

		public float? Temperature { get; set; }

		public WhisperService(OpenAIService openAIService) : base(openAIService)
		{
		}

		public async Task<bool> Send()
		{
			System.Collections.Generic.Dictionary<string, string> messsage = new();
			messsage.Add("FileName", this.FileName);
			if (!string.IsNullOrWhiteSpace(this.Prompt))
				messsage.Add("Prompt", this.Prompt);
			this.Messages.Add(new Models.Message(System.Text.Json.JsonSerializer.Serialize(messsage)));

			byte[] file = new byte[this.FileStream.Length];
			int readSize = 0;
			do
			{
				readSize += await this.FileStream.ReadAsync(file, readSize, file.Length - readSize);
			}
			while (this.FileStream.Length > readSize);

			var audioResult = await this.OpenAIService.Audio.CreateTranscription(new OpenAI.GPT3.ObjectModels.RequestModels.AudioCreateTranscriptionRequest
			{
				FileName = this.FileName,
				File = file,
				Model = this.Model,
				ResponseFormat = this.ResponseFormat,
				Language=this.Language,
				Prompt=this.Prompt,
				Temperature=this.Temperature
			});

			if (audioResult.Successful)
				this.Messages.Add(new Models.Message(Models.MessageRole.Server, audioResult.Text));
			else this.Messages.Add(new Models.Message(Models.MessageRole.Error, $"{audioResult.Error?.Code}: {audioResult.Error?.Message}"));
			return audioResult.Successful;
		}
	}
}