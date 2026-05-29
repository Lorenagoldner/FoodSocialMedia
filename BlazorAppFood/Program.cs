using BlazorAppFood.Auth;
using BlazorAppFood.Configuration;
using BlazorAppFood.Data;
using BlazorAppFood.Models;
using BlazorAppFood.Repositories;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Syncfusion.Blazor;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

       

        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddSignalR();
        builder.Services.AddSyncfusionBlazor();

       
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("FoodSocialMediaDB")));

       
        builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
        builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
        builder.Services.AddScoped<IRegisterRepository, RegisterRepository>();
        builder.Services.AddScoped<ILoginRepository, LoginRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IChatRepository, ChatRepository>();
        builder.Services.AddScoped<IGroupRepository, GroupRepository>();
        builder.Services.AddScoped<IPostRepository, PostRepository>();
        builder.Services.AddScoped<ITagRepository, TagRepository>();

        //Sessão e Autenticação
        builder.Services.AddBlazoredSessionStorage();
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

        
        var sqlConnectionConfiguration = new SqlConnectionConfiguration(builder.Configuration.GetConnectionString("FoodSocialMediaDB"));
        builder.Services.AddSingleton(sqlConnectionConfiguration);

        
        var app = builder.Build();

        //Licença do Syncfusion
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDMzNDY3QDMxMzkyZTMxMmUzMFEvbWN2d2l1SzNWaDBPbnNFTFlUcmtwZnN4NGQ1Q2lMTENyNHJBdWhoYTg9");

        //Configuração de ambiente
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapBlazorHub();
        app.MapHub<GroupChatHub>("/groupChatHub");
        app.MapFallbackToPage("/_Host");


        app.Run();
    }
}