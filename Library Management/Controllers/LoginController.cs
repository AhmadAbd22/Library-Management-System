using Library_Management.Models;
using Library_Management.Models.Context;
using Library_Management.Models.Repositories;
using Microsoft.AspNetCore.Mvc;
using Library_Management.Models.Dtos;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Library_Management.HelpingClasses;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Library_Management.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserRepo _userRepo;
        private readonly GeneralPurpose _generalPurpose;

        public LoginController(IUserRepo userRepo, GeneralPurpose generalPurpose)
        {
            _userRepo = userRepo;
            _generalPurpose = generalPurpose;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto login)
        {
            Models.User? user = await _userRepo.GetUserByLogin(login.Email);
            if (user == null)
            {
                TempData["Error"] = "Email not found. Please check your email or sign up.";
                return RedirectToAction("Login");
            }

            if (user.password != login.Password)
            {
                TempData["Error"] = "Incorrect password.";
                return RedirectToAction("Login");
            }

            await _generalPurpose.SetUserClaims(user);

            if (user.role == 1)
                return RedirectToAction("AdminHome", "Admin");
            else
                return RedirectToAction("UserHome", "UserHome");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
