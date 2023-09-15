using CMS.Domain;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return await _dbContext.Candidates.ToListAsync();
        }

        public async Task<Candidate> GetCandidateByIdAsync(int id)
        {
            return await _dbContext.Candidates.FindAsync(id);
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
            return await _dbContext.Candidates
                .Where(c => c.Interviews.FirstOrDefault().Status == Domain.Enums.InterviewStatus.Accepted)
                .CountAsync();
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
        //private string NormalizeCountryName(string input)
        //{
        //    if (string.IsNullOrWhiteSpace(input))
        //    {
        //        return input; // Return as-is if input is null, empty, or whitespace
        //    }

        //    string[] words = input.ToLower().Split(' '); // Convert to lowercase and split into words
        //    for (int i = 0; i < words.Length; i++)
        //    {
        //        if (!string.IsNullOrWhiteSpace(words[i]))
        //        {
        //            words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1); // Capitalize first character
        //        }
        //    }

        //    return string.Join(" ", words);
        //}
    }
}
