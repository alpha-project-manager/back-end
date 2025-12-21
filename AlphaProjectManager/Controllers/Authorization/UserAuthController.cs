using System.Linq.Expressions;
using AlphaProjectManager.Controllers.Authorization.Requests;
using AlphaProjectManager.Controllers.Authorization.Responses;
using AlphaProjectManager.Controllers.Base.Responses;
using AlphaProjectManager.Controllers.Shared;
using Application.DataQuery;
using Application.Services;
using Application.Services.AuthService;
using Application.Utils;
using Domain;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaProjectManager.Controllers.Authorization;

[Route("/api/auth/")]
public class UserAuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly BaseService<User> _userService;
    private readonly BaseService<Tutor> _tutorService;
    private readonly BaseService<CalendarSettings> _calendarSettingsService;

    public UserAuthController(IAuthService authService,
        BaseService<User> userService, BaseService<Tutor> tutorService, 
        BaseService<CalendarSettings> calendarSettingsService)
    {
        _authService = authService;
        _userService = userService;
        _tutorService = tutorService;
        _calendarSettingsService = calendarSettingsService;
    }
    
    /// <summary>
    /// Авторизация пользователя по email и паролю.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginUserWithEmail([FromBody] LoginRequest dto)
    {
        if (User.Identity!.IsAuthenticated)
        {
            return SharedResponses.FailedRequest("User is already authenticated.");
        }
        
        var loginInfo = await _authService.TryLoginUserAsync(dto.Email, dto.Password);
        if (loginInfo.User == null)
        {
            return SharedResponses.FailedRequest(loginInfo.errorMsg);
        }
        WriteRefreshTokenToCookies(loginInfo.RefreshToken);
        
        return Ok(new LoginResponse
        {
            UserId = loginInfo.User.Id,
            Message = "User successfully authorized.",
            Completed = true,
            AccessToken = loginInfo.AccessToken,
            Fullname = loginInfo.User.FullName
        });
    }
    
    /// <summary>
    /// Обновление access и refresh токенов пользователя
    /// </summary>
    [Authorize]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(RefreshResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refresh()
    {
        if (!Request.Cookies.TryGetValue(AuthOptions.RefreshTokenCookieName, out var refreshToken))
        {
            return Unauthorized(new BaseStatusResponse
            {
                Completed = false,
                Message = "Refresh token not found in cookies."
            });
        }
        /*
        try
        {
            await RevokeAccessTokenAsync();
        }
        catch (Exception e)
        {
            return SharedResponses.FailedRequest($"Error while revoking access token. Exception: {e.Message}");
        }*/
        
        var refreshInfo = await _authService.TryRefreshUsersTokens(refreshToken);
        if (refreshInfo.User == null)
        {
            return Unauthorized(new BaseStatusResponse
            {
                Completed = false,
                Message = refreshInfo.errorMsg
            });
        }
        WriteRefreshTokenToCookies(refreshInfo.RefreshToken);
        return Ok(new RefreshResponse
        {
            AccessToken = refreshInfo.AccessToken
        });
    }
    
    /// <summary>
    /// Выход из аккаунта пользователя.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> LogoutUser()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == AuthOptions.ClaimTypeUserId);
        
        if (userIdClaim == null)
        {
            return Unauthorized(new BaseStatusResponse
            {
                Message = "Access token is invalid.",
                Completed = false
            });
        }
        var userId = new Guid(userIdClaim.Value);
        // await RevokeAccessTokenAsync();
        await _authService.RemoveRefreshTokenAsync(userId);
        
        return SharedResponses.SuccessRequest("User successfully logged out.");
    }
    
    /// <summary>
    /// Удалить аккаунт пользователя.
    /// </summary>
    [HttpPost("delete")]
    [Authorize]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteUser()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == AuthOptions.ClaimTypeUserId);
        RemoveRefreshTokenFromCookies();
        
        if (userIdClaim == null)
        {
            return SharedResponses.FailedRequest("Access token is invalid.");
        }
        var userId = new Guid(userIdClaim.Value);
        
        // await RevokeAccessTokenAsync();
        await _authService.RemoveRefreshTokenAsync(userId);
        await _userService.TryRemoveAsync(userId);
        
        return SharedResponses.SuccessRequest("User successfully deleted.");
    }
    
    /// <summary>
    /// Регистрация Пользователя.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest dto)
    {
        if (User.Identity!.IsAuthenticated)
        {
            return SharedResponses.FailedRequest("User is already authenticated.");
        }
        var foundUsers = await _userService.GetAsync(new DataQueryParams<User>
        {
            Expression = u => u.Email == dto.Email
        });
        if (foundUsers.Length > 0)
        {
            return SharedResponses.FailedRequest("User with that email is already registered.");
        }
        
        var calendarSettings = new CalendarSettings
        {
            Id = Guid.NewGuid(),
            ServerUrl = "",
            Login = "",
            Password = ""
        };
        await _calendarSettingsService.CreateAsync(calendarSettings);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            PasswordHash = PasswordHelper.HashPassword(dto.Password),
            CalendarSettingsId = calendarSettings.Id,
            TutorId = null,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Patronymic = dto.Patronymic
        };
        Guid? tutorId = null;
        if (dto.IsTutor)
        {
            var tutor = new Tutor
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Patronymic = dto.Patronymic
            };
            user.TutorId = tutor.Id;
            await _tutorService.CreateAsync(tutor);
            tutorId = tutor.Id;
        }
        
        try
        {
            var registerInfo = await _authService.RegisterUserOrThrowAsync(user);
            WriteRefreshTokenToCookies(registerInfo.RefreshToken);
            return Ok(new RegisterResponse
            {
                Message = "User successfully registered.",
                UserId = registerInfo.User.Id,
                Completed = true,
                AccessToken = registerInfo.AccessToken
            });
        }
        catch (Exception e)
        {
            await _calendarSettingsService.TryRemoveAsync(calendarSettings.Id);
            if (tutorId.HasValue)
            {
                await _tutorService.TryRemoveAsync(tutorId.Value);
            }
            return SharedResponses.FailedRequest($"Registration failed. Info: {e.Message}");
        }
    }
    
    private async Task RevokeAccessTokenAsync()
    {
        var jtiClaim = User.Claims.FirstOrDefault(c => c.Type == AuthOptions.ClaimTypeJti);
        var expTimeClaim = User.Claims.FirstOrDefault(c => c.Type == AuthOptions.ClaimTypeExpireTime);
        if (jtiClaim != null && expTimeClaim != null)
        {
            var jti = Guid.Parse(jtiClaim.Value);
            var expTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expTimeClaim.Value)).UtcDateTime;
            await _authService.RevokeAccessToken(jti, expTime);
        }
        RemoveRefreshTokenFromCookies();
    }
    
    private void WriteRefreshTokenToCookies(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append(AuthOptions.RefreshTokenCookieName, token, cookieOptions);
    }
    
    private void RemoveRefreshTokenFromCookies()
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddDays(-1)
        };
        Response.Cookies.Append(AuthOptions.RefreshTokenCookieName, "", cookieOptions);
    }
}