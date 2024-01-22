using CertificateCreator.DAL.Context;
using CertificateCreator.DAL.Entities;
using CertificateCreator.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Linq.Expressions;

namespace CertificateCreator.DAL.Repositories {
    public class CertificateRepository : ICertificateRepository {

        private readonly CertificateCreatorContext _certificateCreatorContext;
        private readonly DbSet<Certificate> _certificates;
        public Type EntityType => ((IQueryable)_certificates).ElementType;
        public Type ElementType { get;}
        public Expression Expression => ((IQueryable)_certificates).Expression;
        public IQueryProvider Provider => ((IQueryable)_certificates).Provider;

        public CertificateRepository(CertificateCreatorContext certificateCreatorContext) {
            _certificateCreatorContext = certificateCreatorContext;
            _certificates = _certificateCreatorContext.Set<Certificate>();
            
        }
        public async Task AddAsync(Certificate certificate) {
            await _certificates.AddAsync(certificate);
            try {
                await _certificateCreatorContext.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx) {
                
                Console.WriteLine("Помилка бази даних: " + dbEx.Message);
                if (dbEx.InnerException != null) {
                    Console.WriteLine("Внутрішня помилка: " + dbEx.InnerException.Message);
                }
            }
            catch (Exception ex) {
                
                Console.WriteLine("Помилка: " + ex.Message);
                if (ex.InnerException != null) {
                    Console.WriteLine("Внутрішня помилка: " + ex.InnerException.Message);
                }
            }
        }

        public async Task DeleteAsync(Certificate certificate) {
            _certificates.Remove(certificate);
            await _certificateCreatorContext.SaveChangesAsync();
        }

        public async Task<Certificate> GetByIdAsync(string id) {
            return await _certificates.FindAsync(id);
        }

        public IEnumerator GetEnumerator() {
            return ((IQueryable)_certificates).GetEnumerator();
        }

        public async Task DeleteAllAsync() {
            var allCertificates = await _certificates.ToListAsync();
            _certificates.RemoveRange(allCertificates);

            await _certificateCreatorContext.SaveChangesAsync();
        }

        public async Task<bool> FindId(string id) {
            Certificate certificate = await _certificates.FindAsync(id);
            if (certificate != null) {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
