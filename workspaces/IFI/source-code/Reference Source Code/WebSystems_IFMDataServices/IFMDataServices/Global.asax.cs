using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using DevExpress.XtraSpreadsheet.Model;
using Diamond.Business.ThirdParty.IAALifeCycleUpdateObjects;
using IFM.PrimitiveExtensions;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFMDataServices
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private const string location = "IFMDATASERVICES.Global.MvcApplication";
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            JsonValueProviderConfig.Config(ValueProviderFactories.Factories);
            GlobalFilters.Filters.Add(new GlobalActionFilterAttribute());
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            HttpContext ctx = HttpContext.Current;
            System.Uri url = ctx.Request.Url;
#if !DEBUG
            global::IFM.IFMErrorLogging.LogException(Server.GetLastError(), $"{location}.Application_Error - Requested URL: '{url}' - Request Raw URL: '{ctx.Request.RawUrl}'");
#else
            Exception exception = Server.GetLastError();
#endif
        }

    }

    public class GlobalActionFilterAttribute : FilterAttribute, IActionFilter, IExceptionFilter
    {
        private const string location = "IFMDATASERVICES.Global.GlobalActionFilterAttribute";
        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                IFM.IFMErrorLogging.LogException(filterContext.Exception, $"{location}.OnException: " + filterContext.RouteData.Values["controller"].ToString());
                filterContext.ExceptionHandled = true;
            }
        }

        //Response
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            CommonHelperClass chc = new CommonHelperClass();
            bool doLogging = chc.GetApplicationXMLSettingForBoolean("API_StoreRequestAndReponseJSON", "APISettings.xml");
            if (doLogging)
            {
                LogResponse(filterContext);
            }
        }

        //Request
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            CommonHelperClass chc = new CommonHelperClass();
            bool doLogging = chc.GetApplicationXMLSettingForBoolean("API_StoreRequestAndReponseJSON", "APISettings.xml");
            if (doLogging)
            {
                LogRequest(filterContext);
            }
        }

        private void LogRequest(ActionExecutingContext filterContext)
        {
            List<System.Data.SqlClient.SqlParameter> myInputParams = new List<System.Data.SqlClient.SqlParameter>();
            List<System.Data.SqlClient.SqlParameter> myOutputParams = new List<System.Data.SqlClient.SqlParameter>();
            
            myInputParams.Add(new System.Data.SqlClient.SqlParameter("@requestURL", filterContext.HttpContext.Request.Url.AbsoluteUri));
            myInputParams.Add(new System.Data.SqlClient.SqlParameter("@requestMethod", filterContext.HttpContext.Request.HttpMethod));
            myInputParams.Add(new System.Data.SqlClient.SqlParameter("@requestController", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName));
            myInputParams.Add(new System.Data.SqlClient.SqlParameter("@requestAction", filterContext.ActionDescriptor.ActionName));
            myInputParams.Add(new System.Data.SqlClient.SqlParameter("@userIP", filterContext.HttpContext.Request.UserHostAddress));
            myInputParams.Add(new System.Data.SqlClient.SqlParameter("@userHostName", filterContext.HttpContext.Request.UserHostName));

            if (filterContext.HttpContext.Request.HttpMethod != "GET")
            {
                if (filterContext?.ActionParameters?.IsLoaded() == true && filterContext?.ActionParameters?.Values?.IsLoaded() == true)
                {
                    myInputParams.Add(new System.Data.SqlClient.SqlParameter("@requestJSON", FilterOutItemsForRequestSerialization(filterContext)));
                }
            }

            myOutputParams.Add(new System.Data.SqlClient.SqlParameter("@transHistoryID", System.Data.SqlDbType.Int));
            myOutputParams.Add(new System.Data.SqlClient.SqlParameter("@transRequestJSONID", System.Data.SqlDbType.Int));

            using (var sqle = new SQLexecuteObject(IFM.DataServicesCore.BusinessLogic.AppConfig.ConnDiamondReports, "usp_IFMDataServices_StoreRequest"))
            {
                sqle.InputParameters_TypeSafe = myInputParams;
                sqle.OutputParameters_TypeSafe = myOutputParams;
                sqle.ExecuteStatement();

                if (sqle.OutputParameters_TypeSafe.IsLoaded())
                {
                    foreach (var output in sqle.OutputParameters_TypeSafe)
                    {
                        if (output != null && output.Value != null && output.Value.ToString().HasValue())
                        {
                            if (output.ParameterName == "@transHistoryID")
                                HttpContext.Current.Session.Add("ApiTransHistoryID", output.Value.ToString().TryToGetInt32());
                            else if (output.ParameterName == "@transRequestJSONID")
                                HttpContext.Current.Session.Add("ApiJsonRequestID", output.Value.ToString().TryToGetInt32());
                        }
                    }
                }

                if (sqle.errorMsg.IsNotNullEmptyOrWhitespace())
                {
                    IFM.IFMErrorLogging.LogIssue(sqle.errorMsg, location + ".LogRequest");
                }
            }    
        }

        private void LogResponse(ActionExecutedContext filterContext)
        {
            string apiTransHistoryId = "1";
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["ApiTransHistoryID"] != null && HttpContext.Current.Session["ApiTransHistoryID"].ToString().HasValue())
                apiTransHistoryId = HttpContext.Current.Session["ApiTransHistoryID"].ToString();

            if (apiTransHistoryId.HasValue())
            {
                List<System.Data.SqlClient.SqlParameter> myInputParams = new List<System.Data.SqlClient.SqlParameter>();

                if (HttpContext.Current.Session != null && HttpContext.Current.Session["EndorsementTransHistoryID"] != null && HttpContext.Current.Session["EndorsementTransHistoryID"].ToString().HasValue())
                    myInputParams.Add(new System.Data.SqlClient.SqlParameter("@endorsementTransID", HttpContext.Current.Session["EndorsementTransHistoryID"].ToString()));

                myInputParams.Add(new System.Data.SqlClient.SqlParameter("@transHistoryID", apiTransHistoryId));

                if (filterContext != null && filterContext.Result != null)
                {
                    if (filterContext.Result.GetType() == typeof(IFM.DataServices.Controllers.JsonNetResult))
                    {
                        IFM.DataServices.Controllers.JsonNetResult myResult = (IFM.DataServices.Controllers.JsonNetResult)filterContext.Result;
                        if (myResult.Data != null)
                        {
                            AddGenericDataFromJsonNetResultObjectToInputParamter(myResult, myInputParams); //Should probably just use this for everything... No reason to specify every object.... This will at least capture all the extras now.
                        }
                        else
                        {
                            myInputParams.Add(new System.Data.SqlClient.SqlParameter("@responseJSON", $@"{{""myResultData"":""NULL"""));
                        }
                    }
                    else if (filterContext.Result.GetType() == typeof(FileStreamResult))
                    {
                        FileStreamResult myResult = (FileStreamResult)filterContext.Result;
                        myInputParams.Add(new System.Data.SqlClient.SqlParameter("@responseJSON", $@"{{""FileDownloadName"":""{myResult.FileDownloadName}"", ""ContentType"":""{myResult.ContentType}""}}"));
                    }
                    else
                    {
                        AddGenericDataFromActionResultObjectToInputParamter(filterContext.Result, myInputParams);
                    }
                }

                if (myInputParams.IsLoaded())
                {
                    using (var sqle = new SQLexecuteObject(IFM.DataServicesCore.BusinessLogic.AppConfig.ConnDiamondReports, "usp_IFMDataServices_StoreResponse"))
                    {
                        sqle.InputParameters_TypeSafe = myInputParams;
                        sqle.ExecuteStatement();

                        if (sqle.errorMsg.IsNotNullEmptyOrWhitespace())
                        {
                            IFM.IFMErrorLogging.LogIssue(sqle.errorMsg, location + ".LogResponse");
                        }
                    }
                }
            }
        }

        public void AddGenericDataFromJsonNetResultObjectToInputParamter(IFM.DataServices.Controllers.JsonNetResult myResult, List<System.Data.SqlClient.SqlParameter> myInputParams)
        {
            var myData = myResult.Data;
            if (myData != null)
            {
                myInputParams.Add(new System.Data.SqlClient.SqlParameter("@responseJSON", FilterOutItemsForResponseSerialization(myData)));
            }
        }

        public void AddGenericDataFromActionResultObjectToInputParamter(ActionResult myResult, List<System.Data.SqlClient.SqlParameter> myInputParams)
        {
            if (myResult != null)
            {
                myInputParams.Add(new System.Data.SqlClient.SqlParameter("@responseJSON", Newtonsoft.Json.JsonConvert.SerializeObject(myResult)));
            }
        }

        private static string FilterOutItemsForRequestSerialization(ActionExecutingContext filterContext)
        {
            foreach (var val in filterContext.ActionParameters.Values) //it's a list... but I think there is only ever one item... could there be more??? multiple objects sent in body? Will only work with one item right now.
            {
                if (val?.GetType() == typeof(IFM.DataServices.API.RequestObjects.OnBase.DocumentUpload))
                {
                    var myNewVal = (IFM.DataServices.API.RequestObjects.OnBase.DocumentUpload)TryDeepCloneObject(val);
                    if (myNewVal != null)
                    {
                        myNewVal.FileBytes = new byte[] { 0x20 };
                        return Newtonsoft.Json.JsonConvert.SerializeObject(myNewVal);
                    }
                    else
                    {
                        IFM.IFMErrorLogging.LogIssue("Cloned object is null", location + ".FilterOutItemsForRequestSerialization");
                    }
                }
                else if (val?.GetType() == typeof(IFM.DataServicesCore.CommonObjects.OnBase.UploadPayload))
                {
                    var myNewVal = (IFM.DataServicesCore.CommonObjects.OnBase.UploadPayload)TryDeepCloneObject(val);
                    if(myNewVal != null)
                    {
                        myNewVal.Base64Payload = "Payload removed for logging";
                        return Newtonsoft.Json.JsonConvert.SerializeObject(myNewVal);
                    }
                    else
                    {
                        IFM.IFMErrorLogging.LogIssue("Cloned object is null", location + ".FilterOutItemsForRequestSerialization");
                    }
                }
                else if (val?.GetType() == typeof(IFM.DataServicesCore.CommonObjects.Payments.MemberPortalPaymentData))
                {
                    var myNewVal = (IFM.DataServicesCore.CommonObjects.Payments.MemberPortalPaymentData)TryDeepCloneObject(val);
                    if (myNewVal != null)
                    {
                        if (myNewVal.ECheckPaymentInformation != null)
                        {
                            bool newValsSet = false;
                            if (myNewVal.ECheckPaymentInformation.AccountNumber?.IsNumeric() == true)
                            {
                                newValsSet = true;
                                myNewVal.ECheckPaymentInformation.AccountNumber = myNewVal.ECheckPaymentInformation.AccountNumber.DoubleEncrypt();
                            }
                            if (myNewVal.ECheckPaymentInformation.RoutingNumber?.IsNumeric() == true)
                            {
                                newValsSet = true;
                                myNewVal.ECheckPaymentInformation.RoutingNumber = myNewVal.ECheckPaymentInformation.RoutingNumber.DoubleEncrypt();
                            }
                            if (newValsSet)
                            {
                                return Newtonsoft.Json.JsonConvert.SerializeObject(myNewVal);
                            }
                        }
                    }
                    else
                    {
                        IFM.IFMErrorLogging.LogIssue("Cloned object is null", location + ".FilterOutItemsForRequestSerialization");
                    }
                }
                else if (val?.GetType() == typeof(IFM.DataServicesCore.CommonObjects.OMP.FNOL))
                {
                    
                    var myNewVal = (IFM.DataServicesCore.CommonObjects.OMP.FNOL)TryDeepCloneObject(val);
                    if (myNewVal != null)
                    {
                        if (myNewVal.FNOLAttachements?.Count > 0)
                        {
                            foreach (var attach in myNewVal.FNOLAttachements)
                            {
                                attach.FileBytes = new byte[] { 0x20 };
                            }
                            return Newtonsoft.Json.JsonConvert.SerializeObject(myNewVal);
                        }
                    }
                }
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(filterContext.ActionParameters.Values);
        }

        private static T TryDeepCloneObject<T>(T myObject)
        {
            try
            {
                return myObject.DeepCloneObject();
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#else
                IFM.IFMErrorLogging.LogException(ex, $"{location}.TryDeepCloneObject; {myObject.GetType()}");
#endif

                return default(T);
            }
        }

        private static string FilterOutItemsForResponseSerialization(object responseData)
        {
            if (responseData.GetType() == typeof(byte[]))
            {
                var myNewVal = new byte[] { 0x20 };
                return Newtonsoft.Json.JsonConvert.SerializeObject(myNewVal);
            }
            else if (responseData.GetType() == typeof(FileStreamResult))
            {
                var myNewVal = new FileStreamResult(new MemoryStream(Encoding.UTF8.GetBytes("Print removed for logging.")), "text/html");
                return Newtonsoft.Json.JsonConvert.SerializeObject(myNewVal);
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(responseData);
        }
    }

    public class JsonValueProviderConfig
    {
        public static void Config(ValueProviderFactoryCollection factories)
        {
            var jsonProviderFactory = factories.OfType<JsonValueProviderFactory>().Single();
            factories.Remove(jsonProviderFactory);
            factories.Add(new CustomJsonValueProviderFactory());
        }
    }
    public class CustomJsonValueProviderFactory : ValueProviderFactory
    {

        /// <summary>Returns a JSON value-provider object for the specified controller context.</summary>
        /// <returns>A JSON value-provider object for the specified controller context.</returns>
        /// <param name="controllerContext">The controller context.</param>
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");

            object deserializedObject = CustomJsonValueProviderFactory.GetDeserializedObject(controllerContext);
            if (deserializedObject == null)
                return null;

            Dictionary<string, object> strs = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            CustomJsonValueProviderFactory.AddToBackingStore(new CustomJsonValueProviderFactory.EntryLimitedDictionary(strs), string.Empty, deserializedObject);

            return new DictionaryValueProvider<object>(strs, CultureInfo.CurrentCulture);
        }

        private static object GetDeserializedObject(ControllerContext controllerContext)
        {
            if (!controllerContext.HttpContext.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
                return null;

            string fullStreamString = (new StreamReader(controllerContext.HttpContext.Request.InputStream)).ReadToEnd();
            if (string.IsNullOrEmpty(fullStreamString))
                return null;

            var serializer = new JavaScriptSerializer()
            {
                MaxJsonLength = CustomJsonValueProviderFactory.GetMaxJsonLength()
            };
            return serializer.DeserializeObject(fullStreamString);
        }

        private static void AddToBackingStore(EntryLimitedDictionary backingStore, string prefix, object value)
        {
            IDictionary<string, object> strs = value as IDictionary<string, object>;
            if (strs != null)
            {
                foreach (KeyValuePair<string, object> keyValuePair in strs)
                    CustomJsonValueProviderFactory.AddToBackingStore(backingStore, CustomJsonValueProviderFactory.MakePropertyKey(prefix, keyValuePair.Key), keyValuePair.Value);

                return;
            }

            IList lists = value as IList;
            if (lists == null)
            {
                backingStore.Add(prefix, value);
                return;
            }

            for (int i = 0; i < lists.Count; i++)
            {
                CustomJsonValueProviderFactory.AddToBackingStore(backingStore, CustomJsonValueProviderFactory.MakeArrayKey(prefix, i), lists[i]);
            }
        }

        private class EntryLimitedDictionary
        {
            private static int _maximumDepth;

            private readonly IDictionary<string, object> _innerDictionary;

            private int _itemCount;

            static EntryLimitedDictionary()
            {
                _maximumDepth = CustomJsonValueProviderFactory.GetMaximumDepth();
            }

            public EntryLimitedDictionary(IDictionary<string, object> innerDictionary)
            {
                this._innerDictionary = innerDictionary;
            }

            public void Add(string key, object value)
            {
                int num = this._itemCount + 1;
                this._itemCount = num;
                if (num > _maximumDepth)
                {
                    throw new InvalidOperationException("The length of the string exceeds the value set on the maxJsonLength property.");
                }
                this._innerDictionary.Add(key, value);
            }
        }

        private static string MakeArrayKey(string prefix, int index)
        {
            return string.Concat(prefix, "[", index.ToString(CultureInfo.InvariantCulture), "]");
        }

        private static string MakePropertyKey(string prefix, string propertyName)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return propertyName;
            }
            return string.Concat(prefix, ".", propertyName);
        }

        private static int GetMaximumDepth()
        {
            int num;
            NameValueCollection appSettings = ConfigurationManager.AppSettings;
            if (appSettings != null)
            {
                string[] values = appSettings.GetValues("aspnet:MaxJsonDeserializerMembers");
                if (values != null && values.Length != 0 && int.TryParse(values[0], out num))
                {
                    return num;
                }
            }
            return 1000;
        }

        private static int GetMaxJsonLength()
        {
            int num;
            NameValueCollection appSettings = ConfigurationManager.AppSettings;
            if (appSettings != null)
            {
                string[] values = appSettings.GetValues("aspnet:MaxJsonLength");
                if (values != null && values.Length != 0 && int.TryParse(values[0], out num))
                {
                    return num;
                }
            }
            return Int32.MaxValue;
        }
    }

}
