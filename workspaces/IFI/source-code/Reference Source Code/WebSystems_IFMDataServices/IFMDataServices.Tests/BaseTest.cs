using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using MvcIntegrationTestFramework.Hosting;
using MvcIntegrationTestFramework.Browsing;
using System.Data;
using IFM.DataServices.Tests.Controllers.IFM;
using System.Text.RegularExpressions;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Tests
{
    [TestClass]
    public class BaseTest
    {
        public TestContext TestContext { get; set; }

        protected static AppHost appHost;
        protected static JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            Formatting = Newtonsoft.Json.Formatting.None
        };

        [TestInitialize()]
        public void Initialize()
        {
            if (appHost == null)
            {
                AppHost.LoadAllBinaries = true;
                appHost = AppHost.Simulate("IfmDataServices");
            }
        }

        protected T DeserializeServiceResponse<T>(RequestResult sr)
        {
            try
            {
                return (sr != null) ? JsonConvert.DeserializeObject<T>(sr.ResponseText, serializerSettings) : default(T);
            }
            catch (Exception ex)
            {
#if (DEBUG)
                if (1 == 1) { }
#else

#endif
            }
            return default(T);
        }

        protected APIResponses.Common.ServiceResult DeserializeServiceResponse(RequestResult sr)
        {
            try
            {
                return (sr != null) ? JsonConvert.DeserializeObject<APIResponses.Common.ServiceResult>(sr.ResponseText, serializerSettings) : null;
            }
            catch (Exception ex)
            {
#if (DEBUG)
                if (1 == 1) { }
#else

#endif
            }
            return null;
        }

        protected bool IsPdf(string pdfString)
        {
            if(pdfString != "Print not found.")
            {
                string stringToCheck = pdfString.Substring(0, 1024);
                string pattern = @"%PDF-";
                Match m = Regex.Match(stringToCheck, pattern, RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        protected T DeserializeServiceResponseData<T>(APIResponses.Common.ServiceResult<T> sr)
        {
            if (sr != null)
            {
                return (sr.ResponseData != null) ? JsonConvert.DeserializeObject<T>(sr.ResponseData.ToString(), serializerSettings) : default(T);
            }
            return default(T);
            //try
            //{
            //return (sr.ResponseData != null) ? JsonConvert.DeserializeObject<T>(sr.ResponseData.ToString(), serializerSettings) : default(T);
            //}
            //catch
            //{ }
            //return default(T);
        }

        protected T DeserializeJson<T>(string json)
        {
            //try
            //{
            return (json != null) ? JsonConvert.DeserializeObject<T>(json, serializerSettings) : default(T);
            //}
            //catch
            //{ }
            //return default(T);
        }

        protected void DoBasicResultTests(RequestResult diamondResult, CommonContextItems cci)
        {
            if (cci.ExpectsPayload)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(diamondResult.ResponseText) == true, "Response data was empty.");
                Assert.IsNotNull(diamondResult.ResponseText, "No response data malformed.");
            }
            else
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(diamondResult.ResponseText != null ? diamondResult.ResponseText : string.Empty) == true, "Response data was expected to be empty but wasn't.");
            }

        }

        protected void DoBasicResponseTests(APIResponses.Common.ServiceResult sr, CommonContextItems cci)
        {
            DoBasicResponseTests((APIResponses.Common.ServiceResult<object>)(object)sr, cci);
        }

        protected void DoBasicResponseTests<T>(APIResponses.Common.ServiceResult<T> sr, CommonContextItems cci)
        {
            Assert.IsNotNull(sr, "ServiceResult was null.");
            if (cci.ExpectsErrors == false)
                Assert.AreEqual(sr.HasErrors, cci.ExpectsErrors, $"Service result had errors. - {sr.Messages.ToString()}");
            else
                Assert.AreEqual(sr.HasErrors, cci.ExpectsErrors, $"Had expected errors of... - {sr.Messages.ToString()}");

        }

        protected void DoBasicResponseTestsWithData(APIResponses.Common.ServiceResult sr, object returnedData, CommonContextItems cci)
        {
            DoBasicResponseTestsWithData((APIResponses.Common.ServiceResult<object>)(object)sr, returnedData, cci);
        }

        protected void DoBasicResponseTestsWithData<T>(APIResponses.Common.ServiceResult<T> sr, object returnedData, CommonContextItems cci)
        {
            DoBasicResponseTests(sr, cci);
            //Assert.IsNotNull(sr.ResponseData, "No response data.");
            if (cci.ExpectsPayload)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(sr.ResponseData.ToString()) == true, "Response data was empty.");
                Assert.IsNotNull(returnedData, "No response data malformed.");
            }
            else
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(sr.ResponseData != null ? sr.ResponseData.ToString() : string.Empty) == true, "Response data was expected to be empty but wasn't.");
            }
        }

        protected void DoBasicResponseTestsForPDFReturn(RequestResult rr, CommonContextItems cci)
        {
            Assert.IsNotNull(rr, "RequestResult was null.");
            if (cci.ExpectsPayload)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(rr.ResponseText.ToString()) == true, "Response text was empty.");
                Assert.IsTrue(IsPdf(rr.ResponseText), "Could not find PDF header text.");
            }
            else
            {
                //Not sure on this... ResponseText might always return something even if PDF isn't found.
                if (string.IsNullOrWhiteSpace(rr.ResponseText != null ? rr.ResponseText.ToString() : string.Empty) == false)
                {
                    Assert.IsTrue(IsPdf(rr.ResponseText), "Response Text was expected to not be a PDF but it was.");
                }
            }
        }

        protected void DoBasicResponseTestsForPDFReturn(RequestResult rr, bool expectsPayload)
        {
            Assert.IsNotNull(rr, "RequestResult was null.");
            if (expectsPayload)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(rr.ResponseText.ToString()) == true, "Response text was empty.");
                Assert.IsTrue(IsPdf(rr.ResponseText), "Could not find PDF header text.");
            }
            else
            {
                //Not sure on this... ResponseText might always return something even if PDF isn't found.
                if (string.IsNullOrWhiteSpace(rr.ResponseText != null ? rr.ResponseText.ToString() : string.Empty) == false)
                {
                    Assert.IsTrue(IsPdf(rr.ResponseText), "Response Text was expected to not be a PDF but it was.");
                }
            }
        }

        protected CommonContextItems GetCommonTestContextItems(TestContext tc)
        {
            CommonContextItems cci = new CommonContextItems
            {
                ExpectedResult = tc.DataRow["expectedResult"].ToString().ToLower().Trim(),
                ExpectsErrors = Convert.ToBoolean(tc.DataRow["expectErrors"].ToString()),
                ExpectsPayload = Convert.ToBoolean(tc.DataRow["expectPayload"].ToString())
            };
            return cci;
        }

        protected string ToJson(object item)
        {
            return JsonConvert.SerializeObject(item);
        }

        protected IDbConnection OpenConnection(string connString)
        {
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connString);
            conn.Open();
            return conn;
        }

        private APIResponses.Common.ServiceResult CreateServiceResult()
        {
            return new APIResponses.Common.ServiceResult();
        }

    }

    public class CommonContextItems
    {
        public string ExpectedResult = "";
        public bool ExpectsErrors = false;
        public bool ExpectsPayload = false;

    }

}
