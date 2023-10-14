using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository.Interfaces
{
    public interface ITemplatesRepository
    { 
        Task<IEnumerable<Templates>> GetAllTemplates();
        Task<Templates> GetTemplateById(int templatesId);
        Task Create(Templates entity);
        Task Update(Templates entity);
        Task Delete(Templates entity);
    }
}
