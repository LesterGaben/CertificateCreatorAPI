using CertificateCreator.DAL.Entities;

namespace CertificateCreator.DAL.Repositories.Interfaces {
    public interface ICertificateRepository: IQueryable {
        public Task<Certificate> GetByIdAsync(string id);
        public Task<bool> FindId(string id);
        public Task AddAsync(Certificate certificate);
        public Task DeleteAsync(Certificate certificate);
        public Task DeleteAllAsync();
    }
}
