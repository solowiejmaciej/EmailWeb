﻿namespace EmailWeb.Data
{
    public enum EmailStatus
    {
        New,
        EmailSended,
        ToBeDeleted,
        HasErrors,
        All
    }

    public class Email
    {
        public int Id { get; set; }
        public string? EmailSenderName { get; set; }
        public string Subject { get; set; }
        public string EmailTo { get; set; }
        public string EmailFrom { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedById { get; set; }
        public EmailStatus EmailStatus { get; set; }
    }
}