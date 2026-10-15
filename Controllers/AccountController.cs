using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using TourManagement_MVC.Models;
using TourManagement_MVC.Models.ViewModels;
using TourManagement_MVC.Services;

namespace TourManagement_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ITourService _tourService;

        public AccountController(ITourService tourService)
        {
            _tourService = tourService;
        }

        // GET: /Account/Login
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _tourService.AuthenticateUser(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password. (Demo: user@gmail.com / user123)");
                return View(model);
            }

            // Store in Session
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.FirstName);
            HttpContext.Session.SetString("UserFullName", user.FullName);

            TempData["SuccessMessage"] = $"Welcome back, {user.FirstName}!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Profile");
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View(new User());
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            bool success = _tourService.RegisterUser(user);
            if (!success)
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                return View(user);
            }

            // Auto-login upon registration
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.FirstName);
            HttpContext.Session.SetString("UserFullName", user.FullName);

            TempData["SuccessMessage"] = "Registration successful! Welcome to Tour Management.";
            return RedirectToAction("Index", "Tour");
        }

        // GET: /Account/Profile
        public IActionResult Profile()
        {
            string? email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            var user = _tourService.GetUserByEmail(email);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.UserBookings = _tourService.GetBookingsByUser(email);
            return View(user);
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserEmail");
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("UserFullName");
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }
    }
}
