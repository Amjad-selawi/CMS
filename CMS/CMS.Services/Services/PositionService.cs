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
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace CMS.Services.Services
{
    public class PositionService : IPositionService
    {
        IPositionRepository _repository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAttachmentService _attachmentService;
        public PositionService(IPositionRepository repository,
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager,
            IAttachmentService attachmentService)
        {
            _repository = repository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _attachmentService = attachmentService;
        }

        public void LogException(string methodName, Exception ex, string createdByUserId = null, string additionalInfo = null)
        {
            _repository.LogException(methodName, ex, createdByUserId, additionalInfo);
        }

        public string GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }


        public async Task<Result<PositionDTO>> Delete(int id)
        {
            try
            {
                var pos = await _repository.GetById(id, GetUserId())
;

                if (pos != null)
                {
                    var attachmentToRemove = 0;
                    if (pos.EvaluationId != null)
                    {
                        attachmentToRemove = (int)pos.EvaluationId;
                    }

                    await _repository.Delete(id, GetUserId())
;
                    if (attachmentToRemove != 0)
                    {
                        await _attachmentService.DeleteAttachmentAsync(attachmentToRemove);
                    }

                }


                return Result<PositionDTO>.Success(null);

            }
            catch (Exception ex)
            {
                LogException(nameof(Delete),ex);
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
                        EvaluationId = position.EvaluationId,
                    });
                }
                return Result<IEnumerable<PositionDTO>>.Success(positionDTOS);

            }
            catch (Exception ex)
            {
                LogException(nameof(GetAll), ex);
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
                var position = await _repository.GetById(id, GetUserId());
                var positionDTO = new PositionDTO
                {
                    Id = position.Id,
                    Name = position.Name,
                    EvaluationId= position.EvaluationId,
                };
                return Result<PositionDTO>.Success(positionDTO);
            }
            catch (Exception ex)
            {
                LogException(nameof(GetById), ex);
                return Result<PositionDTO>.Failure(null, $"unable to retrieve the position from the repository{ex.InnerException.Message}");
            }
        }

        public async Task<Result<PositionDTO>> Insert(PositionDTO data)
        {
            if (data == null)
            {
                return Result<PositionDTO>.Failure(data, "the position DTO is null");
            }
            if (data.FileData != null)
            {
                int attachmentId = await _attachmentService.CreateAttachmentAsync(data.FileName, (long)data.FileSize, data.FileData);
                data.EvaluationId = attachmentId;
            }
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            try
            {
                var position = new Position
                {
                    Name = data.Name,
                    CreatedBy = currentUser.Id,
                    CreatedOn=DateTime.Now,
                    EvaluationId=data.EvaluationId,
                };
                await _repository.Insert(position, GetUserId());
                return Result<PositionDTO>.Success(data);
            }
            catch (Exception ex)
            {
                LogException(nameof(Insert), ex);
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
            var previouePos = await _repository.GetById(data.Id, GetUserId());
            try
            {
                var position = new Position
                {
                    Id = data.Id,
                    Name = data.Name,
                    EvaluationId= data.EvaluationId,
                    ModifiedBy = currentUser.Id,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = previouePos.CreatedBy,
                    CreatedOn=previouePos.CreatedOn,
                };
                await _repository.Update(position, GetUserId());
                return Result<PositionDTO>.Success(data);
            }
            catch (Exception ex)
            {
                LogException(nameof(Update), ex);
                return Result<PositionDTO>.Failure(data, $"error updating the position {ex.InnerException.Message}");
            }
        }
        public async Task UpdatePositionEvaluationAsync(int id, string fileName, long fileSize, Stream fileStream)
        {
            try
            {


                var position = await _repository.GetById(id,GetUserId());
                int attachmentId = await _attachmentService.CreateAttachmentAsync(fileName, fileSize, fileStream);

                int attachmentToRemove = 0;
                if (position.EvaluationId != null)
                {
                    attachmentToRemove = (int)position.EvaluationId;

                }

                position.EvaluationId = attachmentId;

                await _repository.Update(position, GetUserId());
                if (attachmentToRemove != 0)
                {
                    await _attachmentService.DeleteAttachmentAsync(attachmentToRemove);
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(Update), ex);
                throw ex;
            }

        }



        public bool DoesPositionNameExist(string name)
        {
            try
            {

            return _repository.DoesPositionNameExist(name);
            }

            catch (Exception ex)
            {
                LogException(nameof(DoesPositionNameExist), ex);
                throw ex;
            }
        }





    }
}
