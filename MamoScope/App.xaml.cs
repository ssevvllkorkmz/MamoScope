using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MamoScope.ViewModels;
using MamoScope.Views;
using MamoScope.Data;
using System;
using Microsoft.EntityFrameworkCore;



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

            services.AddSingleton<PastRecordsViewModel>();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<TestRecordsViewModel>();

            services.AddTransient<MainWindowView>();
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
