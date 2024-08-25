namespace SleepTracker



#nowarn "20"

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

open SleepTracker.Services

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =
        Dapper.FSharp.MSSQL.OptionTypes.register()

        let builder = WebApplication.CreateBuilder(args)
        builder.WebHost.ConfigureAppConfiguration(fun builderContext config -> 
                            config.AddJsonFile(sprintf "appsettings.%s.json" builderContext.HostingEnvironment.EnvironmentName, optional = false, reloadOnChange = true)
                            config.AddUserSecrets()
                            config.AddEnvironmentVariables() |> ignore)

        builder.Services
            .AddControllersWithViews()
            .AddRazorRuntimeCompilation()
        builder.Services.AddRazorPages()
        builder.Services.AddMvc()

        let app = builder.Build()
        if not (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName = "local") then
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
        
        // Verify if the database is created
        let connectionString = app.Configuration.GetConnectionString("SqliteConnection")
        DatabaseService.getConnection(connectionString) |> DatabaseInitializer.safeInit

        app.Run()

        exitCode
