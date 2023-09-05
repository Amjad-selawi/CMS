// CMS.Infrastructure/Repositories/InterviewsRepository.cs
using CMS.Domain;
using CMS.Domain.Entities;

using CMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Repository.Repositories
{
    public class InterviewsRepository : IInterviewsRepository
    {
        private readonly ApplicationDbContext Db;

        public InterviewsRepository(ApplicationDbContext _db)
        {
            Db = _db;
        }

        public async Task<IEnumerable<Interviews>> GetAllInterviews()
        {
            return await Db.Interviews.ToListAsync();
        }

        public async Task<Interviews> GetInterviewById(int interviewId)
        {
            return await Db.Interviews.FindAsync(interviewId);
        }

        public async Task Create(Interviews entity)
        {
            entity.IsActive = true;
            entity.EditId = entity.EditId;
            entity.EditDate = DateTime.Now;

            Db.Interviews.Add(entity);
            await Db.SaveChangesAsync();
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
