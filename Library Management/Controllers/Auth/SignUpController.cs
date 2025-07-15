using Library_Management.Models.Context;
using Library_Management.Models;
using Library_Management.Models.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management.Controllers.Auth
{
    public class SignUpController : Controller
    {
        private readonly IUserRepo _userRepo;
        public SignUpController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        //Sign Up//
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Signup(User signup)
        {
            if (!ModelState.IsValid)
                return View(signup);
            var existingUser = await _userRepo.GetUserByEmail(signup.email);  // check if user exists
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email is already registered.");
                return View(signup);
            }

            var user = new User  //create ew user
            {
                Id = Guid.NewGuid(),
                email = signup.email,
                firstName = signup.firstName!,
                lastName = signup.lastName!,
                password = StringCipher.EncryptString(signup.password),
                isActive = (int)enumActiveStatus.Active,
                role = (int)enumRole.Customer,
                createdAt = DateTime.UtcNow
            };

            await _userRepo.AddUser(user);
            return RedirectToAction("Login");
        }
    }
}
