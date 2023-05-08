using System.Security.Claims;
using AutoMapper;
using EmailWeb.ApplicationUser;
using EmailWeb.Data;
using EmailWeb.Models;
using EmailWeb.Repos;
using Flurl.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EmailWeb.Services
{
    public interface IEmailService
    {
        List<EmailDto> SearchEmails(EmailQuery query);
        Task CreateNewEmailAsync(NewEmail newEmail);
        Task DeleteEmailAsync(List<int> id);
    }

    public class EmailService : IEmailService
    {
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;
        private readonly IEmailsRepo _emailsRepo;

        public EmailService(
            IMapper mapper, 
            IUserContext userContext, 
            IEmailsRepo emailsRepo
            )
        {
            _mapper = mapper;
            _userContext = userContext;
            _emailsRepo = emailsRepo;
        }

        private List<EmailDto> GetAllEmails()
        {
            var allEmails = _emailsRepo.GetAllEmailsByCurrentUser().ToList();
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
                    var result = GetAllEmails().Where(
                        e => e.Subject.Contains(query.QueryPhrase) || e.EmailTo.Contains(query.QueryPhrase)
                             ).ToList();
                    var EmailsDtos = _mapper.Map<List<EmailDto>>(result);
                    return EmailsDtos;
                }

                return GetAllEmails();
            }
            if (!query.QueryPhrase.IsNullOrEmpty())
            {
                var result = GetAllEmails().Where(
                e => (e.Subject.Contains(query.QueryPhrase) || e.EmailTo.Contains(query.QueryPhrase))
                    && e.EmailStatus == query.Status).ToList();
                var EmailsDtos = _mapper.Map<List<EmailDto>>(result);
                return EmailsDtos;
            }
            else
            {
                var result = GetAllEmails().Where(
                    e => e.EmailStatus == query.Status).ToList();
                var EmailsDtos = _mapper.Map<List<EmailDto>>(result);
                return EmailsDtos;
            }
        }

        public async Task CreateNewEmailAsync(NewEmail newEmail)
        {
            var email = _mapper.Map<Email>(newEmail);
            
            var currentUser = _userContext.GetCurrentUser();

            email.EmailSenderName = currentUser.Email;
            email.EmailFrom = currentUser.Email;
            email.CreatedById = currentUser.Id;

            await _emailsRepo.AddEmailAsync(email);
        }

        public async Task DeleteEmailAsync(List<int> ids)
        {
            await _emailsRepo.DeleteSofAsync(ids);
        }
    }
}