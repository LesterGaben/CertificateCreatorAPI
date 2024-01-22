using CertificateCreator.BLL.DTOs;

namespace CertificateCreator.BLL.Services.Interfaces {
    public interface IConvertCertificateToPDFService {
        public Task<string> CertificateToPDF(ConvertCertificateToPdfDTO certificateHTML);
    }
}
