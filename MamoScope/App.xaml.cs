using MamoScope.Core.Interfaces;
using MamoScope.Data;
using MamoScope.Data.Repositories;
using MamoScope.Navigations;
using MamoScope.Services;
using MamoScope.ViewModels;
using MamoScope.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;



namespace MamoScope
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            var servicollection = new ServiceCollection();
            ConfigureServices(servicollection);
            ServiceProvider = servicollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextFactory<AppDbContext>();

            services.AddTransient<IMotorDriversService, MotorDriversService>();
            services.AddSingleton<MotorDriversStore>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddTransient<IMotorDriversRepository, MotorDriversRepository>();


            services.AddTransient<PastRecordsViewModel>();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<TestRecordsViewModel>();

            services.AddSingleton<MainWindowView>();
            services.AddTransient<PastRecordsView>();
            services.AddTransient<TestRecordsView>();

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            
            base.OnStartup(e);

            var dbFactory = ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
            using (var db = dbFactory.CreateDbContext())
            {
                db.Database.EnsureCreated();
            }

            _ = Task.Run(async () =>
            {
                using var db = dbFactory.CreateDbContext();
                db.MotorDrivers.Any();
            });

            var store = ServiceProvider.GetRequiredService<MotorDriversStore>();
            _ = store.YukleAsync();

            var mainLayout = ServiceProvider.GetRequiredService<MainWindowView>();
            var mainVM = ServiceProvider.GetRequiredService<MainWindowViewModel>();

           
            var ilkSayfa = ServiceProvider.GetRequiredService<TestRecordsView>();
            var ilkSayfaVM = ServiceProvider.GetRequiredService<TestRecordsViewModel>();
            ilkSayfa.DataContext = ilkSayfaVM; 

            
            mainVM.CurrentView = ilkSayfa;

            
            mainLayout.DataContext = mainVM;
            mainLayout.Show();
        }

    }

}
