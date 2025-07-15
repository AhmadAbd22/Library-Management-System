using Library_Management.Models;
using Library_Management.Models.Context;
using Library_Management.Models.Repositories;
using Microsoft.AspNetCore.Mvc;
using Library_Management.Models.Dtos;

namespace Library_Management.Controllers.Auth
{
    public class LoginController : Controller
    {
        private readonly IUserRepo _userRepo;

        public LoginController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }
            var user = await _userRepo.GetUserByEmail(login.email);
            if (user == null || user.isActive != (int)enumActiveStatus.Active)
            {
                TempData["Error"] = "Incorrect Email or Password.";
                return View(login);
            }

            //compare password and email
            if (user.password == StringCipher.EncryptString(login.password))
            {
                return RedirectToAction("Index", "Home");
            }
            TempData["Error"] = "Incorrect Email or Password.";
            return View(login);
        }
    }
}   
