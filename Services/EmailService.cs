using System.Data;
using AutoMapper;
using EmailWeb.ApplicationUser;
using EmailWeb.Data;
using EmailWeb.Models;
using EmailWeb.Repos;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;

namespace EmailWeb.Services
{
    public interface IEmailService
    {
        List<EmailDto> SearchEmails(EmailQuery query);

        Task CreateNewEmailAsync(NewEmail newEmail);

        Task DeleteEmailAsync(List<int> id);

        MemoryStream GenerateExcel(IEnumerable<EmailDto> emails);
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
             */

            var searchPhrase = query.QueryPhrase;
            var emailStatus = query.Status;
            var date = query.CreatedAt;
            var allEmailByCurrentUser = _emailsRepo.GetAllEmailsByCurrentUser();
            var emailsByDate = _emailsRepo.GetAllEmailsByCreatedAt(allEmailByCurrentUser, date);

            if (emailStatus == EmailStatus.All)
            {
                var result = _emailsRepo.GetEmailEmailToOrSubject(emailsByDate, searchPhrase);
                return _mapper.Map<List<EmailDto>>(result);
            }
            else
            {
                var emailsBySubjectOrEmailTo = _emailsRepo.GetEmailEmailToOrSubject(emailsByDate, searchPhrase);
                var result = _emailsRepo.GetAllEmailsByStatus(emailsBySubjectOrEmailTo, emailStatus);
                return _mapper.Map<List<EmailDto>>(result);
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

        public MemoryStream GenerateExcel(IEnumerable<EmailDto> emails)
        {
            var fileName = $"emails{DateTime.Today}.xlsx";
            DataTable dataTable = new DataTable("Emails");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Id"),
                new DataColumn("Subject"),
                new DataColumn("EmailTo"),
                new DataColumn("CreatedAt"),
                new DataColumn("EmailStatus")
            });

            foreach (var email in emails)
            {
                dataTable.Rows.Add(email.Id, email.Subject, email.EmailTo, email.CreatedAt, email.EmailStatus);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    return stream;
                }
            }
        }
    }
}