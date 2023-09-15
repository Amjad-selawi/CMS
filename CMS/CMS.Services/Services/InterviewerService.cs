using CMS.Application.DTOs;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Services.Services
{
    public class InterviewerService : IInterviewerService
    {
        private readonly IInterviewerRepository _interviewerRepository;

        public InterviewerService(IInterviewerRepository interviewerRepository)
        {
            _interviewerRepository = interviewerRepository;
        }

        public async Task<IEnumerable<InterviewerDTOs>> GetAllInterviewersAsync()
        {
            var interviewers = await _interviewerRepository.GetAllInterviewersAsync();
            return interviewers.Select(interviewer => new InterviewerDTOs
            {
                Id = interviewer.Id,
                FullName = interviewer.FullName,
                Phone = interviewer.Phone,
                Email = interviewer.Email
            });
        }

        public async Task<InterviewerDTOs> GetInterviewerByIdAsync(int id)
        {
            var interviewer = await _interviewerRepository.GetInterviewerByIdAsync(id);
            if (interviewer == null)
                return null;

            return new InterviewerDTOs
            {
                Id = interviewer.Id,
                FullName = interviewer.FullName,
                Phone = interviewer.Phone,
                Email = interviewer.Email
            };
        }

        public async Task CreateInterviewerAsync(InterviewerDTOs interviewerDTOs)
        {
            var interviewer = new Interviewer
            {
                FullName = interviewerDTOs.FullName,
                Phone = interviewerDTOs.Phone,
                Email = interviewerDTOs.Email
            };
            await _interviewerRepository.CreateInterviewersAsync(interviewer);
        }

        public async Task UpdateInterviewerAsync(int id, InterviewerDTOs interviewerDTOs)
        {
            var existingInterviewer = await _interviewerRepository.GetInterviewerByIdAsync(id);
            if (existingInterviewer == null)
                throw new Exception("Interviewer not found");

            existingInterviewer.FullName = interviewerDTOs.FullName;
            existingInterviewer.Phone = interviewerDTOs.Phone;
            existingInterviewer.Email = interviewerDTOs.Email;

            await _interviewerRepository.UpdateInterviewerAsync(existingInterviewer);
        }

        public async Task DeleteInterviewerAsync(int id)
        {
            var interviewer = await _interviewerRepository.GetInterviewerByIdAsync(id);
            if (interviewer != null)
                await _interviewerRepository.DeleteInterviewerAsync(interviewer);
        }
    }
}

