using MundiPagg.Importador.Core.DTO;
using MundiPagg.Importador.Core.Implementation;
using MundiPagg.Importador.WebApi.Formatters;
using MundiPagg.Importador.WebApi.Models;
using Newtonsoft.JsonResult;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;


namespace MundiPagg.Importador.WebApi.Controllers
{
    public class UploadController : ApiController
    {

        //[Route("authenticate")]
        //[EnableCors("*","*","*")]
        public async Task<JsonResult> Upload(FileUpload<UploadViewModel> model)
        {
            const string privKey = "<RSAKeyValue><Modulus>ry5CqAHxMZU1NfbMMHu5bguQrK5u8wKfuoK4zcMCiVTjdsHyt9u/3ysmg3cKfpCcnGDHiSqcrFOoEnSYeKeEz2vo/KyF330diMl4MdjsvmQX+nq4Lr2Tgy4OAJBTp4dZMlg7wgJs+gVqIbAzYwbOPTZI2TgjsPVcPx5XCym8OpU=</Modulus><Exponent>AQAB</Exponent><P>7foWyYG+KAwpP1ojFUg0/7j+0WQLkwzCX1F7/BwC1dU6x+10t/u8uzfnBXDFCFHU/FcN+0hv8xKfqwU6su+xVw==</P><Q>vHKstraurzentQLYYnE6kMGZ5uxWx44iBecmpeNmfTWQTHW/zg2YOZjsf5HpHhJJ28paoO8GZeNBxBwrmcgj8w==</Q><DP>Gthoc6jYK6pbiNMBPCheGi7jR/myOI9q6dfGVcVPKJIaRG1sCkkynCgNPpbfEzYwAZtMb3lXb6M9McywN3lElw==</DP><DQ>p2NhAMcSMTdJc2J8nCyhbdVaBCGoD4eztvSUgsOT1OIQM6pf3gJ7VaX9ZG9R11E9Y8rBZ7QJRdVwJGc1X1rEew==</DQ><InverseQ>o6c047+veG3SXG5Ze5JAhYRgDDF08jZK2RgpGoIh30tJBks3DkhSM3aecDsqBLEC4SscJI+787TJxulJ9rTkew==</InverseQ><D>BO//BUuSDULhLjqyXvbLGgIYcKLf/K3JFmbYfPAvUkiYEAD/wtUzmhTWGo7QjGhYtFhOohvshjeQvVMXoxBHACJLyoloS2rHm/9VK4S01Kwb5k/7lflU0ZY5MVMQ1nSp9bjQpzg8z2phhSBW2JpEqMN1mskXm5VMlf0PZtdV6Dc=</D></RSAKeyValue>";
            CartaoProcess c = new CartaoProcess();
            try
            {
                string arquivo = System.Text.Encoding.UTF8.GetString(model.Buffer);

                List<CartaoCredito> list = c.ProcessarPlanilha(privKey, arquivo);
                c.EnviarCartoes(list, Guid.Parse("a0ed84cb-8939-4015-a151-3f8c064ed83a"), "https://sandbox.mundipaggone.com");

                return new JsonResult() { Data = "OK" };
            }
            catch(Exception ex)
            {
                return new JsonResult() { Data = "Error" };
            }
        }
    }
}
