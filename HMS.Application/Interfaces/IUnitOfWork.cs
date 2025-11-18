using HMS.Application.Interfaces;
using System.Threading.Tasks;

namespace HMS.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IPatientRepository Patients { get; }
        IMedicalHistoryRepository MedicalHistories { get; }
        IAllergyRepository Allergies { get; }
        IInsuranceRepository Insurances { get; }
        IBillRepository Bills { get; }

        IRoleRepository RoleRepository { get; }

        IPatientPortalRepository PatientPortalRepository { get; }

        IPaymentRepository PaymentRepository { get; }

        ILabRepository LabRepository { get; }

        Task<int> CommitAsync();
    }

}
