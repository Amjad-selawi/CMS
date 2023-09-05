using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Services.Services
{
    public class CountryService:ICountryService
    {
        ICountryRepository _repository;
        public CountryService(ICountryRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<CountryDTO>> Delete(int id)
        {
            try
            {
                await _repository.Delete(id);
                return Result<CountryDTO>.Success(null);
            }
            catch (Exception ex)
            {
                return Result<CountryDTO>.Failure(null, $"An error occurred while deleting the country{ex.InnerException.Message}");
            }
        }

        public async Task<Result<List<CountryDTO>>> GetAll()
        {
            var countries=await _repository.GetAll();
            if(countries == null)
            {
                return Result<List<CountryDTO>>.Failure(null,"no countries found");
            }
            try
            {
                var countryDTOS = new List<CountryDTO>();
                foreach (var co in countries)
                {
                    countryDTOS.Add(new CountryDTO
                    {
                        Id = co.Id,
                        Name = co.Name,
                        companyDTOs = co.Companies.Select(com => new CompanyDTO
                        {
                            Id = com.Id,
                            Name = com.Name,
                            Email = com.Email,
                            Address = com.Address,
                            CountryId = com.CountryId,
                            PhoneNumber = com.PhoneNumber,

                        }).ToList()
                    });
                    
                }
                return Result< List < CountryDTO >>.Success(countryDTOS);
            }
            catch(Exception ex)
            {
                return Result<List<CountryDTO>>.Failure(null, $"unable to get countries{ex.InnerException.Message}");
            }

        }

        public async Task<Result<CountryDTO>> GetById(int id)
        {
            if (id <= 0)
            {
                return Result<CountryDTO>.Failure(null, "Invalid company id");
            }
            try
            {
                var country=await _repository.GetById(id);
                var countryDTOS = new CountryDTO
                {
                    Id = country.Id,
                    Name = country.Name,
                    companyDTOs = country.Companies.Select(com => new CompanyDTO
                    {
                        Id = com.Id,
                        Name = com.Name,
                        Email = com.Email,
                        Address = com.Address,
                        CountryId = com.CountryId,
                        PhoneNumber = com.PhoneNumber,

                    }).ToList()

                };
                return Result<CountryDTO>.Success(countryDTOS);
            }
            catch (Exception ex)
            {
                return Result<CountryDTO>.Failure(null, $"unable to retrieve the country from the repository{ex.InnerException.Message}");
            }
        }

        public async Task<Result<CountryDTO>> Insert(CountryDTO data)
        {
            if(data == null)
            {
                return Result<CountryDTO>.Failure(data,"the country dto is null");
            }
            var country = new Country
            {
                Name = data.Name,
            };
            try
            {
                await _repository.Insert(country);
                return Result<CountryDTO>.Success(data);

            }
            catch (Exception ex)
            {
                return Result<CountryDTO>.Failure(data, $"unable to insert a country: {ex.InnerException.Message}");
            }
        }

        public async Task<Result<CountryDTO>> Update(CountryDTO data)
        {
            if (data == null)
            {
                return Result<CountryDTO>.Failure(null, "can not update a null object");
            }
            var country=new Country { 
                Name = data.Name,
                Id=data.Id,
                //Companies = data.companyDTOs.Select(c=>new Company{
                //    Id=c.Id,
                //    Name=c.Name,
                //    Email=c.Email,
                //    Address=c.Address,
                //    PhoneNumber=c.PhoneNumber,
                //    CountryId=c.CountryId,
                //}).ToList()
            
            };
            try
            {
                await _repository.Update(country);
                return Result<CountryDTO>.Success(data);
            }
            catch (Exception ex)
            {
                return Result<CountryDTO>.Failure(data, $"unable to update the country: {ex.InnerException.Message}");
            }
        }
    }
}
