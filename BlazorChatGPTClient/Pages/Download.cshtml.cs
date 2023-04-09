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
            return this.File(System.Text.Encoding.UTF8.GetBytes(await this.FilesService.DownloadFile(fileId)), "application/octet-stream", fileName);
        }
    }
}