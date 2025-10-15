namespace IFM.DataServices.API.ResponseObjects.OnBase
{
    /// <summary>
    /// Class used for the response from the API
    /// </summary>
    [System.Serializable]
    public class OnBaseUploadResponse : IFM.DataServices.API.ResponseObjects.Common.BaseResult
    {
        public bool Success;
        public long OnBaseDocumentID;
        public string ErrorMessage;
        public long ErrorCode;
    }
}