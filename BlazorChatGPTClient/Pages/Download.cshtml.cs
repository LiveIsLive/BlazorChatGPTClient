using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ColdShineSoft.BlazorChatGPTClient.Pages
{
    public class DownloadModel : PageModel
    {
        protected readonly Services.FilesService FilesService;

        public DownloadModel(Services.FilesService filesService)
        {
            this.FilesService = filesService;
        }

        public async Task<IActionResult> OnGet(string fileId, string fileName)
        {
            KeyValuePair<bool, string> result = await this.FilesService.DownloadFile(fileId);
            if (result.Key)
                return this.File(System.Text.Encoding.UTF8.GetBytes(result.Value), "application/octet-stream", fileName);
            return this.Content(result.Value, "plain/text");
        }
    }
}