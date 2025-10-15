using System;
using IFM.DataServices.API.RequestObjects.OnBase;
using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IFM.DataServices.Tests.Controllers.OnBase.DocumentUpload
{
    [TestClass]
    public class OnBaseUploadTests : BaseTest
    {
        [TestMethod]
        public void TestDocumentUpload()
        {
            //var uploadFile = @"C:\users\dagug\desktop\DownloadInformationFormDL61218-1_fnol_dec_.pdf";
            //var uploadFile = @"C:\users\dagug\desktop\TestingOnBaseSubTypes-pic.png";
            //var uploadFile = @"C:\users\dagug\desktop\TestingOnBaseSubTypes.txt";
            var uploadFile = @"C:\Users\dagug\OneDrive - Indiana Farmers Mutual Insurance Company\Desktop\DiamondNotifications-API-correlation.png";

            byte[] uploadBytes = null;
            if (System.IO.File.Exists(uploadFile))
            {
                uploadBytes = System.IO.File.ReadAllBytes(uploadFile);
            }

            appHost.Start(session =>
            {
                string currentDate = DateTime.Now.ToShortDateString();
                var package = new global::IFM.DataServicesCore.CommonObjects.OnBase.UploadPayload();
                package.SetBase64Payload(uploadBytes);
                package.SetExtensionFromFullFileName(uploadFile);
                package.SourceSystem = API.RequestObjects.OnBase.DocumentUpload.SourceSystems.FNOLAssignment;

                package.Keys.Add(new DataServicesCore.CommonObjects.OnBase.OnBaseKey() { Key = API.RequestObjects.OnBase.DocumentUpload.KeyTypes.policyNumber, Values = new string[] { "PPA2150739" } });
                package.Keys.Add(new DataServicesCore.CommonObjects.OnBase.OnBaseKey() { Key = API.RequestObjects.OnBase.DocumentUpload.KeyTypes.description, Values = new string[] { "Testing OnBase Import from API" } });
                package.Keys.Add(new DataServicesCore.CommonObjects.OnBase.OnBaseKey() { Key = API.RequestObjects.OnBase.DocumentUpload.KeyTypes.needsProcessing, Values = new string[] { "NO" } });

                var json = ToJson(package);
                var result = session.PostJson($"OnBase/Document/Upload",json);
                var sr = JsonConvert.DeserializeObject<Int64>(result.ResponseText);
                Assert.IsTrue(sr > 0);
            });
        }

        [TestMethod]
        public void TestDocumentUploadByTypeWithBuiltInObject()
        {
            //var uploadFile = @"C:\users\dagug\desktop\DownloadInformationFormDL61218-1_fnol_dec_.pdf";
            //var uploadFile = @"C:\users\dagug\desktop\TestingOnBaseSubTypes-pic.png";
            //var uploadFile = @"C:\users\dagug\desktop\TestingOnBaseSubTypes.txt";
            var uploadFile = @"C:\Users\dagug\OneDrive - Indiana Farmers Mutual Insurance Company\Desktop\DiamondNotifications-API-correlation.png";

            byte[] uploadBytes = null;
            if (System.IO.File.Exists(uploadFile))
            {
                uploadBytes = System.IO.File.ReadAllBytes(uploadFile);
            }

            appHost.Start(session =>
            {
                string currentDate = DateTime.Now.ToShortDateString();
                var package = new API.RequestObjects.OnBase.DocumentUpload();

                package.UploadType = API.RequestObjects.OnBase.DocumentUpload.SourceSystems.FNOLAssignment;
                package.FileName = System.IO.Path.GetFileName(uploadFile);
                package.FileBytes = uploadBytes;

                package.Keys.Add(API.RequestObjects.OnBase.DocumentUpload.KeyTypes.policyNumber, "HOM100001");
                package.Keys.Add(API.RequestObjects.OnBase.DocumentUpload.KeyTypes.description, "Testing Onbase Upload " + currentDate);
                package.Keys.Add(API.RequestObjects.OnBase.DocumentUpload.KeyTypes.needsProcessing, "NO");

                //START - THIS IS DONE ON THE UPLOAD DOCUMENT CALL FOR THE DEVELOPER TYPICALLY

                foreach (var kvp in package.Keys)
                {
                    package.OnBaseKeys.Add(((int)kvp.Key).ToString(), kvp.Value);
                }

                //FINISH - THIS IS DONE ON THE UPLOAD DOCUMENT CALL FOR THE DEVELOPER TYPICALLY

                var json = ToJson(package);
                var result = session.PostJson($"OnBase/Document/UploadByType", json);
                var sr = DeserializeServiceResponse(result);
                Assert.IsTrue(sr.HasErrors == false);
            });
        }

        [TestMethod]
        public void TestDocumentUploadByTypeWithCustomObject()
        {
            //var uploadFile = @"C:\users\dagug\desktop\DownloadInformationFormDL61218-1_fnol_dec_.pdf";
            //var uploadFile = @"C:\users\dagug\desktop\TestingOnBaseSubTypes-pic.png";
            //var uploadFile = @"C:\users\dagug\desktop\TestingOnBaseSubTypes.txt";
            var uploadFile = @"C:\Users\dagug\OneDrive - Indiana Farmers Mutual Insurance Company\Desktop\DiamondNotifications-API-correlation.png";

            byte[] uploadBytes = null;
            if (System.IO.File.Exists(uploadFile))
            {
                uploadBytes = System.IO.File.ReadAllBytes(uploadFile);
            }

            appHost.Start(session =>
            {
                string currentDate = DateTime.Now.ToShortDateString();
                var package = new CustomUploadObject();
                //None = 0,
                //VelociraterImporter = 1,
                //FNOLAssignment = 2,
                //Millennium = 3,
                //eChecksNightly = 4,
                //eSignatureApplication = 5,
                //eSignatureProxy = 6,
                //Fiserv = 7,
                //FNOLAssignmentInvestigation = 8

                package.UploadType = 2;
                package.FileName = System.IO.Path.GetFileName(uploadFile);
                package.FileBytes = uploadBytes;

                //none = 1,
                //agencyCode = 2,
                //policyNumber = 3,
                //claimNumber = 4,
                //description = 5,
                //needsProcessing = 6

                package.OnBaseKeys.Add("4", "HOM100001");
                package.OnBaseKeys.Add("5", "Testing Onbase Upload " + currentDate);
                package.OnBaseKeys.Add("6", "NO");

                var json = ToJson(package);
                var result = session.PostJson($"OnBase/Document/UploadByType", json);
                var sr = DeserializeServiceResponse(result);
                Assert.IsTrue(sr.HasErrors == false);
            });
        }
    }

    public class CustomUploadObject
    {
        public int UploadType { get; set; }
        public byte[] FileBytes { get; set; }
        public string FileName { get; set; }
        public Dictionary<string, string> OnBaseKeys { get; set; } = new Dictionary<string, string>();
    }
}
