using System;
using System.Collections.Generic;
using System.Text;

namespace IFI.Integrations.Response.Common
{
    public class ErrorInfo
    {
        public string extraDetail { get; set; }
        public string reason { get; set; }
        public string httpStatusCode { get; set; }
        public string vendorStatusCode { get; set; }
        public bool isValidation { get; set; }
        public bool isTimeout { get; set; }
        public bool isUnrecoverable { get; set; }
    }
}
