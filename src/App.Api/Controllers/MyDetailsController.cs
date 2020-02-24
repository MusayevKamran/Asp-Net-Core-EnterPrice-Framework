using System.Threading.Tasks;
using App.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers
{
    public class MyDetailsController : ApiController
    {
        private readonly IUserService _userService;

        public MyDetailsController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: MyDetails
        [HttpPost("Index")]
        public async Task<IActionResult> Index()
        {

            var userViewModel = await _userService.GetCurrentUserAsync();

            return Ok(userViewModel);
        }

    }
}