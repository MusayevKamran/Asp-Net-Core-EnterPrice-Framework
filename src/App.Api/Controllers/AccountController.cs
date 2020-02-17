using System.Threading.Tasks;
using App.Application.Interfaces;
using App.Application.ViewModels;
using App.Infrastructure.CrossCutting.Identity.Interfaces;
using App.Infrastructure.CrossCutting.Identity.ViewModels.InputModels;
using App.Infrastructure.CrossCutting.Identity.ViewModels.OutputModels;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILoginManager _loginManager;
        private readonly IUserService _userService;

        public AccountController(ILoginManager loginManager, IUserService userService)
        {
            _loginManager = loginManager;
            _userService = userService;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUserInputModel registerUserInputModel)
        {
            var authResponse = await _loginManager.RegisterAsync(registerUserInputModel.Email, registerUserInputModel.Password);
            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedOutputModel()
                {
                    Errors = authResponse.Errors
                });
            }

            var userViewModel = new UserViewModel()
            {
                Email = registerUserInputModel.Email,
                LoginId = authResponse.LoginId
            };
            await _userService.InsertAsync(userViewModel);


            var response = new AuthSuccessOutputModel()
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken,
                ExpiresIn = authResponse.ExpiresIn
            };
            return Ok(new AuthOutputModel<AuthSuccessOutputModel>(response));

        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginInputModel request)
        {
            var authResponse = await _loginManager.LoginAsync(request.Email, request.Password);
            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedOutputModel()
                {
                    Errors = authResponse.Errors
                });
            }

            var response = new AuthSuccessOutputModel()
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken,
                ExpiresIn = authResponse.ExpiresIn
            };
            return Ok(new AuthOutputModel<AuthSuccessOutputModel>(response));

        }
        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenInputModel request)
        {
            var authResponse = await _loginManager.RefreshTokenAsync(request.Token, request.RefreshToken);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedOutputModel()
                {
                    Errors = authResponse.Errors
                });
            }

            var response = new AuthSuccessOutputModel()
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken,
                ExpiresIn = authResponse.ExpiresIn
            };
            return Ok(new AuthOutputModel<AuthSuccessOutputModel>(response));
        }
    }
}