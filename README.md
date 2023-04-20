# 简介
BlazorChatGPTClient是一个调用ChatGPT开放接口的网站GUI，基于开源项目[Betalgo.OpenAI](https://github.com/betalgo/openai/wiki)调用ChatGPT的开放接口。
# 部署
部署本应用包含以下几个步骤：
## 安装.NET 6
到官网下载并安装 ASP.NET Core Runtime 6

<https://dotnet.microsoft.com/en-us/download/dotnet/6.0>
## 下载本应用
到本项目的发布页面下载压缩包ColdShineSoft.BlazorChatGPTClient.zip

<https://github.com/LiveIsLive/BlazorChatGPTClient/releases>

然后解压
## 设置ApiKey
到解压目录打开配置文件appsettings.json并设置你的ApiKey
## 启动网站
如果是Windows系统，执行以下命令启动网站：

ColdShineSoft.BlazorChatGPTClient.exe --urls http://*:8080

如果是Linux系统，执行以下命令启动网站：

dotnet ColdShineSoft.BlazorChatGPTClient.dll --urls http://*:8080
# 使用
执行上述命令后，在浏览器打开网页http://localhost:8080/就可以打开本应用。

打开后看到的将会是登录界面，默认用户名是：a，默认密码是:0

登录后，可以点击右上角菜单“用户管理”来修改用户名或者密码。

登录后可以看到网站的基本界面是这样：

![image](https://user-images.githubusercontent.com/8569038/233302779-45e9b192-6500-4e93-9577-1d8b395a01e5.png)
