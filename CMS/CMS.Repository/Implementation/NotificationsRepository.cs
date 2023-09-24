using CMS.Domain.Entities;
using CMS.Domain;
using CMS.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace CMS.Repository.Implementation
{
    public class NotificationsRepository : INotificationsRepository
    {

        private readonly ApplicationDbContext Db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificationsRepository(ApplicationDbContext _db, RoleManager<IdentityRole> roleManager,
             UserManager<IdentityUser> userManager)
        {
            Db = _db;
            _roleManager = roleManager;
            _userManager = userManager;
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



        public async Task<List<Notifications>> GetSpacificNotificationsforHR()
        {
            var HrId = "";

            var Hr = await _roleManager.FindByNameAsync("HR");

            HrId = (await _userManager.GetUsersInRoleAsync(Hr.Name)).FirstOrDefault().Id;


            return await Db.Notifications.Where(x=>x.ReceiverId == HrId).ToListAsync(); //hrId
        }

        public async Task<List<Notifications>> GetSpacificNotificationsforGeneral()
        {
            var managerId = "";

            var manager = await _roleManager.FindByNameAsync("General Manger");

            managerId = (await _userManager.GetUsersInRoleAsync(manager.Name)).FirstOrDefault().Id;


            return await Db.Notifications.Where(x => x.ReceiverId == managerId).ToListAsync();//GMid
        }


        public async Task<List<Notifications>> GetSpacificNotificationsforInterviewer()
        {
            var interviewerId = "";

            var interviewer = await _roleManager.FindByNameAsync("Interviewer");

            interviewerId = (await _userManager.GetUsersInRoleAsync(interviewer.Name)).FirstOrDefault().Id;

            return await Db.Notifications.Where(x => x.ReceiverId == interviewerId).ToListAsync();//InterviewerId
        }

    }
}
