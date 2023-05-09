using EmailWeb.Models;
using EmailWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailWeb.Controllers
{
    [Authorize]
    public class EmailController : Controller
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public IActionResult Index(EmailQuery query)
        {
            var emails = _emailService.SearchEmails(query);
            TempData["Status"] = query.Status;
            TempData["CreatedAt"] = query.CreatedAt.ToString(@"yyyy-MM-dd");
            TempData["QueryPhrase"] = query.QueryPhrase;

            return View(emails);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(NewEmail newEmail)
        {
            if (!ModelState.IsValid)
            {
                return View(newEmail);
            }
            _emailService.CreateNewEmailAsync(newEmail);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(EmailsToDelete emails)
        {
            _emailService.DeleteEmailAsync(emails.Checked);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<FileResult> ExportToExcel(EmailQuery query)
        {
            var emails = _emailService.SearchEmails(query);
            var fileName = $"emails{DateTime.Today}.xlsx";
            var stream = _emailService.GenerateExcel(emails).ToArray();

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}