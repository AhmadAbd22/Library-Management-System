using Library_Management.Models.Context;
using Library_Management.Models;
using Library_Management.Models.Repositories;
using Microsoft.AspNetCore.Mvc;
using Library_Management.Models.Dtos;
using Library_Management.HelpingClasses;

namespace Library_Management.Controllers
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
        public async Task<IActionResult> Signup(SignUpDto signup)
        {
            if (!ModelState.IsValid)
                return View(signup);

            var existingUser = await _userRepo.GetUserByEmail(signup.Email);  // check if user exists
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email is already registered.");
                return View(signup);
            }
            var user = new Models.User  //create new user
            {
                Id = Guid.NewGuid(),
                email = signup.Email,
                firstName = signup.FirstName,
                lastName = signup.LastName,
                address = signup.Address,
                city = signup.City,
                phoneNumber = signup.Phone,
                password = signup.Password,
                isActive = (int)enumActiveStatus.Active,
                role = (int)enumRole.Customer,
                createdAt = DateTime.UtcNow
            };

            await _userRepo.AddUser(user);
            return RedirectToAction("Login","Login");
        }
    }
}
