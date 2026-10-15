using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TourManagement_MVC.Models;

namespace TourManagement_MVC.Services
{
    public class TourService : ITourService
    {
        private readonly IMongoCollection<Tour> _tours;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Booking> _bookings;
        private readonly IWebHostEnvironment _env;

        public TourService(IConfiguration config, IWebHostEnvironment env)
        {
            _env = env;

            // Connect to MongoDB
            var client = new MongoClient(config["MongoDB:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]);

            _tours    = database.GetCollection<Tour>("tours");
            _users    = database.GetCollection<User>("users");
            _bookings = database.GetCollection<Booking>("bookings");

            SeedData();
        }

        // ── Seed default data only if collections are empty ──
        private void SeedData()
        {
            if (!_tours.Find(_ => true).Any())
            {
                _tours.InsertMany(new List<Tour>
                {
                    new Tour { TourName = "Goa Beach & Sun Paradise",       Place = "Goa",                Days = 4, Price = 12500, Locations = "Baga Beach, Calangute, Aguada Fort, Panaji Cruise",          TourInfo = "Experience pristine sandy beaches, thrilling water sports, and sunset cruises across picturesque Goa.",                                Pic = "goa.jpg" },
                    new Tour { TourName = "Kashmir Valley Enchantment",     Place = "Srinagar & Gulmarg", Days = 6, Price = 28900, Locations = "Dal Lake, Shikara Ride, Gulmarg Gondola, Pahalgam Valley",    TourInfo = "Immerse yourself in snow-capped Himalayan peaks, emerald valleys, and serene floating houseboats.",                                    Pic = "kashmir.jpg" },
                    new Tour { TourName = "Kerala Backwaters & Hills",      Place = "Kerala",             Days = 5, Price = 21000, Locations = "Munnar Tea Gardens, Alleppey Houseboat, Thekkady Sanctuary",  TourInfo = "Relax amidst lush green tea plantations, tranquil lagoons, and world-renowned Ayurvedic retreats.",                                  Pic = "kerala.jpeg" },
                    new Tour { TourName = "Himalayan Mountain Odyssey",     Place = "Manali & Solang",    Days = 5, Price = 18500, Locations = "Hadimba Temple, Solang Valley, Atal Tunnel, Rohtang Pass",   TourInfo = "Adventure packed with paragliding, snow skiing, and pine forests in the lap of Himachal Pradesh.",                                   Pic = "Landscape.jpg" },
                    new Tour { TourName = "Golden Triangle Explorer",       Place = "Delhi, Agra, Jaipur",Days = 6, Price = 24500, Locations = "Taj Mahal, Red Fort, Amber Fort, Hawa Mahal, City Palace",   TourInfo = "Discover India's most celebrated heritage corridor with breathtaking Mughal and Rajput architecture.",                                 Pic = "TravelImage.jpg" }
                });
            }

            if (!_users.Find(_ => true).Any())
            {
                _users.InsertOne(new User
                {
                    Email = "user@gmail.com",
                    FirstName = "Rahul",
                    LastName = "Sharma",
                    Gender = "Male",
                    Password = "user123",
                    Phone = "9876543210",
                    DateOfBirth = new DateTime(1998, 5, 14),
                    Street = "12 Main Market Road",
                    City = "Ahmedabad",
                    State = "Gujarat"
                });
            }
        }

        // ──────────────── TOUR METHODS ────────────────

        public List<Tour> GetAllTours()
            => _tours.Find(_ => true).SortBy(t => t.TourId).ToList();

        public Tour? GetTourById(int id)
            => _tours.Find(t => t.TourId == id).FirstOrDefault();

        public void AddTour(Tour tour, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString("N")[..8] + Path.GetExtension(imageFile.FileName);
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "tours");
                Directory.CreateDirectory(uploadsFolder);
                using var stream = new FileStream(Path.Combine(uploadsFolder, fileName), FileMode.Create);
                imageFile.CopyTo(stream);
                tour.Pic = fileName;
            }
            else if (string.IsNullOrWhiteSpace(tour.Pic))
                tour.Pic = "TravelImage.jpg";

            // Auto-increment TourId
            var maxId = _tours.Find(_ => true).SortByDescending(t => t.TourId).FirstOrDefault()?.TourId ?? 0;
            tour.TourId = maxId + 1;
            _tours.InsertOne(tour);
        }

        public void UpdateTour(Tour tour, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString("N")[..8] + Path.GetExtension(imageFile.FileName);
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "tours");
                Directory.CreateDirectory(uploadsFolder);
                using var stream = new FileStream(Path.Combine(uploadsFolder, fileName), FileMode.Create);
                imageFile.CopyTo(stream);
                tour.Pic = fileName;
            }

            var update = Builders<Tour>.Update
                .Set(t => t.TourName,  tour.TourName)
                .Set(t => t.Place,     tour.Place)
                .Set(t => t.Days,      tour.Days)
                .Set(t => t.Price,     tour.Price)
                .Set(t => t.Locations, tour.Locations)
                .Set(t => t.TourInfo,  tour.TourInfo)
                .Set(t => t.Pic,       tour.Pic);

            _tours.UpdateOne(t => t.TourId == tour.TourId, update);
        }

        public void DeleteTour(int id)
            => _tours.DeleteOne(t => t.TourId == id);

        // ──────────────── USER METHODS ────────────────

        public User? AuthenticateUser(string email, string password)
            => _users.Find(u => u.Email.ToLower() == email.ToLower() && u.Password == password).FirstOrDefault();

        public bool RegisterUser(User user)
        {
            bool exists = _users.Find(u => u.Email.ToLower() == user.Email.ToLower()).Any();
            if (exists) return false;
            _users.InsertOne(user);
            return true;
        }

        public User? GetUserByEmail(string email)
            => _users.Find(u => u.Email.ToLower() == email.ToLower()).FirstOrDefault();

        public List<User> GetAllUsers()
            => _users.Find(_ => true).ToList();

        public void DeleteUser(string email)
            => _users.DeleteOne(u => u.Email.ToLower() == email.ToLower());

        // ──────────────── BOOKING METHODS ────────────────

        public Booking CreateBooking(Booking booking)
        {
            var maxId = _bookings.Find(_ => true).SortByDescending(b => b.BookingId).FirstOrDefault()?.BookingId ?? 0;
            booking.BookingId   = maxId + 1;
            booking.BookingDate = DateTime.Now;
            booking.Status      = "Confirmed";
            _bookings.InsertOne(booking);
            return booking;
        }

        public List<Booking> GetBookingsByUser(string email)
            => _bookings.Find(b => b.Email.ToLower() == email.ToLower())
                        .SortByDescending(b => b.BookingDate)
                        .ToList();

        public List<Booking> GetAllBookings()
            => _bookings.Find(_ => true).SortByDescending(b => b.BookingDate).ToList();

        public void CancelBooking(int bookingId)
        {
            var update = Builders<Booking>.Update.Set(b => b.Status, "Cancelled");
            _bookings.UpdateOne(b => b.BookingId == bookingId, update);
        }
    }
}
