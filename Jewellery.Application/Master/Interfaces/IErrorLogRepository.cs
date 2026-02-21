using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jewellery.Domain.Entities;

namespace Jewellery.Application.Master.Interfaces
{
    public interface IErrorLogRepository
    {
        Task SaveErrorAsync(ErrorLog log);
    }
}
