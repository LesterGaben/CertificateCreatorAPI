using HtmlAgilityPack;
using CertificateCreator.BLL.DTOs;
using CertificateCreator.BLL.Services.Interfaces;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Text.RegularExpressions;
using System.Globalization;


namespace CertificateCreator.BLL.Services {
    public class ConvertCertificateToPDFService : IConvertCertificateToPDFService {

        private readonly ICertificateService _certificateService;

        public ConvertCertificateToPDFService(ICertificateService certificateService) {
            _certificateService = certificateService;
        }
        public async Task<string> CertificateToPDF(ConvertCertificateToPdfDTO certificateHTML) {
            Guid certificateId = Guid.NewGuid();
            
            certificateHTML.PDFCertificate = OpenVisibilityToLabelForIdAndSpanId(certificateHTML.PDFCertificate);
            certificateHTML.PDFCertificate = AddIdToPdf(certificateHTML.PDFCertificate, certificateId);
            certificateHTML.PDFCertificate = AdjustingFieldLocations(certificateHTML.PDFCertificate);
            certificateHTML.PDFCertificate = ConvertImageToBase64(certificateHTML.PDFCertificate);
            certificateHTML.PDFCertificate = IncreaseFontSize(certificateHTML.PDFCertificate);

            CreateCertificateDTO certificateDTO = await ConvertCertificateToPDFAsync(certificateHTML.PDFCertificate, certificateId);
            certificateDTO.Id = certificateId.ToString();
            await _certificateService.AddAsync(certificateDTO);
            return certificateDTO.Id.ToString();
        }

        private string OpenVisibilityToLabelForIdAndSpanId(string certificateHTML) {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(certificateHTML); 

            var labelForId = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'text-string')]");
            if (labelForId != null) {
                var existingStyle = labelForId.GetAttributeValue("style", "");
                if (existingStyle.Contains("visibility: hidden;")) {
                    existingStyle = existingStyle.Replace("visibility: hidden;", "visibility: visible;");
                }
                labelForId.SetAttributeValue("style", existingStyle);
            }

            var spanId = htmlDocument.DocumentNode.SelectSingleNode("//span[contains(@class, 'certificateId')]");
            if (spanId != null) {
                var existingStyle = spanId.GetAttributeValue("style", "");
                if (existingStyle.Contains("visibility: hidden;")) {
                    existingStyle = existingStyle.Replace("visibility: hidden;", "visibility: visible;");
                }
                spanId.SetAttributeValue("style", existingStyle);
            }

