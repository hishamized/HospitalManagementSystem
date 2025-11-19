using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.LabTest
{
    public class DeleteLabTestQuery : IRequest<bool>
    {
        public readonly int TestId;
        public DeleteLabTestQuery(int testId) {
            TestId = testId;
        }
    }
}
