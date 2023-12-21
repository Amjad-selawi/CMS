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
                     .Include(c => c.Status).AsNoTracking().FirstOrDefaultAsync(c => c.InterviewsId == id);
                return interview;
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByIdForEdit), ex, $"Error retrieving interview with ID: {id}");
                throw ex;
            }
        }
        public Task<List<Interviews>> GetCurrentInterviews(string id)
        {
            try
            {
                return _context.Interviews
                     .Include(c => c.Position)
                     .Include(c => c.Candidate)
                     .Include(c => c.Status)
                     // .Include(c=>c.Interviewer)
                     .Where(c => c.InterviewerId == id || c.SecondInterviewerId == id)
                     .AsNoTracking().ToListAsync();


            }
            catch (Exception ex)
            {
                LogException(nameof(GetCurrentInterviews), ex, $"Error retrieving Current Interviews with ID: {id}") ;
                throw ex;
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


        public async Task<string> GetRoleEmail(string roleName)
        {
            try
            {
                var roleId = await _context.Roles
                    .Where(r => r.Name == roleName)
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();

                if (roleId != null)
                {
                    var userEmail = await _context.UserRoles
                        .Where(ur => ur.RoleId == roleId)
                        .Join(_context.Users,
                            ur => ur.UserId,
                            user => user.Id,
                            (ur, user) => user.Email)
                        .FirstOrDefaultAsync();

                    return userEmail;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(GetRoleEmail), ex, $"Error while getting {roleName} Email");
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
                var archiRoleId = (await _roleManager.FindByNameAsync("Interviewer"))?.Id;

                if (!string.IsNullOrEmpty(archiRoleId))
                {
                    var archiId = (await _userManager.GetUsersInRoleAsync("Interviewer")).FirstOrDefault()?.Id;

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

            
            // Assuming you have an "Interviews" DbSet in your DbContext
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
                       .Where(n =>  n.CreatedBy == createdByUser && n.ReceiverId != HrId)
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





    }
}
