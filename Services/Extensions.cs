using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ColdShineSoft.Services
{
	public static class Extensions
	{
		public static Microsoft.Extensions.DependencyInjection.IServiceCollection AddChatGPTServices(this Microsoft.Extensions.DependencyInjection.IServiceCollection services)
		{
			System.Type baseType = typeof(BaseService);
			foreach (System.Type type in System.Reflection.Assembly.GetExecutingAssembly().GetExportedTypes())
			{
				if (type.IsAbstract)
					continue;
				if(baseType.IsAssignableFrom(type))
					services.AddTransient(type);
			}
			services.AddTransient<UserService>();
			services.AddScoped<LoginService>();
			return services;
		}
	}
}
