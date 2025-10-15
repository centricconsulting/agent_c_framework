using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IFM.PrimitiveExtensions;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Reflection;

namespace IFM.DataServices.Controllers.IFM.HtmlToPdf
{
    [RoutePrefix("IFM/HtmlToPdf")]
    public class IFMHtmlToPdf_ProcessorController : BaseController
    {



        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("pdf")]
        public FileStreamResult GetWebPageAsPdf(string url)
        {

            try
            {

                Uri uri = new Uri(url);
                // need to restrict to only ifm site others are unsafe

                if (uri.Host.EqualsAny("www.indianafarmers.com", "www.ifmig.net", "indianafarmers.com", "www.indianafarmers.com", "ifmig.net", "insurance.indianafarmers.com", "insurance.indianafarmers.biz"))
                {
                    string statustext = "";
                    byte[] pdfByte = GetHtmlPageasPdf(url, statustext);

                    if (pdfByte != null)
                    {
                        MemoryStream workStream = new MemoryStream(pdfByte);                        
                        return new FileStreamResult(workStream, "application/pdf");
                    }
                    else
                    {                        
                        return new FileStreamResult(new MemoryStream(Encoding.UTF8.GetBytes("Page not found.")), "text/html");
                    }
                }
                else
                {                    
                    return new FileStreamResult(new MemoryStream(Encoding.UTF8.GetBytes("Domain name invalid.")), "text/html");
                }




            }
            catch (Exception ex)
            {                
                return new FileStreamResult(new MemoryStream(Encoding.UTF8.GetBytes("Could not convert page or url malformed. Requires full url like 'http://www.yahoo.com' and must be an approved domain. ")), "text/html");
            }

        }

        public static byte[] GetHtmlPageasPdf(string url, string status)
        {
            byte[] pdfBytes = null;
            Random rnd = new Random();
            //Dim gfhf = HttpContext.Current.Server.MapPath(".")
            //string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string exePath = Path.Combine(HttpRuntime.AppDomainAppPath, "HtmlToPdf\\wkhtmltopdf.exe");
            string pdfPath = Path.Combine(HttpRuntime.AppDomainAppPath, "HtmlToPdf\\temp\\" + rnd.Next(50000, Int32.MaxValue).ToString() + ".pdf");


            string args = string.Format("--load-error-handling ignore \"{0}\" \"{1}\"", url, pdfPath);
            //Dim args = String.Format("--print-media-type --disable-internal-links --disable-external-links --debug-javascript --load-error-handling ignore --javascript-delay 1000 ""{0}"" ""{1}""", url, pdfPath)
            //Dim args = String.Format("--print-media-type  --load-error-handling ignore --javascript-delay 5000 ""{0}"" ""{1}""", url, pdfPath)

            //Dim args = String.Format("""{0}"" ""--load-error-handling ignore"" ""{1}""", url, pdfPath)
            RunExecutable(exePath, args, ref status);

            if (System.IO.File.Exists(pdfPath) == true)
            {
                using (FileStream fs_pdf = new FileStream(pdfPath, FileMode.Open, FileAccess.Read))
                {
                    pdfBytes = new byte[fs_pdf.Length];
                    fs_pdf.Read(pdfBytes, 0, System.Convert.ToInt32(fs_pdf.Length));
                    fs_pdf.Close();
                    System.IO.File.Delete(pdfPath);
                }
            }

            return pdfBytes;
        }

        private static void RunExecutable(string executable, string arguments, ref string status)
        {
            //using same code as originally used in C:\Users\domin\Documents\Visual Studio 2005\WebSites\TestExecutableCall
            status = "";

            using (Process process = new Process())
            {
                ProcessStartInfo starter = new ProcessStartInfo(executable, arguments)
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };
                process.StartInfo = starter;
                               

                process.Start();
                //updated to use variable
                int waitForExitMilliseconds = 15000;

                process.WaitForExit(waitForExitMilliseconds);

                while (process.HasExited == false)
                {
                    process.CloseMainWindow();
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception ex)
                    {
                        status = "<b>Kill failed-</b>";
                    }
                }

                string strOutput = process.StandardOutput.ReadToEnd();
                string strError = process.StandardError.ReadToEnd();

                if ((process.ExitCode != 0))
                {
                    status += "Error<br><u>Output</u> - " + strOutput + "<br><u>Error</u> - " + strError;
                }
                else
                {
                    status += "Success<br><u>Output</u> - " + strOutput + "<br><u>Error</u> - " + strError;
                }

                process.Close();                
            }

        }



    }
}