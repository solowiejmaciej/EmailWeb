using System.ComponentModel.DataAnnotations;

namespace EmailWeb.Models
{
    public class NewEmail
    {
        [Required]
        public string Subject { get; set; }

        [EmailAddress]
        public string EmailTo { get; set; }

        [Required]
        public string Body { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}