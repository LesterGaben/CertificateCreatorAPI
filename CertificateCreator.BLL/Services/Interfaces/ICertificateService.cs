using CertificateCreator.BLL.DTOs;

namespace CertificateCreator.BLL.Services.Interfaces {
    public interface ICertificateService {
        public Task<CertificateDTO> GetByIdAsync(string id);

        public Task<bool> FindIdAsync(GetCertificateByGuidDTO certificateId);
        public Task AddAsync(CreateCertificateDTO certificate);
        public Task DeleteAsync(string id);
        public Task DeleteAllSync();
    }
}
