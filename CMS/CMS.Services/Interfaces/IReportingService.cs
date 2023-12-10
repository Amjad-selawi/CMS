using CMS.Application.DTOs;
using CMS.Application.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Interfaces
{
    public interface IReportingService
    {
        public Task<Result<PerformanceReportDTO>> GetBusinessPerformanceReport();

        void LogException(string methodName, Exception ex);

    }
}
