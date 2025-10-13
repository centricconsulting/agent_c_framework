using System;
using IFM.DataServices.API.RequestObjects.OnBase;
using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IFM.DataServices.Tests.IFMDataServicesClientModels.OnBase
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
                var package = new API.RequestObjects.OnBase.DocumentUpload("http://ifmdiapatch:1080");
                //var package = new API.RequestObjects.OnBase.DocumentUpload("http://ifmeomtest2:1080");

                package.UploadType = API.RequestObjects.OnBase.DocumentUpload.SourceSystems.FNOLAssignment;
                package.FileName = System.IO.Path.GetFileName(uploadFile);
                package.FileBytes = uploadBytes;

                package.Keys.Add(API.RequestObjects.OnBase.DocumentUpload.KeyTypes.claimNumber, "1214841");
                package.Keys.Add(API.RequestObjects.OnBase.DocumentUpload.KeyTypes.description, "Testing Onbase Upload " + currentDate);
                package.Keys.Add(API.RequestObjects.OnBase.DocumentUpload.KeyTypes.needsProcessing, "NO");

                //FINISH - THIS IS DONE ON THE UPLOAD DOCUMENT CALL FOR THE DEVELOPER TYPICALLY

                var response = package.UploadDocument();
                Assert.IsTrue(response.Success);
                //var json = ToJson(package);
                //var result = session.PostJson($"OnBase/Document/UploadByType", json);
                //var sr = DeserializeServiceResponse(result);
                //Assert.IsTrue(sr.HasErrors == false);
            });
        }
    }
}
