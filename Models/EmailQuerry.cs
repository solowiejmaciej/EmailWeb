using EmailWeb.Data;

namespace EmailWeb.Models
{
    public class EmailQuery
    {
        public string QueryPhrase { get; set; }
        public EmailStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}