using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace IFM.DataServices.Controllers.OMP.Print
{
    [RoutePrefix("OMP/Print")]
    public class OMPPrint_PDFPrintController : BaseController
    {

        //<Authorize>
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(Duration = 30, VaryByParam = "*")]
        [Route("Document/{policyId}/{xmlId}/{description}/{printFormNumber}")]
        public FileStreamResult GetFormPDF(int policyId, int xmlId, string description, string printFormNumber)
        {
            try
            {
                string decodedDescription = System.Web.HttpUtility.UrlDecode(description);
                byte[] pdfByte = global::IFM.DataServicesCore.BusinessLogic.OMP.PrintForms.GetFormBytes(policyId, xmlId, description, printFormNumber, decodedDescription);
                if (pdfByte != null)
                {
                    MemoryStream workStream = new MemoryStream(pdfByte);
                    CodeOk();
                    return new FileStreamResult(workStream, "application/pdf");
                }
                else
                {
                    CodeOk();
                    return new FileStreamResult(new MemoryStream(Encoding.UTF8.GetBytes("Print not found.")), "text/html");
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return new FileStreamResult(new MemoryStream(Encoding.UTF8.GetBytes("Print not found.")), "text/html");
            }
        }

        //<Authorize>
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(Duration = 30, VaryByParam = "*")]
        [Route("DocumentByPrintJobID/{policyId}/{printJobId}/{description}/{printFormNumber}")]
        public FileStreamResult GetFormPDFByPrintJobId(int policyId, int printJobId, string description, string printFormNumber)
        {
            try
            {
                string decodedDescription = System.Web.HttpUtility.UrlDecode(description);
                byte[] pdfByte = global::IFM.DataServicesCore.BusinessLogic.OMP.PrintForms.GetFormBytesByPrintJobId(policyId, printJobId, description, printFormNumber, decodedDescription);
                if (pdfByte != null)
                {
                    MemoryStream workStream = new MemoryStream(pdfByte);
                    CodeOk();
                    return new FileStreamResult(workStream, "application/pdf");
                }
                else
                {
                    CodeOk();
                    return new FileStreamResult(new MemoryStream(Encoding.UTF8.GetBytes("Print not found.")), "text/html");
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return new FileStreamResult(new MemoryStream(Encoding.UTF8.GetBytes("Print not found.")), "text/html");
            }
        }

        // uses query string to get around url encoding issue but will take Leaf help to make happen
        // might have to update PrintUrl in the PrintForms class (code there and just commented out)
        //[AcceptVerbs(HttpVerbs.Get)]
        //[OutputCache(Duration = 30, VaryByParam = "*")]
        //[Route("Documentv2")]
        //public FileStreamResult GetFormPDF(int policyId, int xmlId, string description, string printFormNumber)
        //{
        //    try
        //    {
        //        byte[] pdfByte = global::IFM.DataServicesCore.BusinessLogic.OMP.PrintForms.GetFormBytes(policyId, xmlId, description, printFormNumber);
        //        if (pdfByte != null)
        //        {
        //            MemoryStream workStream = new MemoryStream(pdfByte);
        //            CodeOk();
        //            return new FileStreamResult(workStream, "application/pdf");
        //        }
        //        else
        //        {
        //            CodeNotFound();
        //            return new FileStreamResult(new MemoryStream(Encoding.UTF8.GetBytes("Print not found.")), "text/html");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        //        return new FileStreamResult(new MemoryStream(Encoding.UTF8.GetBytes("Print not found.")), "text/html");
        //    }
        //}

    }

}