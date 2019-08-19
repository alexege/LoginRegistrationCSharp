using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoginRegistration.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace LoginRegistration.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register(LogRegModel model)
        {
            User newUser = model.RegUser;
            //Input fields correct
            if(ModelState.IsValid){

                bool notUnique = dbContext.Users.Any(a => a.Email == newUser.Email);

                if(notUnique)
                {
                    ModelState.AddModelError("Email", "Email already exists, please try a new one");
                    return View("Index");
                }

                // Hash user's password
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                string hash = hasher.HashPassword(newUser, newUser.Password);
                newUser.Password = hash;

                //Add user to database
                dbContext.Users.Add(newUser);
                dbContext.SaveChanges();
                return RedirectToAction("Success");
                }
            //Form not valid, redirect to Index page to show errors.
            return View("Index");
        }

        [HttpGet("success")]
        public string Success()
        {
            return "Success";
        }

        [HttpPost("login")]
        public IActionResult Login(LogRegModel model)
        {
            LogUser user = model.LogUser;
            if(ModelState.IsValid){

                //check to see if user exists in db
                User check_user_exists = dbContext.Users.FirstOrDefault(u => u.Email == user.LogEmail);
                
                //If user not in db
                if(check_user_exists == null){
                    ModelState.AddModelError("LogUser.LogEmail", "Invalid Email address or Password");
                    return View("Index");
                }

                //Compare hashed password to confirmation password
                PasswordHasher<LogUser> checkHash = new PasswordHasher<LogUser>();
                var verified_user = checkHash.VerifyHashedPassword(user, check_user_exists.Password, user.LogPassword);

                //If user cannot be verified
                if(verified_user == 0){
                    ModelState.AddModelError("LogUser.LogEmail", "Invalid Email/Password");
                    return View("Index");
                }

                //Place logged in user's id in session.
                HttpContext.Session.SetInt32("id",check_user_exists.UserId);

                return RedirectToAction("Success");
            }
            return View("Index");
        }

        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            // Grab current user id from session
            int? UserId = HttpContext.Session.GetInt32("id");

            // Grab entire user based on session id
            var user = dbContext.Users.FirstOrDefault(u => u.UserId == UserId);

            if(UserId == null){
                return View("Index");
            } else {
                return View("Dashboard", user);
                // return View("Dashboard", UserId);
            }
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
