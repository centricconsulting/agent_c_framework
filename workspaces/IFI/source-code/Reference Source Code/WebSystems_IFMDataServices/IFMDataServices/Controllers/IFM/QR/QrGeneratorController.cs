using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace IFM.DataServices.Controllers.IFM.QR
{
    [RoutePrefix("IFM/QR")]
    public class QrGeneratorController : BaseController
    {


        [AcceptVerbs(HttpVerbs.Get)]
        [Route("generate/{data}")]
        public FileStreamResult Generate(string data)
        {
            var qrCodeEncoder = new ThoughtWorks.QRCode.Codec.QRCodeEncoder();
            string encoding = "Byte";
            if (encoding == "Byte")
            {
                qrCodeEncoder.QRCodeEncodeMode = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ENCODE_MODE.BYTE;
            }
            else
            {
                if (encoding == "AlphaNumeric")
                {
                    qrCodeEncoder.QRCodeEncodeMode = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
                }
                else
                {
                    if (encoding == "Numeric")
                    {
                        qrCodeEncoder.QRCodeEncodeMode = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ENCODE_MODE.NUMERIC;
                    }
                }
            }

            try
            {
                Int32 scale = Convert.ToInt16(4);
                qrCodeEncoder.QRCodeScale = scale;
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return null;
            }
            try
            {
                Int16 version = Convert.ToInt16("7");
                qrCodeEncoder.QRCodeVersion = version;

            }
            catch (Exception ex)
            {
            }

            string errorCorrect = "M";
            if ((errorCorrect == "L"))
            {
                qrCodeEncoder.QRCodeErrorCorrect = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.L;
            }
            else if ((errorCorrect == "M"))
            {
                qrCodeEncoder.QRCodeErrorCorrect = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.M;
            }
            else if ((errorCorrect == "Q"))
            {
                qrCodeEncoder.QRCodeErrorCorrect = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.Q;
            }
            else if ((errorCorrect == "H"))
            {
                qrCodeEncoder.QRCodeErrorCorrect = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.H;
            }


            System.Drawing.Bitmap image = null;
            image = qrCodeEncoder.Encode(data);

            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

            MemoryStream workStream = new MemoryStream(stream.GetBuffer());
            CodeOk();
            return new FileStreamResult(workStream, "image/jpeg");
        }

    }
}