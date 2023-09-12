using CMS.Domain;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
            return await _dbContext.Candidates.Include(c=>c.Position).AsNoTracking().ToListAsync();
        }

        public async Task<Candidate> GetCandidateByIdAsync(int id)
        {
            return await _dbContext.Candidates.Include(c=>c.Position).AsNoTracking().FirstOrDefaultAsync(c=>c.Id==id);
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

        public async Task<int> DeleteCandidateAsync(Candidate candidate)
        {
            _dbContext.Candidates.Remove(candidate);
           return await _dbContext.SaveChangesAsync();

        }
    }
}
