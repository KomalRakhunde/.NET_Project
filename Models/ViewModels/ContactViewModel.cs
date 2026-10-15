using System.ComponentModel.DataAnnotations;

namespace TourManagement_MVC.Models.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Please enter your full name.")]
        [Display(Name = "Full Name")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "Phone Number (Optional)")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Please select or enter a subject.")]
        [Display(Name = "Subject")]
        public string Subject { get; set; } = "Tour Inquiry";

        [Required(ErrorMessage = "Please write your message or query.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 2000 characters.")]
        [Display(Name = "Your Message")]
        public string Message { get; set; } = string.Empty;
    }
}
