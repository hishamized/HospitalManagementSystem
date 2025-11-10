using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Patient
{
    public class DischargePatientCommand : IRequest<bool>
    {
        public int VisitId { get; set; }

        //public DischargePatientCommand(int PatientId) {
        //    this.PatientId = PatientId;
        //}
    }
}
