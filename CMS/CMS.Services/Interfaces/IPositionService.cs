using CMS.Application.DTOs;
using CMS.Application.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Interfaces
{
    public interface IPositionService
    {
        //Task Insert(PositionDTO data);
        //Task<IEnumerable<PositionDTO>> GetAll();
        //Task Delete(int id);
        //Task<PositionDTO> GetById(int id);
        //Task Update(int positionId,PositionDTO data);


        Task<Result<PositionDTO>> Insert(PositionDTO data);
        Task<Result<List<PositionDTO>>> GetAll();
        Task<Result<PositionDTO>> Delete(int id);
        Task<Result<PositionDTO>> GetById(int id);
        Task<Result<PositionDTO>> Update(PositionDTO data);



    }
}
