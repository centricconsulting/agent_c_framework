using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFM.DataServices.API.RequestObjects.OnBase;

namespace IFM.DataServicesCore.CommonObjects.OnBase
{
    [Serializable]
    public class UploadPayload
    {
        public DocumentUpload.SourceSystems SourceSystem { get; set; }
        public string FileExtension { get; set; }
        public List<OnBaseKey> Keys { get; set; } = new List<OnBaseKey>();
        public string Base64Payload { get; set; }
        public string FullFileName { get; set; }
        public string errorMessage { get; set; }
        internal DocumentUpload.SourceSystems OriginalSourceSystem { get; set; }
        internal String[] DocumentSubType { get; set; }
        internal String[] OriginalDocumentSubType { get; set; }

        public void SetBase64Payload(byte[] byteArray)
        {
            this.Base64Payload = Convert.ToBase64String(byteArray, 0, byteArray.Length, Base64FormattingOptions.None);
        }

        public void SetExtensionFromFullFileName(string fileName)
        {
            this.FileExtension = System.IO.Path.GetExtension(fileName);
            this.FullFileName = fileName;
        }

        public IEnumerable<string> GetOnBaseDocumentType_Collection()
        {
            return DocumentTypes.GetOnBaseDocumentTypeKeyValue(this);
        }

        public IEnumerable<string> GetOnBaseDocumentSubtype_Collection()
        {
            return DocumentSubTypes.GetOnBaseSubDocumentTypeKeyValue(this);
        }

        public IEnumerable<string> GetOnBaseMimeType()
        {
            return MimeTypes.GetMimeTypeFromFileExtension(this.FileExtension);
        }

        public void LogSuccessfulUpload(long docId)
        {
            CommonHelperClass chc = new CommonHelperClass();
            var doLog = chc.GetApplicationXMLSetting("LogSuccessfulOnBaseUploadsToITErrorLog", "OnBaseUploadSettings.xml");
            if (doLog.StringsAreEqual("true"))
            {
                global::IFM.IFMErrorLogging.LogIssue("Succesfully uploaded to OnBase!", $"Returned ID: '{docId}'; SourceSystem: {this.SourceSystem}; Original SourceSystem: {this.OriginalSourceSystem}; DocumentType:{DisplayStringArrayForErrorMessage(this.DocumentSubType)}; Original DocumentType:{DisplayStringArrayForErrorMessage(this.OriginalDocumentSubType)}; FullFileName: {this.FullFileName}; FileExtension: {this.FileExtension}; Payload Keys: {DisplayKeysForErrorMessage()}");
            }
        }

        private string DisplayStringArrayForErrorMessage(string[] stringArray)
        {
            string returnVar = "";
            if (stringArray.IsLoaded())
            {
                if (stringArray.Count() == 1)
                {
                    returnVar = stringArray[0];
                }
                else
                {
                    foreach (var item in stringArray)
                    {
                        returnVar += $"{item},";
                    }
                }
            }
            return returnVar;
        }

