using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository.Interfaces
{
    public interface ICarrerOfferRepository
    {
        //Task<IEnumerable<CarrerOffer>> GetAllCarrerOffersAsync();
        //Task<CarrerOffer> GetCarrerOfferByIdAsync(int carrerOfferId);
        //Task CreateCarrerOfferAsync(CarrerOffer carrerOffer);
        //Task UpdateCarrerOfferAsync(CarrerOffer carrerOffer);
        //Task DeleteCarrerOfferAsync(CarrerOffer carrerOffer);

        Task<int> Insert(CarrerOffer entity);
        Task<int> Update(CarrerOffer entity);
        Task<int> Delete(int id);
        Task<CarrerOffer> GetById(int id);
        Task<List<CarrerOffer>> GetAll();

        Task<Position> GetPositionByIdAsync(int positionId);

    }
}
    