using CMS.Domain.Entities;
using CMS.Domain;
using CMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository.Implementation
{
    public class InterviewerRepository : IInterviewerRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public InterviewerRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Interviewer>> GetAllInterviewersAsync()
        {
            return await _dbContext.Interviewers.ToListAsync();
        }

        public async Task<Interviewer> GetInterviewerByIdAsync(int id)
        {
            return await _dbContext.Interviewers.FindAsync(id);
        }

        public async Task CreateInterviewersAsync(Interviewer interviewer)
        {
            _dbContext.Interviewers.Add(interviewer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateInterviewerAsync(Interviewer interviewer)
        {
            _dbContext.Entry(interviewer).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteInterviewerAsync(Interviewer interviewer)
        {
            _dbContext.Interviewers.Remove(interviewer);
            await _dbContext.SaveChangesAsync();

        }
    }
}
