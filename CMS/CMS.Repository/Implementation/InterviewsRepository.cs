// CMS.Infrastructure/Repositories/InterviewsRepository.cs
using CMS.Application.DTOs;
using CMS.Domain;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Repository.Repositories
{
    public class InterviewsRepository : IInterviewsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager; 
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InterviewsRepository(ApplicationDbContext context,
            UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async void LogException(string methodName, Exception ex, string additionalInfo)
        {
            //var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            //var userId = currentUser?.Id;
            _context.Logs.Add(new Log
            {
                MethodName = methodName,
                ExceptionMessage = ex.Message,
                StackTrace = ex.StackTrace,
                CreatedByUserId = null,
                LogTime = DateTime.Now,

                AdditionalInfo = additionalInfo
            });
            _context.SaveChanges();
        }



        public async Task<int> Delete(int id)
        {
            try
            {
                var interviews = await _context.Interviews.FindAsync(id)
;
                _context.Interviews.Remove(interviews);
                return await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                LogException(nameof(Delete), ex, $"Interview deleted with ID: {id}");
                throw ex;
            }
        }

        public async Task<List<Interviews>> GetAll()
        {
            try
            {

                return await _context.Interviews.
                      Include(c => c.Position)
                     .Include(c => c.Candidate)
                     .Include(c => c.Status)
                     .Include(c => c.Track)
                     .AsNoTracking()
                     .ToListAsync();


            }
            catch (Exception ex)
            {
                LogException(nameof(GetAll), ex,"Enable to get all the interviews");
                throw ex;
            }   
        }

        public async Task<Interviews> GetById(int id)
        {
             try
            {
                var interview = await _context.Interviews.FirstOrDefaultAsync(c => c.InterviewsId == id);
                return interview;
            }
            catch (Exception ex)
            {
                LogException(nameof(GetById), ex, $"Error retrieving interview with ID: {id}");
                throw ex;
            }
      
        }
        public async Task<string> GetRoleById(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);

                if (role != null)
                {
                    return role.Name;
                }

                return null; // Handle the case when the user is not found
            }
            catch (Exception ex)
            {
                LogException(nameof(GetRoleById), ex, $"Error retrieving role for user with ID: {id}");
                throw ex;
            }
        }


        public async Task<Interviews> GetByIdForEdit(int id)
        {

            try
            {
                var interview = await _context.Interviews
                     .Include(c => c.Position)
                     .Include(c => c.Candidate)
                     .Include(c => c.Track)
                     .Include(c => c.Status).AsNoTracking().FirstOrDefaultAsync(c => c.InterviewsId == id);
                return interview;
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByIdForEdit), ex, $"Error retrieving interview with ID: {id}");
                throw ex;
            }
        }
        public async Task<List<Interviews>> GetCurrentInterviews(string userId, int? companyFilter, int? trackFilter)
        {
            try
            {
                var interviewsQuery = _context.Interviews
                    .Include(c => c.Position)
                    .Include(c => c.Candidate)
                    .Include(c => c.Status)
                    .Include(c => c.Track)
                    .Where(c => c.InterviewerId == userId || c.SecondInterviewerId == userId)
                    .AsQueryable();

                if (companyFilter.HasValue)
                {
                    interviewsQuery = interviewsQuery.Where(c => c.Candidate.CompanyId == companyFilter.Value);
                }

                if (trackFilter.HasValue)
                {
                    interviewsQuery = interviewsQuery.Where(c => c.TrackId == trackFilter.Value);
                }

                return await interviewsQuery.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                LogException(nameof(GetCurrentInterviews), ex, $"Error retrieving current interviews with ID: {userId}");
                throw;
            }
        }
        public async Task<int> Insert(Interviews entity)
        {
            try
            {

                _context.Add(entity);
                return await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                LogException(nameof(Insert), ex, $"Interview inserted with ID: {entity.InterviewsId}");
                throw ex;
            }
        }

        public async Task<int> Update(Interviews entity)
        {
            try
            {


                _context.Update(entity);

                return await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                LogException(nameof(Update), ex, $"Error updating interview with ID: {entity.InterviewsId}");
                throw ex;
            }
        }



        public async Task<string> GetInterviewerEmail(string interviewerId)
        {
            try
            {
                var interviewer = await _context.Users.FindAsync(interviewerId);

                if (interviewer != null)
                {
                    return interviewer.Email;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(GetInterviewerEmail), ex, $"Error while getting Interviewer Email");
                throw ex;
            }
        }


        public async Task<string> GetGeneralManagerEmail()
        {
            try
            {
                var generalManagerRoleId = await _context.Roles
                    .Where(r => r.Name == "General Manager")
                    .Select(r => r.Id).FirstOrDefaultAsync();

                if (generalManagerRoleId != null)
                {
                    var generalManagerEmail = await _context.UserRoles
                        .Where(ur => ur.RoleId == generalManagerRoleId)
                        .Join(_context.Users, ur => ur.UserId, user => user.Id, (ur, user) => user.Email)
                        .FirstOrDefaultAsync();

                    return generalManagerEmail;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(GetGeneralManagerEmail), ex, $"Error while getting General Manager Email");
                throw ex;

            }
        }


        //Get HrManager Email
        public async Task<string> GetHREmail()
        {
            try
            {
                var hrRoleId = await _context.Roles
                    .Where(r => r.Name == "HR Manager")
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();

                if (hrRoleId != null)
                {
                    var hrManagerEmail = await _context.UserRoles
                        .Where(ur => ur.RoleId == hrRoleId)
                        .Join(_context.Users,
                            ur => ur.UserId,
                            user => user.Id,
                            (ur, user) => user.Email)
                        .FirstOrDefaultAsync();

                    return hrManagerEmail;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(GetHREmail), ex, $"Error while getting HR Email");
                throw ex;
            }
        }
        public async Task<string> GetArchiEmail()
        {
            try
            {
                var archiRoleId = await _context.Roles
                    .Where(r => r.Name == "Solution Architecture")
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();

                if (archiRoleId != null)
                {
                    var archiEmail = await _context.UserRoles
                        .Where(ur => ur.RoleId == archiRoleId)
                        .Join(_context.Users,
                            ur => ur.UserId,
                            user => user.Id,
                            (ur, user) => user.Email)
                        .FirstOrDefaultAsync();

                    return archiEmail;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(GetArchiEmail), ex, $"Error while getting Archi Email");
                throw ex;
            }
        }



        public async Task<bool> HasGivenStatusAsync(string interviewerId, int interviewId)
        {
            try
            {
                var interview = await _context.Interviews
                    .FirstOrDefaultAsync(c => c.InterviewsId == interviewId && c.InterviewerId == interviewerId);

                if (interview != null)
                {
                    return interview.Status.Code == StatusCode.Pending;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogException(nameof(HasGivenStatusAsync), ex, $"Error while having a status");
                throw ex;
            }
        }

        public async Task<Interviews> GetGeneralManagerInterviewForCandidate(int candidateId)
        {
            try
            {
                var generalManagerRoleId = (await _roleManager.FindByNameAsync("General Manager"))?.Id;

                if (!string.IsNullOrEmpty(generalManagerRoleId))
                {
                    var generalManagerId = (await _userManager.GetUsersInRoleAsync("General Manager")).FirstOrDefault()?.Id;

                    if (!string.IsNullOrEmpty(generalManagerId))
                    {
                        return await _context.Interviews
                            .Where(i => i.CandidateId == candidateId && i.InterviewerId == generalManagerId)
                            .FirstOrDefaultAsync();
                    }
                }

                return null; // Handle the case when role or user is not found
            }
            catch (Exception ex)
            {
                LogException(nameof(GetGeneralManagerInterviewForCandidate), ex, $"Error retrieving General Manager interview for candidate with ID: {candidateId}");
                throw ex;
            }
        }

        public async Task<Interviews> GetArchiInterviewForCandidate(int candidateId)
        {
            try
            {
                var archiRoleId = (await _roleManager.FindByNameAsync("Solution Architecture"))?.Id;

                if (!string.IsNullOrEmpty(archiRoleId))
                {
                    var archiId = (await _userManager.GetUsersInRoleAsync("Solution Architecture")).FirstOrDefault()?.Id;

                    if (!string.IsNullOrEmpty(archiId))
                    {
                        return await _context.Interviews
                            .Where(i => i.CandidateId == candidateId && i.InterviewerId == archiId)
                            .FirstOrDefaultAsync();
                    }
                }

                return null; 
            }
            catch (Exception ex)
            {
                LogException(nameof(GetGeneralManagerInterviewForCandidate), ex, $"Error retrieving General Manager interview for candidate with ID: {candidateId}");
                throw ex;
            }
        }

        public async Task<Interviews> GetinterviewerInterviewForCandidate(int candidateId)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

                var interviewerRoleId = (await _userManager.GetRolesAsync(currentUser)).FirstOrDefault();

                if (!string.IsNullOrEmpty(interviewerRoleId))
                {
                    var interviewerId = currentUser.Id;

                    return await _context.Interviews
                        .Where(i => i.CandidateId == candidateId && i.InterviewerId == interviewerId)
                        .FirstOrDefaultAsync();
                }

                return null;
            }
            catch (Exception ex)
            {
                LogException(nameof(GetinterviewerInterviewForCandidate), ex, $"Error retrieving Interviewer interview for candidate with ID: {candidateId}");
                throw ex;
            }
        }




        public async Task<int> GetInterviewCountForCandidate(int candidateId)
        {
            try
            {

            
            var interviewCount = await _context.Interviews
                .Where(i => i.CandidateId == candidateId)
                .CountAsync();

            return interviewCount;
            }
            catch (Exception ex) {
                LogException(nameof(GetInterviewCountForCandidate), ex, $"Error while counting interviews for candidate with ID: {candidateId}");
                throw ex;
            }
        }

        public async Task<bool> DeletePendingInterviews(int candidateId, int positionId, string userId)
        {
            try
            {

            
           
            var approvedInterviewsCreatedByUser = await _context.Interviews
                 .Where(i => i.CandidateId == candidateId && i.PositionId == positionId && i.Status.Code == Domain.Enums.StatusCode.Approved && (i.InterviewerId == userId || i.SecondInterviewerId == userId))
                 .Select(i => i.InterviewerId)
                 .Distinct()
                 .ToListAsync();

            // Exclude the interviews created by the current user
            var pendingInterviews = await _context.Interviews
                .Where(i => i.CandidateId == candidateId && i.PositionId == positionId && i.Status.Code == Domain.Enums.StatusCode.Pending )
                .ToListAsync();

            if (pendingInterviews.Count > 0)
            {
                // Delete pending interviews
                _context.Interviews.RemoveRange(pendingInterviews);

                // Delete associated notifications created by the user who approved the interview
                foreach (var createdByUser in approvedInterviewsCreatedByUser)
                {
                        var HrId = "";

                        var Hr = await _roleManager.FindByNameAsync("HR Manager");

                        HrId = (await _userManager.GetUsersInRoleAsync(Hr.Name)).FirstOrDefault().Id;


                        var notificationsToDelete = _context.Notifications
                      .Where(n => n.CreatedBy == createdByUser && n.ReceiverId != HrId && n.CandidateId == candidateId)
                      .ToList();

                        if (notificationsToDelete.Count > 0)
                    {
                        _context.Notifications.RemoveRange(notificationsToDelete);
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }

            return false;
            }

            catch (Exception ex) {
                LogException(nameof(DeletePendingInterviews), ex, $"Error while deleting Pending Interviews and Notification for candidate with ID: {candidateId}");
                throw ex;
            }
        }


        public async Task<string?> GetStatusOfNextInterview(int candidateId, int currentInterviewId)
        {
            var nextInterview = await _context.Interviews
                .Where(i => i.CandidateId == candidateId && i.InterviewsId > currentInterviewId)
                .OrderBy(i => i.InterviewsId)
                .FirstOrDefaultAsync();

            return nextInterview?.Status?.Code;
        }

        public InterviewsDTO GetInterviewByCandidateIdWithParentId(int candidateId)
        { 
            var intervew = _context.Interviews.Include(i=>i.Candidate)
                .Where(i=>i.ParentId == null && i.CandidateId == candidateId)
                .Select(i=> new InterviewsDTO
                {
                    InterviewsId = i.InterviewsId,
                    ArchitectureInterviewerId = i.ArchitectureInterviewerId,
                     CandidateId = i.CandidateId,

                }).FirstOrDefault();
        return intervew;
        
        }



        public async Task<bool> DeletePendingInterviewsforStopCycle(int candidateId, int positionId)
        {
            try
            {

                // Exclude the interviews created by the current user
                var pendingInterviews = await _context.Interviews
                    .Where(i => i.CandidateId == candidateId && i.PositionId == positionId && i.Status.Code == Domain.Enums.StatusCode.Pending)
                    .ToListAsync();

                if (pendingInterviews.Count > 0)
                {
                    // Delete pending interviews
                    _context.Interviews.RemoveRange(pendingInterviews);

                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }

            catch (Exception ex)
            {
                LogException(nameof(DeletePendingInterviewsforStopCycle), ex, $"Error while deleting Pending Interviews and Notification for candidate with ID: {candidateId}");
                throw ex;
            }
        }

        public async Task<Interviews> GetLastInterviewBeforePendingByCandidateId(int candidateId)
        {
            try
            {
                var pendingInterview = await _context.Interviews
                    .Where(i => i.CandidateId == candidateId && i.Status.Code == Domain.Enums.StatusCode.Pending)
                    .OrderByDescending(i => i.InterviewsId)
                    .FirstOrDefaultAsync();

                if (pendingInterview != null)
                {
                    var previousInterview = await _context.Interviews
                        .Where(i => i.CandidateId == candidateId && i.InterviewsId < pendingInterview.InterviewsId)
                        .OrderByDescending(i => i.InterviewsId)
                        .FirstOrDefaultAsync();

                    return previousInterview ?? pendingInterview;
                }

                return null; // No pending interview found
            }
            catch (Exception ex)
            {
                LogException(nameof(GetLastInterviewBeforePendingByCandidateId), ex, $"Error retrieving previous interview before pending for candidate with ID: {candidateId}");
                throw ex;
            }
        }


        public async Task<Interviews> GetByParentIdAsync(int candidateId)
        {
            try
            {
                return await _context.Interviews.FirstOrDefaultAsync(i => i.CandidateId == candidateId && i.ParentId == null);
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByParentIdAsync), ex, $"Error Getting ByParentIdAsync");
                throw ex;

            }
        }

        public async Task<Interviews> GetByInterviewerRoleAsync(int candidateId, string roleName)
        {
            try
            {
                // Find the user with the specified role
                var usersWithRole = await _userManager.GetUsersInRoleAsync(roleName);

                // Get the user IDs of users with the specified role
                var userIds = usersWithRole.Select(u => u.Id).ToList();

                // Retrieve the interview where the candidate is the interviewee
                // and the interviewer has the specified role
                var interview = await _context.Interviews
                    .Include(i => i.Interviewer)
                    .FirstOrDefaultAsync(i => i.CandidateId == candidateId &&
                                               userIds.Contains(i.InterviewerId));

                return interview;
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByInterviewerRoleAsync), ex, $"Error Getting ByInterviewerRoleAsync");
                throw ex;

            }

        }

        public async Task<Interviews> GetThirdInterviewAsync(int candidateId)
        {
            try
            {
                return await _context.Interviews
              .Where(i => i.CandidateId == candidateId && i.ActualExperience == null)
              .OrderByDescending(i => i.Date)
              .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                LogException(nameof(GetThirdInterviewAsync), ex, $"Error Getting Third Interview");
                throw ex;

            }

        }





    }
}
