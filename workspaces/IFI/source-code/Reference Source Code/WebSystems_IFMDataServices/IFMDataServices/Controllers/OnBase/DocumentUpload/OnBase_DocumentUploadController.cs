using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IFM.PrimitiveExtensions;
using OBS = IFM.DataServices.OnBaseSoapService;
using IFM.DataServices.API.RequestObjects.OnBase;
using IFM.DataServices.API.ResponseObjects.OnBase;

namespace IFM.DataServices.Controllers.OnBase.DocumentUpload
{
    [RoutePrefix("OnBase/Document")]
    public class OnBase_DocumentUploadController : BaseController
    {
        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("Upload")]
        public JsonResult UploadADocument(global::IFM.DataServicesCore.CommonObjects.OnBase.UploadPayload payLoad)
        {
            string errorMessage = "";
            return Json(UploadDocumentToOnBase(payLoad, ref errorMessage));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("UploadByType")]
        public JsonResult UploadADocumentByType(API.RequestObjects.OnBase.DocumentUpload basicpaylod)
        {
            OnBaseUploadResponse response = new OnBaseUploadResponse();
            global::IFM.DataServicesCore.CommonObjects.OnBase.UploadPayload payLoad = global::IFM.DataServicesCore.CommonObjects.OnBase.UploadPayload.CreatePayloadObject(basicpaylod);
            long returnID = 0;

            if (payLoad.errorMessage.IsNullEmptyOrWhitespace())
                returnID = UploadDocumentToOnBase(payLoad, ref response.ErrorMessage);

            if (returnID > 0)
            {
                response.Success = true;
                response.OnBaseDocumentID = returnID;
            }
            else
            {
                response.Success = false;
                response.ErrorCode = returnID;
                response.ErrorMessage = payLoad.errorMessage;
                response.Messages.CreateErrorMessage(payLoad.errorMessage);
                response.DetailedErrorMessages.CreateErrorMessage(payLoad.errorMessage);
            }
            return Json(response);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("UploadPayload")] //Same as Upload except returns an easier to tell success / errorcode
        public JsonResult UploadPayload(global::IFM.DataServicesCore.CommonObjects.OnBase.UploadPayload payLoad)
        {
            OnBaseUploadResponse response = new OnBaseUploadResponse();
            long returnID = UploadDocumentToOnBase(payLoad, ref response.ErrorMessage);

            if (returnID > 0)
            {
                response.Success = true;
                response.OnBaseDocumentID = returnID;
            }
            else
            {
                response.Success = false;
                response.ErrorCode = returnID;
                response.ErrorMessage = payLoad.errorMessage;
                response.Messages.CreateErrorMessage(payLoad.errorMessage);
                response.DetailedErrorMessages.CreateErrorMessage(payLoad.errorMessage);
            }

            return Json(response);
        }

        private OBS.StandAloneDocument_Upload GetKeys(global::IFM.DataServicesCore.CommonObjects.OnBase.UploadPayload payLoad)
        {
            OBS.StandAloneDocument_Upload keyValues = new OBS.StandAloneDocument_Upload();

            keyValues.OnBaseDocumentType_Collection = payLoad.GetOnBaseDocumentType_Collection().ToArray();
            keyValues.OnBaseDocumentSubtype_Collection = payLoad.GetOnBaseDocumentSubtype_Collection().ToArray();

            if (keyValues.OnBaseDocumentSubtype_Collection == null || keyValues.OnBaseDocumentSubtype_Collection.Any() == false || keyValues.OnBaseDocumentSubtype_Collection[0] == null)
            {
                // no document type
                string errmsg = "";
                LogDocumentUploadIssue(payLoad, $"Failed to identify OnBase document sub type for extension of '{payLoad.FileExtension}'", ref errmsg);
            }

            foreach (var key in payLoad.Keys)
            {
                switch(key.Key)
                {
                    case API.RequestObjects.OnBase.DocumentUpload.KeyTypes.agencyCode:
                        keyValues.AgencyCode_Collection = key.Values;
                        break;
                    case API.RequestObjects.OnBase.DocumentUpload.KeyTypes.claimNumber:
                        keyValues.ClaimNumber_Collection = key.Values;
                        break;
                    case API.RequestObjects.OnBase.DocumentUpload.KeyTypes.description:
                        keyValues.OnBaseDocumentDescription_Collection = key.Values;
                        break;
                    case API.RequestObjects.OnBase.DocumentUpload.KeyTypes.policyNumber:
                        keyValues.PolicyNumber_Collection = key.Values;
                        break;
                    case API.RequestObjects.OnBase.DocumentUpload.KeyTypes.needsProcessing:
                        keyValues.ProcessingNeeded_Collection = key.Values;
                        break;
                    default:
                        break;

                }
            }
            return keyValues;
        }

