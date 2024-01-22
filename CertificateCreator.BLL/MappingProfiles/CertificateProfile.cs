using AutoMapper;
using CertificateCreator.BLL.DTOs;
using CertificateCreator.DAL.Entities;

namespace CertificateCreator.BLL.MappingProfiles {
    public class CertificateProfile : Profile {
        public CertificateProfile() {
            CreateMap<Certificate,CertificateDTO>();
            CreateMap<CertificateDTO, Certificate>();
            CreateMap<CreateCertificateDTO, Certificate>();
            CreateMap<Certificate, CreateCertificateDTO>();
        }
    }
}
