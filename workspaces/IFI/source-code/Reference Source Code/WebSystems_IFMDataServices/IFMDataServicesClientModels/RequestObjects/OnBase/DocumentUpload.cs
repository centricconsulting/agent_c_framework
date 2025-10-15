using IFM.DataServices.API.ResponseObjects.OnBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFM.DataServices.API.RequestObjects.OnBase
{
    /// <summary>
    /// Class used for making call to API to upload documents into OnBase
    /// </summary>
    [System.Serializable]
    public class DocumentUpload : BaseRequest
    {
        public enum SourceSystems
        {
            None = 0,
            VelociraterImporter = 1,
            FNOLAssignment = 2,
            Millennium = 3,
            eChecksNightly = 4,
            eSignatureApplication = 5,
            eSignatureProxy = 6,
            Fiserv = 7,
            FNOLAssignmentInvestigation = 8
        }

        public enum KeyTypes
        {
            none = 1,
            agencyCode = 2,
            policyNumber = 3,
            claimNumber = 4,
            description = 5,
            needsProcessing = 6
        }

        public SourceSystems UploadType { get; set; }
        public byte[] FileBytes { get; set; }
        public string FileName { get; set; }
        public Dictionary<KeyTypes, string> Keys { get; set; } = new Dictionary<KeyTypes, string>();
        /// <summary>
        /// Legacy - Developers should use "Keys" instead so that they can use the Enums for easier coding. This object is here due to a bug in how Newtonsoft translates Dictionaries with int values
        /// </summary>
        [Obsolete("Please use Keys instead.")]
        public Dictionary<string, string> OnBaseKeys { get; set; } = new Dictionary<string, string>();

        public DocumentUpload(string APIAddress) : base(APIAddress)
        {
            API_Path = "OnBase/Document";
        }

        public DocumentUpload() : base("")
        {
            API_Path = "OnBase/Document";
        }

        public OnBaseUploadResponse UploadDocument()
        {
            TestRequiredVariable(UploadType, (int)SourceSystems.None, nameof(UploadType));
            TestRequiredVariable(FileName, nameof(FileName));
            TestRequiredVariable(FileBytes, nameof(FileBytes));
            TestRequiredVariable(Keys, nameof(Keys));

            foreach (var kvp in Keys)
            {
                OnBaseKeys.Add(((int)kvp.Key).ToString(), kvp.Value);
            }
            var a = new OnBaseUploadResponse();
            API_Endpoint = "UploadByType";
            return Post<OnBaseUploadResponse>(this);
        }
    }
}