using AutoMapper;
using CertificateCreator.BLL.DTOs;
using CertificateCreator.BLL.Services.Interfaces;
using CertificateCreator.DAL.Entities;
using CertificateCreator.DAL.Repositories.Interfaces;

namespace CertificateCreator.BLL.Services {
    public class CertificateService : ICertificateService {

        private readonly ICertificateRepository _certificateRepository;
        private readonly IMapper _mapper;

        public CertificateService(ICertificateRepository certificateRepository, IMapper mapper) {
            _certificateRepository = certificateRepository;
            _mapper = mapper;
        }
        public async Task<CertificateDTO> GetByIdAsync(string id) {
            var certificate = await _certificateRepository.GetByIdAsync(id);
            return _mapper.Map<CertificateDTO>(certificate);
        }

        public async Task AddAsync(CreateCertificateDTO certificate) {
            var certificateEntity = _mapper.Map<Certificate>(certificate);
            await _certificateRepository.AddAsync(certificateEntity);
        }

        public async Task DeleteAsync(string id) {

            var certificate = await _certificateRepository.GetByIdAsync(id);
            await _certificateRepository.DeleteAsync(certificate);
        }

        public async Task DeleteAllSync() {
            await _certificateRepository.DeleteAllAsync();
        }

        public async Task<bool> FindIdAsync(GetCertificateByGuidDTO certificateId) {
            return await _certificateRepository.FindId(certificateId.certificateId);
        }
    }
}
