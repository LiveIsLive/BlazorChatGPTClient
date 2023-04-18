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

		public System.IO.Stream FileStream { get; set; } = null!;

		public List<OpenAI.GPT3.ObjectModels.SharedModels.FileResponse> UploadedFiles { get; protected set; } = null!;

		public FilesService(OpenAI.GPT3.Managers.OpenAIService openAIService) : base(openAIService)
		{
			
		}

		public async Task<bool> Send()
		{
			var uploadFilesResponse = await this.OpenAIService.Files.FileUpload(this.FilePurpose, this.FileStream, this.FileName);

			if (uploadFilesResponse.Successful)
            {
				this.LastError = null;
 				return await this.ListFile();
           }
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
            {
				this.UploadedFiles = uploadedFiles.Data;
				this.LastError = null;
            }
			else this.LastError = $"{uploadedFiles.Error?.Code}: {uploadedFiles.Error?.Message}";
			return uploadedFiles.Successful;
		}

		//public async Task<bool> RetrieveFile(OpenAI.GPT3.ObjectModels.SharedModels.FileResponse file)
  //      {
		//	var retrieveFileResponse = await this.OpenAIService.Files.RetrieveFile(file.Id);

		//	if (retrieveFileResponse.Successful)
		//	{
		//		// Congrats
		//	}
		//	return retrieveFileResponse.Successful;
		//}

		public async Task<System.Collections.Generic.KeyValuePair<bool,string>>DownloadFile(string fileId)
		{
			var fileContentResponse = await this.OpenAIService.Files.RetrieveFileContent(fileId);
			if (fileContentResponse.Successful)
				return new(true,fileContentResponse.Content??"");
			return new(false,$"{fileContentResponse.Error?.Code}: {fileContentResponse.Error?.Message}");
		}

		public async Task<bool> DeleteFile(OpenAI.GPT3.ObjectModels.SharedModels.FileResponse file)
        {
			var deleteResponse = await this.OpenAIService.Files.DeleteFile(file.Id);

			if (deleteResponse.Successful)
				this.LastError = null;
			else this.LastError = $"{deleteResponse.Error?.Code}: {deleteResponse.Error?.Message}";
			return deleteResponse.Successful;
		}

	}
}