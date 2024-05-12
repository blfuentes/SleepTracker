namespace SleepTracker

#nowarn "20"

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration.UserSecrets


module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =
        Dapper.FSharp.MSSQL.OptionTypes.register()
        let builder = WebApplication.CreateBuilder(args)

        builder
            .Services
            .AddControllersWithViews()
            .AddRazorRuntimeCompilation()

        builder.Services.AddRazorPages()
        builder.Services.AddMvc()

        let app = builder.Build()

        if not (builder.Environment.IsDevelopment()) then
            app.UseExceptionHandler("/Home/Error")
            app.UseHsts() |> ignore // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.

        app.UseHttpsRedirection()

        app.UseStaticFiles()
        app.UseRouting()
        app.UseAuthorization()

        app.MapControllerRoute(name = "default", pattern = "{controller=Home}/{action=Index}/{id?}")

        app.MapRazorPages()

        // Use Endpoints
        app.UseEndpoints(fun endpoints ->
            endpoints.MapControllers() |> ignore
            // Add other endpoint mappings as needed
        ) |> ignore
        
        app.Run()

        exitCode