            return htmlDocument.DocumentNode.OuterHtml;
        }

        private string AddIdToPdf(string certificateHTML, Guid certificateId) {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(certificateHTML); 

            var spanId = htmlDocument.DocumentNode.SelectSingleNode("//span[contains(@class, 'certificateId')]");
            if (spanId != null) {
                spanId.InnerHtml = certificateId.ToString();
            }
            return htmlDocument.DocumentNode.OuterHtml;
        }

        private string AdjustingFieldLocations(string certificateHTML) {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(certificateHTML);

            int numOfCertificateTemplate = GetNumOfCertificatetemplate(certificateHTML);

            if (numOfCertificateTemplate != 0) {
                if (numOfCertificateTemplate == 1) {
                    var date = htmlDocument.DocumentNode.SelectSingleNode("//span[contains(@class, 'creation-date')]");
                    if (date != null) {
                        var existingStyle = date.GetAttributeValue("style", "");
                        if (existingStyle.Contains("left:")) {
                            var leftRegex = new Regex(@"left:\s*(\d+\.?\d*)%");
                            var match = leftRegex.Match(existingStyle);
                            if (match.Success) {
                                var leftValue = match.Groups[1].Value; 
                                var newLeftValue = "68.75";

                                var newStyle = existingStyle.Replace($"left: {leftValue}%;", $"left: {newLeftValue}%;");
                                date.SetAttributeValue("style", newStyle);
                            }
                        }
                    }
                }
            }

            return htmlDocument.DocumentNode.OuterHtml;
        }

        private int GetNumOfCertificatetemplate(string certificateHTML) {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(certificateHTML);

            int numOfCertificateTemplate = 0;
            var image = htmlDocument.DocumentNode.SelectSingleNode("//img[@class='image']");
            if (image != null) {
                var alt = image.GetAttributeValue("alt", "");
                numOfCertificateTemplate = Int32.Parse((alt[alt.Length - 1]).ToString());
            }

            return numOfCertificateTemplate;
        }

        private string ConvertImageToBase64(string certificateHTML) {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(certificateHTML);

            string numOfCertificateTemplate = GetNumOfCertificatetemplate(certificateHTML).ToString();

            
            var imagePath = $@"C:\Users\ARTEM\Desktop\Certificate Creator\certificate-creator\src\assets\images\certificate-templates\certificate-template-{numOfCertificateTemplate}.png";
            string base64Image = ImageToBase64(imagePath);
            string imgTag = $"data:image/png;base64,{base64Image}";

            
            var image = htmlDocument.DocumentNode.SelectSingleNode("//img[@class='image']");
            if (image != null) {
                image.SetAttributeValue("src", imgTag);
            }

            return htmlDocument.DocumentNode.OuterHtml;
        }

        private string ImageToBase64(string imagePath) {
            byte[] imageArray = File.ReadAllBytes(imagePath);
            return Convert.ToBase64String(imageArray);
        }

        private string IncreaseFontSize(string certificateHTML) {
            double scaleFactor = 1.6;

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(certificateHTML);

            var certificateFonts = htmlDocument.DocumentNode.SelectNodes("//*[contains(@class, 'certificate-font')]");
            if (certificateFonts != null) {
                foreach (var certificateFont in certificateFonts) {
                    var existingStyle = certificateFont.GetAttributeValue("style", "");
                    if (!string.IsNullOrWhiteSpace(existingStyle) && existingStyle.Contains("font-size:")) {
                        var match = Regex.Match(existingStyle, @"font-size:\s*([\d\.]+)vw");
                        if (match.Success) {
                            var fontSizeStr = match.Groups[1].Value;
                            if (Double.TryParse(fontSizeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double fontSize)) {
                                var newFontSize = fontSize * scaleFactor;

                                var newStyle = Regex.Replace(existingStyle, @"font-size:\s*[\d\.]+vw", $"font-size: {newFontSize.ToString("0.##", CultureInfo.InvariantCulture)}vw");
                                certificateFont.SetAttributeValue("style", newStyle);
                            }
                            else {
                                Console.WriteLine($"Could not parse font size: {fontSizeStr}");
                            }
                        }
                        else {
                            Console.WriteLine($"No font size in style: {existingStyle}");
                        }
                    }
                    else {
                        Console.WriteLine($"No style or font-size in style for element: {certificateFont.OuterHtml}");
                    }
                }
            }
            else {
                Console.WriteLine("No elements with class 'certificate-font' found.");
            }
            return htmlDocument.DocumentNode.OuterHtml;
        }



        private async Task<CreateCertificateDTO> ConvertCertificateToPDFAsync(string certificateHTML, Guid certificateId) {
            
            string folderPath = @"C:/Users/ARTEM/Desktop\Web";
            string fileName = "Certificate_" + certificateId.ToString() + ".pdf";
            string fullPath = Path.Combine(folderPath, fileName);

            
            var browserFetcherOptions = new BrowserFetcherOptions();
            using var browserFetcher = new BrowserFetcher(browserFetcherOptions);
            await browserFetcher.DownloadAsync();

            
            var options = new LaunchOptions { Headless = true };
            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = await browser.NewPageAsync()) {
                
                await page.SetContentAsync(certificateHTML);

                
                var pdfOptions = new PdfOptions {
                    Format = PaperFormat.A4,
                    Landscape = true,
                    MarginOptions = { Top = "0px", Bottom = "0px", Left = "0px", Right = "0px" }
                };
                byte[] pdfBytes = await page.PdfDataAsync(pdfOptions);

                
                File.WriteAllBytes(fullPath, pdfBytes);

                
                var certificateDto = new CreateCertificateDTO { PDFCertificate = pdfBytes };
                return certificateDto;
            }
        }

    }
}
