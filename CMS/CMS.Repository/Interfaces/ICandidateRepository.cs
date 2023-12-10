using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository.Interfaces
{
    public interface ICandidateRepository
    {


        Task<IEnumerable<Candidate>> GetAllCandidatesAsync();
        Task<Candidate> GetCandidateByIdAsync(int id, string userId);
        Task CreateCandidateAsync(Candidate candidate,string userId);
        Task UpdateCandidateAsync(Candidate candidate, string userId);

        //Task<int> DeleteCandidateAsync(int id);
        Task DeleteCandidateAsync(Candidate candidate, string userId);
        Task<int> CountAllAsync();
        Task<int> CountAcceptedAsync();
        Task<int> CountPendingAsync();
        Task<int> CountRejectedAsync();

        Task<int?> GetCVAttachmentIdByCandidateId(int candidateId);

        void LogException(string methodName, Exception ex, string createdByUserId, string additionalInfo);


        //Task<Dictionary<string, int>> CountCandidatesPerCompanyAsync();
    }
}
