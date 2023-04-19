using ColdShineSoft.BlazorChatGPTClient.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Services;
using OpenAI.GPT3.Extensions;


//System.Net.Http.HttpClient httpClient = new(new System.Net.Http.HttpClientHandler { Proxy = new System.Net.WebProxy("http://104.27.34.224"), UseProxy = true });
//var httpResult = await httpClient.GetAsync("https://chat.openai.com/chat");
//var html=await httpResult.Content.ReadAsStringAsync();

//var openAiService = new OpenAI.GPT3.Managers.OpenAIService(new OpenAI.GPT3.OpenAiOptions()
//{
//	ApiKey = "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"
//});
//openAiService.SetDefaultModelId(OpenAI.GPT3.ObjectModels.Models.ChatGpt3_5Turbo);


//var completionResult = await openAiService.ChatCompletion.CreateCompletion(new OpenAI.GPT3.ObjectModels.RequestModels.ChatCompletionCreateRequest
//{
//    Messages = new List<OpenAI.GPT3.ObjectModels.RequestModels.ChatMessage>
//    {
//        OpenAI.GPT3.ObjectModels.RequestModels.ChatMessage.FromUser("����C#д��һ�����ͼƬ��Controller")
//    }
//});
//if (completionResult.Successful)
//{
//    Console.WriteLine(completionResult.Choices.First().Message.Content);
//}
//else
//{
//    Console.WriteLine($"{completionResult.Error.Code}: {completionResult.Error.Message}");
//}
//System.Console.ReadKey();
//return;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

//builder.Services.AddOpenAIService(settings => settings.ApiKey = builder.Configuration["OpenAIServiceOptions:ApiKey"]);
builder.Services.AddSingleton<OpenAI.GPT3.Managers.OpenAIService>(new OpenAI.GPT3.Managers.OpenAIService(new OpenAI.GPT3.OpenAiOptions { ApiKey = builder.Configuration["OpenAIServiceOptions:ApiKey"], DefaultModelId = OpenAI.GPT3.ObjectModels.Models.ChatGpt3_5Turbo }));

builder.Services.AddMudServices();
builder.Services.AddMudMarkdownServices();
builder.Services.AddTransient<ColdShineSoft.Services.BasicChatService>();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	System.Console.Write("IsDevelopment");
}
else System.Console.Write("IsProduction");


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
