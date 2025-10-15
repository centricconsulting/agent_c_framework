using IFI.Integrations.Response.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using IFI.Integrations.InternalExtensions;
using System.Security.Authentication;
using System.Reflection;
using System.Linq.Expressions;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace IFI.Integrations.Objects
{
    public class APIClass
    {
        internal string API_Address;
        internal string API_Endpoint;
        internal string API_Path;
        internal string API_EndpointPathUsed;
        internal Dictionary<string, string> HeadersToAdd;

        [XmlIgnore]
        public string InternalExceptionMessages
        {
            get
            {
                return GetExceptionMessages();
            }
        }

        [NonSerialized]
        public Exception InternalException;
        [NonSerialized]
        internal string lastLocation;
        [NonSerialized]
        internal System.Net.Http.HttpResponseMessage recievedResponse;
        [NonSerialized]
        internal object deserializedResponseObj;
        [NonSerialized]
        private readonly JsonSerializerSettings CamelCaseFormatter = new JsonSerializerSettings();

        internal APIClass(string API_Address)
        {
            this.API_Address = API_Address;
        }

        internal string GetJsonAsStringForRequestObject(object payload)
        {
            CamelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return JsonConvert.SerializeObject(payload, CamelCaseFormatter);
        }

        public string GetDebugInfo()
        {
            string myDebugInfo = "";
            myDebugInfo = myDebugInfo.AppendText(recievedResponse.ReasonPhrase, "; ", "ResponseReason: ");
            myDebugInfo = myDebugInfo.AppendText(lastLocation, "; ", "Locations: ");
            if (InternalException != null)
            {
                myDebugInfo = myDebugInfo.AppendText(InternalException.Message, "; ", "ExceptionMessage: ");
            }
            if (deserializedResponseObj != null)
            {
                myDebugInfo = myDebugInfo.AppendText(Newtonsoft.Json.JsonConvert.SerializeObject(deserializedResponseObj), "; ", "deserializedResponse-reserialized: ");
            }
            return myDebugInfo;
        }

        public bool ServiceResultHasException<T>(ServiceResult<T> sr, out string errorMessageIfNoExceptionIsFound)
        {
            errorMessageIfNoExceptionIsFound = "";
            if (sr != null)
            {
                if (sr.APIResponseForClientModel?.ExceptionCaught == true)
                {
                    return true;
                }
                else
                {
                    errorMessageIfNoExceptionIsFound = sr.APIResponseForClientModel.ResponseDebugErrorMessage;
                    errorMessageIfNoExceptionIsFound = errorMessageIfNoExceptionIsFound.AppendText(lastLocation, " - ", "CodeLocationsHit: ");
                }
            }
            else
            {
                errorMessageIfNoExceptionIsFound = "PolicyAPI ServiceResult object was null";
            }
            return false;
        }

        private string GetAPIEndpointPath()
        {
            if (string.IsNullOrWhiteSpace(API_Path) == false)
            {
                API_EndpointPathUsed = API_Path + "/" + API_Endpoint;
            }
            else
            {
                API_EndpointPathUsed = API_Endpoint;
            }
            return API_EndpointPathUsed;
        }

        internal T SendRequest<T>(InternalEnums.TransType transType)
            where T : Response.Common.BaseResult, new()
        {
            return SendRequest<T>(transType, null);
        }

        internal T SendRequest<T>(InternalEnums.TransType transType, object payload)
            where T : Response.Common.BaseResult, new()
        {
            var returnVar = doTransaction<T>(transType, payload);
            if (returnVar.responseData != null)
            {
                returnVar.responseData.APIResponseForClientModel = returnVar.APIResponseInfo;
            }
            else
            {
                returnVar.responseData = new T();
                returnVar.responseData.APIResponseForClientModel = returnVar.APIResponseInfo;
            }
            return returnVar.responseData;
        }

        internal T SendRequestForListResponse<T>(InternalEnums.TransType transType)
        {
            return SendRequestForListResponse<T>(transType, null);
        }

        internal T SendRequestForListResponse<T>(InternalEnums.TransType transType, object payload)
        {
            var returnVar = doTransaction<T>(transType, payload);

            return returnVar.responseData;
        }

        internal async Task<T> SendRequestForListResponse_Async<T>(InternalEnums.TransType transType, object payload)
        {
            var returnVar = await doTransaction_Async<T>(transType, payload);

            return returnVar.responseData;
        }

        private (T responseData, APIResponseForClientModel APIResponseInfo) doTransaction<T>(InternalEnums.TransType transType)
        {
            return doTransaction<T>(transType, null);
        }

        private async Task<(T responseData, APIResponseForClientModel APIResponseInfo)> doTransaction_Async<T>(InternalEnums.TransType transType)
        {
            return await doTransaction_Async<T>(transType, null);
        }

        private (T responseData, APIResponseForClientModel APIResponseInfo) doTransaction<T>(IFI.Integrations.Objects.InternalEnums.TransType transType, object payload)
        {
            T responseObj = default;
            SetLastLocation("new responseObj");
            var myAPIResponseForClientModel = new Response.Common.APIResponseForClientModel();
            SetLastLocation("new myAPIResponseForClientModel");
            try
            {
                SetLastLocation("GetStart");
                TestRequiredVariable(API_Address, nameof(API_Address));
                TestRequiredVariable(API_Endpoint, nameof(API_Endpoint));
                if (HeadersToAdd != null && HeadersToAdd.Count > 0)
                {
                    foreach (var header in HeadersToAdd)
                    {
                        if (string.IsNullOrWhiteSpace(header.Key) == false)
                        {
                            TestRequiredVariable(header.Value, header.Key);
                        }
                    }
                }
                SetLastLocation("Finished Required Vars Check");

                HttpResponseMessage response = null;
                using (var proxy = new IFI.Integrations.Objects.JsonProxyClient.ProxyClient(API_Address))
                {
                    SetLastLocation("inside Using");
                    if (HeadersToAdd != null && HeadersToAdd.Count > 0)
                    {
                        foreach(var header in HeadersToAdd)
                        {
                            proxy.AddHeader(header.Key, header.Value);
                        }
                    }
                    SetLastLocation("adding new headers");
                    switch (transType)
                    {
                        case InternalEnums.TransType.Get:
                            response = proxy.Get(GetAPIEndpointPath());
                            break;
                        case InternalEnums.TransType.Post:
                            if (payload != null)
                            {
                                response = proxy.Post(GetAPIEndpointPath(), payload);
                            }
                            break;
                        case InternalEnums.TransType.Delete:
                            response = proxy.Delete(GetAPIEndpointPath());
                            break;
                    }
                    recievedResponse = response;
                    SetLastLocation("attempted to retrieved response via " + transType.ToString());
                    if (response != null)
                    {
                        SetLastLocation("response is not null");
                        myAPIResponseForClientModel._responseStatusCode = ((int)response.StatusCode).ToString();
                        myAPIResponseForClientModel._responseMessage = response.ReasonPhrase;
                        if (response.IsSuccessStatusCode)
                        {
                            SetLastLocation("response has success status code");
                            responseObj = proxy.Deserialize<T>(response);
                            deserializedResponseObj = responseObj;
                            SetLastLocation("deserialized response");
                            if (responseObj != null)
                            {
                                myAPIResponseForClientModel._responseDeserialized = true;
                                //responseObj.APIResponseForClientModel = myAPIResponseForClientModel;
                                SetLastLocation("Set myAPIResponseforClientModel to responseObj - 1");
                                //return responseObj;
                            }
                            else
                            {
                                responseObj = default;
                                //responseObj..DetailedErrorMessages.CreateErrorMessage("Deserialized response object was null");
                                //responseObj.APIResponseForClientModel = myAPIResponseForClientModel;
                                SetLastLocation("responseObj is null");
                                //return null;
                            }
                        }
                    }
                    else
                    {
                        SetLastLocation("response is null");
                        myAPIResponseForClientModel._isResponseNull = true;
                    }
                }
                SetLastLocation("Back outside Using statement");
                //responseObj.APIResponseForClientModel = myAPIResponseForClientModel;
                //SetLastLocation("Set myAPIResponseforClientModel to responseObj - 2");
                return (responseObj, myAPIResponseForClientModel);
            }
            catch (Exception ex)
            {
                SetLastLocation("Exception caught");
                InternalException = ex;
                SetLastLocation("Set internalException equal to Exception");
                myAPIResponseForClientModel._exception = ex;
                SetLastLocation("gather exception info");
                return (responseObj, myAPIResponseForClientModel);
            }
        }

        private async Task<(T responseData, APIResponseForClientModel APIResponseInfo)> doTransaction_Async<T>(IFI.Integrations.Objects.InternalEnums.TransType transType, object payload)
        {
            T responseObj = default;
            SetLastLocation("new responseObj");
            var myAPIResponseForClientModel = new Response.Common.APIResponseForClientModel();
            SetLastLocation("new myAPIResponseForClientModel");
            try
            {
                SetLastLocation("GetStart");
                TestRequiredVariable(API_Address, nameof(API_Address));
                TestRequiredVariable(API_Endpoint, nameof(API_Endpoint));
                if (HeadersToAdd != null && HeadersToAdd.Count > 0)
                {
                    foreach (var header in HeadersToAdd)
                    {
                        if (string.IsNullOrWhiteSpace(header.Key) == false)
                        {
                            TestRequiredVariable(header.Value, header.Key);
                        }
                    }
                }
                SetLastLocation("Finished Required Vars Check");

                HttpResponseMessage response = null;
                using (var proxy = new IFI.Integrations.Objects.JsonProxyClient.ProxyClient(API_Address))
                {
                    SetLastLocation("inside Using");
                    if (HeadersToAdd != null && HeadersToAdd.Count > 0)
                    {
                        foreach (var header in HeadersToAdd)
                        {
                            proxy.AddHeader(header.Key, header.Value);
                        }
                    }
                    SetLastLocation("adding new headers");
                    switch (transType)
                    {
                        case InternalEnums.TransType.Get:
                            response = await proxy.GetAsync(GetAPIEndpointPath());
                            break;
                        case InternalEnums.TransType.Post:
                            if (payload != null)
                            {
                                response = await proxy.PostAsync(GetAPIEndpointPath(), payload);
                            }
                            break;
                        case InternalEnums.TransType.Delete:
                            response = await proxy.DeleteAsync(GetAPIEndpointPath());
                            break;
                    }
                    recievedResponse = response;
                    SetLastLocation("attempted to retrieved response " + transType.ToString());
                    if (response != null)
                    {
                        SetLastLocation("response is not null");
                        myAPIResponseForClientModel._responseStatusCode = ((int)response.StatusCode).ToString();
                        myAPIResponseForClientModel._responseMessage = response.ReasonPhrase;
                        if (response.IsSuccessStatusCode)
                        {
                            SetLastLocation("response has success status code");
                            responseObj = proxy.Deserialize<T>(response);
                            deserializedResponseObj = responseObj;
                            SetLastLocation("deserialized response");
                            if (responseObj != null)
                            {
                                myAPIResponseForClientModel._responseDeserialized = true;
                                //responseObj.APIResponseForClientModel = myAPIResponseForClientModel;
                                SetLastLocation("Set myAPIResponseforClientModel to responseObj - 1");
                                //return responseObj;
                            }
                            else
                            {
                                responseObj = default;
                                //responseObj..DetailedErrorMessages.CreateErrorMessage("Deserialized response object was null");
                                //responseObj.APIResponseForClientModel = myAPIResponseForClientModel;
                                SetLastLocation("responseObj is null");
                                //return null;
                            }
                        }
                    }
                    else
                    {
                        SetLastLocation("response is null");
                        myAPIResponseForClientModel._isResponseNull = true;
                    }
                }
                SetLastLocation("Back outside Using statement");
                //responseObj.APIResponseForClientModel = myAPIResponseForClientModel;
                //SetLastLocation("Set myAPIResponseforClientModel to responseObj - 2");
                return (responseObj, myAPIResponseForClientModel);
            }
            catch (Exception ex)
            {
                SetLastLocation("Exception caught");
                InternalException = ex;
                SetLastLocation("Set internalException equal to Exception");
                myAPIResponseForClientModel._exception = ex;
                SetLastLocation("gather exception info");
                return (responseObj, myAPIResponseForClientModel);
            }
        }

        private void SetLastLocation(string message)
        {
            lastLocation = lastLocation.AppendText(message, "; ");
        }

        /// <summary>
        /// Create a new argument exception object to throw. Pass in NameOfVariable by using nameof(MyVariable)
        /// </summary>
        /// <param name="NameOfVariable"></param>
        /// <returns></returns>
        internal ArgumentException CreateArgumentException(string NameOfVariable)
        {
            return new ArgumentException("Parameter is required", NameOfVariable);
        }

        internal ArgumentException CreateArgumentException_OneIsRequired(params string[] NameOfVariables)
        {
            var allElementsExceptLast = NameOfVariables.Take(NameOfVariables.Length - 1).ToArray();
            string requiredVars = string.Join(", ", allElementsExceptLast) + " and " + NameOfVariables[NameOfVariables.Length - 1] ;
            return new ArgumentException("One of the following parameters are required", string.Join(", ", NameOfVariables));
        }

        internal ArgumentException CreateZipException(string NameOfVariable)
        {
            return new ArgumentException("Zip must be 5 digits", NameOfVariable);
        }

        internal ArgumentException CreateTwoLEtterException(string NameOfVariable)
        {
            return new ArgumentException("Parameter must be 2 letters", NameOfVariable);
        }

        internal ArgumentException CreateOutOfRangeException(string NameOfVariable, int CountRequired)
        {
            string plural = CountRequired > 1 ? "s" : "";
            string errorMsg = $"Parameter can only contain {CountRequired} item{plural}";
            return new ArgumentOutOfRangeException(NameOfVariable, errorMsg);
        }

        internal void TestTwoLetterVariable(string myVar, string NameOfVariable)
        {
            if (string.IsNullOrWhiteSpace(myVar))
            {
                throw CreateArgumentException(NameOfVariable);
            }
            if (myVar.Length != 2 || myVar.All(char.IsLetter) == false)
            {
                throw CreateTwoLEtterException(NameOfVariable);
            }
        }

        internal void TestZip(string myVar, string NameOfVariable)
        {
            if (string.IsNullOrWhiteSpace(myVar))
            {
                throw CreateArgumentException(NameOfVariable);
            }
            if (myVar.Length != 5 || myVar.All(char.IsNumber) == false)
            {
                throw CreateZipException(NameOfVariable);
            }
        }

        internal void TestRequiredVariable_OneIsRequired(string[] requiredVariables, string[] requiredVariableNames)
        {
            bool requiredVariableFound = false;
            foreach (var requiredVariable in requiredVariables)
            {
                if (string.IsNullOrWhiteSpace(requiredVariable) == false)
                {
                    requiredVariableFound = true;
                    break;
                }
            }
            if (requiredVariableFound == false)
            {
                throw CreateArgumentException_OneIsRequired(requiredVariableNames);
            }
        }

        internal void TestRequiredVariable(string myVar, string NameOfVariable)
        {
            if (string.IsNullOrWhiteSpace(myVar))
            {
                throw CreateArgumentException(NameOfVariable);
            }
        }

        internal void TestRequiredVariable(int myVar, string NameOfVariable)
        {
            if (myVar <= 0)
            {
                throw CreateArgumentException(NameOfVariable);
            }
        }

        internal void TestRequiredVariable(byte[] myVar, string NameOfVariable)
        {
            if (myVar == null || myVar.Length < 1)
            {
                throw CreateArgumentException(NameOfVariable);
            }
        }

        internal void TestRequiredVariable<T>(IEnumerable<T> myVar, string NameOfVariable)
        {
            if (myVar == null || myVar.Count() < 1)
            {
                throw CreateArgumentException(NameOfVariable);
            }
        }

        internal void TestRequiredVariable<T>(IEnumerable<T> myVar, string NameOfVariable, int RequiredCount)
        {
            if (myVar == null || myVar.Count() < 1)
            {
                throw CreateArgumentException(NameOfVariable);
            }
            if (myVar.Count() != RequiredCount)
            {
                throw CreateOutOfRangeException(NameOfVariable, RequiredCount);
            }
        }

        internal void TestRequiredVariable<TEnum>(TEnum myEnum, int unacceptableValue, string NameOfVariable)
        {
            if (typeof(TEnum).IsEnumDefined(myEnum) == false || (int)(object)myEnum == unacceptableValue)
            {
                throw CreateArgumentException(NameOfVariable);
            }
        }

        internal void DefaultVariableIfNullOrEmpty(ref int myVar, int myDefaultValue = 0)
        {
            if (myVar == 0)
            {
                myVar = myDefaultValue;
            }
        }

        internal string GetExceptionMessages()
        {
            string returnVar = "";
            if (InternalException != null && InternalException.Message != null && InternalException.Message.Trim() != "")
            {
                returnVar = InternalException.Message;
                if (InternalException.InnerException != null)
                {
                    var innerMsg = GetInnerExceptionMesages(InternalException.InnerException, returnVar, 0);
                    if (innerMsg != null && innerMsg.Trim() != "")
                    {
                        returnVar += ";" + innerMsg;
                    }
                }
                returnVar = Regex.Replace(returnVar, @";+", ";");
                returnVar = returnVar.Replace(";", "; ");
                if (returnVar.Substring(returnVar.Length - 1, 1) != ";")
                {
                    returnVar += ";";
                }
            }
            return returnVar;
        }

        internal string GetInnerExceptionMesages(Exception ex, string currentMessage, int count)
        {
            string returnVar = "";
            count += 1;
            if (ex != null && ex.Message != null && ex.Message.Trim() != "")
            {
                if (currentMessage.Contains(ex.Message.Trim()) == false)
                {
                    returnVar += ";" + ex.Message.Trim();
                }
                if (count <= 10 && ex.InnerException != null)
                {
                    var innerMsg = GetInnerExceptionMesages(ex.InnerException, returnVar, count);
                    if (innerMsg != null && innerMsg.Trim() != "")
                    {
                        returnVar += ";" + innerMsg;
                    }
                }
            }
            return returnVar;
        }
    }
}
