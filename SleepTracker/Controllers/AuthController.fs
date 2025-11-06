namespace SleepTracker.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.Authorization
open Microsoft.AspNetCore.Identity
open Microsoft.AspNetCore.Http
open System.Threading.Tasks

open SleeptTracker.Models.Account
open System.IdentityModel.Tokens.Jwt
open System.Text
open Microsoft.IdentityModel.Tokens
open System.Security.Claims
open System

[<ApiController>]
[<Route("api/[controller]")>]
type AuthController (
    logger : ILogger<AuthController>,
    signInManager: SignInManager<IdentityUser>,
    userManager: UserManager<IdentityUser>,
    context: IHttpContextAccessor,
    configuration: IConfiguration) =
    inherit ControllerBase()

    [<HttpPost("register")>]
    member this.Register([<FromBody>] model: RegisterModel) : Task<IActionResult> =
        task {
            if model.Password <> model.ConfirmPassword then
                return this.BadRequest([{ Code = "Password"; Description = "Passwords do not match" }]) :> IActionResult
            else
                let user = new IdentityUser(UserName = model.Email, Email = model.Email)
                let! result = userManager.CreateAsync(user, model.Password)
                if result.Succeeded then
                    return this.Ok("User registered successfully") :> IActionResult
                else
                    return this.BadRequest(result.Errors) :> IActionResult
        }

    [<HttpPost("login")>]
    member this.Login([<FromBody>] model: LoginModel) : Task<IActionResult> =
        task {
            // Validate the user by their email and password
            let! user = userManager.FindByEmailAsync(model.Email)
            if user = null then
                return this.Unauthorized("Invalid email or password") :> IActionResult
            else
                let! passwordValid = userManager.CheckPasswordAsync(user, model.Password)
                if passwordValid then
                    // Password IS valid, now generate the token
                    let tokenHandler = JwtSecurityTokenHandler()
                    printfn "using key:%s" configuration["Jwt:Key"]
                    let key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"])
                    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear()
                    let tokenDescriptor = SecurityTokenDescriptor(
                        Subject = ClaimsIdentity([|
                            Claim(ClaimTypes.Name, user.UserName)
                            Claim(ClaimTypes.NameIdentifier, user.Id)
                            Claim(JwtRegisteredClaimNames.Sub, user.Id)
                        |]),
                        Expires = DateTime.UtcNow.AddHours(1),
                        SigningCredentials = SigningCredentials(SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    )
                    let token = tokenHandler.CreateToken(tokenDescriptor)
                    let tokenString = tokenHandler.WriteToken(token)
                    let expiresIn = int (tokenDescriptor.Expires.Value - DateTime.UtcNow).TotalSeconds
                    
                    let refreshToken = Guid.NewGuid().ToString()
                    let response = { TokenType = "Bearer"; AccessToken = tokenString; ExpiresIn = expiresIn; RefreshToken = refreshToken }
                    
                    return this.Ok(response) :> IActionResult
                else
                    // Password was invalid
                    return this.Unauthorized("Invalid email or password") :> IActionResult
        }

    [<Authorize>]
    [<HttpPost("logout")>]
    member this.Logout() =
        task {        
            match context.HttpContext.User.Identity.IsAuthenticated with
            | true ->
                do! signInManager.SignOutAsync()
                return Results.Ok("User logged out successfully")
            | false ->
                return Results.BadRequest()
        }