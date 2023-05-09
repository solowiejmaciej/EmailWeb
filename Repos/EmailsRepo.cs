using EmailWeb.ApplicationUser;
using EmailWeb.Data;
using EmailWeb.Data.Migrations;
using Microsoft.IdentityModel.Tokens;

namespace EmailWeb.Repos
{
    public interface IEmailsRepo
    {
        Task AddEmailAsync(Email email);

        List<Email> GetAllEmails();

        List<Email> GetAllEmailsByCreatedAt(List<Email> emails, DateTime date);

        List<Email> GetAllEmailsByCurrentUser();

        List<Email> GetAllEmailsByEmailTo(List<Email> emails, string emailTo);

        List<Email> GetAllEmailsByStatus(List<Email> emails, EmailStatus emailStatus);

        List<Email> GetAllEmailsBySubject(List<Email> emails, string subject);

        List<Email> GetEmailEmailToOrSubject(List<Email> emails, string query);

        Task DeleteSofAsync(List<int> ids);
    }

    public class EmailsRepo : IEmailsRepo
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserContext _userContext;

        public EmailsRepo(ApplicationDbContext dbContext, IUserContext userContext)
        {
            _dbContext = dbContext;
            _userContext = userContext;
        }

        private void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public List<Email> GetAllEmailsByCurrentUser()
        {
            var currentUser = _userContext.GetCurrentUser();
            return _dbContext.Emails.Where(e => e.CreatedById.Equals(currentUser.Id)).ToList();
        }

        public List<Email> GetAllEmails()
        {
            return _dbContext.Emails.ToList();
        }

        public List<Email> GetAllEmailsByCreatedAt(List<Email> emails, DateTime date)
        {
            return emails.Where(e => e.CreatedAt >= date).ToList();
        }

        public List<Email> GetAllEmailsByStatus(List<Email> emails, EmailStatus emailStatus)
        {
            return emails.Where(e => e.EmailStatus.Equals(emailStatus)).ToList();
        }

        public List<Email> GetAllEmailsByEmailTo(List<Email> emails, string emailTo)
        {
            return emails.Where(e => e.EmailTo.Equals(emailTo)).ToList();
        }

        public List<Email> GetEmailEmailToOrSubject(List<Email> emails, string query)
        {
            if (query.IsNullOrEmpty()) return emails;
            return emails.Where(e => e.EmailTo.Contains(query) || e.EmailTo.Contains(query)).ToList();
        }

        public List<Email> GetAllEmailsBySubject(List<Email> emails, string subject)
        {
            return emails.Where(e => e.Subject.Contains(subject)).ToList();
        }

        public async Task AddEmailAsync(Email email)
        {
            await _dbContext.AddAsync(email);
            SaveChanges();
        }

        public async Task DeleteSofAsync(List<int> ids)
        {
            foreach (int id in ids)
            {
                var emailToDelete = GetAllEmailsByCurrentUser().FirstOrDefault(e => e.Id == id);
                if (emailToDelete != null)
                {
                    emailToDelete.EmailStatus = EmailStatus.ToBeDeleted;
                    SaveChanges();
                }
            }
        }
    }
}