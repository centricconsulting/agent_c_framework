using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.SessionState;

namespace VrTests
{
    [TestClass]
    public class VRQQLibBase
    {
        // This class provides some basic enviroment setup needed to test quick quote objects
        // most depend on the static data file
        // some will likely depend on the web.config file as well

        QuickQuote.CommonMethods.QuickQuoteHelperClass _QQHelper = null;
        public QuickQuote.CommonMethods.QuickQuoteHelperClass QQHelper
        {
            get
            {
                if (_QQHelper == null)
                    _QQHelper = new QuickQuote.CommonMethods.QuickQuoteHelperClass();

                return _QQHelper;
            }
        }

        #region Initialize

        /// <summary>
        ///Initialize() is called once during test execution before
        ///test methods in this test class are executed.
        ///</summary>
        [TestInitialize()]
        public void Initialize()
        {
            Setup(null);            
        }

        private static void Setup(TestContext testContext)
        {
            SetupStaticDataFile();
        }

        private static void SetupHttpContext()
        {
            if (HttpContext.Current == null)
            {
                var httpRequest = new HttpRequest("", "http://mySomething/", "");
                var stringWriter = new StringWriter();
                var httpResponce = new HttpResponse(stringWriter);
                var httpContext = new HttpContext(httpRequest, httpResponce);

                var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                                     new HttpStaticObjectsCollection(), 10, true,
                                                                     HttpCookieMode.AutoDetect,
                                                                     SessionStateMode.InProc, false);

                httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                                                         BindingFlags.NonPublic | BindingFlags.Instance,
                                                         null, CallingConventions.Standard,
                                                         new[] { typeof(HttpSessionStateContainer) },
                                                         null)
                                                    .Invoke(new object[] { sessionContainer });

                HttpContext.Current = httpContext;
            }
        }

        private static void SetupStaticDataFile()
        {
            SetupHttpContext();
            //HttpContext.Current.Cache["QuickQuote_StaticDataXmlFilePath"] = "g:\\ClassFiles\\DiamondStaticData.xml"; not needed now that we have the config file working
        }

        #endregion Initialize

        public static string PrintTestValue(object value)
        {
            string returnValue = "";
            if (value == null)
            {
                returnValue = "[NULL]";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(value.ToString()))
                    returnValue = "[EMPTY]";
                else
                    returnValue = value.ToString();
            }

            return " - (" + returnValue + ")";
        }

        /// <summary>
        /// Returns an empty new quote object.
        /// </summary>
        /// <returns></returns>
        public static QuickQuote.CommonObjects.QuickQuoteObject GetNewQuickQuote()
        {
            return new QuickQuote.CommonObjects.QuickQuoteObject();
        }

        /// <summary>
        /// Use this to loop through all lob types.
        /// </summary>
        /// <returns></returns>
        public static QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType[] GetLobTypeList()
        {
            return (QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType[])Enum.GetValues(typeof(QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType));
        }
    }
}