using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Services
{
    public class StatusService : IStatusService
    {
        IStatusRepository _repository;
        public StatusService(IStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<StatusDTO>>> GetAll()
        {
            var statuses = await _repository.GetAll();
            if (statuses == null)
            {
                return Result<List<StatusDTO>>.Failure(null, "no statuses found");
            }
            try
            {
                var statusDTOs = new List<StatusDTO>();
                foreach (var s in statuses)
                {
                    statusDTOs.Add(new StatusDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Code = s.Code,
                        
                    });

                }
                return Result<List<StatusDTO>>.Success(statusDTOs);

            }
            catch (Exception ex)
            {
                return Result<List<StatusDTO>>.Failure(null, $"unable to get statuses{ex.InnerException.Message}");

            }
          

        }

        public async  Task<Result<StatusDTO>> Insert(StatusDTO data)
        {
            if (data == null)
            {
                return Result<StatusDTO>.Failure(data, "the status dto is null");
            }
            var status = new Status
            {
                Name = data.Name,
                Code = data.Code,
            };
            try
            {
                await _repository.Insert(status);
                return Result<StatusDTO>.Success(data);

            }
            catch (Exception ex)
            {
                return Result<StatusDTO>.Failure(data, $"unable to insert a status: {ex.InnerException.Message}");
            }
        }
    }
}
