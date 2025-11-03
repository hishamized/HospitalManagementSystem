using AutoMapper;
using HMS.Application.Interfaces;
using HMS.Domain.Interfaces;
using HMS.Infrastructure.Data;

namespace HMS.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly DapperContext _dapperContext;
        private readonly IMapper _mapper;

        private IUserRepository _userRepository;
        private IPatientRepository _patientRepository;
        private IMedicalHistoryRepository _medicalHistoryRepository;
        private IAllergyRepository _allergyRepository;
        private IInsuranceRepository _insuranceRepository;
        private IBillRepository _billRepository;

        public UnitOfWork(ApplicationDbContext context, DapperContext dapperContext, IMapper mapper)
        {
            _context = context;
            _dapperContext = dapperContext;
            _mapper = mapper;
        }

        public IUserRepository Users => _userRepository ??= new UserRepository(_dapperContext);
        public IPatientRepository Patients => _patientRepository ??= new PatientRepository(_dapperContext);

        public IMedicalHistoryRepository MedicalHistories =>
            _medicalHistoryRepository ??= new MedicalHistoryRepository(this, _dapperContext);

        public IAllergyRepository Allergies => _allergyRepository ??= new AllergyRepository(_dapperContext, this);

        public IInsuranceRepository Insurances => _insuranceRepository ??= new InsuranceRepository(this, _dapperContext);

        public IBillRepository Bills => _billRepository ??= new BillRepository(this, _dapperContext, _mapper);

        public async Task<int> CommitAsync() => await _context.SaveChangesAsync();
    }
}
