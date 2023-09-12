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
        private readonly ApplicationDbContext Db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InterviewsRepository(ApplicationDbContext _db,IHttpContextAccessor httpContextAccessor)
        {
            Db = _db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Interviews>> GetAllInterviews()
        {
            return await Db.Interviews.Include(c=>c.Position).Include(c=>c.Candidate).Include(c=>c.Status).AsNoTracking().ToListAsync();
        }

        public async Task<Interviews> GetInterviewById(int interviewId)
        {
            //  return await Db.Interviews.FindAsync(interviewId);
            return await Db.Interviews.Include(c => c.Position).Include(c => c.Candidate).Include(c=>c.Status).AsNoTracking().FirstOrDefaultAsync(c=>c.InterviewsId== interviewId);
        }

        public async Task Create(Interviews entity)
      
        {
            try
            {
                entity.IsActive = true;
               // entity.CreatId =_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
                entity.CreatDate = DateTime.Now;

            Db.Interviews.Add(entity);
            await Db.SaveChangesAsync();
            }catch (Exception ex)
            {
                throw ex.InnerException;
            }

           }

        public async Task Update(Interviews entity)
        {
            entity.IsActive = true;
            entity.EditId = entity.EditId;
            entity.EditDate = DateTime.Now;

            Db.Interviews.Update(entity);
            await Db.SaveChangesAsync();
        }

        public async Task Delete(Interviews entity)
        {
            entity.IsDelete = true;
            entity.EditId = entity.EditId;
            entity.EditDate = DateTime.Now;

            Db.Interviews.Remove(entity);
            await Db.SaveChangesAsync();
        }
    }
}
