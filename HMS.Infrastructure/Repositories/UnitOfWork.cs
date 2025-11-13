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
        private readonly IRepository _dbRepository;

        public UnitOfWork(IRepository dbRepository, IMapper Mapper, ApplicationDbContext context, DapperContext dapperContext)
        {
            _dbRepository = dbRepository;
            _mapper = Mapper;
            _context = context;
            _dapperContext = dapperContext;
        }

        private IUserRepository _userRepository;
        private IPatientRepository _patientRepository;
        private IMedicalHistoryRepository _medicalHistoryRepository;
        private IAllergyRepository _allergyRepository;
        private IInsuranceRepository _insuranceRepository;
        private IBillRepository _billRepository;
        private IRoleRepository _roleRepository;
        private IPatientPortalRepository _patientPortalRepository;

    

        public IUserRepository Users => _userRepository ??= new UserRepository(_dapperContext);
        public IPatientRepository Patients => _patientRepository ??= new PatientRepository(_dapperContext);

        public IMedicalHistoryRepository MedicalHistories =>
            _medicalHistoryRepository ??= new MedicalHistoryRepository(this, _dapperContext);

        public IAllergyRepository Allergies => _allergyRepository ??= new AllergyRepository(_dapperContext, this);

        public IInsuranceRepository Insurances => _insuranceRepository ??= new InsuranceRepository(this, _dapperContext);

        public IBillRepository Bills => _billRepository ??= new BillRepository(this, _dapperContext, _mapper);

        public IRoleRepository RoleRepository { get { _roleRepository = (_roleRepository == null) ? new RoleRepository(_dbRepository, _mapper) : _roleRepository; return _roleRepository; } }

        public IPatientPortalRepository PatientPortalRepository { get { _patientPortalRepository = (_patientPortalRepository == null) ? new PatientPortalRepository(_dbRepository) : _patientPortalRepository;
                return _patientPortalRepository;
            } }

        public async Task<int> CommitAsync() => await _context.SaveChangesAsync();
    }
}
