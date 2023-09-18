// CMS.Infrastructure/Repositories/InterviewsRepository.cs
using CMS.Application.DTOs;
using CMS.Domain;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Repository.Repositories
{
    public class InterviewsRepository : IInterviewsRepository
    {
        private readonly ApplicationDbContext _context;
       
        public InterviewsRepository(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<int> Delete(int id)
        {
            try
            {
                var interviews = await _context.Interviews.FindAsync(id);
                _context.Interviews.Remove(interviews);
                return await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       

        public async Task<List<Interviews>> GetAll()
        {
            try
            {

                return await _context.Interviews
                    .Include(c => c.Position)
                    .Include(c=>c.Candidate)
                    .Include(c=>c.Status)
                    //.Include(c=>c.Interviewer)
                    .AsNoTracking().ToListAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Interviews> GetById(int id)
        {
            try
            {
                var interview = await _context.Interviews
                    .Include(c => c.Position)
                    .Include(c => c.Candidate)
                    .Include(c=>c.Status)
                   // .Include(c=>c.Interviewer)
                    .AsNoTracking().FirstOrDefaultAsync(c => c.InterviewsId == id);
                return interview;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<List<Interviews>> GetCurrentInterviews(string id)
        {
            try
            {
               return  _context.Interviews
                    .Include(c => c.Position)
                    .Include(c=>c.Candidate)
                    .Include(c=>c.Status)
                   // .Include(c=>c.Interviewer)
                    .Where(c=>c.InterviewerId == id)
                    .AsNoTracking() .ToListAsync();
                    

            }
            catch (Exception ex)
            {
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
                throw ex.InnerException;
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
                throw ex;
            }
        }

    }
}
