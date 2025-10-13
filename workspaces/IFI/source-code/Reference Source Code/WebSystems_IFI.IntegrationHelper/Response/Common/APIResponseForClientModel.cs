using System;
using IFI.Integrations.InternalExtensions;

namespace IFI.Integrations.Response.Common
{
    [System.Serializable]
    public class APIResponseForClientModel
    {
        internal string _responseStatusCode;
        public string ResponseStatusCode
        {
            get { return _responseStatusCode; }
        }
        internal string _responseMessage;
        public string ResponseMessage
        {
            get { return _responseMessage; }
        }
        internal bool _responseDeserialized;
        public bool ResponseDeserialized
        {
            get { return _responseDeserialized; }
        }
        internal bool _isResponseNull;
        public bool IsResponseNull
        {
            get { return _isResponseNull; }
        }
        public bool ExceptionCaught
        {
            get { return Exception == null ? false : true; }
        }
        internal Exception _exception;
        public Exception Exception
        {
            get { return _exception; }
        }

        /// <summary>
        /// Will put status code, response message, exception message and stack trace, if applicable, into a string. Meant mainly for loggin or debugging purposes. Not meant for sending back to UI.
        /// </summary>
        public string ResponseDebugErrorMessage
        {
            get
            {
                string errMsg = "";
                if (string.IsNullOrWhiteSpace(ResponseStatusCode) == false)
                {
                    errMsg = errMsg.AppendText(ResponseStatusCode, "; ", "StatusCode: ");
                }
                if (string.IsNullOrWhiteSpace(ResponseMessage) == false)
                {
                    errMsg = errMsg.AppendText(ResponseMessage, "; ", "ResponseMessage: ");
                }
                if (ExceptionCaught)
                {
                    errMsg = errMsg.AppendText(Exception.Message, "; ", "ExceptionMessage: ");
                    errMsg = errMsg.AppendText(Exception.StackTrace, "; ", "StackTrace: ");
                }
                if (IsResponseNull)
                {
                    errMsg = errMsg.AppendText("true", "; ", "IsResponseNull: ");
                }
                return errMsg;
            }
        }
    }
}
