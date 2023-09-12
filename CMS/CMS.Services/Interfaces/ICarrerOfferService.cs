using CMS.Application.DTOs;
using CMS.Application.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Interfaces
{
    public interface ICarrerOfferService
    {
        //Task<IEnumerable<CarrerOfferDTO>> GetAllCarrerOffersAsync();
        //Task<CarrerOfferDTO> GetCarrerOfferByIdAsync(int carrerOfferId);
        //Task CreateCarrerOfferAsync(CarrerOfferDTO carrerOfferDTO);
        //Task UpdateCarrerOfferAsync(int carrerOfferId, CarrerOfferDTO carrerOfferDTO);
        //Task DeleteCarrerOfferAsync(int carrerOfferId);

        Task<Result<CarrerOfferDTO>> Insert(CarrerOfferDTO data);
        Task<Result<List<CarrerOfferDTO>>> GetAll();
        Task<Result<CarrerOfferDTO>> Delete(int id);
        Task<Result<CarrerOfferDTO>> GetById(int id);
        Task<Result<CarrerOfferDTO>> Update(CarrerOfferDTO data);

    }
}
