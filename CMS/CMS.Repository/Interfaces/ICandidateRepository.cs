﻿using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository.Interfaces
{
    public interface ICandidateRepository
    {
        Task<IEnumerable<Candidate>> GetAllCandidatesAsync();
        Task<Candidate> GetCandidateByIdAsync(int id);
        Task CreateCandidateAsync(Candidate candidate);
        Task UpdateCandidateAsync(Candidate candidate);
        Task DeleteCandidateAsync(Candidate candidate);
    }
}