using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoginRegistration.Models;

using Microsoft.AspNetCore.Hosting;
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

        public IActionResult Register(User newUser)
        {
            //Input fields correct
            if(ModelState.IsValid){

                //If User already exists, redirect back to index.
                if(dbContext.Users.Any(user => user.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use");
                    return View("Index");
                } else {
                    PasswordHasher<User> hasher = new PasswordHasher<User>();
                    string hashed_password = hasher.HashPassword(newUser, newUser.Password);
                    newUser.Password = hashed_password;

                    dbContext.Users.Add(newUser);
                    dbContext.SaveChanges();

                    //Place user's id in session
                    HttpContext.Session.SetInt32("id", dbContext.Users.Last().Id);

                    return RedirectToAction("Dashboard");
                }
            } else {

                //Form not valid, redirect to Index page to show errors.
                return View("Index");
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LogUser entered_user)
        {
            if(ModelState.IsValid){
                //check to see if user exists in db
                var user_exists = dbContext.Users.FirstOrDefault(user => user.Email == entered_user.Email);
                
                //If user not in db
                if(user_exists == null){
                    ModelState.AddModelError("Email", "Invalid Email address or Password");
                    return View("Login");
                }

                //Compare hashed password to confirmation password
                var checkHash = new PasswordHasher<LogUser>();
                var verified_user = checkHash.VerifyHashedPassword(entered_user, user_exists.Password, entered_user.Password);

                //If user cannot be verified
                if(verified_user == 0){
                    return View("Login");
                }

                //Place logged in user's id in session.
                HttpContext.Session.SetInt32("id", dbContext.Users.Last().Id);
                return RedirectToAction("Dashboard");
            } else {
                return View("Login");
            }
        }

        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            // Grab current user id from session
            int? UserId = HttpContext.Session.GetInt32("id");

            if(UserId == null){
                return View("Login");
            } else {
                return View("Dashboard", UserId);
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
