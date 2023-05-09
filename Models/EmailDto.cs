using EmailWeb.Data;

namespace EmailWeb.Models
{
    public class EmailDto
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string EmailTo { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedById { get; set; }
        public EmailStatus EmailStatus { get; set; }
    }
}