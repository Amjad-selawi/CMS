using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository.Interfaces
{
    public interface IPositionRepository
    {

        Task<int> Insert(Position entity, string createdByUserId);
        Task<int> Update(Position entity, string modifiedByUserId);
        Task<int> Delete(int id, string deletedByUserId);
        Task<Position> GetById(int id, string createdByUserId);
        Task<List<Position>> GetAll();

        bool DoesPositionNameExist(string name);

        void LogException(string methodName, Exception ex, string createdByUserId, string additionalInfo);


    }
}
