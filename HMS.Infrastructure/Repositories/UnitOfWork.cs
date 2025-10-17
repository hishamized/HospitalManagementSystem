using HMS.Application.Interfaces;
using HMS.Domain.Interfaces;
using HMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly DapperContext _dapperContext;

        // Private backing fields for repositories
        private IUserRepository _userRepository;
        private IPatientRepository _patientRepository;
        private IMedicalHistoryRepository _medicalHistoryRepository;
        private IAllergyRepository _allergyRepository;
        private IInsuranceRepository _insuranceRepository;


        public UnitOfWork(ApplicationDbContext context, DapperContext dapperContext)
        {
            _context = context;
            _dapperContext = dapperContext;
        }

        // Expose repositories through interface
        public IUserRepository Users => _userRepository ??= new UserRepository(_dapperContext);
        public IPatientRepository Patients => _patientRepository ??= new PatientRepository(_dapperContext);

        public IMedicalHistoryRepository MedicalHistories =>
     _medicalHistoryRepository ??= new MedicalHistoryRepository(this, _dapperContext);

        public IAllergyRepository Allergies => _allergyRepository ??= new AllergyRepository(_dapperContext, this);

        public IInsuranceRepository Insurances => _insuranceRepository ??= new InsuranceRepository(this, _dapperContext);


        // Commit method (optional for Dapper, but required if EF changes are tracked)
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
