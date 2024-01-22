using CertificateCreator.BLL.DTOs;
using CertificateCreator.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CertificateCreator.API.Controllers {
    [ApiController]
    [Route("certificates")]
    public class CertificateController : ControllerBase {

        private readonly ICertificateService _certificateService;
        private readonly IConvertCertificateToPDFService _convertCertificateToPDFService;

        public CertificateController(ICertificateService certificateService, IConvertCertificateToPDFService convertCertificateToPDFService) {

            _certificateService = certificateService;
            _convertCertificateToPDFService = convertCertificateToPDFService;
        }

        [HttpGet("by-id/")]
        public async Task<IActionResult> GetByIdAsync([FromQuery] string id) {
            CertificateDTO certificate = await _certificateService.GetByIdAsync(id);
            return File(certificate.PDFCertificate, "application/pdf", "certificate.pdf");
        }

        [HttpGet("check-id/{certificateId}")]
        public async Task<IActionResult> CheckId(string certificateId) {
            GetCertificateByGuidDTO getCertificateByGuidDTO = new GetCertificateByGuidDTO();
            getCertificateByGuidDTO.certificateId = certificateId;
            return Ok(await _certificateService.FindIdAsync(getCertificateByGuidDTO));
        }

        [HttpPost]
        public async Task<IActionResult> AddCertificateSync([FromBody] ConvertCertificateToPdfDTO certificate) {
            GetCertificateByGuidDTO certificateId = new GetCertificateByGuidDTO();
            certificateId.certificateId = await _convertCertificateToPDFService.CertificateToPDF(certificate);
            return Ok(certificateId);
        }

        [HttpGet("download/")]
        public async Task<IActionResult> DownloadCertificate([FromQuery] string id) {
            CertificateDTO certificate = await _certificateService.GetByIdAsync(id);

            if (certificate == null) {
                return NotFound();
            }

            return File(certificate.PDFCertificate, "application/pdf", "certificate.pdf");
        }

        [HttpDelete("delete-all/")]
        public async Task<IActionResult> DeleteAllAsync() {
            await _certificateService.DeleteAllSync();
            return NoContent();
        }

        [HttpDelete("delete-id/")]
        public async Task<IActionResult> DeleteAsync(string id) {
            await _certificateService.DeleteAsync(id);
            return NoContent();
        }
    }
}
