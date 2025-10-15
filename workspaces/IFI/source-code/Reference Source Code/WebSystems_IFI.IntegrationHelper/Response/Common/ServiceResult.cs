using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IFI.Integrations.Response.Common
{
    [System.Serializable]
    public class ServiceResult<T> : BaseResult
    {
        public T responseData { get; set; }
        public bool isCachedResult { get; set; }
        public bool isReorder { get; set; }
        public bool hasError { get; set; }
        public ErrorInfo errorInfo { get; set; }
        public bool isNoHit { get; set; }
        public string applicationRequestId { get; set; }
    }
}
