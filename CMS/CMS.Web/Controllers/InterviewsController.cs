using Microsoft.AspNetCore;
using CMS.Application.DTOs;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Services.Interfaces;
using CMS.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using CMS.Web.Utils;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CMS.Domain;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Habanero.Util;
using CMS.Repository.Interfaces;
using Hangfire;
using System.Security.Claims;

namespace CMS.Web.Controllers
{

    public class InterviewsController : Controller
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

        public InterviewsController(IInterviewsService interviewsService, ICandidateService candidateService,
            IPositionService positionService, IStatusService statusService, IWebHostEnvironment env,
            IAccountService accountService, INotificationsService notificationsService, IInterviewsRepository interviewsRepository, IHttpContextAccessor httpContextAccessor)
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


        private string GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }


        public async Task<ActionResult> MyInterviews(int? statusFilter)
        {
            if (User.IsInRole("Interviewer") || User.IsInRole("General Manager") || User.IsInRole("HR Manager") || User.IsInRole("Solution Architecture"))
            { 
                // Get all statuses
                var statusesResult = await _StatusService.GetAll();
            if (!statusesResult.IsSuccess)
            {
                ModelState.AddModelError("", statusesResult.Error);
                return View(new List<InterviewsDTO>()); // Return an empty list if there was an error
            }

            var statuses = statusesResult.Value;
            ViewBag.StatusList = new SelectList(statuses, "Id", "Name");

            // Default to "Pending" status if no filter is specified
            if (!statusFilter.HasValue)
            {
                statusFilter = await _StatusService.GetStatusIdByName("Pending"); 
            }

            var result = await _interviewsService.MyInterviews();
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Error);
                return View();
            }

            var interviewsDTOs = result.Value;

            // Filter interviews based on the selected status if a filter is applied
            if (statusFilter.HasValue && statusFilter.Value > 0)
            {
                interviewsDTOs = interviewsDTOs
                    .Where(i => i.StatusId == statusFilter.Value)
                    .ToList();
            }

                interviewsDTOs = interviewsDTOs.OrderBy(i => i.Date).ToList();

                return View(interviewsDTOs);
            }
            else
            {
                return View("AccessDenied");
            }
        }



        // GET: InterviewsController
        public async Task<ActionResult> Index()
        {

            if (User.IsInRole("Admin") || User.IsInRole("HR Manager"))
            {

                var result = await _interviewsService.GetAll();
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

        // GET: InterviewsController/Details/5
        public async Task<ActionResult> Details(int id, string previousAction)
        {
            ViewBag.PreviousAction = previousAction;
            var result = await _interviewsService.GetById(id, GetUserId());

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

        public async Task<ActionResult> ShowHistory(int id)
        {
            var result =await _interviewsService.ShowHistory(id);

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

        //[Authorize(Roles = "General Manager")]
        // GET: InterviewsController/Create
        public async Task<ActionResult> Create()
        {
            await LoadSelectionLists();
            return View();
        }
        private async Task LoadSelectionLists()
        {
            var positions = await _positionService.GetAll();
            ViewBag.positionList = new SelectList(positions.Value, "Id", "Name");
            var candidates = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateList = new SelectList(candidates, "Id", "FullName");
            var interviewers = await _accountService.GetAllInterviewers();
            ViewBag.interviewersList = new SelectList(interviewers.Value, "Id", "UserName");
            var architectures = await _accountService.GetAllArchitectureInterviewers();
            ViewBag.architecturesList = new SelectList(architectures.Value, "Id", "UserName");
            var statuses = await _StatusService.GetAll();
            ViewBag.statusList = new SelectList(statuses.Value, "Id", "Name");
        }
        // POST: InterviewsController/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InterviewsDTO collection)
        {

            await LoadSelectionLists();

            if (ModelState.IsValid)
            {
                var result = await _interviewsService.Insert(collection, GetUserId());


                if (result.IsSuccess)
                {
                    if (User.IsInRole("HR Manager") || User.IsInRole("Admin"))
                    {
                        var selectedInterviewerId = collection.InterviewerId;
                        HttpContext.Session.SetString("ArchitectureInterviewerId", collection.ArchitectureInterviewerId ?? "");

                        await _notificationsService.CreateInterviewNotificationForInterviewerAsync(collection.Date, collection.CandidateId, collection.PositionId, selectedInterviewerId, isCanceled: false);

                        ScheduleInterviewReminder(collection);

                        var interviewerEmail = await GetInterviewerEmail(collection.InterviewerId);
                        EmailDTOs emailModel = new EmailDTOs
                        {
                            EmailTo = new List<string> { interviewerEmail },
                            EmailBody = $"You have an interview scheduled on {collection.Date}. Please be prepared.",
                            Subject = "Interview Invitation"
                        };

                        //Send an Email to the interviewer
                        //await SendEmailToInterviewer(interviewerEmail, collection, emailModel);

                        var reminderJobId = BackgroundJob.Schedule(() => ReminderJobAsync(selectedInterviewerId, collection), collection.Date.AddHours(16));


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

        // GET: InterviewsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var StatusDTOs = await _StatusService.GetAll();
            ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");
            var result = await _interviewsService.GetById(id, GetUserId());
            var interviewDTO = result.Value;
            if (interviewDTO == null)
            {
                return NotFound();
            }
            await LoadSelectionLists();

            return View(interviewDTO);
        }

        // POST: InterviewsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, InterviewsDTO collection)
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
                await _notificationsService.CreateInterviewNotificationForInterviewerAsync(collection.Date, collection.CandidateId, collection.PositionId, selectedInterviewerId, isCanceled: true);
            }


            if (ModelState.IsValid)
            {
                var result = await _interviewsService.Update(collection, GetUserId());

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


        // GET: InterviewsController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _interviewsService.GetById(id, GetUserId());
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

        // POST: InterviewsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, InterviewsDTO collection)
        {
            if (id <= 0)
            {
                return BadRequest("invalid career offer id");
            }
            var result = await _interviewsService.Delete(id, GetUserId());
            if (result.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", result.Error);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAttachment(int id, IFormFile file)
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
                    await _interviewsService.UpdateInterviewAttachmentAsync(id, file.FileName, file.Length, stream, GetUserId());
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
        public async Task<IActionResult> UpdateAfterInterview(int id)
        {
            var StatusDTOs = await _StatusService.GetAll();
            ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");

            var result = await _interviewsService.GetById(id, GetUserId());
            var InterviewsDTO = result.Value;

            return View(InterviewsDTO);
        }
     

        [HttpPost]
        public async Task<IActionResult> UpdateAfterInterview(InterviewsDTO interviewsDTO, IFormFile file)
        {
            var StatusDTOs = await _StatusService.GetAll();
            ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");

            var validationErrors = new List<string>();

            if ((file == null || file.Length == 0) && User.IsInRole("Interviewer"))
            {
                ModelState.AddModelError("AttachmentId", "Please choose a file to upload.");
            }

            if (interviewsDTO.ActualExperience == null)
            {
                ModelState.AddModelError("ActualExperience", "Please add the actual experience.");
            }

            if (!interviewsDTO.StatusId.HasValue)
            {
                ModelState.AddModelError("StatusId", "Please select a status.");
            }
            else
            {
                var statusResult = await _StatusService.GetById(interviewsDTO.StatusId.Value);
                if (!statusResult.IsSuccess)
                {
                    ModelState.AddModelError("StatusId", "Invalid status selected.");
                }
                else
                {
                    var status = statusResult.Value;
                    if (status.Code == Domain.Enums.StatusCode.Rejected && string.IsNullOrWhiteSpace(interviewsDTO.Notes))
                    {
                        ModelState.AddModelError("Notes", "Please add a note for why it was rejected.");
                    }
                }
            }


            if (interviewsDTO.Score == null && User.IsInRole("Interviewer"))
            {
                ModelState.AddModelError("Score", "Please add a score.");
            }

            if (interviewsDTO.StatusId == null)
            {
                ModelState.AddModelError("StatusId", "Please select a status.");
            }

            if (validationErrors.Count() > 0)
            {
                foreach (var validation in validationErrors)
                {
                    ModelState.AddModelError("", validation);
                }
                return View(interviewsDTO);
            }

            FileStream attachmentStream = null;
            if (file != null && file.Length != 0)
            {
                attachmentStream = await AttachmentHelper.handleUpload(file, _attachmentStoragePath);
                interviewsDTO.FileName = file.FileName;
                interviewsDTO.FileSize = file.Length;
                interviewsDTO.FileData = attachmentStream;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _interviewsService.ConductInterview(interviewsDTO, GetUserId());

                    if (attachmentStream != null)
                    {
                        // Close the file stream and release the file
                        attachmentStream.Close();
                        attachmentStream.Dispose();
                        AttachmentHelper.removeFile(file.FileName, _attachmentStoragePath);
                    }


                    if (User.IsInRole("Interviewer"))
                    {
                        var statusResult = await _StatusService.GetById(interviewsDTO.StatusId.Value);
                        var status = statusResult.Value;


                        if (status.Code == Domain.Enums.StatusCode.Rejected || status.Code == Domain.Enums.StatusCode.Approved)
                        {
                            await _notificationsService.CreateNotificationForGeneralManagerAsync(interviewsDTO.StatusId.Value, interviewsDTO.Notes, interviewsDTO.CandidateId, interviewsDTO.PositionId);

                            var architectureInterviewerId = HttpContext.Session.GetString("ArchitectureInterviewerId");
                            if(architectureInterviewerId != "")
                            {
                                await _notificationsService.CreateNotificationForArchiAsync(interviewsDTO.StatusId.Value, interviewsDTO.Notes, interviewsDTO.CandidateId, interviewsDTO.PositionId);
                            }


                            if (status.Code == Domain.Enums.StatusCode.Approved)
                            {
                                string userName = GetLoggedInUserName();
                                var GMEmail = await GetGMEmail();
                                var HREmail = await GetHREmail();
                                var ArchiEmail = await GetArchiEmail();

                                EmailDTOs emailModel = new EmailDTOs
                                {
                                    EmailTo = new List<string> { GMEmail },
                                    EmailBody = $"You have a second interview, Check the system. Please be prepared.",
                                    Subject = "Interview Invitation"
                                };


                                EmailDTOs emailModelToHR = new EmailDTOs
                                {
                                    EmailTo = new List<string> { HREmail },
                                    EmailBody = $"The first interview Approved by {userName}.",
                                    Subject = "Interview Approval"
                                };


                                if (architectureInterviewerId != "" && status.Code == Domain.Enums.StatusCode.Approved)
                                {
                                    EmailDTOs architectureEmailModel = new EmailDTOs
                                    {
                                        EmailTo = new List<string> { ArchiEmail },
                                        EmailBody = $"An interview has been scheduled with Saeed, and you are assigned as the Architecture Interviewer. Please chech the system.",
                                        Subject = "Architecture Interview Assignment"
                                    };
                                    if (!string.IsNullOrEmpty(ArchiEmail))
                                    {
                                        //Send an Email to the Archi if it was selceted
                                        //await SendEmailToInterviewer(ArchiEmail, interviewsDTO, architectureEmailModel);
                                    }
                                }

                                //var reminderJobId = BackgroundJob.Schedule(() => ReminderJobAsync(GMEmail, interviewsDTO), interviewsDTO.Date.AddHours(16));


                                //if (!string.IsNullOrEmpty(GMEmail))
                                //{
                                //    await SendEmailToInterviewer(GMEmail, interviewsDTO, emailModel);
                                //}

                                //if (!string.IsNullOrEmpty(HREmail))
                                //{
                                //    await SendEmailToInterviewer(HREmail, interviewsDTO, emailModelToHR);
                                //}
                               

                                return RedirectToAction(nameof(MyInterviews));
                            }

                            else if (status.Code == Domain.Enums.StatusCode.Rejected)
                            {
                                string userName = GetLoggedInUserName();
                                var HREmail = await GetHREmail();
                                EmailDTOs emailModel = new EmailDTOs
                                {
                                    EmailTo = new List<string> { HREmail },
                                    EmailBody = $"The first interview was rejected by {userName} .",
                                    Subject = "Interview Rejection"
                                };


                                //if (!string.IsNullOrEmpty(HREmail))
                                //{
                                //    await SendEmailToInterviewer(HREmail, interviewsDTO, emailModel);
                                //}

                                return RedirectToAction(nameof(MyInterviews));
                            }

                            else
                            {
                                return RedirectToAction(nameof(MyInterviews));

                            }


                        }
                        else
                        {
                            return RedirectToAction(nameof(MyInterviews));

                        }
                    }
                    else if (User.IsInRole("General Manager"))
                    {
                        var statusResult = await _StatusService.GetById(interviewsDTO.StatusId.Value);
                        var status = statusResult.Value;

                        if ((status.Code == Domain.Enums.StatusCode.Rejected || status.Code == Domain.Enums.StatusCode.Approved))
                        {
                            await _notificationsService.CreateInterviewNotificationForHRInterview(interviewsDTO.StatusId.Value, interviewsDTO.Notes, interviewsDTO.CandidateId, interviewsDTO.PositionId);


                            if (status.Code == Domain.Enums.StatusCode.Approved)
                            {
                                string userName = GetLoggedInUserName();
                                var HREmail = await GetHREmail();
                                EmailDTOs emailModel = new EmailDTOs
                                {
                                    EmailTo = new List<string> { HREmail },
                                    EmailBody = $"You have a thierd interview, Check the system. Please be prepared.",
                                    Subject = "Interview Invitation"
                                };

                                EmailDTOs emailModelApproval = new EmailDTOs
                                {
                                    EmailTo = new List<string> { HREmail },
                                    EmailBody = $"The Second Interview Approved by {userName}.",
                                    Subject = "Interview Approval"
                                };

                                //var reminderJobId = BackgroundJob.Schedule(() => ReminderJobAsync(HREmail, interviewsDTO), interviewsDTO.Date.AddHours(16));

                                //if (!string.IsNullOrEmpty(HREmail))
                                //{
                                //    await SendEmailToInterviewer(HREmail, interviewsDTO, emailModel);
                                //}

                                return RedirectToAction(nameof(MyInterviews));
                            }

                            else if (status.Code == Domain.Enums.StatusCode.Rejected)
                            {
                                string userName = GetLoggedInUserName();
                                var HREmail = await GetHREmail();
                                EmailDTOs emailModel = new EmailDTOs
                                {
                                    EmailTo = new List<string> { HREmail },
                                    EmailBody = $"The second interview was rejected by {userName} .",
                                    Subject = "Interview Rejection"
                                };
                                //if (!string.IsNullOrEmpty(HREmail))
                                //{
                                //    await SendEmailToInterviewer(HREmail, interviewsDTO, emailModel);
                                //}

                                return RedirectToAction(nameof(MyInterviews));
                            }

                            else
                            {
                                return RedirectToAction(nameof(MyInterviews));

                            }
                        }

                    }
                    else if (User.IsInRole("Solution Architecture"))
                    {
                        var statusResult = await _StatusService.GetById(interviewsDTO.StatusId.Value);
                        var status = statusResult.Value;

                        if ((status.Code == Domain.Enums.StatusCode.Rejected || status.Code == Domain.Enums.StatusCode.Approved))
                        {
                            await _notificationsService.CreateInterviewNotificationForHRInterview(interviewsDTO.StatusId.Value, interviewsDTO.Notes, interviewsDTO.CandidateId, interviewsDTO.PositionId);


                            if (status.Code == Domain.Enums.StatusCode.Approved)
                            {
                                string userName = GetLoggedInUserName();
                                var HREmail = await GetHREmail();
                                EmailDTOs emailModel = new EmailDTOs
                                {
                                    EmailTo = new List<string> { HREmail },
                                    EmailBody = $"You have a thierd interview, Check the system. Please be prepared.",
                                    Subject = "Interview Invitation"
                                };

                                EmailDTOs emailModelApproval = new EmailDTOs
                                {
                                    EmailTo = new List<string> { HREmail },
                                    EmailBody = $"The Second Interview Approved by {userName}.",
                                    Subject = "Interview Approval"
                                };

                                //var reminderJobId = BackgroundJob.Schedule(() => ReminderJobAsync(HREmail, interviewsDTO), interviewsDTO.Date.AddHours(16));

                                //if (!string.IsNullOrEmpty(HREmail))
                                //{
                                //    await SendEmailToInterviewer(HREmail, interviewsDTO, emailModel);
                                //}

                                return RedirectToAction(nameof(MyInterviews));
                            }

                            else if (status.Code == Domain.Enums.StatusCode.Rejected)
                            {
                                string userName = GetLoggedInUserName();
                                var HREmail = await GetHREmail();
                                EmailDTOs emailModel = new EmailDTOs
                                {
                                    EmailTo = new List<string> { HREmail },
                                    EmailBody = $"The second interview was rejected by {userName} .",
                                    Subject = "Interview Rejection"
                                };
                                //if (!string.IsNullOrEmpty(HREmail))
                                //{
                                //    await SendEmailToInterviewer(HREmail, interviewsDTO, emailModel);
                                //}

                                return RedirectToAction(nameof(MyInterviews));
                            }

                            else
                            {
                                return RedirectToAction(nameof(MyInterviews));

                            }
                        }

                    }
                    else
                    {
                        return RedirectToAction(nameof(MyInterviews));
                    }

                    // Redirect to the appropriate action
                    return RedirectToAction(nameof(MyInterviews));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred: " + ex.Message);
                }
            }

            return View(interviewsDTO);
        }




        public async Task SendEmailToInterviewer(string interviewerEmail, InterviewsDTO interview,EmailDTOs emailmodel)
        {
            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.sssprocess.com";
                smtp.Port = 587;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = true;
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
                ModelState.AddModelError("", "Failed to send email: " + ex.Message);
            }

        }



        public async Task<string> GetInterviewerEmail(string interviewerId)
        {

            var email = await _interviewsRepository.GetInterviewerEmail(interviewerId, GetUserId());

            if (email != null)
            {
                return email;
            }
            else
            {
                return null;
            }
         
        }


        public async Task<string> GetHREmail()
        {
            var email = await _interviewsRepository.GetHREmail(GetUserId());

            if (email != null)
            {
                return email;
            }
            else
            {
                return null;
            }
        }
        public async Task<string> GetArchiEmail()
        {
            var email = await _interviewsRepository.GetArchiEmail(GetUserId());

            if (email != null)
            {
                return email;
            }
            else
            {
                return null;
            }
        }



        public async Task<string> GetGMEmail()
        {
            var email = await _interviewsRepository.GetGeneralManagerEmail(GetUserId());

            if (email != null)
            {
                return email;
            }
            else
            {
                return null;
            }
        }


        public string GetLoggedInUserName()
        {
            return _httpContextAccessor.HttpContext.User.Identity.Name;
        }


        public async Task ReminderJobAsync(string interviewerId, InterviewsDTO collection)
        {
            // Check if the interviewer has given a score, and if not, send a reminder email
            bool hasGivenScore = await _interviewsRepository.HasGivenStatusAsync(interviewerId, collection.InterviewsId, GetUserId());

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

        public async Task ReminderJobAsyncForGM(string interviewerId, InterviewsDTO collection)
        {
            // Check if the interviewer has given a score, and if not, send a reminder email
            bool hasGivenScore = await _interviewsRepository.HasGivenStatusAsync(interviewerId, collection.InterviewsId, GetUserId());

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

        public async Task ReminderJobAsyncForHR(string interviewerId, InterviewsDTO collection)
        {
            // Check if the interviewer has given a score, and if not, send a reminder email
            bool hasGivenScore = await _interviewsRepository.HasGivenStatusAsync(interviewerId, collection.InterviewsId, GetUserId());

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

        public void ScheduleInterviewReminder(InterviewsDTO collection)
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

        public async Task SendInterviewReminderEmail(InterviewsDTO collection)
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




    }
}