        private string DisplayKeysForErrorMessage()
        {
            if (this?.Keys?.IsLoaded() == true)
            {
                int count = 0;
                string errorString = "";
                foreach (var key in this.Keys)
                {
                    if (key.Values.IsLoaded())
                    {
                        if (errorString != "")
                        {
                            errorString += "; ";
                        }
                        errorString += $"Key '{count}': '{key.Key}' = ";
                        string errValues = "";
                        foreach (var val in key.Values)
                        {
                            if (errValues != "")
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

        public static UploadPayload CreatePayloadObject(DocumentUpload basicpayload)
        {
            UploadPayload payload = new UploadPayload();

            if (basicpayload.UploadType > 0)
            {
                if (Enum.IsDefined(typeof(DocumentUpload.SourceSystems), basicpayload.UploadType))
                    payload.SourceSystem = (DocumentUpload.SourceSystems)basicpayload.UploadType;
                else
                    payload.errorMessage = "Upload Type is invalid.";
            }
            else
            {
                payload.errorMessage = "Upload Type is required.";
            }

            if (basicpayload.FileBytes != null)
                payload.SetBase64Payload(basicpayload.FileBytes);
            else
                payload.errorMessage = "File Bytes is required.";

            if (basicpayload.FileName != null)
                payload.SetExtensionFromFullFileName(basicpayload.FileName);
            else
                payload.errorMessage = "File Extension is required.";

            foreach (var kvp in basicpayload.OnBaseKeys)
            {
                if (kvp.Key.TryToGetInt32() > 1)
                {
                    if (Enum.IsDefined(typeof(DocumentUpload.KeyTypes), kvp.Key.TryToGetInt32()))
                    {
                        payload.Keys.Add(new OnBaseKey() { Key = (DocumentUpload.KeyTypes)kvp.Key.TryToGetInt32(), Values = new string[] { kvp.Value } });
                    }
                    else
                        payload.errorMessage = "Key Type is invalid.";
                }
                else
                    payload.errorMessage = "Key Type must be greater than 1.";
            }

            return payload;
        }
    }

    [Serializable]
    public class OnBaseKey
    {
        public DocumentUpload.KeyTypes Key { get; set; } = DocumentUpload.KeyTypes.none;
        public string[] Values { get; set; }
    }

    [Serializable]
    internal class DocumentTypes
    {
        public readonly static List<DocumentTypes> DocumentTypeList = new List<DocumentTypes>
        {
            new DocumentTypes { SourceSystem = DocumentUpload.SourceSystems.VelociraterImporter, DocumentType = "UW Change Request"},

            //new DocumentTypes { SourceSystem = DocumentUpload.SourceSystems.FNOLAssignment, DocumentType = "CLM FNOL"},

            new DocumentTypes { SourceSystem = DocumentUpload.SourceSystems.FNOLAssignment, DocumentType = "CLM Coverage"},

            new DocumentTypes { SourceSystem = DocumentUpload.SourceSystems.FNOLAssignmentInvestigation, DocumentType = "CLM Investigation"},

            new DocumentTypes { SourceSystem = DocumentUpload.SourceSystems.Millennium, DocumentType = "UW NB & Reinspections (Millennium)"},

            new DocumentTypes { SourceSystem = DocumentUpload.SourceSystems.eChecksNightly, DocumentType = "FIN Echeck Payment Confirmation Letter"},

            new DocumentTypes { SourceSystem = DocumentUpload.SourceSystems.eSignatureApplication, DocumentType = "UW Signed Application"},

            new DocumentTypes { SourceSystem = DocumentUpload.SourceSystems.eSignatureProxy, DocumentType = "UW Signed Proxy"},

            new DocumentTypes { SourceSystem = DocumentUpload.SourceSystems.Fiserv, DocumentType = "UW Certificate of Bulk Mailing"}
        };

        //upload file extension types
        const string multiMediaExtensions = "avi|mp3|mp4|m4a|wav|wmv|mpg|mov";
        const string pictureExtensions = "bmp|gif|jpg|png|tif|jpeg";
        const string documentExtensions = "csv|doc|docx|rtf|txt|xls|xlsx|pdf|html";

        //upload document sub types
        const string multiMediaSubType = "Multimedia";
        const string pictureSubType = "Picture";
        const string documentSubType = "Document";

        public DocumentUpload.SourceSystems SourceSystem { get; set; }
        public string DocumentType { get; set; }

        public static IEnumerable<string> GetOnBaseDocumentTypeKeyValue(UploadPayload fileUpload)
        {
            if (fileUpload.OriginalSourceSystem == DocumentUpload.SourceSystems.None)
            {
                if (fileUpload.SourceSystem == DocumentUpload.SourceSystems.None)
                    throw new ArgumentException($"{nameof(SourceSystem)} is invalid.");

                var fileExtension = fileUpload.FileExtension.Replace(".", "").Trim().ToLower();
                fileUpload.OriginalSourceSystem = fileUpload.SourceSystem;
                switch (fileUpload.SourceSystem)
                {
                    case DocumentUpload.SourceSystems.FNOLAssignment:
                        if (!string.IsNullOrWhiteSpace(fileUpload.FullFileName))
                        {
                            if (fileExtension == "html" || fileExtension == "htm" || fileUpload.FullFileName.ToLower().Contains("_fnol_dec_"))
                            {
                                fileUpload.SourceSystem = DocumentUpload.SourceSystems.FNOLAssignment;
                            }
                            else if (pictureExtensions.Split('|').ToList().Contains(fileExtension.ToLower()))
                            {
                                fileUpload.SourceSystem = DocumentUpload.SourceSystems.FNOLAssignmentInvestigation;
                            }
                            else if (documentExtensions.Split('|').ToList().Contains(fileExtension.ToLower()))
                            {
                                fileUpload.SourceSystem = DocumentUpload.SourceSystems.FNOLAssignmentInvestigation;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            return (from type in DocumentTypes.DocumentTypeList
                        where type.SourceSystem == fileUpload.SourceSystem
                        select type.DocumentType);
        }
    }

    [Serializable]
    internal class DocumentSubTypes
    {
        //upload file extension types
        const string multiMediaExtensions = "avi|mp3|mp4|m4a|wav|wmv|mpg|mov";
        const string pictureExtensions = "bmp|gif|jpg|png|tif|jpeg";
        const string documentExtensions = "csv|doc|docx|rtf|txt|xls|xlsx|pdf|html";

        //upload document sub types
        const string multiMediaSubType = "Multimedia";
        const string pictureSubType = "Picture";
        const string documentSubType = "Document";

        private readonly static List<DocumentSubTypes> DocumentSubTypeList = new List<DocumentSubTypes>
        {
            new DocumentSubTypes { FileExtensions = multiMediaExtensions, DocumentSubType = multiMediaSubType},

            new DocumentSubTypes { FileExtensions = pictureExtensions, DocumentSubType = pictureSubType},

            new DocumentSubTypes { FileExtensions = documentExtensions, DocumentSubType = documentSubType},
        };

        private string FileExtensions { get; set; }
        private string DocumentSubType { get; set; }

        public static IEnumerable<string> GetOnBaseSubDocumentTypeKeyValue(UploadPayload fileUpload)
        {
            if (fileUpload.OriginalDocumentSubType == null)
            {
                if (string.IsNullOrWhiteSpace(fileUpload.FileExtension))
                    throw new ArgumentException($"{nameof(fileUpload.FileExtension)} was null or empty.");
                var fileExtension = fileUpload.FileExtension.Replace(".", "").Trim().ToLower();

                var subDocType = new string[] {(from type in DocumentSubTypes.DocumentSubTypeList
                                                                    where type.FileExtensions.Contains(fileExtension)
                                                                    select type.DocumentSubType).FirstOrDefault() };

                fileUpload.OriginalDocumentSubType = subDocType;

                switch (fileUpload.SourceSystem)
                {
                    case DocumentUpload.SourceSystems.FNOLAssignment:
                        if (!string.IsNullOrWhiteSpace(fileUpload.FullFileName))
                        {
                            if (fileExtension == "html" || fileExtension == "htm")
                                subDocType = new string[] { "FNOL" };
                            else if (fileUpload.FullFileName.ToLower().Contains("_fnol_dec_"))
                            {
                                subDocType = new string[] { "Policy Forms" };
                            }
                            else if (pictureExtensions.Split('|').ToList().Contains(fileExtension.ToLower()))
                            {
                                subDocType = new string[] { "Photos & Diagrams" };
                            }
                            else if (documentExtensions.Split('|').ToList().Contains(fileExtension.ToLower()))
                            {
                                subDocType = new string[] { "Correspondence & Other" };
                            }
                        }
                        break;
                    case DocumentUpload.SourceSystems.FNOLAssignmentInvestigation:
                        if (!string.IsNullOrWhiteSpace(fileUpload.FullFileName))
                        {
                            if (pictureExtensions.Split('|').ToList().Contains(fileExtension.ToLower()))
                            {
                                subDocType = new string[] { "Photos & Diagrams" };
                            }
                            else if (documentExtensions.Split('|').ToList().Contains(fileExtension.ToLower()))
                            {
                                subDocType = new string[] { "Correspondence & Other" };
                            }
                        }
                        break;
                    default:
                        break;
                }
                fileUpload.DocumentSubType = subDocType;
            }
            return fileUpload.DocumentSubType;
        }
    }

    [Serializable]
    internal class MimeTypes
    {
        private readonly static List<MimeTypes> mimeList = new List<MimeTypes>
        {
            new MimeTypes { FileExtension = "avi", MimeType = "video/avi" },
            new MimeTypes { FileExtension = "bmp", MimeType = "image/bmp" },
            new MimeTypes { FileExtension = "csv", MimeType = "text/.csv" },
            new MimeTypes { FileExtension = "html", MimeType = "text/html" },
            new MimeTypes { FileExtension = "htm", MimeType = "text/html" },
            new MimeTypes { FileExtension = "doc", MimeType = "application/msword" },
            new MimeTypes { FileExtension = "docx", MimeType = "application/mswordnew" },
            new MimeTypes { FileExtension = "gif", MimeType = "image/gif" },
            new MimeTypes { FileExtension = "jpg", MimeType = "image/jpg" },
            new MimeTypes { FileExtension = "jpeg", MimeType = "image/jpg" },
            new MimeTypes { FileExtension = "m4a", MimeType = "video/m4a" },
            new MimeTypes { FileExtension = "mov", MimeType = "video/quicktime" },
            new MimeTypes { FileExtension = "mp3", MimeType = "video/mp3" },
            new MimeTypes { FileExtension = "mp4", MimeType = "video/mp4" },
            new MimeTypes { FileExtension = "mpg", MimeType = "video/mpg" },
            new MimeTypes { FileExtension = "pdf", MimeType = "application/pdf" },
            new MimeTypes { FileExtension = "png", MimeType = "image/png" },
            new MimeTypes { FileExtension = "rtf", MimeType = "text/richtext" },
            new MimeTypes { FileExtension = "xls", MimeType = "application/vnd.ms-excel" },
            new MimeTypes { FileExtension = "xlsx", MimeType = "application/vnd.ms-excelnew" },
            new MimeTypes { FileExtension = "tif", MimeType = "image/tiff" },
            new MimeTypes { FileExtension = "txt", MimeType = "text/plain" },
            new MimeTypes { FileExtension = "wav", MimeType = "audio/x-wav" },
            new MimeTypes { FileExtension = "wmv", MimeType = "video/wmv" }
        };

        private string FileExtension { get; set; }
        private string MimeType { get; set; }
        public static IEnumerable<string> GetMimeTypeFromFileExtension(string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(fileExtension))
                throw new ArgumentException($"{nameof(fileExtension)} was null or empty.");
            fileExtension = fileExtension.Replace(".", "").Trim().ToLower();

            return from type in MimeTypes.mimeList
                   where type.FileExtension.Contains(fileExtension)
                   select type.MimeType;
        }
    }

}
