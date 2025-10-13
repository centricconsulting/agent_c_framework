using System;

namespace IFM.VR.Validation.ObjectValidation
{
    [Serializable()]
    public class ValidationItem
    {
        public enum ValidationType : int
        {
            //quote = 1,
            quoteRate = 5,

            //app = 2,
            appRate = 3,

            issuance = 4,

            //  CAH 05/07/2019
            endorsement = 6,
            readOnly = 7,
            billing = 8

        }

        //public const string CustomMessageIndicator = "!-*+$()";
        public string Message { get; set; }

        public string FieldId { get; set; }

        public bool IsWarning { get; set; }

        public bool RequiresRouteToUw { get; set; }

        public ValidationItem(string msg)
        {
            this.Message = msg;
        }

        public ValidationItem(string msg, string fieldID)
        {
            this.Message = msg;
            this.FieldId = fieldID;
        }

        public ValidationItem(string msg, string fieldID, bool IsWarning)
        {
            this.Message = msg;
            this.FieldId = fieldID;
            this.IsWarning = IsWarning;
        }

        public ValidationItem(string msg, string fieldID, bool IsWarning, bool RequireRouteToUw)
        {
            this.Message = msg;
            this.FieldId = fieldID;
            this.IsWarning = IsWarning;
            this.RequiresRouteToUw = RequireRouteToUw;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}