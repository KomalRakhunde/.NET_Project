using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TourManagement_MVC.Models
{
    public class Tour
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("tourId")]
        public int TourId { get; set; }

        [Required(ErrorMessage = "Tour name is required")]
        [Display(Name = "Tour Name")]
        [StringLength(100)]
        [BsonElement("tourName")]
        public string TourName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Destination place is required")]
        [StringLength(50)]
        [BsonElement("place")]
        public string Place { get; set; } = string.Empty;

        [Required(ErrorMessage = "Duration in days is required")]
        [Range(1, 90, ErrorMessage = "Days must be between 1 and 90")]
        [BsonElement("days")]
        public int Days { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(100, 1000000, ErrorMessage = "Price must be at least ₹100")]
        [DataType(DataType.Currency)]
        [BsonElement("price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Locations covered are required")]
        [Display(Name = "Covered Locations")]
        [BsonElement("locations")]
        public string Locations { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tour description is required")]
        [Display(Name = "Tour Information")]
        [BsonElement("tourInfo")]
        public string TourInfo { get; set; } = string.Empty;

        [Display(Name = "Image File Name")]
        [BsonElement("pic")]
        public string Pic { get; set; } = "TravelImage.jpg";
    }
}
