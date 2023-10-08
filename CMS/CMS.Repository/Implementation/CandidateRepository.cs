
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
        public async Task<IEnumerable<Candidate>> GetAllCandidatesAsync()
        {
            return await _dbContext.Candidates
                .Include(c => c.Position)
                .Include(c=>c.Company)
                .Include(c => c.Country)
                .AsNoTracking().ToListAsync();
        }

        public async Task<Candidate> GetCandidateByIdAsync(int id)
        {
            return await _dbContext.Candidates
                .Include(c => c.Position)
                .Include(c=>c.Company)
                 .Include(c => c.Country)
                .AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task CreateCandidateAsync(Candidate candidate)
        {
            _dbContext.Candidates.Add(candidate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCandidateAsync(Candidate candidate)
        {
            _dbContext.Entry(candidate).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        //public async Task<int> DeleteCandidateAsync(int id) {
        //    try
        //    {

        //        var candidates = await _dbContext.Candidates.Include(c => c.Interviews)
        //    .FirstOrDefaultAsync(c => c.Id == id);

        //        if (candidates.Interviews != null && candidates.Interviews.Any())
        //        {
        //            foreach (var c in candidates.Interviews.ToList())
        //            {
        //                _dbContext.Interviews.Remove(c);
        //            }
        //        }

        //        _dbContext.Candidates.Remove(candidates);
        //        return await _dbContext.SaveChangesAsync();

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}



        public async Task DeleteCandidateAsync(Candidate candidate)
        {


            _dbContext.Candidates.Remove(candidate);
            await _dbContext.SaveChangesAsync();

        }
        public async Task<int> CountAllAsync()
        {
            return await _dbContext.Candidates.CountAsync();
        }
        public async Task<int> CountAcceptedAsync()
        {
            int candidateCounts = await _dbContext.Candidates
            .Where(candidate => !candidate.Interviews.Any(interview => interview.Status.Code == StatusCode.Rejected) && candidate.Interviews.Any(interview => interview.Status.Code == StatusCode.Approved))
            .CountAsync();
            return candidateCounts;
        }
        public async Task<int> CountRejectedAsync()
        {
            int candidateCounts = await _dbContext.Candidates
            .Where(candidate => candidate.Interviews.Any(interview => interview.Status.Code == StatusCode.Rejected))
            .CountAsync();
            return candidateCounts;
        }
        public async Task<Dictionary<string, int>> CountCandidatesPerCountry()
        {
            var candidateCounts = await _dbContext.Candidates
                .GroupBy(c => c.Country.Name) 
                .Select(g => new { CountryName = g.Key, Count = g.Count() })
                .ToListAsync();
            var result = candidateCounts.ToDictionary(x => x.CountryName, x => x.Count);

            return result;
        }
    }
}
