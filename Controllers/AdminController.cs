using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using TourManagement_MVC.Models;
using TourManagement_MVC.Models.ViewModels;
using TourManagement_MVC.Services;

namespace TourManagement_MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ITourService _tourService;

        public AdminController(ITourService tourService)
        {
            _tourService = tourService;
        }

        private bool IsAdminAuthenticated()
        {
            return HttpContext.Session.GetString("IsAdmin") == "True";
        }

        // GET: /Admin/Login
        public IActionResult Login()
        {
            if (IsAdminAuthenticated())
            {
                return RedirectToAction("Dashboard");
            }
            return View(new AdminLoginViewModel());
        }

        // POST: /Admin/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Hardcoded admin check as in original AdminLogin2.aspx.cs
            if (model.Email.Trim().Equals("admin@gmail.com", StringComparison.OrdinalIgnoreCase) && model.Password == "admin")
            {
                HttpContext.Session.SetString("IsAdmin", "True");
                HttpContext.Session.SetString("AdminName", "Administrator");
                TempData["SuccessMessage"] = "Admin login successful! Welcome to the Admin Control Panel.";
                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError(string.Empty, "Invalid Admin credentials. (Default: admin@gmail.com / admin)");
            return View(model);
        }

        // GET: /Admin/Dashboard
        public IActionResult Dashboard()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login");
            }

            ViewBag.TotalTours = _tourService.GetAllTours().Count;
            ViewBag.TotalBookings = _tourService.GetAllBookings().Count;
            ViewBag.TotalUsers = _tourService.GetAllUsers().Count;
            ViewBag.RecentBookings = _tourService.GetAllBookings().Take(5).ToList();

            return View();
        }

        // GET: /Admin/AddTour
        public IActionResult AddTour()
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");
            return View(new Tour());
        }

        // POST: /Admin/AddTour
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTour(Tour tour, IFormFile? imageFile)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                return View(tour);
            }

            _tourService.AddTour(tour, imageFile);
            TempData["SuccessMessage"] = $"Tour '{tour.TourName}' added successfully!";
            return RedirectToAction("ManageTours");
        }

        // GET: /Admin/EditTour/5
        public IActionResult EditTour(int id)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            var tour = _tourService.GetTourById(id);
            if (tour == null) return NotFound();

            return View(tour);
        }

        // POST: /Admin/EditTour/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTour(Tour tour, IFormFile? imageFile)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                return View(tour);
            }

            _tourService.UpdateTour(tour, imageFile);
            TempData["SuccessMessage"] = $"Tour '{tour.TourName}' updated successfully!";
            return RedirectToAction("ManageTours");
        }

        // POST: /Admin/DeleteTour/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTour(int id)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            _tourService.DeleteTour(id);
            TempData["SuccessMessage"] = "Tour deleted successfully.";
            return RedirectToAction("ManageTours");
        }

        // GET: /Admin/ManageTours
        public IActionResult ManageTours()
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");
            var tours = _tourService.GetAllTours();
            return View(tours);
        }

        // GET: /Admin/AllBookings
        public IActionResult AllBookings()
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");
            var bookings = _tourService.GetAllBookings();
            return View(bookings);
        }

        // GET: /Admin/ManageUsers
        public IActionResult ManageUsers()
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");
            var users = _tourService.GetAllUsers();
            return View(users);
        }

        // POST: /Admin/DeleteUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(string email)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");
            _tourService.DeleteUser(email);
            TempData["SuccessMessage"] = $"User '{email}' deleted successfully.";
            return RedirectToAction("ManageUsers");
        }

        // GET: /Admin/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsAdmin");
            HttpContext.Session.Remove("AdminName");
            TempData["SuccessMessage"] = "Admin logged out successfully.";
            return RedirectToAction("Login");
        }
    }
}