        private long UploadDocumentToOnBase(global::IFM.DataServicesCore.CommonObjects.OnBase.UploadPayload payLoad, ref string errorMessage)
        {
            if (KeysAreValid(payLoad, out errorMessage))
            {
                if (payLoad.FileExtension.IsNullEmptyOrWhitespace())
                {
                    LogDocumentUploadIssue(payLoad, "File extension was null or empty.", ref errorMessage);
                    return -100;
                }

                if (payLoad.SourceSystem == API.RequestObjects.OnBase.DocumentUpload.SourceSystems.None)
                {
                    LogDocumentUploadIssue(payLoad, "Source System of 'None' is invalid.", ref errorMessage);
                    return -100;
                }

                var keyValues = GetKeys(payLoad);
                using (var obObject = new OBS.HylandOutBoundContractClient("BasicHttpBinding_HylandOutBoundContract",
                System.Configuration.ConfigurationManager.AppSettings["ifmDataServices_OnBaseServiceEndpoint"]))
                {
                    obObject.ClientCredentials.UserName.UserName = System.Configuration.ConfigurationManager.AppSettings["onBaseSoapUsername"];
                    obObject.ClientCredentials.UserName.Password = System.Configuration.ConfigurationManager.AppSettings["onBaseSoapPass"];

                    OBS.FileInfoDocument_Upload fileInfoUploadDocument = new OBS.FileInfoDocument_Upload();
                    OBS.OBDocumentArchiveDocument_Upload archiveUploadDocument = new OBS.OBDocumentArchiveDocument_Upload();
                    OBS.KeywordsDocument_Upload keyWordsUploadDocument = new OBS.KeywordsDocument_Upload();

                    fileInfoUploadDocument.Base64FileStream = payLoad.Base64Payload;
                    fileInfoUploadDocument.FileExtension = payLoad.FileExtension;
                    fileInfoUploadDocument.MIMEType = payLoad.GetOnBaseMimeType().FirstOrDefault();

                    keyWordsUploadDocument.StandAlone = keyValues;

                    archiveUploadDocument.Keywords = keyWordsUploadDocument;
                    archiveUploadDocument.FileInfo = fileInfoUploadDocument;
                    try
                    {
                        string response = obObject.Document_Upload(archiveUploadDocument).ToString();
                        if (response.IsNumeric() && long.TryParse(response, out long docId))
                        {
                            if (docId < 1)
                            {
                                LogDocumentUploadIssue(payLoad, response, ref errorMessage);
                                return docId;
                            }
                            else
                            {
                                //payLoad.LogSuccessfulUpload(docId); //Time to get rid of this - This was meant as a temporary thing to see items getting uploaded succesfully. Now we log api calls and should be able to tell.
                                return docId;
                            }
                        }
                        else
                        {
                            LogDocumentUploadIssue(payLoad, response, ref errorMessage);
                            return -100;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogDocumentUploadException(ex, payLoad, ref errorMessage);
                        return -100;
                    }
                }
            }
            else
            {
                LogDocumentUploadIssue(payLoad, "Keys are invalid.", ref errorMessage);
                return -100;
            }
        }

        private void LogDocumentUploadIssue(global::IFM.DataServicesCore.CommonObjects.OnBase.UploadPayload payLoad, string errorMessage, ref string errorOutMessage)
        {
            payLoad.errorMessage = payLoad.errorMessage.AppendText(errorMessage, "; ");
            errorOutMessage = $"IFMDATASERVICES -> OnBase_DocumentUploadController.cs -> Function UploadDocumentToOnBase - OnBase Upload Issue from '{payLoad.SourceSystem}' with file '{payLoad.FullFileName}'. Payload Keys: {DisplayKeysForErrorMessage(payLoad)} Error: '{errorMessage}'";
#if !DEBUG
            global::IFM.IFMErrorLogging.LogIssue("An error occured while uploading the document.", errorOutMessage);
#else

#endif
        }

        private void LogDocumentUploadException(Exception ex, global::IFM.DataServicesCore.CommonObjects.OnBase.UploadPayload payLoad, ref string errorOutMessage)
        {
            errorOutMessage = $"IFMDATASERVICES -> OnBase_DocumentUploadController.cs -> Function UploadDocumentToOnBase - OnBase Upload Exception from '{payLoad.SourceSystem}' with file '{payLoad.FullFileName}'. Payload Keys: {DisplayKeysForErrorMessage(payLoad)} Error: '{ex}'";
#if !DEBUG
                global::IFM.IFMErrorLogging.LogException(ex, errorOutMessage);
#else
            Debugger.Break();
#endif
        }

        private string DisplayKeysForErrorMessage(global::IFM.DataServicesCore.CommonObjects.OnBase.UploadPayload payLoad)
        {
            if (payLoad?.Keys?.IsLoaded() == true)
            {
                int count = 0;
                string errorString = "";
                foreach (var key in payLoad.Keys)
                {
                    if (key.Values.IsLoaded())
                    {
                        if(errorString != "")
                        {
                            errorString += "; ";
                        }
                        errorString += $"Key '{count}': '{key.Key}' = ";
                        string errValues = "";
                        foreach (var val in key.Values)
                        {
                            if(errValues != "")
                            {
                                errValues += ", ";
                            }
                            errValues += val;
                        }
                        errorString += $"'{errValues}'";
                    }
                    count += 1;
                }
                return errorString;
            }
            return "";
        }

        private bool KeysAreValid(global::IFM.DataServicesCore.CommonObjects.OnBase.UploadPayload payLoad, out string errorMessage)
        {

            var KeyHasOneOrMoreNonEmptyValues = new Func<IEnumerable<string>, bool>(x =>
            {
                return x != null && x.Any() && !string.IsNullOrWhiteSpace(x.FirstOrDefault());
            });

            var KeyHasExactlyOneValue = new Func<IEnumerable<string>, bool>(x =>
            {
                return KeyHasOneOrMoreNonEmptyValues(x) && x.Count() == 1;
            });

            var GetKeyOfType = new Func<API.RequestObjects.OnBase.DocumentUpload.KeyTypes, DataServicesCore.CommonObjects.OnBase.OnBaseKey>(x =>
            {
                return (from k in payLoad.Keys where k.Key == x select k).FirstOrDefault();
            });


            if (!KeyHasExactlyOneValue(payLoad.GetOnBaseDocumentType_Collection()))
            {
                errorMessage = "1 or more keys do not have exactly one value.";
                return false;
            }

            var policyNumberKey = GetKeyOfType(API.RequestObjects.OnBase.DocumentUpload.KeyTypes.policyNumber);
            var claimNumberKey = GetKeyOfType(API.RequestObjects.OnBase.DocumentUpload.KeyTypes.claimNumber);
            var agencyCodeKey = GetKeyOfType(API.RequestObjects.OnBase.DocumentUpload.KeyTypes.agencyCode);
            var descriptionKey = GetKeyOfType(API.RequestObjects.OnBase.DocumentUpload.KeyTypes.description);

            if (descriptionKey != null && descriptionKey.Values.Length > 0)
            {
                foreach (var desc in descriptionKey.Values)
                {
                    if (desc.Length > 100)
                    {
                        errorMessage = "A value for the description key was found to have a length greater than 100 characters.";
                        return false;
                    }
                }
            }

            switch (payLoad.SourceSystem)
            {
                case API.RequestObjects.OnBase.DocumentUpload.SourceSystems.VelociraterImporter:
                    if (!KeyHasExactlyOneValue(GetKeyOfType(API.RequestObjects.OnBase.DocumentUpload.KeyTypes.policyNumber).Values))
                    {
                        errorMessage = "The policyNumber key does not have exactly 1 value.";
                        return false;
                    }
                    break;
            }

            errorMessage = "";
            return true;
        }
    }
}