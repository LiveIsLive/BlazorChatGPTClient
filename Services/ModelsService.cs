using OpenAI.GPT3.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdShineSoft.Services
{
	public class ModelsService : BaseService
	{
		public ModelsService(OpenAIService openAIService) : base(openAIService)
		{
		}

		public async Task<string[]> ListAllModels()
        {
			var modelList = await this.OpenAIService.Models.ListModel();
			//foreach (var modelItem in modelList.Models)
			//         {
			//	var retrievedModelResponse = await this.OpenAIService.Models.RetrieveModel(modelItem.Id);
			//	if (retrievedModelResponse.Successful)
			//	{
			//		//Congrats
			//	}
			//}
			return modelList.Models.Select(m => m.Id).OrderBy(m => m).ToArray();
		}
	}
}