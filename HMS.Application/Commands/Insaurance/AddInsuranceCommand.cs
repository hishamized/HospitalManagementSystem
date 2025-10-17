using HMS.Application.DTO.Insurance;
using MediatR;

namespace HMS.Application.Commands.Insurance
{
    public class AddInsuranceCommand : IRequest<int>
    {
        public AddInsuranceDto Insurance { get; set; }

        public AddInsuranceCommand(AddInsuranceDto insurance)
        {
            Insurance = insurance;
        }
    }
}
