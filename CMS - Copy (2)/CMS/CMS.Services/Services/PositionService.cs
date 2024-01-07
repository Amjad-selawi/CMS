using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain.Entities;
using CMS.Repository.Implementation;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CMS.Services.Services
{
    public class PositionService : IPositionService
    {
        IPositionRepository _repository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PositionService(IPositionRepository repository,
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<PositionDTO>> Delete(int id)
        {
            try
            {
                await _repository.Delete(id);
                return Result<PositionDTO>.Success(null);

            }
            catch (Exception ex)
            {
                return Result<PositionDTO>.Failure(null, $"An error occurred while deleting the position {ex.InnerException.Message}");
            }



        }

        public async Task<Result<IEnumerable<PositionDTO>>> GetAll()
        {
            var positions = await _repository.GetAll();
            if (positions == null)
            {
                return Result<IEnumerable<PositionDTO>>.Failure(null, "no positions found");
            }
            try
            {
                var positionDTOS = new List<PositionDTO>();
                foreach (var position in positions)
                {
                    positionDTOS.Add(new PositionDTO
                    {
                        Id = position.Id,
                        Name = position.Name,
                    });
                }
                return Result<IEnumerable<PositionDTO>>.Success(positionDTOS);

            }
            catch (Exception ex)
            {
                return Result<IEnumerable<PositionDTO>>.Failure(null, $"unable to get positions{ex.InnerException.Message}");
            }
        }

        public async Task<Result<PositionDTO>> GetById(int id)
        {
            if (id <= 0)
            {
                return Result<PositionDTO>.Failure(null, "Invalid position id");
            }
            try
            {
                var position = await _repository.GetById(id);
                var positionDTO = new PositionDTO
                {
                    Id = position.Id,
                    Name = position.Name,
                };
                return Result<PositionDTO>.Success(positionDTO);
            }
            catch (Exception ex)
            {
                return Result<PositionDTO>.Failure(null, $"unable to retrieve the position from the repository{ex.InnerException.Message}");
            }
        }

        public async Task<Result<PositionDTO>> Insert(PositionDTO data)
        {
            if (data == null)
            {
                return Result<PositionDTO>.Failure(data, "the position DTO is null");
            }
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            try
            {
                var position = new Position
                {
                    Name = data.Name,
                    CreatedBy = currentUser.Id,
                    CreatedOn=DateTime.Now,
                };
                await _repository.Insert(position);
                return Result<PositionDTO>.Success(data);
            }
            catch (Exception ex)
            {
                return Result<PositionDTO>.Failure(data, $"unable to insert a position: {ex.InnerException.Message}");
            }
        }

        public async Task<Result<PositionDTO>> Update(PositionDTO data)
        {
            if (data == null)
            {
                return Result<PositionDTO>.Failure(null, "can not update a null object");
            }
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var previouePos = await _repository.GetById(data.Id);
            try
            {
                var position = new Position
                {
                    Id = data.Id,
                    Name = data.Name,
                    ModifiedBy = currentUser.Id,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = previouePos.CreatedBy,
                    CreatedOn=previouePos.CreatedOn,
                };
                await _repository.Update(position);
                return Result<PositionDTO>.Success(data);
            }
            catch (Exception ex)
            {
                return Result<PositionDTO>.Failure(data, $"error updating the position {ex.InnerException.Message}");
            }
        }



        public bool DoesPositionNameExist(string name)
        {
            return _repository.DoesPositionNameExist(name);
        }





    }
}
