using System.Security.Claims;
using AutoMapper;
using EmailWeb.ApplicationUser;
using EmailWeb.Data;
using EmailWeb.Models;
using Flurl.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EmailWeb.Services
{
    public interface IEmailService
    {
        List<EmailDto> SearchEmails(EmailQuery query);

        Task CreateNewEmailAsync(NewEmail newEmail);
    }

    public class EmailService : IEmailService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;

        public EmailService(ApplicationDbContext dbContext, IMapper mapper, IUserContext userContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userContext = userContext;
        }

        private List<EmailDto> GetAllEmails()
        {
            var allEmails = _dbContext.Emails.Where(e => e.CreatedById == _userContext.GetCurrentUser().Id).ToList();
            var allEmailsDtos = _mapper.Map<List<EmailDto>>(allEmails);

            return allEmailsDtos;
        }

        public List<EmailDto> SearchEmails(EmailQuery query)
        {
            /*Since EmailStatus.All is not in DB we have to check if the status is All if it is then we search on allEmails
            TODO:
            SortBy:
            -Status
            -ASC,DESC
            SearchBy:
            -Date
             */
            if (query.Status == EmailStatus.All)
            {
                if (!query.QueryPhrase.IsNullOrEmpty())
                {
                    var result = _dbContext.Emails.Where(
                        e => e.Subject.Contains(query.QueryPhrase) || e.EmailTo.Contains(query.QueryPhrase)
                             ).ToList();
                    var EmailsDtos = _mapper.Map<List<EmailDto>>(result);
                    return EmailsDtos;
                }

                return GetAllEmails();
            }
            if (!query.QueryPhrase.IsNullOrEmpty())
            {
                var result = _dbContext.Emails.Where(
                e => (e.Subject.Contains(query.QueryPhrase) || e.EmailTo.Contains(query.QueryPhrase))
                    && e.EmailStatus == query.Status).ToList();
                var EmailsDtos = _mapper.Map<List<EmailDto>>(result);
                return EmailsDtos;
            }
            else
            {
                var result = _dbContext.Emails.Where(
                    e => e.EmailStatus == query.Status).ToList();
                var EmailsDtos = _mapper.Map<List<EmailDto>>(result);
                return EmailsDtos;
            }
        }

        public async Task CreateNewEmailAsync(NewEmail newEmail)
        {
            var accessor = new HttpContextAccessor();
            var email = _mapper.Map<Email>(newEmail);
            var currentUserEmail = accessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            email.EmailSenderName = currentUserEmail;
            email.EmailFrom = currentUserEmail;
            await _dbContext.Emails.AddAsync(email);
            await _dbContext.SaveChangesAsync();
        }
    }
}