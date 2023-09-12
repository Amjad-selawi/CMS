// CMS.Infrastructure/Repositories/InterviewsRepository.cs
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
                var careerOffer = await _context.CarrerOffers.FindAsync(id);
                _context.CarrerOffers.Remove(careerOffer);
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

                return await _context.Interviews.Include(c => c.Position).Include(c=>c.Candidate).AsNoTracking().ToListAsync();



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
                var interview = await _context.Interviews.Include(c => c.Position).Include(c => c.Candidate).AsNoTracking().FirstOrDefaultAsync(c => c.InterviewsId == id);
                return interview;
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
                throw ex;
            }
        }

        //public async Task<IEnumerable<Interviews>> GetAllInterviews()
        //{
        //    return await Db.Interviews.ToListAsync();
        //}

        //public async Task<Interviews> GetInterviewById(int interviewId)
        //{
        //    return await Db.Interviews.FindAsync(interviewId);
        //}

        //public async Task Create(Interviews entity)
        //{
        //    entity.IsActive = true;
        //    entity.ModifiedBy = entity.ModifiedBy;
        //    entity.ModifiedOn = DateTime.Now;

        //    Db.Interviews.Add(entity);
        //    await Db.SaveChangesAsync();
        //}

        //public async Task Update(Interviews entity)
        //{
        //    entity.IsActive = true;
        //    entity.ModifiedBy = entity.ModifiedBy;
        //    entity.ModifiedOn = DateTime.Now;

        //    Db.Interviews.Update(entity);
        //    await Db.SaveChangesAsync();
        //}

        //public async Task Delete(Interviews entity)
        //{
        //    entity.IsDelete = true;
        //    entity.ModifiedBy = entity.ModifiedBy;
        //    entity.ModifiedOn = DateTime.Now;

        //    Db.Interviews.Remove(entity);
        //    await Db.SaveChangesAsync();
        //}
    }
}
