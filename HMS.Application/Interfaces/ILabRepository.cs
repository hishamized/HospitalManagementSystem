using HMS.Application.Commands.LabTest;
using HMS.Application.DTO.LabTest;
using HMS.Application.Queries.LabTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface ILabRepository 
    {
        Task<long> AddLabTestAsync(AddLabTestDto request, CancellationToken cancellationtoken);
        Task<IEnumerable<FetchLabTestsDto>> FetchLabTestsAsync(FetchLabTestsQuery request, CancellationToken cancellationToken);
    }
}
