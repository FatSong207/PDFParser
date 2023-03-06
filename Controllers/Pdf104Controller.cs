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

namespace PdfParser.API.Controllers
{
    /// <summary>
    /// An example controller for testing <code>multipart/form-data</code> submission
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class Pdf104Controller : ControllerBase
    {
        private readonly ILogger<Pdf104Controller> _logger;

        public Pdf104Controller(ILogger<Pdf104Controller> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 上傳PDF 104格式.
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <param name="form">A form which contains the FormId and a file</param>
        /// <returns></returns>
        [HttpPost("forms")]
        [ProducesResponseType(typeof(PdfForm104SubmissionResult), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PdfForm104SubmissionResult>> SubmitForm104([FromForm] PdfForm form)
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
            PdfParser.Format104.Format Format = null;
            try
            {
                PdfParser.Format104.Format fromat = null;
                using (var fileSteam = form.PdfFile.OpenReadStream())
                {
                    fromat = PdfParser.Format104.Body.Format(fileSteam);
                }
                //Format = JsonSerializer.Serialize<PdfParser.Format104.Format>(fromat, options);
                Format = fromat;
                Result = "OK";

                if(new LicenceCryption().CheckLicense(form.LicenseId, form.LicenseKey))
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

            var result = new PdfForm104SubmissionResult { Result = Result, ResultDetail = ResultDetail, Format = Format };
            //return CreatedAtAction(nameof(ViewForm), new { }, result);
            return CreatedAtAction("SubmitForm104", new { }, result);
        }



        public class PdfForm
        {
            [Required] public string LicenseId { get; set; }
            [Required] public string LicenseKey { get; set; }
            [Required] public IFormFile PdfFile { get; set; }
        }

        public class PdfForm104SubmissionResult
        {
            public string Result { get; set; }
            //public string Format { get; set; }
            public string ResultDetail { get; set; }
            public PdfParser.Format104.Format Format { get; set; }
        }
    }
}

