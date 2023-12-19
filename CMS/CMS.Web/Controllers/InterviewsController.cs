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
using CMS.Domain.Migrations;
using CMS.Repository.Implementation;

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
        private readonly UserManager<IdentityUser> _userManager;

        public InterviewsController(IInterviewsService interviewsService, ICandidateService candidateService,
            IPositionService positionService, IStatusService statusService, IWebHostEnvironment env,
            IAccountService accountService, INotificationsService notificationsService, 
            IInterviewsRepository interviewsRepository, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _interviewsService = interviewsService;
            _candidateService = candidateService;
            _positionService = positionService;
            _StatusService = statusService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
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
     
      


        public async Task<ActionResult> MyInterviews(int? statusFilter)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(MyInterviews), ex, "Faild to load MyInterviews");
                throw ex;
            }
        }



        // GET: InterviewsController
        public async Task<ActionResult> Index()
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(Index), ex, "Faild to load Interviews index page");
                throw ex;
            }
        }

    
        // GET: InterviewsController/Details/5
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

        public async Task<ActionResult> ShowHistory(int id)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(ShowHistory), ex, "Faild to load show history page");
                throw ex;
            }
        }

        //[Authorize(Roles = "General Manager")]
        // GET: InterviewsController/Create
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
        private async Task LoadSelectionLists()
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(LoadSelectionLists), ex, "Faild to LoadSelectionLists");
                throw ex;
            }
        }
        // POST: InterviewsController/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InterviewsDTO collection)
        {
            try
            {
                var firstInterviewerRoles = await _interviewsService.GetInterviewerRole(collection.InterviewerId);
                var secondInterviewerRoles = await _interviewsService.GetInterviewerRole(collection.SecondInterviewerId);

                await LoadSelectionLists();

                if (ModelState.IsValid)
                {
                    var result = await _interviewsService.Insert(collection);

                    if (result.IsSuccess)
                    {
                        if (User.IsInRole("HR Manager") || User.IsInRole("Admin"))
                        {
                            var selectedInterviewerId = collection.InterviewerId;
                            var insertedInterview = result.Value;
                            collection.InterviewsId = insertedInterview.InterviewsId;

                            HttpContext.Session.SetString($"SecondInterviewerId_{collection.InterviewsId}", collection.SecondInterviewerId ?? "");
                            HttpContext.Session.SetString($"InterviewerId_{collection.InterviewsId}", collection.InterviewerId ?? "");
                            HttpContext.Session.SetString("ArchitectureInterviewerId", collection.ArchitectureInterviewerId ?? "");
                            // Get Candidate Name By Id
                            var candidateName = await _candidateService.GetCandidateByIdAsync(collection.CandidateId);
                            var candidateNameresult = candidateName.FullName;

                            // Get Position Name By Id
                            var positionName = await _positionService.GetById(collection.PositionId);
                            var positionNameresult = positionName.Value;
                            var lastPositionName = positionNameresult.Name;

                            string userName = GetLoggedInUserName();

                            string userSecondInterviewer = null;

                            var secondInterviewerEmail = await GetInterviewerEmail(collection.SecondInterviewerId);
                            if (secondInterviewerEmail != null)
                            {
                                var userSecondInterviewerObj = await _userManager.FindByEmailAsync(secondInterviewerEmail);
                                userSecondInterviewer = userSecondInterviewerObj.UserName;
                            }

                            var formattedDate = collection.Date.ToString("dd/MM/yyyy hh:mm tt");

                            // Prepare the email model for the first interviewer
                            var interviewerEmail = await GetInterviewerEmail(collection.InterviewerId);
                            var userInterviewer = await _userManager.FindByEmailAsync(interviewerEmail);

                            EmailDTOs emailModel = new EmailDTOs
                            {
                                EmailTo = new List<string> { interviewerEmail },
                                Subject = "First Interview Invitation",
                                EmailBody = $@"<html>
                            <body style='font-family: Arial, sans-serif;'>
                                <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                    <p style='font-size: 18px; color: #333;'>
                                        Dear {userInterviewer},
                                    </p>
                                    <p style='font-size: 16px; color: #555;'>
                                {(collection.SecondInterviewerId != null ? $"You and {userSecondInterviewer} are" : "You are")} assigned to have a first interview for {candidateNameresult} scheduled on {formattedDate} for the {lastPositionName} position, kindly login to the system using the below link <a href='https://apps.sssprocess.com:6134/'>Click here</a>
                                    </p>
                                    <p style='font-size: 14px; color: #777;'>
                                        Regards,
                                    </p>
                                </div>
                            </body>
                        </html>"
                            };

                            await _notificationsService.CreateInterviewNotificationForInterviewerAsync(collection.Date, collection.CandidateId, collection.PositionId, new List<string> { collection.InterviewerId, collection.SecondInterviewerId }, isCanceled: false);

                            // Prepare the email model for the second interviewer if selected
                            if (collection.SecondInterviewerId != null)
                            {
                              
                                EmailDTOs emailModel2 = new EmailDTOs
                                {
                                    EmailTo = new List<string> { secondInterviewerEmail },
                                    Subject = "First Interview Invitation",
                                    EmailBody = $@"<html>
                                <body style='font-family: Arial, sans-serif;'>
                                    <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                        <p style='font-size: 18px; color: #333;'>
                                            Dear {userSecondInterviewer},
                                        </p>
                                        <p style='font-size: 16px; color: #555;'>
                                            You and {userInterviewer} are assigned to have a first interview with {candidateNameresult} for the {lastPositionName} position scheduled on {collection.Date} , kindly login to the system using the below link <a href='https://apps.sssprocess.com:6134/'>Click here</a>
                                        </p>
                                        <p style='font-size: 14px; color: #777;'>
                                            Regards,
                                        </p>
                                    </div>
                                </body>
                            </html>"
                                };

                                // Send emails to both first and second interviewers
                                await SendEmailToInterviewer(secondInterviewerEmail, collection, emailModel2);
                                await SendEmailToInterviewer(interviewerEmail, collection, emailModel);
                            }
                            else
                            {
                                // Send email only to the first interviewer if the second interviewer is not selected
                                await SendEmailToInterviewer(interviewerEmail, collection, emailModel);
                            }

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
                    ModelState.AddModelError("", "Error validating the model");
                }

                return View(collection);
            }
            catch (Exception ex)
            {
                LogException(nameof(Create), ex, "Failed to create interview");
                throw ex;
            }
        }



        // GET: InterviewsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(Edit), ex, "Faild to load edit interview page");
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

        public async Task<IActionResult> UpdateAfterInterviewForEdit(int id)
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
     

        [HttpPost]
        public async Task<IActionResult> UpdateAfterInterview(InterviewsDTO interviewsDTO, IFormFile file)
        {
            try
            {
                var newStatusResult = await _StatusService.GetById(interviewsDTO.StatusId.Value);
                if (newStatusResult.IsSuccess)
                {
                    var newStatus = newStatusResult.Value;
                    if (newStatus.Code == Domain.Enums.StatusCode.OnHold)
                    {
                        var interviewCount = await _interviewsRepository.GetInterviewCountForCandidate(interviewsDTO.CandidateId);
                        if ((interviewCount > 1 && interviewCount <= 2) || ((interviewCount == 3 || interviewCount == 4) && User.IsInRole("General Manager")))
                        {
                            var interviewsDeleted = await _interviewsRepository.DeletePendingInterviews(interviewsDTO.CandidateId, interviewsDTO.PositionId, userId: User.FindFirstValue(ClaimTypes.NameIdentifier));
                            if (!interviewsDeleted)
                            {
                                // Show a pop-up or handle the case where there are no pending interviews to delete
                                ModelState.AddModelError("StatusId", "Cannot put this interview on hold.");
                                return View(interviewsDTO);
                            }
                        }
                        else
                        {
                            // Show a pop-up or handle the case where there's only one interview
                            ModelState.AddModelError("StatusId", "Cannot put this interview on hold.");
                            return View(interviewsDTO);
                        }
                    }
                }


                var firstInterviewerRoles = await _interviewsService.GetInterviewerRole(interviewsDTO.InterviewerId);
                var secondInterviewerRoles = await _interviewsService.GetInterviewerRole(interviewsDTO.SecondInterviewerId);


                //Get Candidate Name By Id
                var candidateName = await _candidateService.GetCandidateByIdAsync(interviewsDTO.CandidateId);
                var candidateNameresult = candidateName.FullName;

                //Get Position Name By Id
                var positionName = await _positionService.GetById(interviewsDTO.PositionId);
                var positionNameresult = positionName.Value;
                var lastPositionName = positionNameresult.Name;

                var StatusDTOs = await _StatusService.GetAll();
            ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");

            var validationErrors = new List<string>();

            if ((file == null || file.Length == 0) && User.IsInRole("Interviewer"))
            {
                ModelState.AddModelError("AttachmentId", "Please choose a file to upload.");
            }

                if (User.IsInRole("Interviewer") || User.IsInRole("General Manager") || User.IsInRole("Solution Architecture"))
                {
                    if (interviewsDTO.ActualExperience == null)
                    {
                        ModelState.AddModelError("ActualExperience", "Please add the actual experience.");
                    }
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
                        var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

                        if (await _userManager.IsInRoleAsync(currentUser, "General Manager"))
                        {
                            await _interviewsService.ConductInterviewForGm(interviewsDTO);

                        }
                        else if(await _userManager.IsInRoleAsync(currentUser, "Interviewer"))
                        {
                            var secondInterviewerId = HttpContext.Session.GetString($"SecondInterviewerId_{interviewsDTO.InterviewsId}");
                            var interviewerId = HttpContext.Session.GetString($"InterviewerId_{interviewsDTO.InterviewsId}");


                            await _interviewsService.ConductInterview(interviewsDTO, interviewerId, secondInterviewerId);
                        }
                        else
                        {
                            var firstinterviewer = await _userManager.FindByIdAsync(interviewsDTO.InterviewerId);

                            var secondInterviewerId = HttpContext.Session.GetString($"SecondInterviewerId_{interviewsDTO.InterviewsId}");
                            var secondInterviewer = await _userManager.FindByIdAsync(secondInterviewerId);
                            var interviewerId = HttpContext.Session.GetString($"InterviewerId_{interviewsDTO.InterviewsId}");

                            if (secondInterviewer !=null)
                            {
                                var isInterviewerGMCombo = await IsUserInRolesAsync(firstinterviewer.Id, secondInterviewer.Id, "Solution Architecture", "General Manager");
                                var isGMInterviewerCombo = await IsUserInRolesAsync(firstinterviewer.Id, secondInterviewer.Id, "General Manager", "Solution Architecture");
                            




                            if (isInterviewerGMCombo || isGMInterviewerCombo)
                            {
                                await _interviewsService.ConductInterviewForArchi(interviewsDTO);
                            }
                                else
                                {
                                    await _interviewsService.ConductInterview(interviewsDTO, interviewerId, secondInterviewerId);

                                }
                            }
                            else
                            {
                                await _interviewsService.ConductInterview(interviewsDTO, interviewerId, secondInterviewerId);

                            }
                        }

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

                                    var userGM = await _userManager.FindByEmailAsync(GMEmail);
                                    var userHR = await _userManager.FindByEmailAsync(HREmail);

                                    var firstinterviewer = await _userManager.FindByIdAsync(interviewsDTO.InterviewerId);

                                    var secondInterviewerId = HttpContext.Session.GetString($"SecondInterviewerId_{interviewsDTO.InterviewsId}");
                                    var secondInterviewer = await _userManager.FindByIdAsync(secondInterviewerId);

                                    if (secondInterviewer != null)
                                    {

                                   
                                    var isInterviewerGMCombo = await IsUserInRolesAsync(firstinterviewer.Id, secondInterviewer.Id, "Interviewer", "General Manager");
                                    var isGMInterviewerCombo = await IsUserInRolesAsync(firstinterviewer.Id, secondInterviewer.Id, "General Manager", "Interviewer");


                                    if (isInterviewerGMCombo || isGMInterviewerCombo)
                                    {
                                        await _notificationsService.CreateInterviewNotificationForFinalHRInterview(interviewsDTO.StatusId.Value, interviewsDTO.Notes, interviewsDTO.CandidateId, interviewsDTO.PositionId);
                                        EmailDTOs emailModels = new EmailDTOs
                                        {
                                            EmailTo = new List<string> { HREmail },
                                            Subject = "Final Interview Invitation",
                                            EmailBody = $@"<html>
                                       <body style='font-family: Arial, sans-serif;'>
                                           <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                               <p style='font-size: 18px; color: #333;'>
                                                   Dear {userHR},
                                               </p>
                                               <p style='font-size: 16px; color: #555;'>
                                               You are assigned to have a Final interview for {candidateNameresult} with {lastPositionName} position, kindly login to the system using the below link <a href='https://apps.sssprocess.com:6134/'>Click here</a>
                                               </p>
                                               <p style='font-size: 14px; color: #777;'>
                                                   Regards,
                                               </p>
                                           </div>
                                       </body>
                                    </html>"
                                        };

                                            EmailDTOs emailModelToHR = new EmailDTOs
                                            {
                                                EmailTo = new List<string> { HREmail },
                                                Subject = $"First Interview Approval",
                                                EmailBody = $@"<html>
                                       <body style='font-family: Arial, sans-serif;'>
                                           <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                               <p style='font-size: 18px; color: #333;'>
                                                   Dear HR,
                                               </p>
                                               <p style='font-size: 16px; color: #555;'>
                                                    The first interview with {candidateNameresult} Approved by {userName}
                                               </p>
                                               <p style='font-size: 14px; color: #777;'>
                                                   Regards,
                                               </p>
                                           </div>
                                       </body>
                                    </html>"
                                            };

                                            if (!string.IsNullOrEmpty(HREmail))
                                        {
                                            await SendEmailToInterviewer(HREmail, interviewsDTO, emailModels);
                                        }
                                            if (!string.IsNullOrEmpty(HREmail))
                                            {
                                                await SendEmailToInterviewer(HREmail, interviewsDTO, emailModelToHR);
                                            }
                                        }
                                        else
                                        {
                                            await _notificationsService.CreateNotificationForGeneralManagerAsync(interviewsDTO.StatusId.Value, interviewsDTO.Notes, interviewsDTO.CandidateId, interviewsDTO.PositionId);

                                            //from interviewer to GM
                                            EmailDTOs emailModel = new EmailDTOs
                                            {
                                                EmailTo = new List<string> { GMEmail },
                                                Subject = "Second Interview Invitation",
                                                EmailBody = $@"<html>
                                       <body style='font-family: Arial, sans-serif;'>
                                           <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                               <p style='font-size: 18px; color: #333;'>
                                                   Dear {userGM},
                                               </p>
                                               <p style='font-size: 16px; color: #555;'>
                                               You are assigned to have a second interview for {candidateNameresult} with {lastPositionName} position, kindly login to the system using the below link <a href='https://apps.sssprocess.com:6134/'>Click here</a>
                                               </p>
                                               <p style='font-size: 14px; color: #777;'>
                                                   Regards,
                                               </p>
                                           </div>
                                       </body>
                                    </html>"
                                            };


                                            EmailDTOs emailModelToHR = new EmailDTOs
                                            {
                                                EmailTo = new List<string> { HREmail },
                                                Subject = $"First Interview Approval",
                                                EmailBody = $@"<html>
                                       <body style='font-family: Arial, sans-serif;'>
                                           <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                               <p style='font-size: 18px; color: #333;'>
                                                   Dear HR,
                                               </p>
                                               <p style='font-size: 16px; color: #555;'>
                                                    The first interview with {candidateNameresult} Approved by {userName}
                                               </p>
                                               <p style='font-size: 14px; color: #777;'>
                                                   Regards,
                                               </p>
                                           </div>
                                       </body>
                                    </html>"
                                            };
                                            if (!string.IsNullOrEmpty(GMEmail))
                                            {
                                                await SendEmailToInterviewer(GMEmail, interviewsDTO, emailModel);
                                            }

                                            if (!string.IsNullOrEmpty(HREmail))
                                            {
                                                await SendEmailToInterviewer(HREmail, interviewsDTO, emailModelToHR);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        await _notificationsService.CreateNotificationForGeneralManagerAsync(interviewsDTO.StatusId.Value, interviewsDTO.Notes, interviewsDTO.CandidateId, interviewsDTO.PositionId);

                                        //from interviewer to GM
                                        EmailDTOs emailModel = new EmailDTOs
                                {
                                    EmailTo = new List<string> { GMEmail },
                                    Subject = "Second Interview Invitation",
                                    EmailBody = $@"<html>
                                       <body style='font-family: Arial, sans-serif;'>
                                           <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                               <p style='font-size: 18px; color: #333;'>
                                                   Dear {userGM},
                                               </p>
                                               <p style='font-size: 16px; color: #555;'>
                                               You are assigned to have a second interview for {candidateNameresult} with {lastPositionName} position, kindly login to the system using the below link <a href='https://apps.sssprocess.com:6134/'>Click here</a>
                                               </p>
                                               <p style='font-size: 14px; color: #777;'>
                                                   Regards,
                                               </p>
                                           </div>
                                       </body>
                                    </html>"
                                };


                                EmailDTOs emailModelToHR = new EmailDTOs
                                {
                                    EmailTo = new List<string> { HREmail },
                                    Subject = $"First Interview Approval",
                                    EmailBody = $@"<html>
                                       <body style='font-family: Arial, sans-serif;'>
                                           <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                               <p style='font-size: 18px; color: #333;'>
                                                   Dear HR,
                                               </p>
                                               <p style='font-size: 16px; color: #555;'>
                                                    The first interview with {candidateNameresult} Approved by {userName}
                                               </p>
                                               <p style='font-size: 14px; color: #777;'>
                                                   Regards,
                                               </p>
                                           </div>
                                       </body>
                                    </html>"
                                };


                                if (architectureInterviewerId != "" && status.Code == Domain.Enums.StatusCode.Approved)
                                {
                                        var userArchi = await _userManager.FindByEmailAsync(ArchiEmail);

                                        EmailDTOs architectureEmailModel = new EmailDTOs
                                    {
                                        EmailTo = new List<string> { ArchiEmail },
                                        Subject = "Second Interview Invitation",
                                        EmailBody = $@"<html>
                                           <body style='font-family: Arial, sans-serif;'>
                                               <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                                   <p style='font-size: 18px; color: #333;'>
                                                       Dear {userArchi},
                                                   </p>
                                                   <p style='font-size: 16px; color: #555;'>
                                                       An interview has been scheduled with Saeed, and you are assigned as the Architecture Interviewer for the {candidateNameresult} with position: {lastPositionName}. kindly login to the system using the below link.<a href='https://apps.sssprocess.com:6134/'>Click here</a>”
                                                   </p>
                                                   <p style='font-size: 14px; color: #777;'>
                                                       Regards,
                                                   </p>
                                               </div>
                                           </body>
                                        </html>"
                                       
                                    };
                                    if (!string.IsNullOrEmpty(ArchiEmail))
                                    {
                                            //Send an Email to the Archi if it was selceted
                                            await SendEmailToInterviewer(ArchiEmail, interviewsDTO, architectureEmailModel);
                                        }
                                    }

                                    //var reminderJobId = BackgroundJob.Schedule(() => ReminderJobAsync(GMEmail, interviewsDTO), TimeSpan.FromHours(hoursUntil3PM));

                                    if (!string.IsNullOrEmpty(GMEmail))
                                    {
                                        await SendEmailToInterviewer(GMEmail, interviewsDTO, emailModel);
                                    }

                                    if (!string.IsNullOrEmpty(HREmail))
                                    {
                                        await SendEmailToInterviewer(HREmail, interviewsDTO, emailModelToHR);
                                    }


                                    return RedirectToAction(nameof(MyInterviews));
                            }
                                }

                                else if (status.Code == Domain.Enums.StatusCode.Rejected)
                            {
                                string userName = GetLoggedInUserName();
                                var HREmail = await GetHREmail();

                                    EmailDTOs emailModel = new EmailDTOs
                                {
                                    EmailTo = new List<string> { HREmail },
                                    Subject = "First Interview Rejection",
                                    EmailBody = $@"<html>
                                       <body style='font-family: Arial, sans-serif;'>
                                           <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                               <p style='font-size: 18px; color: #333;'>
                                                   Dear HR,
                                               </p>
                                               <p style='font-size: 16px; color: #555;'>
                                                    The first interview with {candidateNameresult} Rejected by {userName}
                                               </p>
                                               <p style='font-size: 14px; color: #777;'>
                                                   Regards,
                                               </p>
                                           </div>
                                       </body>
                                    </html>"

                                };


                                    if (!string.IsNullOrEmpty(HREmail))
                                    {
                                        await SendEmailToInterviewer(HREmail, interviewsDTO, emailModel);
                                    }

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
                                    var userHR = await _userManager.FindByEmailAsync(HREmail);
                                    EmailDTOs emailModel = new EmailDTOs
                                {
                                    EmailTo = new List<string> { HREmail },
                                    Subject = "Thierd Interview Invitation",
                                    EmailBody = $@"<html>
                                           <body style='font-family: Arial, sans-serif;'>
                                               <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                                   <p style='font-size: 18px; color: #333;'>
                                                       Dear {userHR},
                                                   </p>
                                                   <p style='font-size: 16px; color: #555;'>
                                                       You are assigned to have a thierd interview for candidate: {candidateNameresult} with position: {lastPositionName}, kindly login to the system using the below link <a href='https://apps.sssprocess.com:6134/'>Click here</a>.
                                                   </p>
                                                   <p style='font-size: 14px; color: #777;'>
                                                       Regards,
                                                   </p>
                                               </div>
                                           </body>
                                        </html>"
                                };

                                EmailDTOs emailModelApproval = new EmailDTOs
                                {
                                    EmailTo = new List<string> { HREmail },
                                    Subject = "Second Interview Approval",
                                    EmailBody = $@"<html>
                                       <body style='font-family: Arial, sans-serif;'>
                                           <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                               <p style='font-size: 18px; color: #333;'>
                                                   Dear HR,
                                               </p>
                                               <p style='font-size: 16px; color: #555;'>
                                                    The Second interview with {candidateNameresult} Approved by {userName}
                                               </p>
                                               <p style='font-size: 14px; color: #777;'>
                                                   Regards,
                                               </p>
                                           </div>
                                       </body>
                                    </html>"
                                    
                                };

                                    //var reminderJobId = BackgroundJob.Schedule(() => ReminderJobAsync(HREmail, interviewsDTO), interviewsDTO.Date.AddHours(16));

                                    if (!string.IsNullOrEmpty(HREmail))
                                    {
                                        await SendEmailToInterviewer(HREmail, interviewsDTO, emailModel);
                                        await SendEmailToInterviewer(HREmail, interviewsDTO, emailModelApproval);

                                    }

                                    return RedirectToAction(nameof(MyInterviews));
                            }

                            else if (status.Code == Domain.Enums.StatusCode.Rejected)
                            {
                                string userName = GetLoggedInUserName();
                                var HREmail = await GetHREmail();
                                EmailDTOs emailModel = new EmailDTOs
                                {
                                    EmailTo = new List<string> { HREmail },
                                    Subject = "Second Interview Rejection",
                                    EmailBody = $@"<html>
                                       <body style='font-family: Arial, sans-serif;'>
                                           <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                               <p style='font-size: 18px; color: #333;'>
                                                   Dear HR,
                                               </p>
                                               <p style='font-size: 16px; color: #555;'>
                                                    The Second interview with {candidateNameresult} Rejected by {userName}
                                               </p>
                                               <p style='font-size: 14px; color: #777;'>
                                                   Regards,
                                               </p>
                                           </div>
                                       </body>
                                    </html>"
                                };
                                    if (!string.IsNullOrEmpty(HREmail))
                                    {
                                        await SendEmailToInterviewer(HREmail, interviewsDTO, emailModel);
                                    }

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
                                var firstinterviewer = await _userManager.FindByIdAsync(interviewsDTO.InterviewerId);

                                var secondInterviewerId = HttpContext.Session.GetString($"SecondInterviewerId_{interviewsDTO.InterviewsId}");
                                var secondInterviewer = await _userManager.FindByIdAsync(secondInterviewerId);

                               



                                if (status.Code == Domain.Enums.StatusCode.Approved)
                            {
                                string userName = GetLoggedInUserName();
                                var HREmail = await GetHREmail();
                                 var GMEmail = await GetGMEmail();


                                 var userHR = await _userManager.FindByEmailAsync(HREmail);
                                 var userGM = await _userManager.FindByEmailAsync(GMEmail);

                                    if (secondInterviewer !=null)
                                    {

                                    
                                    var isInterviewerGMCombo = await IsUserInRolesAsync(firstinterviewer.Id, secondInterviewer.Id, "Solution Architecture", "Interviewer");
                                    var isGMInterviewerCombo = await IsUserInRolesAsync(firstinterviewer.Id, secondInterviewer.Id, "Interviewer", "Solution Architecture");


                                    if (isInterviewerGMCombo || isGMInterviewerCombo)
                                    {

                                        await _notificationsService.CreateNotificationForGeneralManagerAsync(interviewsDTO.StatusId.Value, interviewsDTO.Notes, interviewsDTO.CandidateId, interviewsDTO.PositionId);

                                            EmailDTOs emailModels = new EmailDTOs
                                        {
                                            EmailTo = new List<string> { GMEmail },
                                            Subject = "Second Interview Invitation",
                                            EmailBody = $@"<html>
                                           <body style='font-family: Arial, sans-serif;'>
                                               <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                                   <p style='font-size: 18px; color: #333;'>
                                                       Dear {userGM},
                                                   </p>
                                                   <p style='font-size: 16px; color: #555;'>
                                                       You are assigned to have a Second interview for candidate : {candidateNameresult} with position: {lastPositionName}, kindly login to the system using the below link <a href='https://apps.sssprocess.com:6134/'>Click here</a>.
                                                   </p>
                                                   <p style='font-size: 14px; color: #777;'>
                                                       Regards,
                                                   </p>
                                               </div>
                                           </body>
                                        </html>"
                                        };
                                            EmailDTOs emailModelApproval = new EmailDTOs
                                            {
                                                EmailTo = new List<string> { HREmail },
                                                Subject = "First Interview Approval",
                                                EmailBody = $@"<html>
                                       <body style='font-family: Arial, sans-serif;'>
                                           <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                               <p style='font-size: 18px; color: #333;'>
                                                   Dear HR,
                                               </p>
                                               <p style='font-size: 16px; color: #555;'>
                                                    The First interview with {candidateNameresult} Approved by {userName}
                                               </p>
                                               <p style='font-size: 14px; color: #777;'>
                                                   Regards,
                                               </p>
                                           </div>
                                       </body>
                                    </html>"
                                            };
                                            if (!string.IsNullOrEmpty(GMEmail))
                                                {
                                                    await SendEmailToInterviewer(GMEmail, interviewsDTO, emailModels);
                                                }

                                            if (!string.IsNullOrEmpty(HREmail))
                                            {
                                                await SendEmailToInterviewer(HREmail, interviewsDTO, emailModelApproval);

                                            }

                                        }


                                        var isInterviewersolCombo = await IsUserInRolesAsync(firstinterviewer.Id, secondInterviewer.Id, "Solution Architecture", "General Manager");
                                    var isGMMInterviewerCombo = await IsUserInRolesAsync(firstinterviewer.Id, secondInterviewer.Id, "General Manager", "Solution Architecture");

                                    if (isInterviewersolCombo || isGMMInterviewerCombo)
                                    {
                                            await _notificationsService.CreateInterviewNotificationForHRInterview(interviewsDTO.StatusId.Value, interviewsDTO.Notes, interviewsDTO.CandidateId, interviewsDTO.PositionId);
                                            //from Archi to HR
                                            EmailDTOs emailModel = new EmailDTOs
                                        {
                                            EmailTo = new List<string> { HREmail },
                                            Subject = "Final Interview Invitation",
                                            EmailBody = $@"<html>
                                           <body style='font-family: Arial, sans-serif;'>
                                               <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                                   <p style='font-size: 18px; color: #333;'>
                                                       Dear {userHR},
                                                   </p>
                                                   <p style='font-size: 16px; color: #555;'>
                                                       You are assigned to have a Final interview for candidate : {candidateNameresult} with position: {lastPositionName}, kindly login to the system using the below link <a href='https://apps.sssprocess.com:6134/'>Click here</a>.
                                                   </p>
                                                   <p style='font-size: 14px; color: #777;'>
                                                       Regards,
                                                   </p>
                                               </div>
                                           </body>
                                        </html>"
                                        };

                                        EmailDTOs emailModelApproval = new EmailDTOs
                                        {
                                            EmailTo = new List<string> { HREmail },
                                            Subject = "First Interview Approval",
                                            EmailBody = $@"<html>
                                       <body style='font-family: Arial, sans-serif;'>
                                           <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                               <p style='font-size: 18px; color: #333;'>
                                                   Dear HR,
                                               </p>
                                               <p style='font-size: 16px; color: #555;'>
                                                    The First interview with {candidateNameresult} Approved by {userName}
                                               </p>
                                               <p style='font-size: 14px; color: #777;'>
                                                   Regards,
                                               </p>
                                           </div>
                                       </body>
                                    </html>"
                                        };

                                        if (!string.IsNullOrEmpty(HREmail))
                                        {
                                            await SendEmailToInterviewer(HREmail, interviewsDTO, emailModel);
                                            await SendEmailToInterviewer(HREmail, interviewsDTO, emailModelApproval);

                                        }
                                        }
                                       
                                    }
                                    else
                                    {

                                        if (secondInterviewer == null)
                                        {
                                            if (interviewsDTO.ArchitectureInterviewerId != null)
                                            {
                                                await _notificationsService.CreateNotificationForGeneralManagerAsync(interviewsDTO.StatusId.Value, interviewsDTO.Notes, interviewsDTO.CandidateId, interviewsDTO.PositionId);

                                                EmailDTOs emailModels = new EmailDTOs
                                                {
                                                    EmailTo = new List<string> { GMEmail },
                                                    Subject = "Second Interview Invitation",
                                                    EmailBody = $@"<html>
                                                       <body style='font-family: Arial, sans-serif;'>
                                                           <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                                               <p style='font-size: 18px; color: #333;'>
                                                                   Dear {userGM},
                                                               </p>
                                                               <p style='font-size: 16px; color: #555;'>
                                                                   You are assigned to have a Second interview for candidate : {candidateNameresult} with position: {lastPositionName}, kindly login to the system using the below link <a href='https://apps.sssprocess.com:6134/'>Click here</a>.
                                                               </p>
                                                               <p style='font-size: 14px; color: #777;'>
                                                                   Regards,
                                                               </p>
                                                           </div>
                                                       </body>
                                                    </html>"
                                                };
                                                if (!string.IsNullOrEmpty(GMEmail))
                                                {
                                                    await SendEmailToInterviewer(GMEmail, interviewsDTO, emailModels);
                                                }
                                            }
                                            else
                                            {
                                                await _notificationsService.CreateInterviewNotificationForHRInterview(interviewsDTO.StatusId.Value, interviewsDTO.Notes, interviewsDTO.CandidateId, interviewsDTO.PositionId);

                                                EmailDTOs emailModels = new EmailDTOs
                                                {
                                                    EmailTo = new List<string> { HREmail },
                                                    Subject = "Thierd Interview Invitation",
                                                    EmailBody = $@"<html>
                                                       <body style='font-family: Arial, sans-sexrif;'>
                                                           <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                                               <p style='font-size: 18px; color: #333;'>
                                                                   Dear {userHR},
                                                               </p>
                                                               <p style='font-size: 16px; color: #555;'>
                                                                   You are assigned to have a thierd interview for candidate : {candidateNameresult} with position: {lastPositionName}, kindly login to the system using the below link <a href='https://apps.sssprocess.com:6134/'>Click here</a>.
                                                               </p>
                                                               <p style='font-size: 14px; color: #777;'>
                                                                   Regards,
                                                               </p>
                                                           </div>
                                                       </body>
                                                    </html>"
                                                };
                                                if (!string.IsNullOrEmpty(HREmail))
                                                {
                                                    await SendEmailToInterviewer(HREmail, interviewsDTO, emailModels);
                                                }
                                            }

                                            
                                            
                                        }
                                        else
                                        {

                                        
                                        await _notificationsService.CreateInterviewNotificationForHRInterview(interviewsDTO.StatusId.Value, interviewsDTO.Notes, interviewsDTO.CandidateId, interviewsDTO.PositionId);
                                        //from Archi to HR
                                        EmailDTOs emailModel = new EmailDTOs
                                        {
                                            EmailTo = new List<string> { HREmail },
                                            Subject = "Final Interview Invitation",
                                            EmailBody = $@"<html>
                                           <body style='font-family: Arial, sans-serif;'>
                                               <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                                   <p style='font-size: 18px; color: #333;'>
                                                       Dear {userHR},
                                                   </p>
                                                   <p style='font-size: 16px; color: #555;'>
                                                       You are assigned to have a Final interview for candidate : {candidateNameresult} with position: {lastPositionName}, kindly login to the system using the below link <a href='https://apps.sssprocess.com:6134/'>Click here</a>.
                                                   </p>
                                                   <p style='font-size: 14px; color: #777;'>
                                                       Regards,
                                                   </p>
                                               </div>
                                           </body>
                                        </html>"
                                        };
                                        if (!string.IsNullOrEmpty(HREmail))
                                        {
                                            await SendEmailToInterviewer(HREmail, interviewsDTO, emailModel);

                                        }
                                        }

                                    }


                                    //var reminderJobId = BackgroundJob.Schedule(() => ReminderJobAsync(HREmail, interviewsDTO), interviewsDTO.Date.AddHours(16));


                                    return RedirectToAction(nameof(MyInterviews));
                            }

                            else if (status.Code == Domain.Enums.StatusCode.Rejected)
                            {
                                string userName = GetLoggedInUserName();
                                var HREmail = await GetHREmail();
                                EmailDTOs emailModel = new EmailDTOs
                                {
                                    EmailTo = new List<string> { HREmail },
                                    Subject = "Second Interview Rejection",
                                    EmailBody = $@"<html>
                                       <body style='font-family: Arial, sans-serif;'>
                                           <div style='background-color: #f5f5f5; padding: 20px; border-radius: 10px;'>
                                               <p style='font-size: 18px; color: #333;'>
                                                   Dear HR,
                                               </p>
                                               <p style='font-size: 16px; color: #555;'>
                                                    The Second interview with {candidateNameresult} rejected by {userName}
                                               </p>
                                               <p style='font-size: 14px; color: #777;'>
                                                   Regards,
                                               </p>
                                           </div>
                                       </body>
                                    </html>"
                                };
                                    if (!string.IsNullOrEmpty(HREmail))
                                    {
                                        await SendEmailToInterviewer(HREmail, interviewsDTO, emailModel);
                                    }

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
            catch (Exception ex)
            {
                LogException(nameof(UpdateAfterInterview), ex, "Faild to save a status");
                throw ex;
            }
        }


        public async Task<bool> IsUserInRolesAsync(string firstUserId, string secondUserId, string firstRole, string secondRole)
        {
            var firstUser = await _userManager.FindByIdAsync(firstUserId);
            var secondUser = await _userManager.FindByIdAsync(secondUserId);

            if (firstUser == null || secondUser == null)
            {
                // Handle the case when the user is not found
                return false;
            }

            var isFirstUserInRoles = await _userManager.IsInRoleAsync(firstUser, firstRole);
            var isSecondUserInRoles = await _userManager.IsInRoleAsync(secondUser, secondRole);

            return isFirstUserInRoles && isSecondUserInRoles;
        }

        public async Task SendEmailToInterviewer(string interviewerEmail, InterviewsDTO interview,EmailDTOs emailmodel)
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
                string UserName = "notifications@sss-process.org";
                string Password = "P@ssw0rd";
                smtp.Credentials = new NetworkCredential(UserName, Password);

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress("notifications@techprocess.net");

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
                throw ex;
            }
        }



        public async Task<string> GetInterviewerEmail(string interviewerId)
        {
            try
            {
                


                var email = await _interviewsRepository.GetInterviewerEmail(interviewerId );

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
        public async Task<string> GetArchiEmail()
        {
            try
            {
                


                var email = await _interviewsRepository.GetArchiEmail();

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
                LogException(nameof(GetHREmail), ex, "Faild to get archi email");
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



    }
}