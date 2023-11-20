using Berzerk.Logic.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;

namespace Berzerk.UI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            var services = new ServiceCollection();
            RegisterServices(services);
            var serviceProvider = services.BuildServiceProvider();
            var mainMenu = serviceProvider.GetRequiredService<MainMenu>();
            Application.Run(mainMenu);
        }

        static void RegisterServices(ServiceCollection services)
        {
            services.AddScoped<MainMenu>();
            services.AddScoped<GameWindow>();
            services.AddScoped<IMapService, MapService>();
        }
    }
}