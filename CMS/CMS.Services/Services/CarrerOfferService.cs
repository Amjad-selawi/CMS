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
        private readonly IPositionService _positionService;
        public CarrerOfferService(ICarrerOfferRepository carrerOfferRepository, IPositionService positionService)
        {
            _carrerOfferRepository = carrerOfferRepository;
            _positionService = positionService;
        }
        public async Task<Result<CarrerOfferDTO>> Delete(int id)
        {

            try
            {
                await _carrerOfferRepository.Delete(id);
                return Result<CarrerOfferDTO>.Success(null);
            }

            catch (Exception ex)
            {
                Id = co.Id,
                PositionId = co.PositionId,
                YearsOfExperience = co.YearsOfExperience,
                LongDescription = co.LongDescription,
                CreatedBy = co.CreatedBy,
                CreatedDateTime = co.CreatedDateTime
            });
        }
                return Result<CarrerOfferDTO>.Failure(null, $"An error occurred while deleting the career offer{ex.InnerException.Message}");
            }
        }

        public async Task<Result<List<CarrerOfferDTO>>> GetAll()
        {
            try
            {
                var careerOffers = await _carrerOfferRepository.GetAll();
                if (careerOffers == null)
                {
                    return Result<List<CarrerOfferDTO>>.Failure(null, "No career offers found");
                }

                var carrerOfferDTOs = new List<CarrerOfferDTO>();
                foreach (var c in careerOffers)
                {

                    var com = new CarrerOfferDTO
                    {
                        Id = c.Id,
                        LongDescription = c.LongDescription,
                        YearsOfExperience = c.YearsOfExperience,
                        PositionId = c.PositionId,
                        PositionName = c.Positions.Name,
                        CreatedBy = carrerOffer.CreatedBy,
                        CreatedDateTime = carrerOffer.CreatedDateTime


                    };
                    carrerOfferDTOs.Add(com);

                }
                return Result<List<CarrerOfferDTO>>.Success(carrerOfferDTOs);


            }
            catch (Exception ex)
            {
                return Result<List<CarrerOfferDTO>>.Failure(null, $"Unable to get career offers: {ex.InnerException.Message}");
            }

        }

        public async Task<Result<CarrerOfferDTO>> GetById(int id)
        {
            if (id <= 0)
            {
                return Result<CarrerOfferDTO>.Failure(null, "Invalid career offer id");
            }
            try
            {
                var careeroffer = await _carrerOfferRepository.GetById(id);
                var careerofferDTO = new CarrerOfferDTO
                {
                    Id = careeroffer.Id,
                    LongDescription = careeroffer.LongDescription,
                    YearsOfExperience = careeroffer.YearsOfExperience,
                    PositionId = careeroffer.PositionId,
                    PositionName = careeroffer.Positions.Name

                };
                return Result<CarrerOfferDTO>.Success(careerofferDTO);
            }
            catch (Exception ex)
            {
                return Result<CarrerOfferDTO>.Failure(null, $"unable to retrieve the career offer from the repository{ex.InnerException.Message}");
            }

        }

        public async Task<Result<CarrerOfferDTO>> Insert(CarrerOfferDTO data)
        {
            if (data == null)
            {
                return Result<CarrerOfferDTO>.Failure(data, "the career offer DTO is null");
            }

            var careeroffer = new CarrerOffer
            {
                PositionId=data.PositionId,
                LongDescription=data.LongDescription,
                YearsOfExperience=data.YearsOfExperience,
               


            };

            try
            {
                await _carrerOfferRepository.Insert(careeroffer);

                return Result<CarrerOfferDTO>.Success(data);

            }
            catch (Exception ex)
            {
                return Result<CarrerOfferDTO>.Failure(data, $"unable to insert a career offer: {ex.InnerException.Message}");

            existingCarrerOffer.PositionId = carrerOfferDTO.PositionId;
            existingCarrerOffer.YearsOfExperience = carrerOfferDTO.YearsOfExperience;
            existingCarrerOffer.LongDescription = carrerOfferDTO.LongDescription;
            existingCarrerOffer.CreatedBy = carrerOfferDTO.CreatedBy;

        }

        public async Task<Result<CarrerOfferDTO>> Update(CarrerOfferDTO data)
        {
            if (data == null)
            {
                return Result<CarrerOfferDTO>.Failure(data, "can not update a null object");
            }
            var careeroffer = new CarrerOffer
            {
                Id = data.Id,
                PositionId = data.PositionId,
                LongDescription = data.LongDescription,
                YearsOfExperience = data.YearsOfExperience,



            };

            try
            {
                await _carrerOfferRepository.Update(careeroffer);
                return Result<CarrerOfferDTO>.Success(data);
            }
            catch (Exception ex)
            {
                return Result<CarrerOfferDTO>.Failure(data, $"error updating the career offer {ex.Message}");
            }
        }
        
    }
}
