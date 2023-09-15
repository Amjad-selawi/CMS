using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CMS.Services.Services
{
    public class CarrerOfferService : ICarrerOfferService
    {
        private readonly ICarrerOfferRepository _carrerOfferRepository;

        public CarrerOfferService(ICarrerOfferRepository carrerOfferRepository)
        {
            _carrerOfferRepository = carrerOfferRepository;
        }

        public async Task<IEnumerable<CarrerOfferDTO>> GetAllCarrerOffersAsync()
        {
            var carrerOffers = await _carrerOfferRepository.GetAllCarrerOffersAsync();
            return carrerOffers.Select(co => new CarrerOfferDTO
            {
                Id = co.Id,
                PositionId = co.PositionId,
                YearsOfExperience = co.YearsOfExperience,
                LongDescription = co.LongDescription,
                CreatedBy = co.CreatedBy,
                CreatedDateTime = co.CreatedDateTime
            });
        }

        public async Task<CarrerOfferDTO> GetCarrerOfferByIdAsync(int carrerOfferId)
        {
            var carrerOffer = await _carrerOfferRepository.GetCarrerOfferByIdAsync(carrerOfferId);
            if (carrerOffer == null)
                return null;

            return new CarrerOfferDTO
            {
                Id = carrerOffer.Id,
                PositionId = carrerOffer.PositionId,
                YearsOfExperience = carrerOffer.YearsOfExperience,
                LongDescription = carrerOffer.LongDescription,
                CreatedBy = carrerOffer.CreatedBy,
                CreatedDateTime = carrerOffer.CreatedDateTime
            };
        }

        public async Task CreateCarrerOfferAsync(CarrerOfferDTO carrerOfferDTO)
        {
            var carrerOffer = new CarrerOffer
            {
                PositionId = carrerOfferDTO.PositionId,
                YearsOfExperience = carrerOfferDTO.YearsOfExperience,
                LongDescription = carrerOfferDTO.LongDescription,
                CreatedBy = carrerOfferDTO.CreatedBy
               
            };
            await _carrerOfferRepository.CreateCarrerOfferAsync(carrerOffer);
        }

        public async Task UpdateCarrerOfferAsync(int carrerOfferId, CarrerOfferDTO carrerOfferDTO)
        {
            var existingCarrerOffer = await _carrerOfferRepository.GetCarrerOfferByIdAsync(carrerOfferId);
            if (existingCarrerOffer == null)
                throw new Exception("Carrer offer not found");

            existingCarrerOffer.PositionId = carrerOfferDTO.PositionId;
            existingCarrerOffer.YearsOfExperience = carrerOfferDTO.YearsOfExperience;
            existingCarrerOffer.LongDescription = carrerOfferDTO.LongDescription;
            existingCarrerOffer.CreatedBy = carrerOfferDTO.CreatedBy;

            await _carrerOfferRepository.UpdateCarrerOfferAsync(existingCarrerOffer);
        }

        public async Task DeleteCarrerOfferAsync(int carrerOfferId)
        {
            var carrerOffer = await _carrerOfferRepository.GetCarrerOfferByIdAsync(carrerOfferId);
            if (carrerOffer != null)
                await _carrerOfferRepository.DeleteCarrerOfferAsync(carrerOffer);
        }
    }
}
