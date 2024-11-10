namespace SleepTracker

#nowarn "20"

open Microsoft.AspNetCore.Authentication.JwtBearer
open System.Text
open Microsoft.IdentityModel.Tokens
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Identity 
open Microsoft.EntityFrameworkCore

open SleepTracker.IdentityLibrary
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

        builder.Services.AddHttpContextAccessor()

        builder.Services.AddAuthorization()

        // Configure JWT Authentication
        let key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
        builder.Services.AddAuthentication(fun options ->
                // Disable cookie authentication
                options.DefaultScheme <- JwtBearerDefaults.AuthenticationScheme
                options.DefaultChallengeScheme <- JwtBearerDefaults.AuthenticationScheme
                options.RequireAuthenticatedSignIn <- false
            )
            .AddJwtBearer(fun options ->
                options.SaveToken = true
                options.TokenValidationParameters <- TokenValidationParameters(
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                )
                options.Events <- JwtBearerEvents(
                    OnChallenge = fun context ->
                        // Override the default challenge behavior to return 401 Unauthorized
                        context.Response.StatusCode <- 401
                        context.Response.ContentType <- "application/json"
                        task {
                            // Create a stream writer and write the response
                            use writer = new System.IO.StreamWriter(context.Response.Body)
                            do! writer.WriteAsync("{\"message\":\"Unauthorized\"}")
                            do! writer.FlushAsync() // Ensure content is sent
                        }
                ))

        builder.Services
            .AddDbContext<ApplicationDbContext>(fun options -> 
                options
                    .UseSqlite("Data Source=./Data/Identity.db") |> ignore)

        builder.Services
            .AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()

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

        app.UseEndpoints(fun endpoints ->
            endpoints.MapControllers() |> ignore
        ) |> ignore

        // Verify if the database is created
        let connectionString = app.Configuration.GetConnectionString("SqliteConnection")
        DatabaseService.getConnection(connectionString) |> DatabaseInitializer.safeInit

        app.Run()

        exitCode
