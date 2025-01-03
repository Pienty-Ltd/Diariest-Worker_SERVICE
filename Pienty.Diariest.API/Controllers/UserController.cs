using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pienty.CRM.Core.Helpers;
using Pienty.Diariest.Core.Models.API;
using Pienty.Diariest.Core.Models.Database;
using Pienty.Diariest.Core.Services;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.API.Controllers
{
    [ApiController]
    [Route("api/v1/user")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly APIMessageService _apiMessageService;

        public UserController(ILogger<UserController> logger, IUserService userService, APIMessageService apiMessageService)
        {
            _logger = logger;
            _userService = userService;
            _apiMessageService = apiMessageService;
        }
        
        [HttpGet("GetUser")]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<bool>()
                {
                    Data = true,
                    Success = true
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<APIResponse.LogoutResponse>()
                {
                    Message = _apiMessageService.GetMessage(APIMessage.Error),
                    Error = new APIResponse.ErrorResponse()
                    {
                        Message = ex.Message
                    }
                }));
            }
        }
        
        [HttpPost("CreateUser")]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<APIResponse.CreateUserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<APIResponse.CreateUserResponse>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser(APIRequest.LoginRequest model)
        {
            try
            {
                var newUser = new User()
                {
                    Name = "Tuna",
                    Email = "tuna@pienty.com",
                    Password = CryptoHelper.EncryptPassword("123456"),
                    PhoneNumber = "5305757860",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Deleted = false,
                    Permission = UserPermission.Admin,
                    Language = Language.Turkish
                };
                _userService.AddUser(newUser);
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<bool>()
                {
                    Data = true,
                    Success = true
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<APIResponse.LogoutResponse>()
                {
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