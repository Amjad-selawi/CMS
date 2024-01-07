using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository.Interfaces
{
    public interface ITrackRepository
    {
        Task<List<Track>> GetAll();
        Task<Track> GetById(int? id);
    }
}
