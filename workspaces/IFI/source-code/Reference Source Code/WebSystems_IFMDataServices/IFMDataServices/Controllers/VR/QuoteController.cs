using QuickQuote.CommonMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IFM.DataServices.Controllers.VR
{
    [RoutePrefix("VR/Quote")]
    public class QuoteController : BaseController
    {
        readonly QuickQuoteXML qqxml = new QuickQuoteXML();

        [AcceptVerbs(HttpVerbs.Get)]
        [Route("{quoteid}/{saveType?}")]
        public JsonResult Get(int quoteId, QuickQuoteXML.QuickQuoteSaveType saveType = QuickQuoteXML.QuickQuoteSaveType.Quote)
        {
            // This is junk it will fail because the qq is not serializeable

            var sr = this.CreateServiceResult();            

            QuickQuote.CommonObjects.QuickQuoteObject qq = null;
            string err = null;
            qqxml.GetQuoteForSaveType(quoteId.ToString(), saveType, ref qq, ref err);
            sr.Messages.CreateGeneralMessage($"Requested quoteid of {quoteId}");
            sr.ResponseData = qq;
            if (!string.IsNullOrWhiteSpace(err))
                sr.Messages.CreateErrorMessage(err);
            CodeOk();
            return Json(sr);
        }
    }
}