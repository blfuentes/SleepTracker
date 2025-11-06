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
open System.Threading.Tasks

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

        builder.Services
            //.AddIdentity<IdentityUser, IdentityRole>()
            .AddIdentityCore<IdentityUser>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders()

        builder.Services.AddAuthorization()

        // Configure JWT Authentication
        let key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
        printfn "using key %s" builder.Configuration["Jwt:Key"]
        builder.Services.AddAuthentication(fun options ->
                // Disable cookie authentication
                options.DefaultScheme <- JwtBearerDefaults.AuthenticationScheme
                options.DefaultChallengeScheme <- JwtBearerDefaults.AuthenticationScheme
                options.RequireAuthenticatedSignIn <- false
            )
            .AddJwtBearer(fun options ->
                options.SaveToken <- true
                options.TokenValidationParameters <- TokenValidationParameters(
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                )
            
                options.Events <- JwtBearerEvents()
                options.Events.OnAuthenticationFailed <- fun ctx ->
                    printfn "JWT authentication failed: %s" ctx.Exception.Message
                    Task.CompletedTask
                
                options.Events.OnTokenValidated <- fun ctx ->
                    printfn "JWT validated for user: %s" (ctx.Principal.Identity.Name)
                    Task.CompletedTask
                
                options.Events.OnChallenge <- fun ctx ->
                    ctx.HandleResponse()

                    ctx.Response.StatusCode <- 401
                    ctx.Response.ContentType <- "application/json"
                    task {
                        use writer = new System.IO.StreamWriter(ctx.Response.Body)
                        do! writer.WriteAsync("{\"message\":\"Unauthorized\"}")
                        do! writer.FlushAsync()
                    }
            )

        builder.Services
            .AddDbContext<ApplicationDbContext>(fun options -> 
                options
                    .UseSqlite("Data Source=./Data/Identity.db") |> ignore)

        builder.Services.AddControllers()

        builder.Services.AddCors(fun options -> 
            options.AddPolicy("AllowAll", fun builder -> 
                 builder
                        //.WithOrigins("http://localhost:8080")
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .WithMethods("GET", "POST", "PUT", "OPTIONS")
                        //.AllowCredentials()
                        |> ignore))
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
