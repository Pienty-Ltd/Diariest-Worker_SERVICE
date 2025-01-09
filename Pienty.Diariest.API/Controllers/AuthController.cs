using Microsoft.AspNetCore.Mvc;
using Pienty.Diariest.API.Authentication;
using Pienty.Diariest.Core.Helpers;
using Pienty.Diariest.Core.Models.API;
using Pienty.Diariest.Core.Models.Database;
using Pienty.Diariest.Core.Models.Database.Redis;
using Pienty.Diariest.Core.Services;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserService _userService;
        private readonly IRedisService _redisService;
        private readonly APIMessageService _apiMessageService;
        private readonly ILoginHistoryService _loginHistoryService;

        public AuthController(
            ILogger<AuthController> logger, 
            IUserService userService, 
            IRedisService redisService, 
            APIMessageService apiMessageService,
            ILoginHistoryService loginHistoryService
            )
        {
            _logger = logger;
            _userService = userService;
            _redisService = redisService;
            _apiMessageService = apiMessageService;
            _loginHistoryService = loginHistoryService;
        }

        [UserAuth(UserPermission.Admin, UserPermission.Agency, UserPermission.Client)]
        [HttpPost("Ping")]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Ping()
        {
            try
            {
                var authToken = HttpContext.Items["AuthToken"] as string;
                if (authToken == null)
                {
                    return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<bool>()
                    {
                        Message = null,
                        Error = new APIResponse.ErrorResponse()
                        {
                            Logout = true
                        }
                    }));
                }

                string cachedPingKey = RedisHelper.GetKey_Ping(authToken);
                var cachedPing = _redisService.Get<int>(cachedPingKey);
                if (cachedPing == 1)
                {
                    return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<bool>()
                    {
                        Error = new APIResponse.ErrorResponse()
                        {
                            Logout = true
                        }
                    }));
                }
                
                var cachedAuthToken = _redisService.Get<AuthenticationToken>(RedisHelper.GetKey_AuthToken(authToken));
                if (cachedAuthToken == null)
                {
                    return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<bool>()
                    {
                        Error = new APIResponse.ErrorResponse()
                        {
                            Logout = true
                        }
                    }));
                }
                
                _redisService.Set(cachedPingKey, 1, TimeSpan.FromMinutes(1));
                _redisService.Set(RedisHelper.GetKey_AuthToken(authToken), cachedAuthToken, TimeSpan.FromHours(1));
                
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<bool>()
                {
                    Data = true,
                    Success = true
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<string>()
                {
                    Data = null,
                    Message = _apiMessageService.GetMessage(APIMessage.Error),
                    Error = new APIResponse.ErrorResponse()
                    {
                        Logout = true,
                        Message = ex.Message
                    }
                }));
            }
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<APIResponse.LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<APIResponse.LoginResponse>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(APIRequest.LoginRequest model)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                if (ipAddress == null) ipAddress = "127.0.0.1";
                
                if (!StringHelper.IsValidEmail(model.Email))
                {
                    return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<APIResponse.LoginResponse>()
                    {
                        Success = false,
                        Message = _apiMessageService.GetMessage(APIMessage.WrongEmail)
                    }));
                }

                var user = _userService.GetUserWithEmail(model.Email);
                if (user == null)
                {
                    return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<APIResponse.LoginResponse>()
                    {
                        Success = false,
                        Message = _apiMessageService.GetMessage(APIMessage.WrongEmail)
                    }));
                }
                if (!CryptoHelper.VerifyPassword(model.Password, user.password))
                {
                    return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<APIResponse.LoginResponse>()
                    {
                        Success = false,
                        Message = _apiMessageService.GetMessage(APIMessage.WrongPassword)
                    }));
                }

                if (!user.active)
                {
                    return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<APIResponse.LoginResponse>()
                    {
                        Success = false,
                        Message = _apiMessageService.GetMessage(APIMessage.UserDisabled)
                    }));
                }

                if (user.deleted)
                {
                    return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<APIResponse.LoginResponse>()
                    {
                        Success = false,
                        Message = _apiMessageService.GetMessage(APIMessage.UserDeleted)
                    }));
                }

                var authToken = CryptoHelper.GenerateSecureToken();
                var tokenObj = new AuthenticationToken()
                {
                    UserId = user.id,
                    AccessToken = authToken,
                    Expiration = DateTime.Now.AddHours(1),
                    Permission = user.permission
                };
                
                var ipTokenList = _redisService.Get<List<string>>(RedisHelper.GetKey_Limit(ipAddress));
                if (ipTokenList == null) ipTokenList = new List<string>();
                if (ipTokenList.Count > 5)
                {
                    return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<APIResponse.LoginResponse>()
                    {
                        Success = false,
                        Message = _apiMessageService.GetMessage(APIMessage.MaxLoginLimit)
                    }));
                }
                if (!ipTokenList.Contains(authToken))
                {
                    ipTokenList.Add(authToken);
                }
                _redisService.Set(RedisHelper.GetKey_Limit(ipAddress), ipTokenList, TimeSpan.FromMinutes(15));
                
                _redisService.Set(RedisHelper.GetKey_User(user.id), user, TimeSpan.FromHours(1));
                _redisService.Set(RedisHelper.GetKey_AuthToken(authToken), tokenObj, TimeSpan.FromHours(1));
                
                //Login History
                var loginHistory = new UserLoginHistory()
                {
                    user_id = user.id,
                    ip_address = ipAddress,
                    success = true,
                    login_date = DateTime.Now
                };
                await Task.Run(() => _loginHistoryService.AddLoginHistory(loginHistory));

                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<APIResponse.LoginResponse>()
                {
                    Success = true,
                    Message = _apiMessageService.GetMessage(APIMessage.SuccessLogin),
                    Data = new APIResponse.LoginResponse()
                    {
                        Authentication = tokenObj
                    }
                }));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<APIResponse.LoginResponse>()
                {
                    Success = false,
                    Message = _apiMessageService.GetMessage(APIMessage.Error),
                    Error = new APIResponse.ErrorResponse()
                    {
                        Message = ex.Message
                    }
                }));
            }
        }

        [UserAuth(new UserPermission[]
        {
            UserPermission.Agency, UserPermission.Client, UserPermission.Admin
        })]
        [HttpPost("Logout")]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var authToken = HttpContext.Items["AuthToken"] as string;
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                
                var ipTokenList = _redisService.Get<List<string>>(RedisHelper.GetKey_Limit(ipAddress));
                if (ipTokenList.Contains(authToken))
                {
                    ipTokenList.Remove(authToken);
                }

                if (ipTokenList.Count == 0)
                {
                    _redisService.Remove(RedisHelper.GetKey_Limit(ipAddress));
                }
                else
                {
                    _redisService.Set(RedisHelper.GetKey_Limit(ipAddress), ipTokenList);
                }
                _redisService.Remove(RedisHelper.GetKey_AuthToken(authToken));
                
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<bool>()
                {
                    Success = true,
                    Data = true,
                    Message = _apiMessageService.GetMessage(APIMessage.SuccessLogout)
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<bool>()
                {
                    Success = false,
                    Data = false,
                    Message = _apiMessageService.GetMessage(APIMessage.Error),
                    Error = new APIResponse.ErrorResponse()
                    {
                        Message = ex.Message
                    }
                }));
            }
        }

    }
}