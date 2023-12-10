
using CMS.Domain;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CMS.Repository.Implementation
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CandidateRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void LogException(string methodName, Exception ex, string createdByUserId, string additionalInfo)
        {

            _dbContext.Logs.Add(new Log
            {
                MethodName = methodName,
                ExceptionMessage = ex.Message,
                StackTrace = ex.StackTrace,
                LogTime = DateTime.Now,
                CreatedByUserId = createdByUserId,
                AdditionalInfo = additionalInfo
            });
            _dbContext.SaveChanges();
        }




        public async Task<IEnumerable<Candidate>> GetAllCandidatesAsync()
        {
            try
            {

            
            return await _dbContext.Candidates
                .Include(c => c.Position)
                .Include(c => c.Company)
                .Include(c => c.Country)
                .AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                LogException(nameof(GetAllCandidatesAsync), ex,null, null );
                throw ex;
            }
        }

        public async Task<Candidate> GetCandidateByIdAsync(int id, string createdByUserId)
        {
            try
            {

           
            return await _dbContext.Candidates
                .Include(c => c.Position)
                .Include(c => c.Company)
                 .Include(c => c.Country)
                .AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                LogException(nameof(GetCandidateByIdAsync), ex, $"Done by : {createdByUserId}", $"Error retrieving candidate with ID: {id}");
                throw ex;
            }
        }

        public async Task CreateCandidateAsync(Candidate candidate, string createdByUserId)
        {
            try
            {

           
            _dbContext.Candidates.Add(candidate);
            await _dbContext.SaveChangesAsync();
 }
             catch (Exception ex)
            {
                LogException(nameof(CreateCandidateAsync), ex, $"Created by : {createdByUserId}", $"Candiate inserted with ID: {candidate.Id}");
                throw ex;
            }
        }

        public async Task UpdateCandidateAsync(Candidate candidate,string modifiedByUserId)
        {
            try
            {

            _dbContext.Entry(candidate).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                LogException(nameof(UpdateCandidateAsync), ex, $"Modified by : {modifiedByUserId}", $"Error updating candiate with ID: {candidate.Id}");
                throw ex;
            }
        }


        public async Task DeleteCandidateAsync(Candidate candidate, string deletedByUserId)
        {

            try
            {

            _dbContext.Candidates.Remove(candidate);
            await _dbContext.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                LogException(nameof(DeleteCandidateAsync), null, $"Deleted by : {deletedByUserId}", $"Candidate deleted with ID: {candidate.Id}");
                throw ex;
            }
        }
        public async Task<int> CountAllAsync()
        {
            try
            {

            return await _dbContext.Candidates.CountAsync();
            }

            catch (Exception ex)
            {
                LogException(nameof(CountAllAsync), ex,null,null);
                throw ex;
            }

        }
    

        public async Task<int> CountAcceptedAsync()
        {
            try
            {

            int candidateCounts = await _dbContext.Candidates
             .Include(a => a.Interviews)
             .ThenInclude(a => a.Status)
             .Where(a => (a.Interviews.Count == 3 || a.Interviews.Count == 4) && a.Interviews.All(a => a.Status.Code == StatusCode.Approved))
             .CountAsync();

            return candidateCounts;
            }

            catch (Exception ex)
            {
                LogException(nameof(CountAcceptedAsync), ex, null, null);
                throw ex;
            }

        }
        public async Task<int> CountRejectedAsync()
        {
            try
            {

            int rejectedCount = await _dbContext.Candidates
              .Include(a => a.Interviews)
              .ThenInclude(a => a.Status)
              .Where(a => a.Interviews.Count > 0 && a.Interviews.Any(a => a.Status.Code == StatusCode.Rejected))
              .CountAsync();

            return rejectedCount;
            }

            catch (Exception ex)
            {
                LogException(nameof(CountRejectedAsync), ex, null, null);
                throw ex;
            }
        }
        public async Task<int> CountPendingAsync()
        {
            try
            {

            int candidateCounts = await _dbContext.Candidates
        .Include(a => a.Interviews)
        .ThenInclude(a => a.Status)
        .Where(candidate =>
            candidate.Interviews.Any(interview => interview.Status.Code == StatusCode.Pending) ||
           candidate.Interviews.All(interview => interview.Status.Code != StatusCode.Rejected) &&
           candidate.Interviews.All(interview => interview.Status.Code != StatusCode.Approved))
        .CountAsync();

            return candidateCounts;
            }

            catch (Exception ex)
            {
                LogException(nameof(CountPendingAsync), ex, null, null);
                throw ex;
            }

        }


        public async Task<int?> GetCVAttachmentIdByCandidateId(int candidateId)
        {
            try
            {
                var candidate = await _dbContext.Candidates
                    .Include(c => c.CV)
                    .FirstOrDefaultAsync(c => c.Id == candidateId);

                if (candidate != null && candidate.CV != null)
                {
                    return candidate.CV.Id;
                }

                return null;
            }
            catch (Exception ex)
            {
                LogException(nameof(GetCVAttachmentIdByCandidateId), ex, null, null);
                throw ex;
            }
        }


    }
}
