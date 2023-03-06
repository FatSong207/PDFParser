using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static PdfParser.API.Controllers.Pdf104Controller;
using PdfParser.Cryption;

namespace PdfParser.API.Controllers
{
    /// <summary>
    /// An example controller for testing <code>multipart/form-data</code> submission
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PdfLinkedinController : ControllerBase
    {
        private readonly ILogger<PdfLinkedinController> _logger;

        public PdfLinkedinController(ILogger<PdfLinkedinController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 上傳PDF Linkedin格式.
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <param name="form">A form which contains the FormId and a file</param>
        /// <returns></returns>
        [HttpPost("forms")]
        [ProducesResponseType(typeof(PdfFormLinkedinSubmissionResult), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PdfFormLinkedinSubmissionResult>> SubmitFormLinkedin([FromForm] PdfForm form)
        {
            
            _logger.LogInformation($"saving file [{form.PdfFile.FileName}]");
            //await Task.Delay(1500);
            _logger.LogInformation("file upload.");

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            };

            //string Format = "";
            string Result = "";
            string ResultDetail = "";
            PdfParser.FormatLinkedin.FormatLinkedin Format = null;
            
            try
            {
                PdfParser.FormatLinkedin.FormatLinkedin fromat = null;
                using (var fileSteam = form.PdfFile.OpenReadStream())
                {
                    fromat = PdfParser.FormatLinkedin.Body.Format(fileSteam);
                }
                //Format = JsonSerializer.Serialize<PdfParser.Format104.Format>(fromat, options);
                Format = fromat;
                Result = "OK";

                if (new LicenceCryption().CheckLicense(form.LicenseId, form.LicenseKey))
                {
                    //合法授權
                }
                else
                {
                    //未授權
                    ResultDetail = "未授權";
                    Format.UserName = LicenceCryption.DoMask(Format.UserName);
                }

            }
            catch (System.Exception e)
            {
                Result = "Exception";
                Result = e.Message;
                ResultDetail = e.StackTrace;
            }
            
            //var result = new PdfFormLinkedinSubmissionResult { Result = Result, ResultDetail = ResultDetail, Format = Format };
            var result = new PdfFormLinkedinSubmissionResult { Result = Result, ResultDetail = ResultDetail, Format = Format };
            return CreatedAtAction("SubmitFormLinkedin", new { }, result);
        }


        
        public class PdfForm
        {
            [Required] public string LicenseId { get; set; }
            [Required] public string LicenseKey { get; set; }
            [Required] public IFormFile PdfFile { get; set; }
        }

        public class PdfFormLinkedinSubmissionResult
        {
            public string Result { get; set; }
            //public string Format { get; set; }
            public string ResultDetail { get; set; }
            public PdfParser.FormatLinkedin.FormatLinkedin Format { get; set; }
        }
        
    }

}

