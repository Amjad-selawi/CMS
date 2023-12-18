using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using CMS.Services.Interfaces;
using CMS.Services.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using CMS.Repository.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using CMS.Application.DTOs;
using Hangfire;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Linq;
using CMS.Web.Utils;

namespace CMS.Web.Controllers
{
    public class InterviewsGMController : Controller
    {
        private readonly IInterviewsService _interviewsService;
        private readonly ICandidateService _candidateService;
        private readonly IPositionService _positionService;
        private readonly IStatusService _StatusService;
        private readonly IAccountService _accountService;
        private readonly INotificationsService _notificationsService;
        private readonly IInterviewsRepository _interviewsRepository;
        private readonly string _attachmentStoragePath;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InterviewsGMController(IInterviewsService interviewsService, ICandidateService candidateService,
            IPositionService positionService, IStatusService statusService, IWebHostEnvironment env,
            IAccountService accountService, INotificationsService notificationsService,
            IInterviewsRepository interviewsRepository, IHttpContextAccessor httpContextAccessor)
        {
            _interviewsService = interviewsService;
            _candidateService = candidateService;
            _positionService = positionService;
            _StatusService = statusService;
            _httpContextAccessor = httpContextAccessor;
            _accountService = accountService;
            _notificationsService = notificationsService;
            _interviewsRepository = interviewsRepository;
            _attachmentStoragePath = Path.Combine(env.WebRootPath, "attachments");

            if (!Directory.Exists(_attachmentStoragePath))
            {
                Directory.CreateDirectory(_attachmentStoragePath);
            }
        }
        public void LogException(string methodName, Exception ex, string additionalInfo = null)
        {

            _interviewsService.LogException(methodName, ex, additionalInfo);
        }

        public async Task<ActionResult> Index()
        {
            try
            {


                if (User.IsInRole("Admin") || User.IsInRole("HR Manager"))
                {

                    var result = await _interviewsService.GetAllForGeneralManager();
                    if (result.IsSuccess)
                    {
                        var interviewsDTOs = result.Value;

                        return View(interviewsDTOs);
                    }
                    else
                    {
                        ModelState.AddModelError("", result.Error);
                        return View();
                    }
                }
                else
                {
                    return View("AccessDenied");
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(Index), ex, "Faild to load Interviews index page");
                throw ex;
            }
        }


        public async Task<ActionResult> ShowHistory(int id)
        {
            try
            {


                var result = await _interviewsService.ShowHistory(id);

                if (result.IsSuccess)
                {
                    var interviewsDTOs = result.Value;
                    return View(interviewsDTOs);
                }
                else
                {
                    ModelState.AddModelError("", result.Error);
                    return View();
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(ShowHistory), ex, "Faild to load show history page");
                throw ex;
            }
        }

    
        private async Task LoadSelectionLists()
        {
            try
            {




                var positions = await _positionService.GetAll();
                ViewBag.positionList = new SelectList(positions.Value, "Id", "Name");
                var candidates = await _candidateService.GetAllCandidatesAsync();
                ViewBag.candidateList = new SelectList(candidates, "Id", "FullName");
                var interviewers = await _accountService.GetAllInterviewersGM();
                ViewBag.interviewersList = new SelectList(interviewers.Value, "Id", "UserName");
                var architectures = await _accountService.GetAllArchitectureInterviewers();
                ViewBag.architecturesList = new SelectList(architectures.Value, "Id", "UserName");
                var statuses = await _StatusService.GetAll();
                ViewBag.statusList = new SelectList(statuses.Value, "Id", "Name");
            }
            catch (Exception ex)
            {
                LogException(nameof(LoadSelectionLists), ex, "Faild to LoadSelectionLists");
                throw ex;
            }
        }

        public async Task<ActionResult> Details(int id, string previousAction)
        {
            try
            {



                ViewBag.PreviousAction = previousAction;
                var result = await _interviewsService.GetById(id);

                await LoadSelectionLists();

                if (result.IsSuccess)
                {
                    var interviewsDTO = result.Value;

                    interviewsDTO.InterviewerName = await _interviewsService.GetInterviewerName(interviewsDTO.InterviewerId);


                    return View(interviewsDTO);
                }


                else
                {
                    ModelState.AddModelError("", result.Error);
                    return View();
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(Details), ex, $"Faild to load Interview details with ID: {id} ");
                throw ex;
            }
        }

