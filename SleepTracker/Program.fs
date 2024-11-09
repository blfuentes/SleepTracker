namespace SleepTracker

open System


#nowarn "20"

open Microsoft.EntityFrameworkCore.Migrations.Internal
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open System.IO 
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.EntityFrameworkCore
open Microsoft.AspNetCore.Identity 

open SleepTracker.Services

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =
        Dapper.FSharp.MSSQL.OptionTypes.register()

        let builder = WebApplication.CreateBuilder(args)
        builder.WebHost
            .ConfigureAppConfiguration(fun builderContext config -> 
                            config.AddJsonFile(sprintf "appsettings.%s.json" builderContext.HostingEnvironment.EnvironmentName, optional = false, reloadOnChange = true)
                            config.AddUserSecrets()
                            config.AddEnvironmentVariables()
                            |> ignore)

        builder.Services
            .AddDbContext<ApplicationDbContext.ApplicationDbContext>(fun options -> 
                options
                    .UseSqlite("Data Source=./Data/Identity.db") |> ignore)

        // IdentityUser, IdentityRole from MS.ASP.Identity
        builder.Services.AddIdentity<IdentityUser, IdentityRole>(fun options -> 
                options.Password.RequireLowercase <- true
                options.Password.RequireUppercase <- true
                options.Password.RequireDigit <- true
                options.Lockout.MaxFailedAccessAttempts <- 5
                options.Lockout.DefaultLockoutTimeSpan <- TimeSpan.FromMinutes(15)
                options.User.RequireUniqueEmail <- true
                // enable this if we use email verification 
                // options.SignIn.RequireConfirmedEmail <- true;
                )
            // tell asp.net identity to use the above store
            .AddEntityFrameworkStores<ApplicationDbContext.ApplicationDbContext>()
            .AddDefaultTokenProviders() // need for email verification token generation
            |> ignore
        //builder.Services.AddIdentityCore<IdentityUser>(fun options -> options.SignIn.RequireConfirmedAccount <- true) |> ignore
        
        builder.Services.AddControllers()

        builder.Services.AddCors(fun options -> 
            options.AddPolicy("AllowAll", fun builder -> 
                 builder.AllowAnyHeader()
                        .AllowAnyOrigin()
                        .WithMethods("GET", "POST", "PUT") |> ignore))
            |> ignore


        let app = builder.Build()

        app.UseCors("AllowAll")

        if not (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName = "local") then
            app.UseExceptionHandler("/Home/Error")
            app.UseHsts() |> ignore // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.

        app.UseHttpsRedirection()

        app.UseStaticFiles()
        app.UseRouting()
        app.UseAuthentication()
        app.UseAuthorization()

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
