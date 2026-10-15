using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using TourManagement_MVC.Services;

namespace TourManagement_MVC.Controllers
{
    public class TourController : Controller
    {
        private readonly ITourService _tourService;

        public TourController(ITourService tourService)
        {
            _tourService = tourService;
        }

        // GET: /Tour/Index
        public IActionResult Index(string? search, string? place, decimal? maxPrice)
        {
            var tours = _tourService.GetAllTours();

            if (!string.IsNullOrWhiteSpace(search))
            {
                tours = tours.Where(t => 
                    t.TourName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    t.Place.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    t.Locations.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(place))
            {
                tours = tours.Where(t => t.Place.Contains(place, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (maxPrice.HasValue && maxPrice.Value > 0)
            {
                tours = tours.Where(t => t.Price <= maxPrice.Value).ToList();
            }

            ViewBag.SearchTerm = search;
            ViewBag.SelectedPlace = place;
            ViewBag.MaxPrice = maxPrice;

            return View(tours);
        }

        // GET: /Tour/Details/5
        public IActionResult Details(int id)
        {
            var tour = _tourService.GetTourById(id);
            if (tour == null)
            {
                return NotFound();
            }
            return View(tour);
        }
    }
}
