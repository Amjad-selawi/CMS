using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain.Entities;
using CMS.Repository.Implementation;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CMS.Services.Services
{
    public class PositionService:IPositionService
    {
        IPositionRepository _repository;
        public PositionService(IPositionRepository repository)
        {
            _repository = repository;
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
                return Result<PositionDTO>.Failure(null, $"An error occurred while deleting the position{ex.InnerException.Message}");
            }
        }

        public async Task<Result<List<PositionDTO>>> GetAll()
        {
            var position = await _repository.GetAll();
            if (position == null)
            {
                return Result<List<PositionDTO>>.Failure(null, "no positions found");
            }
            try
            {
                var positionDTOS = new List<PositionDTO>();
                foreach (var co in position)
                {
                    positionDTOS.Add(new PositionDTO
                    {
                        PositionId = co.PositionId,
                        Name = co.Name,
                        CarrerOfferDTO = co.CareerOffer.Select(com => new CarrerOfferDTO
                        {
                            Id = com.Id,
                            PositionId = com.PositionId,
                            LongDescription=com.LongDescription,
                            YearsOfExperience=com.YearsOfExperience,
                       

                        }).ToList(),
                        InterviewsDTO = co.Interviews.Select(com => new InterviewsDTO
                        {
                            InterviewsId = com.InterviewsId,
                            CandidateId = com.CandidateId,
                            Date = com.Date,
                            Notes = com.Notes,
                            InterviewerId = com.InterviewerId,
                            ParentId = com.ParentId,
                            Score = com.Score,
                            Status = com.Status.ToString(),


                        }).ToList()

                    });

                }
                return Result<List<PositionDTO>>.Success(positionDTOS);
            }
            catch (Exception ex)
            {
                return Result<List<PositionDTO>>.Failure(null, $"unable to get countries{ex.InnerException.Message}");
            }

        }

        public async Task<Result<PositionDTO>> GetById(int id)
        {
            if (id <= 0)
            {
                return Result<PositionDTO>.Failure(null, "Invalid carrerOffer id");
            }
            try
            {
                var position = await _repository.GetById(id);
                var positionDTOS = new PositionDTO
                {
                    PositionId = position.PositionId,
                    Name = position.Name,
                    CarrerOfferDTO = position.CareerOffer.Select(com => new CarrerOfferDTO
                    {
                        Id = com.Id,
                        PositionId = com.PositionId,
                        LongDescription = com.LongDescription,
                        YearsOfExperience = com.YearsOfExperience,


                    }).ToList(),
                    InterviewsDTO = position.Interviews.Select(com => new InterviewsDTO
                    {
                        InterviewsId = com.InterviewsId,
                        CandidateId = com.CandidateId,
                        Date = com.Date,
                        Notes = com.Notes,
                        InterviewerId = com.InterviewerId,
                        ParentId = com.ParentId,
                        Score = com.Score,
                        Status = com.Status.ToString(),


                    }).ToList()

                };
                return Result<PositionDTO>.Success(positionDTOS);
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
                return Result<PositionDTO>.Failure(data, "the position dto is null");
            }
            var postion = new Position
            {
                Name = data.Name,
            };
            try
            {
                await _repository.Insert(postion);
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
            var position = new Position
            {
                Name = data.Name,
                PositionId = data.PositionId,

            };
            try
            {
                await _repository.Update(position);
                return Result<PositionDTO>.Success(data);
            }
            catch (Exception ex)
            {
                return Result<PositionDTO>.Failure(data, $"unable to update the position: {ex.InnerException.Message}");
            }
        }

        //public async  Task Delete(int id)
        //{

        //    var position = await _repository.GetById(id);
        //    if (position != null)
        //        await _repository.Delete(position);



        //    //try
        //    //{
        //    //    await _repository.Delete(id);
        //    //    return Result<PositionDTO>.Success(null);

        //    //}  
        //    //catch (Exception ex)
        //    //{
        //    //    return Result<PositionDTO>.Failure(null,  $"An error occurred while deleting the position {ex.InnerException.Message}");
        //    //}



        //}

        //public async Task<IEnumerable<PositionDTO>> GetAll()
        //{

        //    var poistion = await _repository.GetAll();
        //    return poistion.Select(i => new PositionDTO
        //    {
        //        PositionId = i.PositionId,
        //        Name=i.Name

        //    });




        //    //var positions = await _repository.GetAll();
        //    //if (positions == null)
        //    //{
        //    //    return Result<List<PositionDTO>>.Failure(null, "no positions found");
        //    //}
        //    //try
        //    //{
        //    //    var positionDTOS = new List<PositionDTO>();
        //    //    foreach (var position in positions)
        //    //    {
        //    //        positionDTOS.Add(new PositionDTO{
        //    //            Id = position.Id,
        //    //            Name = position.Name,
        //    //        });
        //    //    }
        //    //    return Result<List<PositionDTO>>.Success(positionDTOS);

        //    //}
        //    //catch (Exception ex) {
        //    //    return Result<List<PositionDTO>>.Failure(null, $"unable to get positions{ex.InnerException.Message}");
        //    //}
        //}

        //public async Task<PositionDTO> GetById(int id)
        //{

        //    var positions = await _repository.GetById(id);
        //    if (positions == null)
        //        return null;

        //    return new PositionDTO
        //    {
        //        PositionId = positions.PositionId,
        //        Name = positions.Name,

        //    };





        //    //if (id <= 0)
        //    //{
        //    //    return Result<PositionDTO>.Failure(null, "Invalid position id");
        //    //}
        //    //try
        //    //{
        //    //    var position = await _repository.GetById(id);
        //    //    var positionDTO = new PositionDTO
        //    //    {
        //    //        Id = position.Id,
        //    //        Name = position.Name,
        //    //    };
        //    //    return Result<PositionDTO>.Success(positionDTO);
        //    //} catch (Exception ex)
        //    //{
        //    //    return Result<PositionDTO>.Failure(null, $"unable to retrieve the position from the repository{ex.InnerException.Message}");
        //    //}
        //}

        //public async Task Insert(PositionDTO data)
        //{

        //    var position = new Position
        //    {
        //        Name = data.Name,
        //    };

        //    await _repository.Insert(position);

        //    //if(data == null)
        //    //{
        //    //    return Result<PositionDTO>.Failure(data, "the position DTO is null");
        //    //}
        //    //try
        //    //{
        //    //    var position = new Position
        //    //    {
        //    //        Name = data.Name,
        //    //    };
        //    //   await _repository.Insert(position);
        //    //    return Result<PositionDTO>.Success(data);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    return Result<PositionDTO>.Failure(data, $"unable to insert a position: {ex.InnerException.Message}");
        //    //}
        //}



        //public async Task Update(int positionId, PositionDTO data)
        //{
        //    var position = await _repository.GetById(positionId);
        //    if(position == null)
        //        throw new Exception("Position not found");

        //    position.Name = data.Name;

        //    await _repository.Update(position);
        //}


        ////public async Task<Result<PositionDTO>> Update(PositionDTO data)
        ////{
        ////    if (data == null)
        ////    {
        ////        return Result<PositionDTO>.Failure(null, "can not update a null object");
        ////    }
        ////    try
        ////    {
        ////        var position = new Position
        ////        {
        ////            Id = data.Id,
        ////            Name = data.Name,
        ////        };
        ////        await _repository.Update(position);
        ////        return Result<PositionDTO>.Success(data);
        ////    } catch (Exception ex)
        ////    {
        ////        return Result<PositionDTO>.Failure(data, $"error updating the position {ex.InnerException.Message}");
        ////    }
        ////}
    }
}
