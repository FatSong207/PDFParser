using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PdfParser.Cryption;
using PdfParser.Formatyes123;

namespace PdfParserWeb.API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class Pdfyes123Controller : ControllerBase
    {
        private readonly ILogger<Pdfyes123Controller> _logger;

        public Pdfyes123Controller(ILogger<Pdfyes123Controller> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost("forms")]
        [ProducesResponseType(typeof(PdfResult), StatusCodes.Status201Created)]
        public PdfResult SubmitFormYes123([FromForm] PdfForm form)
        {
            _logger.LogInformation($"saving file [{form.PdfFile.FileName}]");
            //await Task.Delay(1500);
            _logger.LogInformation("file upload.");
            var result = new PdfResult();

            try {
                using (var fileSteam = form.PdfFile.OpenReadStream()) {
                    result.Format = Format.ParsePDF(fileSteam);
                    result.Result = "OK";
                }
                if (new LicenceCryption().CheckLicense(form.LicenseId, form.LicenseKey)) {
                    //合法授權
                } else {
                    //未授權
                    result.ResultDetail = "未授權";
                    result.Format.UserInfo.UserName = LicenceCryption.DoMask(result.Format.UserInfo.UserName);
                }
            } catch (System.Exception e) {
                result.Result = e.Message;
                result.ResultDetail = e.StackTrace;
            }

            return result;
        }
        public class PdfForm
        {
            [Required] public string LicenseId { get; set; }
            [Required] public string LicenseKey { get; set; }
            [Required] public IFormFile PdfFile { get; set; }
        }
        public class PdfResult
        {
            public string Result { get; set; }
            //public string Format { get; set; }
            public string ResultDetail { get; set; }
            public Format_yes123 Format { get; set; }
        }
    }
}