        public async Task<ActionResult> Create()
        {
            try
            {


                await LoadSelectionLists();
                return View();
            }
            catch (Exception ex)
            {
                LogException(nameof(Create), ex, "Faild to load interview create page");
                throw ex;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InterviewsDTO collection)
        {
            try
            {

                //Get Candidate Name By Id
                var candidateName = await _candidateService.GetCandidateByIdAsync(collection.CandidateId);
                var candidateNameresult = candidateName.FullName;

                //Get Position Name By Id
                var positionName = await _positionService.GetById(collection.PositionId);
                var positionNameresult = positionName.Value;
                var lastPositionName = positionNameresult.Name;

                string userName = GetLoggedInUserName();

                await LoadSelectionLists();

                if (ModelState.IsValid)
                {
                    var result = await _interviewsService.Insert(collection);


                    if (result.IsSuccess)
                    {
                        if (User.IsInRole("HR Manager") || User.IsInRole("Admin"))
                        {
                            var selectedInterviewerId = collection.InterviewerId;
                            HttpContext.Session.SetString("ArchitectureInterviewerId", collection.ArchitectureInterviewerId ?? "");

                            await _notificationsService.CreateInterviewNotificationForInterviewerAsync(collection.Date, collection.CandidateId, collection.PositionId, new List<string> { collection.InterviewerId, collection.SecondInterviewerId }, isCanceled: false);

                            ScheduleInterviewReminder(collection);

                            var interviewerEmail = await GetInterviewerEmail(collection.InterviewerId);
                            EmailDTOs emailModel = new EmailDTOs
                            {
                                EmailTo = new List<string> { interviewerEmail },
                                Subject = "PM Interview Invitation",
                                EmailBody = $@"<html>
                                 <body style='font-family: Arial, sans-serif;'>
                                     <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                         <p style='font-size: 18px; color: #333;'>
                                             Dear {userName},
                                         </p>
                                         <p style='font-size: 16px; color: #555;'>
                                           You have an interview scheduled on {collection.Date} for candidate: {candidateNameresult} with position: {lastPositionName}. Please be prepared.
                                         </p>
                                         <p style='font-size: 14px; color: #777;'>
                                             Regards,
                                         </p>
                                     </div>
                                 </body>
                              </html>"
                            };

                            //Send an Email to the interviewer
                            await SendEmailToInterviewer(interviewerEmail, collection, emailModel);

                            //var reminderJobId = BackgroundJob.Schedule(() => ReminderJobAsync(selectedInterviewerId, collection), collection.Date.AddHours(16));



                            return RedirectToAction("Index");
                        }


                        else
                        {
                            return RedirectToAction("Index");
                        }


                    }

                    ModelState.AddModelError("", result.Error);
                }
                else
                {
                    ModelState.AddModelError("", "error validating the model");
                }

                return View(collection);
            }
            catch (Exception ex)
            {
                LogException(nameof(Create), ex, "Faild to create interview");
                throw ex;
            }

        }




        // POST: InterviewsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, InterviewsDTO collection)
        {
            try
            {




                if (collection == null)
                {
                    ModelState.AddModelError("", $"the interview dto you are trying to update is null ");
                    return RedirectToAction("Index");
                }
                var StatusDTOs = await _StatusService.GetAll();
                ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");

                await LoadSelectionLists();

                HttpContext.Session.SetString("ArchitectureInterviewerId", collection.ArchitectureInterviewerId ?? "");


                var statusResult = await _StatusService.GetById((int)collection.StatusId);
                var status = statusResult.Value;

                if (status.Code == Domain.Enums.StatusCode.Rejected && collection.Notes == null)
                {
                    ModelState.AddModelError("Notes", "Please add note why it was rejected.");
                }
                if (status.Code == Domain.Enums.StatusCode.Rejected)
                {
                    var selectedInterviewerId = collection.InterviewerId;
                    await _notificationsService.CreateInterviewNotificationForInterviewerAsync(collection.Date, collection.CandidateId, collection.PositionId, new List<string> { collection.InterviewerId, collection.SecondInterviewerId }, isCanceled: false);
                }


                if (ModelState.IsValid)
                {
                    var result = await _interviewsService.Update(collection);

                    if (result.IsSuccess)
                    {
                        return RedirectToAction("Index");
                    }

                    ModelState.AddModelError("", result.Error);
                    return View(collection);
                }
                else
                {
                    ModelState.AddModelError("", $"");
                }
                return View(collection);
            }
            catch (Exception ex)
            {
                LogException(nameof(Edit), ex, "Faild to edit interview");
                throw ex;
            }
        }


        // GET: InterviewsController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            try
            {



                var result = await _interviewsService.GetById(id);
                if (result.IsSuccess)
                {
                    var interviewDTO = result.Value;

                    interviewDTO.InterviewerName = await _interviewsService.GetInterviewerName(interviewDTO.InterviewerId);



                    return View(interviewDTO);
                }


                else
                {
                    ModelState.AddModelError("", result.Error);
                    return View();
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(Delete), ex, "Faild to load delete interview page");
                throw ex;
            }
        }

