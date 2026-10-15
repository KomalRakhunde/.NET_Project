using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using TourManagement_MVC.Models;
using TourManagement_MVC.Services;

namespace TourManagement_MVC.Controllers
{
    public class BookingController : Controller
    {
        private readonly ITourService _tourService;

        public BookingController(ITourService tourService)
        {
            _tourService = tourService;
        }

        // GET: /Booking/Create?tourId=1
        public IActionResult Create(int? tourId)
        {
            var booking = new Booking();

            if (tourId.HasValue)
            {
                var tour = _tourService.GetTourById(tourId.Value);
                if (tour != null)
                {
                    booking.TourName = tour.TourName;
                    booking.Place = tour.Place;
                    ViewBag.Tour = tour;
                }
            }

            // Pre-fill from session if user is logged in
            string? userEmail = HttpContext.Session.GetString("UserEmail");
            string? userName = HttpContext.Session.GetString("UserName");
            if (!string.IsNullOrEmpty(userEmail))
            {
                booking.Email = userEmail;
                booking.FirstName = userName ?? "";
            }

            return View(booking);
        }

        // POST: /Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return View(booking);
            }

            var created = _tourService.CreateBooking(booking);
            TempData["SuccessMessage"] = $"Congratulations! Your tour '{booking.TourName}' has been successfully booked.";
            return RedirectToAction("Confirmation", new { id = created.BookingId });
        }

        // GET: /Booking/Confirmation/5
        public IActionResult Confirmation(int id)
        {
            var all = _tourService.GetAllBookings();
            var booking = all.FirstOrDefault(b => b.BookingId == id);
            if (booking == null)
            {
                return RedirectToAction("MyBookings");
            }
            return View(booking);
        }

        // GET: /Booking/MyBookings
        public IActionResult MyBookings()
        {
            string? userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["ErrorMessage"] = "Please log in to view your bookings.";
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("MyBookings", "Booking") });
            }

            var bookings = _tourService.GetBookingsByUser(userEmail);
            return View(bookings);
        }

        // POST: /Booking/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int id)
        {
            _tourService.CancelBooking(id);
            TempData["SuccessMessage"] = "Your booking was successfully cancelled.";
            return RedirectToAction("MyBookings");
        }
    }
}
