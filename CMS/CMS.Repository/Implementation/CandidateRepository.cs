
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
            return await _dbContext.Candidates
                .Include(c => c.Position)
                .Include(c=>c.Company)
                .AsNoTracking().ToListAsync();
        }

        public async Task<Candidate> GetCandidateByIdAsync(int id)
        {
            return await _dbContext.Candidates
                .Include(c => c.Position)
                .Include(c=>c.Company)
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

        public async Task<int> DeleteCandidateAsync(Candidate candidate)
        {
            _dbContext.Candidates.Remove(candidate);
            return await _dbContext.SaveChangesAsync();

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


//using CMS.Domain;
//using CMS.Domain.Entities;
//using CMS.Repository.Interfaces;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace CMS.Repository.Implementation
//{
//    public class CandidateRepository : ICandidateRepository
//    {
//        private readonly ApplicationDbContext _dbContext;

//        public CandidateRepository(ApplicationDbContext Context)
//        {
//            _dbContext = Context;
//        }

//        //public async Task<int> Delete(int id)
//        //{
//        //    try
//        //    {
//        //        // var country=await _context.Countries.FindAsync(id);
//        //        var country = await _context.Countries.Include(c => c.Companies)
//        //             .FirstOrDefaultAsync(c => c.Id == id);

//        //        foreach (var com in country.Companies)
//        //        {
//        //            _context.Companies.Remove(com);
//        //        }
//        //        _context.Countries.Remove(country);
//        //        return await _context.SaveChangesAsync();

//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        throw ex;
//        //    }
//        //}

//        //public async Task<List<Candidate>> GetAll()
//        //{
//        //    try
//        //    {

//        //        return await _context.Candidates.Include(c => c.Interviews).AsNoTracking().ToListAsync();

//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        throw ex;
//        //    }
//        //}

//        //public async Task<Candidate> GetById(int id)
//        //{
//        //    try
//        //    {
//        //        var candidate = await _context.Candidates
//        //            .Include(c => c.Interviews)
//        //            .AsNoTracking()
//        //            .FirstOrDefaultAsync(c => c.CandidateId == id);
//        //        return candidate;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        throw ex;
//        //    }

//        //}

//        //public async Task<int> Insert(Candidate entity)
//        //{
//        //    try
//        //    {
//        //        _context.Add(entity);
//        //        return await _context.SaveChangesAsync();

//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        throw ex;
//        //    }
//        //}

//        //public async Task<int> Update(Candidate entity)
//        //{
//        //    try
//        //    {

//        //        _context.Update(entity);
//        //        return await _context.SaveChangesAsync();


//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        throw ex;
//        //    }
//        //}


//        public async Task<IEnumerable<Candidate>> GetAllCandidatesAsync()
//        {
//            return await _dbContext.Candidates.Include(c=>c.Position).AsNoTracking().ToListAsync();
//        }

//        public async Task<Candidate> GetCandidateByIdAsync(int id)
//        {
//            return await _dbContext.Candidates.Include(c=>c.Position).AsNoTracking().FirstOrDefaultAsync(c=>c.Id==id);
//        }

//        public async Task CreateCandidateAsync(Candidate candidate)
//        {
//            candidate.IsActive = true;
//            candidate.ModifiedBy = candidate.ModifiedBy;
//            candidate.ModifiedOn = DateTime.Now;

//            _dbContext.Candidates.Add(candidate);
//            await _dbContext.SaveChangesAsync();
//        }

//        public async Task UpdateCandidateAsync(Candidate candidate)
//        {
//            candidate.IsActive = true;
//            candidate.ModifiedBy = candidate.ModifiedBy;
//            candidate.ModifiedOn = DateTime.Now;

//            _dbContext.Entry(candidate).State = EntityState.Modified;
//            await _dbContext.SaveChangesAsync();
//        }

//        public async Task<int> DeleteCandidateAsync(Candidate candidate)
//        {
//            candidate.IsDelete = true;
//            candidate.ModifiedBy = candidate.ModifiedBy;
//            candidate.ModifiedOn = DateTime.Now;


//            _dbContext.Candidates.Remove(candidate);
//           return await _dbContext.SaveChangesAsync();

//        }
//    }
//}
