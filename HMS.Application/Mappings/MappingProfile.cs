using AutoMapper;
using HMS.Application.DTO.MedicalHistory;
using HMS.Application.DTO.Patient;
using HMS.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HMS.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Create Mappings between Entity ↔ DTOs
            CreateMap<Patient, CreatePatientDto>().ReverseMap();
            CreateMap<Patient, UpdatePatientDto>().ReverseMap();
            CreateMap<Patient, PatientDto>().ReverseMap();
            CreateMap<CreateMedicalHistoryDto, MedicalHistory>().ReverseMap();
            // You can add more later (e.g., PatientDto for reads)
        }
    }
}