        // POST: InterviewsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, InterviewsDTO collection)
        {
            try
            {



                if (id <= 0)
                {
                    return BadRequest("invalid career offer id");
                }
                var result = await _interviewsService.Delete(id);
                if (result.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", result.Error);
                return View();
            }
            catch (Exception ex)
            {
                LogException(nameof(Delete), ex, "Faild to delete interview");
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAttachment(int id, IFormFile file)
        {

            try
            {



                if (file == null || file.Length == 0)
                {
                    ModelState.AddModelError("File", "Please choose a file to upload.");
                    return View();
                }
                if (ModelState.IsValid)
                {
                    var stream = await AttachmentHelper.handleUpload(file, _attachmentStoragePath);
                    try
                    {
                        await _interviewsService.UpdateInterviewAttachmentAsync(id, file.FileName, file.Length, stream);
                        return RedirectToAction(nameof(Index));
                    }
                    finally
                    {
                        stream.Close();
                        AttachmentHelper.removeFile(file.FileName, _attachmentStoragePath);
                    }

                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                LogException(nameof(UpdateAttachment), ex, "UpdateAttachment not working");
                throw ex;
            }
        }







        public async Task<IActionResult> UpdateAfterInterview(int id)
        {
            try
            {



                var StatusDTOs = await _StatusService.GetAll();
                ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");

                var result = await _interviewsService.GetById(id);
                var InterviewsDTO = result.Value;

                return View(InterviewsDTO);
            }
            catch (Exception ex)
            {
                LogException(nameof(UpdateAfterInterview), ex, "UpdateAfterInterview not working");
                throw ex;
            }
        }


    


        public async Task<string> GetHREmail()
        {
            try
            {



                var email = await _interviewsRepository.GetHREmail();

                if (email != null)
                {
                    return email;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(GetHREmail), ex, "Faild to get hr email");
                throw ex;
            }
        }

        public async Task<string> GetGMEmail()
        {
            try
            {



                var email = await _interviewsRepository.GetGeneralManagerEmail();

                if (email != null)
                {
                    return email;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(GetGMEmail), ex, "Faild to get genetal manager email");
                throw ex;
            }
        }

        public string GetLoggedInUserName()
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






        public async Task SendEmailToInterviewer(string interviewerEmail, InterviewsDTO interview, EmailDTOs emailmodel)
        {
            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.sssprocess.com";
                smtp.Port = 587;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = false;
                smtp.UseDefaultCredentials = true;
                string UserName = "notifications@sss-process.org";
                string Password = "P@ssw0rd";
                smtp.Credentials = new NetworkCredential(UserName, Password);

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress("notifications@techprocess.net");

                    if (emailmodel.EmailTo != null && emailmodel.EmailTo.Any())
                    {
                        foreach (var to in emailmodel.EmailTo)
                        {
                            message.To.Add(to);
                        }
                    }


                    message.Body = emailmodel.EmailBody;
                    message.Subject = emailmodel.Subject;
                    message.IsBodyHtml = true;

                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(SendEmailToInterviewer), ex, "Faild to send an email");
                throw ex;
            }

        }

        public void ScheduleInterviewReminder(InterviewsDTO collection)
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

        public async Task SendInterviewReminderEmail(InterviewsDTO collection)
        {
            try
            {


                string interviewerEmail = await GetInterviewerEmail(collection.InterviewerId);

                if (!string.IsNullOrEmpty(interviewerEmail))
                {
                    EmailDTOs emailModel = new EmailDTOs
                    {
                        EmailTo = new List<string> { interviewerEmail },
                        EmailBody = "Your interview is scheduled to start in 15 minutes. Please be prepared.",
                        Subject = "Interview Reminder"
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
        public async Task<string> GetInterviewerEmail(string interviewerId)
        {
            try
            {



                var email = await _interviewsRepository.GetInterviewerEmail(interviewerId);

                if (email != null)
                {
                    return email;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(GetInterviewerEmail), ex, "Faild to get interviewer email");
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

    }
}
