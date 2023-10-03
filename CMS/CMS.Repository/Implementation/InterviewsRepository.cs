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
                var interviews = await _context.Interviews.FindAsync(id)
;
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

                return await _context.Interviews.ToListAsync();


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
                var interview = await _context.Interviews.FirstOrDefaultAsync(c => c.InterviewsId == id);
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
                return _context.Interviews
                     .Include(c => c.Position)
                     .Include(c => c.Candidate)
                     .Include(c => c.Status)
                     // .Include(c=>c.Interviewer)
                     .Where(c => c.InterviewerId == id)
                     .AsNoTracking().ToListAsync();


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


        public async Task<string> GetInterviewerEmail(string interviewerId)
        {
            try
            {
                var interviewer = await _context.Users.FindAsync(interviewerId);

                if (interviewer != null)
                {
                    return interviewer.Email;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here if needed
                return null;
            }
        }


        public async Task<string> GetGeneralManagerEmail()
        {
            try
            {
                var generalManagerRoleId = await _context.Roles
                    .Where(r => r.Name == "General Manager")
                    .Select(r => r.Id).FirstOrDefaultAsync();

                if(generalManagerRoleId != null)
                {
                    var generalManagerEmail = await _context.UserRoles
                        .Where(ur => ur.RoleId == generalManagerRoleId)
                        .Join(_context.Users, ur => ur.UserId,user => user.Id,(ur, user) => user.Email)
                        .FirstOrDefaultAsync(); 
                    
                    return generalManagerEmail;
                }
                else
                {
                    return null; 
                }
            }
            catch (Exception ex)
            {
                return null;

            }
        }


        //Get HrManager Email
        public async Task<string> GetHREmail()
        {
            try
            {
                var hrRoleId = await _context.Roles
                    .Where(r => r.Name == "HR Manager")
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();

                if (hrRoleId != null)
                {
                    var hrManagerEmail = await _context.UserRoles
                        .Where(ur => ur.RoleId == hrRoleId)
                        .Join(_context.Users,
                            ur => ur.UserId,
                            user => user.Id,
                            (ur, user) => user.Email)
                        .FirstOrDefaultAsync();

                    return hrManagerEmail;
                }
                else
                {
                    return null; 
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }






    }
}
