﻿using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Interfaces
{
    public interface ICountryService
    {
        Task<Result<CountryDTO>> Insert(CountryDTO data);
        Task<Result<List<CountryDTO>>> GetAll();
        Result<CountryDTO> Delete(int id);

        //Task<Result<CountryDTO>> Delete(int id);
        Task<Result<CountryDTO>> GetById(int id);
        Task<Result<CountryDTO>> Update(CountryDTO data);

        Task<IEnumerable<Country>> GetAllCountriesAsync(); // Add this method
        Task<Country> GetCountryByNameAsync(string countryName);
        bool DoesCountryNameExist(string name);

    }
}
