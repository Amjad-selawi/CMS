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

        public async Task<ActionResult> MyInterviews(int? statusFilter)
        {
            if (User.IsInRole("Interviewer") || User.IsInRole("General Manager") || User.IsInRole("HR Manager"))
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
                statusFilter = await _StatusService.GetStatusIdByName("Pending"); // Replace with your actual method to get status by name
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

            return View(interviewsDTOs);
            }
            else
            {
                return View("AccessDenied");
            }
        }




        //public async Task<ActionResult> MyInterviews()
        //{

        //    var result = await _interviewsService.MyInterviews();
        //    if (result.IsSuccess)
        //    {
        //        var interviewsDTOs = result.Value;
        //        return View(interviewsDTOs);
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", result.Error);
        //        return View();
        //    }

        //}

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
                var result = await _interviewsService.Insert(collection);


                if (result.IsSuccess)
                {
                    if (User.IsInRole("HR Manager"))
                    {
                        var selectedInterviewerId = collection.InterviewerId;

                        await _notificationsService.CreateInterviewNotificationForInterviewerAsync(collection.Date, collection.CandidateId, collection.PositionId, selectedInterviewerId, isCanceled: false);




                        var interviewerEmail = await GetInterviewerEmail(collection.InterviewerId);
                        EmailDTOs emailModel = new EmailDTOs
                        {
                            EmailTo = new List<string> { interviewerEmail },
                            EmailBody = $"You have an interview scheduled on {collection.Date}. Please be prepared.",
                            Subject = "Interview Invitation"
                        };


                        var reminderJobId = BackgroundJob.Schedule(() => ReminderJobAsync(selectedInterviewerId, collection), collection.Date.AddHours(16));

                        //if (!string.IsNullOrEmpty(interviewerEmail))
                        //{
                        //    await SendEmailToInterviewer(interviewerEmail, collection, emailModel);
                        //}

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
            var result = await _interviewsService.GetById(id);
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
            if (collection.StatusId ==3  && collection.Notes == null)
            {
                ModelState.AddModelError("Notes", "Please add note why it was rejected.");
            }
            if (collection.StatusId == 3)
            {
                var selectedInterviewerId = collection.InterviewerId;
                await _notificationsService.CreateInterviewNotificationForInterviewerAsync(collection.Date, collection.CandidateId, collection.PositionId, selectedInterviewerId, isCanceled: true);
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


        // GET: InterviewsController/Delete/5
        public async Task<ActionResult> Delete(int id)
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

        // POST: InterviewsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, InterviewsDTO collection)
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
        public async Task<IActionResult> UpdateAfterInterview(int id)
        {
            var StatusDTOs = await _StatusService.GetAll();
            ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");

            var result = await _interviewsService.GetById(id);
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
         

            if(interviewsDTO.StatusId == 3 && interviewsDTO.Notes == null)
            {
                 ModelState.AddModelError("Notes", "Please add note why it was rejected.");
            }
            if (interviewsDTO.Score == null && User.IsInRole("Interviewer"))
            {
               
                ModelState.AddModelError("Score", "Please add score.");

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




                await _interviewsService.ConductInterview(interviewsDTO);
                if (attachmentStream != null)
                {
                    attachmentStream.Close();
                    attachmentStream.Dispose(); // Dispose the stream to release the file
                    AttachmentHelper.removeFile(file.FileName, _attachmentStoragePath);
                }
                   

                if (User.IsInRole("Interviewer"))
                {
                    if (interviewsDTO.StatusId == 2 || interviewsDTO.StatusId == 3)
                    {
                        await _notificationsService.CreateNotificationForGeneralManagerAsync(interviewsDTO.StatusId.Value, interviewsDTO.Notes,interviewsDTO.CandidateId ,interviewsDTO.PositionId);

                        if(interviewsDTO.StatusId == 2)
                        {
                            string userName = GetLoggedInUserName();
                            var GMEmail = await GetGMEmail();
                            var HREmail = await GetHREmail();

                            EmailDTOs emailModel = new EmailDTOs
                            {
                                EmailTo = new List<string> { GMEmail },
                                EmailBody = $"You have a second interview. Please be prepared.",
                                Subject = "Interview Invitation"
                            };


                            EmailDTOs emailModelToHR = new EmailDTOs
                            {
                                EmailTo = new List<string> { HREmail },
                                EmailBody = $"The first interview Approved by {userName}.",
                                Subject = "Interview Approval"
                            };

                            var reminderJobId = BackgroundJob.Schedule(() => ReminderJobAsync(GMEmail, interviewsDTO), interviewsDTO.Date.AddHours(16));


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

                        else if (interviewsDTO.StatusId == 3)
                        {
                            string userName = GetLoggedInUserName();
                            var HREmail = await GetHREmail();
                            EmailDTOs emailModel = new EmailDTOs
                            {
                                EmailTo = new List<string> { HREmail },
                                EmailBody = $"The first interview was rejected by {userName} .",
                                Subject = "Interview Rejection"
                            };

                            var reminderJobId = BackgroundJob.Schedule(() => ReminderJobAsync(HREmail, interviewsDTO), interviewsDTO.Date.AddHours(16));

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
                    if (interviewsDTO.StatusId == 2 || interviewsDTO.StatusId == 3)
                    {
                        await _notificationsService.CreateInterviewNotificationForHRInterview(interviewsDTO.StatusId.Value, interviewsDTO.Notes,interviewsDTO.CandidateId,interviewsDTO.PositionId);


                        if (interviewsDTO.StatusId == 2)
                        {
                            string userName = GetLoggedInUserName();
                            var HREmail = await GetHREmail();
                            EmailDTOs emailModel = new EmailDTOs
                            {
                                EmailTo = new List<string> { HREmail },
                                EmailBody = $"You have a thierd interview. Please be prepared.",
                                Subject = "Interview Invitation"
                            };

                            EmailDTOs emailModelApproval = new EmailDTOs
                            {
                                EmailTo = new List<string> { HREmail },
                                EmailBody = $"The Second Interview Approved by {userName}.",
                                Subject = "Interview Approval"
                            };


                            //if (!string.IsNullOrEmpty(HREmail))
                            //{
                            //    await SendEmailToInterviewer(HREmail, interviewsDTO, emailModel);
                            //}

                            return RedirectToAction(nameof(MyInterviews));
                        }

                        else if (interviewsDTO.StatusId == 3)
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
                string UserName = "notifications";
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


        public async Task<string> GetHREmail()
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


        public async Task<string> GetGMEmail()
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


        public string GetLoggedInUserName()
        {
            return _httpContextAccessor.HttpContext.User.Identity.Name;
        }


        public async Task ReminderJobAsync(string interviewerId, InterviewsDTO collection)
        {
            // Check if the interviewer has given a score, and if not, send a reminder email
            bool hasGivenScore = await _interviewsRepository.HasGivenScoreAsync(interviewerId, collection.InterviewsId);

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
            bool hasGivenScore = await _interviewsRepository.HasGivenScoreAsync(interviewerId, collection.InterviewsId);

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
            bool hasGivenScore = await _interviewsRepository.HasGivenScoreAsync(interviewerId, collection.InterviewsId);

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



    }
}