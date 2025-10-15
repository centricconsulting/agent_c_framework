using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using APIResponse = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Controllers
{
    public class BaseController : Controller
    {
        public APIResponse.Common.ServiceResult CreateServiceResult()
        {
            return new APIResponse.Common.ServiceResult();
        }

        protected void AddFluentErrorsToServiceResult(APIResponse.Common.MessagesList ml, IEnumerable<FluentValidation.Results.ValidationFailure> fErrors)
        {
            if (ml != null && fErrors != null)
            {
                foreach (var Er in fErrors)
                {
                    ml.CreateErrorMessage(Er.ErrorMessage);
                }
            }
        }

        protected void AddFluentErrorsToServiceResult(APIResponse.Common.ServiceResult sr, IEnumerable<FluentValidation.Results.ValidationFailure> fErrors)
        {
            if (sr != null && fErrors != null)
            {
                foreach (var Er in fErrors)
                {
                    sr.Messages.CreateErrorMessage(Er.ErrorMessage);
                }
            }
        }

        //CANNOT BE PUBLIC!!!... Will cause the API to have runtime errors when called if changed to public
        protected void AddFluentErrorsToServiceResult<T>(APIResponse.Common.ServiceResult<T> sr, IEnumerable<FluentValidation.Results.ValidationFailure> fErrors)
        {
            if (sr != null && fErrors != null)
            {
                foreach (var Er in fErrors)
                {
                    sr.Messages.CreateErrorMessage(Er.ErrorMessage);
                }
            }
        }

        new public JsonNetResult Json(object data)
        {
            JsonNetResult JsonNetResult = new JsonNetResult
            {
                Data = data
            };
            return JsonNetResult;
        }

        //Public Function GetErrorListFromModelState(modelState As ModelStateDictionary) As List(Of String)
        //    Dim query = From state In modelState.Values
        //                From [errors] In state.Errors
        //                Select [errors].ErrorMessage

        //    Dim errorList = query.ToList()
        //    Return errorList
        //End Function


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Dim domains = New List(Of String)() From {
            //    "domain2.com",
            //    "domain1.com"
            //}

            //If domains.Contains(filterContext.RequestContext.HttpContext.Request.UrlReferrer.Host) Then
            //    filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*")
            //End If

            //MyBase.OnActionExecuting(filterContext)
        }

        protected void CodeOk()
        {
            Response.StatusCode = (int)HttpStatusCode.OK;
        }

        protected void CodeNotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        protected void CodeBadRequest()
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
}