using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TourManagement_MVC.Models
{
    public class Booking
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("bookingId")]
        public int BookingId { get; set; }

        [Required]
        [Display(Name = "Tour Package")]
        [BsonElement("tourName")]
        public string TourName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Destination")]
        [BsonElement("place")]
        public string Place { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Traveler Email")]
        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Traveler Name")]
        [BsonElement("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Mobile Number")]
        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Booking Date")]
        [BsonElement("bookingDate")]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Display(Name = "Status")]
        [BsonElement("status")]
        public string Status { get; set; } = "Confirmed";
    }
}
