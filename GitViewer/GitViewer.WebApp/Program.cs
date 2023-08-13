using GitViewer.GitStorage;
using GitViewer.GitStorage.Local;
using GitViewer.GitStorage.Remote;
using GitViewer.WebApp.Models;

namespace GitViewer.WebApp
{
    public class Program
    { 
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            builder.Services.AddDbContext<GitLocalStorageDbContext>(ServiceLifetime.Singleton);
            builder.Services.AddSingleton<GitRemoteStorage>();
            builder.Services.AddSingleton<GitLocalStorage>();
            builder.Services.AddSingleton<GitStorageFasade>();
            builder.Services.AddScoped<CommitsViewModel>();
            builder.Services.AddScoped<GitConfigViewModel>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //}
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Index}");

            app.Run();
        }
    }
}