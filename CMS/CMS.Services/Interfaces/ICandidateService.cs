using CMS.Application.DTOs;
using CMS.Application.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Interfaces
{
    public interface ICandidateService
    {
        Task<IEnumerable<CandidateDTO>> GetAllCandidatesAsync();
        Task<CandidateDTO> GetCandidateByIdAsync(int id);
        Task CreateCandidateAsync(CandidateCreateDTO candidateDTO);
        Task UpdateCandidateAsync(int id, CandidateDTO candidateDTO);
        Task UpdateCandidateCVAsync(int id, string fileName, long fileSize, Stream fileStream);
        //Task<Result<CandidateDTO>> DeleteCandidateAsync(int id);
        Task DeleteCandidateAsync(int id);

        Task<int?> GetCVAttachmentIdByCandidateId(int candidateId);

        void LogException(string methodName, Exception ex, string createdByUserId = null, string additionalInfo = null);

        string GetUserId();
    }
}
