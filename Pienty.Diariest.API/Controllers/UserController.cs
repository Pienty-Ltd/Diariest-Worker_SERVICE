using Microsoft.AspNetCore.Mvc;
using Pienty.CRM.Core.Helpers;
using Pienty.Diariest.API.Authentication;
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
        
        [TestController]
        [HttpGet("GetUser")]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<User>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUser(APIRequest.GetUserRequest model)
        {
            try
            {
                var user = _userService.GetUserWithId(model.Id);
                if (user == null)
                {
                    return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<User>()
                    {
                        Message = "Kullanıcı bulunamadı.",
                        Success = false
                    }));
                }
                
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<User>()
                {
                    Data = user,
                    Success = true
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<User>()
                {
                    Message = _apiMessageService.GetMessage(APIMessage.Error),
                    Error = new APIResponse.ErrorResponse()
                    {
                        Message = ex.Message
                    }
                }));
            }
        }
        
        [TestController]
        [HttpPost("CreateUser")]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<APIResponse.CreateUserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse.BaseResponse<APIResponse.CreateUserResponse>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser(APIRequest.CreateUserRequest model)
        {
            try
            {
                var userIsExist = _userService.IsUserExistWithEmail(model.Email);
                if (userIsExist)
                {
                    return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<string>()
                    {
                        Data = null,
                        Message = "Bu e-mail zaten kullanılıyor.",
                        Success = false
                    }));
                }
                
                var newUser = new User()
                {
                    name = model.Name,
                    email = model.Email,
                    password = CryptoHelper.EncryptPassword(model.Password),
                    phone_number = model.PhoneNumber,
                    created_date = DateTime.Now,
                    updated_date = DateTime.Now,
                    deleted = false,
                    permission = UserPermission.Admin,
                    language = Language.Turkish
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
                return await Task.FromResult<IActionResult>(Ok(new APIResponse.BaseResponse<string>()
                {
                    Data = null,
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