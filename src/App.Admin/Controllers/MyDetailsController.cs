using System.Threading.Tasks;
using App.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Controllers
{
    public class MyDetailsController : Controller
    {
        private readonly IUserService _userService;

        public MyDetailsController(IUserService userService)
        {
            _userService = userService;
        }
        // GET: MyDetails
        public async Task<ActionResult> Index()
        {

            var userViewModel = await _userService.GetCurrentUserAsync();

            return View(userViewModel);
        }


        // GET: MyDetails/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MyDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

    }
}