using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI.GPT3.Interfaces;

namespace ColdShineSoft.Services
{
	public class FilesService : BaseService
	{
		public string? LastError { get; protected set; }

		public OpenAI.GPT3.ObjectModels.UploadFilePurposes.UploadFilePurpose FilePurpose { get; set; }

		public string FileName { get; set; } = null!;

		public byte[] FileContent { get; set; } = null!;

		public List<OpenAI.GPT3.ObjectModels.SharedModels.FileResponse> UploadedFiles { get; protected set; } = new();

		public FilesService(OpenAI.GPT3.Managers.OpenAIService openAIService) : base(openAIService)
		{
			
		}

		public async Task<bool> Send()
		{
			var uploadFilesResponse = await this.OpenAIService.Files.FileUpload(this.FilePurpose, this.FileContent, this.FileName);

			if (uploadFilesResponse.Successful)
				return await this.ListFile();
			else
			{
				this.LastError = $"{uploadFilesResponse.Error?.Code}: {uploadFilesResponse.Error?.Message}";
				return false;
			}
		}

		public async Task<bool> ListFile()
        {
			var uploadedFiles = await this.OpenAIService.Files.ListFile();
			if(uploadedFiles.Successful)
				this.UploadedFiles = uploadedFiles.Data;
			else this.LastError = $"{uploadedFiles.Error?.Code}: {uploadedFiles.Error?.Message}";
			return uploadedFiles.Successful;
		}

	}
}