using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using TourManagement_MVC.Models;

namespace TourManagement_MVC.Services
{
    public interface ITourService
    {
        // Tour operations
        List<Tour> GetAllTours();
        Tour? GetTourById(int id);
        void AddTour(Tour tour, IFormFile? imageFile);
        void UpdateTour(Tour tour, IFormFile? imageFile);
        void DeleteTour(int id);

        // User operations
        User? AuthenticateUser(string email, string password);
        bool RegisterUser(User user);
        User? GetUserByEmail(string email);
        List<User> GetAllUsers();
        void DeleteUser(string email);

        // Booking operations
        Booking CreateBooking(Booking booking);
        List<Booking> GetBookingsByUser(string email);
        List<Booking> GetAllBookings();
        void CancelBooking(int bookingId);
    }
}
