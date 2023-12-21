using CMS.Application.DTOs;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Hangfire;
namespace CMS.Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IInterviewsRepository _interviewsRepository;
        private readonly ICandidateService _candidateService;
        private readonly IInterviewsService _interviewsService;

        public EmailService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager,
            IInterviewsRepository interviewsRepository,
            ICandidateService candidateService,IInterviewsService interviewsService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _interviewsRepository = interviewsRepository;
            _candidateService = candidateService;
            _interviewsService = interviewsService;
        }

        public void LogException(string methodName, Exception ex, string additionalInfo = null)
        {

            _interviewsService.LogException(methodName, ex, additionalInfo);
        }


        public async Task<string> GetArchiEmail()
        {
            try
            {
                return await _interviewsRepository.GetRoleEmail("Solution Architecture");
            }
            catch (Exception ex)
            {
                LogException(nameof(GetArchiEmail), ex, "Faild to get Archi email");
                throw ex;
            }
        }

        public async Task<string> GetGMEmail()
        {
            try
            {
                return await _interviewsRepository.GetRoleEmail("General Manager");
            }
            catch (Exception ex)
            {
                LogException(nameof(GetGMEmail), ex, "Faild to get General Manager email");
                throw ex;
            }
        }

        public async Task<string> GetHREmail()
        {
            try
            {
                return await _interviewsRepository.GetRoleEmail("HR Manager");
            }
            catch (Exception ex)
            {
                LogException(nameof(GetHREmail), ex, "Faild to get HR email");
                throw ex;
            }
        }
        public async Task<string> GetInterviewerEmail(string interviewerId)
        {
            try
            {
                return await _interviewsRepository.GetRoleEmail("Interviewer");
            }
            catch (Exception ex)
            {
                LogException(nameof(GetInterviewerEmail), ex, "Faild to get interviewer email");
                throw ex;
            }
        }

        public  string GetLoggedInUserName()
        {
            try
            {
                return _httpContextAccessor.HttpContext.User.Identity.Name;
            }
            catch (Exception ex)
            {
                LogException(nameof(GetLoggedInUserName), ex, "Faild to get Logged In UserName");
                throw ex;
            }
        }

        public async Task ReminderJobAsync(string interviewerId, InterviewsDTO collection)
        {
            try
            {
                // Check if the interviewer has given a score, and if not, send a reminder email
                bool hasGivenScore = await _interviewsRepository.HasGivenStatusAsync(interviewerId, collection.InterviewsId);

                if (!hasGivenScore)
                {
                    var interviewerEmail2 = await GetInterviewerEmail(collection.InterviewerId);
                    EmailDTOs emailModel = new EmailDTOs
                    {
                        EmailTo = new List<string> { interviewerEmail2 },
                        EmailBody = "You haven't provided a score for the interview. Please provide a score.",
                        Subject = "Interview Score Reminder"
                    };

                    await SendEmailToInterviewer(interviewerEmail2, collection, emailModel);
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(ReminderJobAsync), ex, "Faild to send a reminder email");
                throw ex;
            }
        }

        public async Task ResendFailedEmail(EmailDTOs emailToResend)
        {
            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.sssprocess.com";
                smtp.Port = 587;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = false;
                smtp.UseDefaultCredentials = true;
                string UserName = "CMS@sss-process.org";
                string Password = "P@ssw0rd2023";
                smtp.Credentials = new NetworkCredential(UserName, Password);

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress("cms@techprocess.net");

                    if (emailToResend.EmailTo != null && emailToResend.EmailTo.Any())
                    {
                        foreach (var to in emailToResend.EmailTo)
                        {
                            message.To.Add(to);
                        }
                    }

                    message.Body = emailToResend.EmailBody;
                    message.Subject = emailToResend.Subject;
                    message.IsBodyHtml = true;

                    await smtp.SendMailAsync(message);

                }
            }
            catch (Exception ex)
            {
                LogException(nameof(ResendFailedEmail), ex, "Failed to resend an email");
            }
        }

        public async void RetryFailedEmails(EmailDTOs emailmodel)
        {
            try
            {
                List<EmailDTOs> failedEmails = new List<EmailDTOs>();

                failedEmails.Add(emailmodel);

                var emailsToResend = failedEmails.Take(10).ToList();
                foreach (var emailToResend in emailsToResend)
                {
                    BackgroundJob.Schedule(() => ResendFailedEmail(emailToResend), TimeSpan.FromMinutes(20));


                    // Remove the resent email from the list of failed emails
                    failedEmails.Remove(emailToResend);
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(RetryFailedEmails), ex, "Failed to Retry Failed Emails");
            }
        }

        public async void ScheduleInterviewReminder(InterviewsDTO collection)
        {
            try
            {
                var reminderTime = collection.Date.AddMinutes(-15);

                if (DateTime.UtcNow < reminderTime)
                {
                    var interviewReminderJobId = BackgroundJob.Schedule(
                        () => SendInterviewReminderEmail(collection),
                        reminderTime
                    );
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(ScheduleInterviewReminder), ex, "Faild to Schedule Interview Reminder");
                throw ex;
            }
        }

        public async Task SendEmailToInterviewer(string interviewerEmail, InterviewsDTO interview, EmailDTOs emailModel)
        {
            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.sssprocess.com";
                smtp.Port = 587;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = false;
                smtp.UseDefaultCredentials = true;
                string UserName = "CMS@sss-process.org";
                string Password = "P@ssw0rd2023";
                smtp.Credentials = new NetworkCredential(UserName, Password);

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress("cms@techprocess.net");

                    if (emailModel.EmailTo != null && emailModel.EmailTo.Any())
                    {
                        foreach (var to in emailModel.EmailTo)
                        {
                            if (to.EndsWith("@techprocess.net"))
                            {
                                message.To.Add(to);
                            }
                        }
                    }


                    message.Body = emailModel.EmailBody;
                    message.Subject = emailModel.Subject;
                    message.IsBodyHtml = true;

                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(SendEmailToInterviewer), ex, "Faild to send an email");
                RetryFailedEmails(emailModel);

            }
        }

        public async Task SendEmailToInterviewers(List<string> interviewersEmails, InterviewsDTO interview, EmailDTOs emailModel)
        {
            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.sssprocess.com";
                smtp.Port = 587;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = false;
                smtp.UseDefaultCredentials = true;
                string UserName = "CMS@sss-process.org";
                string Password = "P@ssw0rd2023";
                smtp.Credentials = new NetworkCredential(UserName, Password);

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress("cms@techprocess.net");

                    if (emailModel.EmailTo != null && emailModel.EmailTo.Any())
                    {
                        foreach (var to in interviewersEmails)
                        {
                            message.To.Add(to);
                        }
                    }

                    message.Body = emailModel.EmailBody;
                    message.Subject = emailModel.Subject;
                    message.IsBodyHtml = true;

                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(SendEmailToInterviewers), ex, "Failed to send an email");
                RetryFailedEmails(emailModel);
            }
        }

        public async Task SendInterviewReminderEmail(InterviewsDTO collection)
        {
            try
            {
                string interviewerEmail = await GetInterviewerEmail(collection.InterviewerId);
                var userInterviewer = await _userManager.FindByEmailAsync(interviewerEmail);
                var candidateName = await _candidateService.GetCandidateByIdAsync(collection.CandidateId);
                var candidateNameresult = candidateName.FullName;


                if (!string.IsNullOrEmpty(interviewerEmail))
                {
                    EmailDTOs emailModel = new EmailDTOs
                    {
                        EmailTo = new List<string> { interviewerEmail },
                        Subject = $"Interview Reminder ( {candidateNameresult} )",
                        EmailBody = $@"<html>
                    <body style='font-family: Arial, sans-serif;'>
                        <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                            <p style='font-size: 18px; color: #333;'>
                                Dear {userInterviewer.UserName.Replace("_", " ")},
                            </p>
                            <p style='font-size: 16px; color: #555;'>
                           Your interview is scheduled to start in 15 minutes. Please be prepared.
                            </p>
                            <p style='font-size: 14px; color: #777;'>
                                Regards,
                            </p>

                    <p style='font-size: 14px; color: #777;'>Sent by: CMS</p>
                        </div>
                    </body>
                 </html>"
                    };

                    await SendEmailToInterviewer(interviewerEmail, collection, emailModel);
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(ScheduleInterviewReminder), ex, "Faild to Send Interview Reminder Email");
                throw ex;
            }
        }
    }
}
