// CMS.Domain.Interfaces/IInterviewsRepository.cs
using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.Repository.Interfaces
{
    public interface IInterviewsRepository
    {
        Task<int> Insert(Interviews entity, string UserId);
        Task<int> Update(Interviews entity, string UserId);
        Task<int> Delete(int id, string UserId);
        Task<Interviews> GetById(int id, string UserId);
        Task<Interviews> GetByIdForEdit(int id, string UserId);
        Task<List<Interviews>> GetAll();
        
        //Task<bool> HasGivenScoreAsync(string interviewerId, int interviewId);
        Task<bool> HasGivenStatusAsync(string interviewerId, int interviewId, string UserId);
        Task<List<Interviews>> GetCurrentInterviews(string id, string UserId);

        Task<string> GetInterviewerEmail(string interviewerId, string UserId);
        Task<string> GetGeneralManagerEmail(string UserId);

        Task<string> GetHREmail( string UserId);
        Task<string> GetArchiEmail( string UserId);
        void LogException(string methodName, Exception ex, string createdByUserId, string additionalInfo);


    }
}
