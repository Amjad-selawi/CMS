using CMS.Domain.Entities;
using CMS.Domain;
using CMS.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CMS.Repository.Implementation
{
    public class NotificationsRepository : INotificationsRepository
    {

        private readonly ApplicationDbContext Db;

        public NotificationsRepository(ApplicationDbContext _db)
        {
            Db = _db;
        }

        public async Task<IEnumerable<Notifications>> GetAllNotifications()
        {
            return await Db.Notifications.ToListAsync();
        }

        public async Task<Notifications> GetNotificationsById(int interviewId)
        {
            return await Db.Notifications.FindAsync(interviewId);
        }

        public async Task Create(Notifications entity)
        {
            entity.IsActive = true;
            entity.ModifiedBy = entity.ModifiedBy;
            entity.ModifiedOn = DateTime.Now;

            Db.Notifications.Add(entity);
            await Db.SaveChangesAsync();
        }

        public async Task Update(Notifications entity)
        {
            entity.IsActive = true;
            entity.ModifiedBy = entity.ModifiedBy;
            entity.ModifiedOn = DateTime.Now;

            Db.Notifications.Update(entity);
            await Db.SaveChangesAsync();
        }

        public async Task Delete(Notifications entity)
        {
            entity.IsDelete = true;
            entity.ModifiedBy = entity.ModifiedBy;
            entity.ModifiedOn = DateTime.Now;

            Db.Notifications.Remove(entity);
            await Db.SaveChangesAsync();
        }

    }
}
