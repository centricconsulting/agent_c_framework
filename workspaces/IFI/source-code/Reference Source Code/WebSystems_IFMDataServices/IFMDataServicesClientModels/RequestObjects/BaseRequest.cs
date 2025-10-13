using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFM.DataServices.API.RequestObjects
{
    [System.Serializable]
    public class BaseRequest
    {
        internal string API_Address;
        internal string API_Endpoint;
        internal string API_Path;
        [NonSerialized]
        internal Exception internalException;
        [NonSerialized]
        internal string lastLocation;
        [NonSerialized]
        internal System.Net.Http.HttpResponseMessage recievedResponse;
        [NonSerialized]
        internal object deserializedResponseObj;
        [NonSerialized]
        private readonly JsonSerializerSettings CamelCaseFormatter = new JsonSerializerSettings();

        public string GetDebugInfo()
        {
            string myDebugInfo = "";
            myDebugInfo = myDebugInfo.AppendText(recievedResponse.ReasonPhrase, "; ", "ResponseReason: ");
            myDebugInfo = myDebugInfo.AppendText(lastLocation, "; ", "Locations: ");
            if (internalException != null)
            {
                myDebugInfo = myDebugInfo.AppendText(internalException.Message, "; ", "ExceptionMessage: ");
            }
            if (deserializedResponseObj != null)
            {
                myDebugInfo = myDebugInfo.AppendText(Newtonsoft.Json.JsonConvert.SerializeObject(deserializedResponseObj), "; ", "deserializedResponse-reserialized: ");
            }
            return myDebugInfo;
        }

        public System.Net.Http.HttpResponseMessage GetResponse()
        {
            return recievedResponse;
        }

        internal BaseRequest(string API_Address)
        {
            this.API_Address = API_Address;
        }

        internal string GetJsonAsStringForRequestObject(object payload)
        {
            CamelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return JsonConvert.SerializeObject(payload, CamelCaseFormatter);
        }

        private string GetAPIEndpointPath()
        {
            if (API_Path.IsNotNullEmptyOrWhitespace())
            {
                return API_Path + "/" + API_Endpoint;
            }
            else
            {
                return API_Endpoint;
            }
        }

        internal T Get<T>()
            where T : IFM.DataServices.API.ResponseObjects.Common.BaseResult, new()
        {
            try
            {
                SetLastLocation("GetStart");
                TestRequiredVariable(API_Address, nameof(API_Address));
                TestRequiredVariable(API_Endpoint, nameof(API_Endpoint));
                SetLastLocation("Finished Required Vars Check");
                var responseObj = new T();
                SetLastLocation("new responseObj");
                var myAPIResponseForClientModel = new ResponseObjects.Common.APIResponseForClientModel();
                SetLastLocation("new myAPIResponseForClientModel");
                using (var proxy = new IFM.JsonProxyClient.ProxyClient(API_Address))
                {
                    SetLastLocation("inside Using");
                    var response = proxy.Get(GetAPIEndpointPath());
                    recievedResponse = response;
                    SetLastLocation("attempted to retrieved response via GET");
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
                            if(responseObj != null)
                            {
                                myAPIResponseForClientModel._responseDeserialized = true;
                                responseObj.APIResponseForClientModel = myAPIResponseForClientModel;
                                SetLastLocation("Set myAPIResponseforClientModel to responseObj - 1");
                                return responseObj;
                            }
                            else
                            {
                                responseObj = new T();
                                responseObj.DetailedErrorMessages.CreateErrorMessage("Deserialized response object was null");
                                responseObj.APIResponseForClientModel = myAPIResponseForClientModel;
                                SetLastLocation("responseObj is null");
                                return null;
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
                responseObj.APIResponseForClientModel = myAPIResponseForClientModel;
                SetLastLocation("Set myAPIResponseforClientModel to responseObj - 2");
                return responseObj;
            }
            catch(Exception ex)
            {
                SetLastLocation("Exception caught");
                internalException = ex;
                SetLastLocation("Set internalException equal to Exception");
                var responseObj = new T();
                SetLastLocation("new responseObj");
                var myAPIResponseForClientModel = new ResponseObjects.Common.APIResponseForClientModel();
                SetLastLocation("new myAPIResponseForClientModel");
                myAPIResponseForClientModel._exception = ex;
                SetLastLocation("gather exception info");
                return responseObj;
            }
        }

        internal T Post<T>(object payload)
            where T : IFM.DataServices.API.ResponseObjects.Common.BaseResult, new()
        {
            try
            {
                SetLastLocation("GetStart");
                TestRequiredVariable(API_Address, nameof(API_Address));
                TestRequiredVariable(API_Endpoint, nameof(API_Endpoint));
                SetLastLocation("Finished Required Vars Check");
                var responseObj = new T();
                SetLastLocation("new responseObj");
                var myAPIResponseForClientModel = new ResponseObjects.Common.APIResponseForClientModel();
                SetLastLocation("new myAPIResponseForClientModel");

                using (var proxy = new IFM.JsonProxyClient.ProxyClient(API_Address))
                {
                    SetLastLocation("inside Using");
                    var response = proxy.Post(GetAPIEndpointPath(), payload);
                    recievedResponse = response;
                    SetLastLocation("attempted to retrieved response via POST");
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
                                responseObj.APIResponseForClientModel = myAPIResponseForClientModel;
                                SetLastLocation("Set myAPIResponseforClientModel to responseObj - 1");
                                return responseObj;
                            }
                            else
                            {
                                responseObj = new T();
                                responseObj.DetailedErrorMessages.CreateErrorMessage("Deserialized response object was null");
                                responseObj.APIResponseForClientModel = myAPIResponseForClientModel;
                                SetLastLocation("responseObj is null");
                                return null;
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
                responseObj.APIResponseForClientModel = myAPIResponseForClientModel;
                SetLastLocation("Set myAPIResponseforClientModel to responseObj - 2");
                return responseObj;
            }
            catch(Exception ex)
            {
                SetLastLocation("Exception caught");
                internalException = ex;
                SetLastLocation("Set internalException equal to Exception");
                var responseObj = new T();
                SetLastLocation("new responseObj");
                var myAPIResponseForClientModel = new ResponseObjects.Common.APIResponseForClientModel();
                SetLastLocation("new myAPIResponseForClientModel");
                myAPIResponseForClientModel._exception = ex;
                SetLastLocation("gather exception info");
                return responseObj;
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

        internal void TestRequiredVariable(string myVar, string NameOfVariable)
        {
            if (myVar.IsNullEmptyOrWhitespace())
            {
                throw CreateArgumentException(NameOfVariable);
            }
        }

        internal void TestRequiredVariable(int myVar, string NameOfVariable)
        {
            if (myVar > 0)
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

        internal void TestRequiredVariable<TEnum>(TEnum myEnum, int unacceptableValue, string NameOfVariable)
{
            if (typeof(TEnum).IsEnumDefined(myEnum) == false || (int)(object)myEnum == unacceptableValue)
            {
                throw CreateArgumentException(NameOfVariable);
            }
        }
    }
}