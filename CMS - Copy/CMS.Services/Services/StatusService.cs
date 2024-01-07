using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Services
{
    public class StatusService : IStatusService
    {
        IStatusRepository _repository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public StatusService(IStatusRepository repository
            ,IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;

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
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var status = new Status
            {
                Name = data.Name,
                Code = data.Code,
                CreatedOn = DateTime.Now,
                CreatedBy = currentUser.Id,
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



        public async Task<int> GetStatusIdByName(string statusName)
        {
            try
            {
                var status = await _repository.GetStatusByNameAsync(statusName);

                if (status != null)
                {
                    return status.Id;
                }
                else
                {
                    return 0; 
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting status ID by name: {ex.Message}", ex);
            }
        }
        public async Task<Result<StatusDTO>> GetById(int id)
        {
            try
            {
                var status = await _repository.GetById(id);
                if (status != null)
                {
                    var statusDTO = new StatusDTO
                    {
                        Id = status.Id,
                        Name = status.Name,
                        Code = status.Code,
                    };

                    return Result<StatusDTO>.Success(statusDTO);
                }
                else
                {
                    return Result<StatusDTO>.Failure(null, "no statuse found");
                }

            }
            catch (Exception ex)
            {
                return Result<StatusDTO>.Failure(null, $"unable to get status{ex.InnerException.Message}");
            }


        }




    }
}
