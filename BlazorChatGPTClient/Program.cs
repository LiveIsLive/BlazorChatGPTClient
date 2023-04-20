using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Services;
using OpenAI.GPT3.Extensions;
using ColdShineSoft.Services;


//System.Net.Http.HttpClient httpClient = new(new System.Net.Http.HttpClientHandler { Proxy = new System.Net.WebProxy("http://104.27.34.224"), UseProxy = true });
//var httpResult = await httpClient.GetAsync("https://chat.openai.com/chat");
//var html = await httpResult.Content.ReadAsStringAsync();

//var openAiService = new OpenAI.GPT3.Managers.OpenAIService(new OpenAI.GPT3.OpenAiOptions()
//{
//	ApiKey = builder.Configuration["OpenAIServiceOptions:ApiKey"]
//});
//openAiService.SetDefaultModelId(OpenAI.GPT3.ObjectModels.Models.ChatGpt3_5Turbo);


//var completionResult = await openAiService.ChatCompletion.CreateCompletion(new OpenAI.GPT3.ObjectModels.RequestModels.ChatCompletionCreateRequest
//{
//	Messages = new List<OpenAI.GPT3.ObjectModels.RequestModels.ChatMessage>
//	{
//		OpenAI.GPT3.ObjectModels.RequestModels.ChatMessage.FromUser("你是谁？")
//	}
//});
//if (completionResult.Successful)
//{
//	Console.WriteLine(completionResult.Choices.First().Message.Content);
//}
//else
//{
//	Console.WriteLine($"{completionResult.Error.Code}: {completionResult.Error.Message}");
//}
//System.Console.ReadKey();
//return;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

//builder.Services.AddOpenAIService(settings => settings.ApiKey = builder.Configuration["OpenAIServiceOptions:ApiKey"]);
builder.Services.AddTransient<OpenAI.GPT3.Managers.OpenAIService>(provider => new OpenAI.GPT3.Managers.OpenAIService(new OpenAI.GPT3.OpenAiOptions
{
	ApiKey = builder.Configuration["OpenAIServiceOptions:ApiKey"],
	Organization = builder.Configuration["OpenAIServiceOptions:Organization"],
	DefaultModelId = builder.Configuration["OpenAIServiceOptions:DefaultModelId"]
}));

LiteDB.LiteDatabase database = new LiteDB.LiteDatabase(System.IO.Path.Combine(builder.Environment.ContentRootPath, "Data.db"));
database.Mapper.EnumAsInteger = true;
builder.Services.AddSingleton<LiteDB.ILiteDatabase>(database);

//var collection = database.GetCollection<ColdShineSoft.Models.User>();
//collection.EnsureIndex(u => u.UserId, true);
//collection.EnsureIndex(u => u.UserName, true);
//collection.Insert(new ColdShineSoft.Models.User { Role = ColdShineSoft.Models.UserRole.管理员, UserName = "a", Password = System.BitConverter.ToString(System.Security.Cryptography.MD5.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes("0"))).Replace("-", "") });
//var u=collection.FindOne(u => u.UserName == "a");

builder.Services.AddMudServices();
builder.Services.AddMudMarkdownServices();
//builder.Services.AddTransient<ColdShineSoft.Services.BasicChatService>();
builder.Services.AddChatGPTServices();

builder.Services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider, AuthenticationStateProvider>();
//#region BLAZOR COOKIE Auth
//builder.Services.Configure<CookiePolicyOptions>(options =>
//{
//	options.CheckConsentNeeded = context => true;
//	options.MinimumSameSitePolicy = SameSiteMode.None;
//});
//builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
//builder.Services.AddHttpContextAccessor();
//builder.Services.AddScoped<HttpContextAccessor>();
//builder.Services.AddHttpClient();
//builder.Services.AddScoped<HttpClient>();
//#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
	app.UseExceptionHandler("/Error");


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

#region BLAZOR COOKIE Auth
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseAuthentication();
#endregion

app.Run();
