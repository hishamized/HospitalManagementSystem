using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using System.Data;
using System.Threading.Tasks;

namespace HMS.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IMedicalHistoryRepository MedicalHistories { get; }
        IUserRepository Users { get; }
        IPatientRepository Patients { get; }
        Task<int> CommitAsync(); 
    }
}
