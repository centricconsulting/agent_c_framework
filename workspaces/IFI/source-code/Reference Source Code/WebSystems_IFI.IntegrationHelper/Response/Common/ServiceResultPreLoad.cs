using System;
using System.Collections.Generic;
using System.Text;

namespace IFI.Integrations.Response.Common
{
    [System.Serializable]
    public class ServiceResultPreLoad : BaseResult
    {
        public bool requestReceived { get; set; }
        public bool hasError { get; set; }
        public string errorMessage { get; set; }
    }
}
