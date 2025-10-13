using System;
using System.Collections.Generic;

namespace IFM.PolicyLoader
{
    public class LoadPolicyRequest<T>
    {
        public string PolicyNumber { get; set; }

        public T ContextData { get; set; }

        public LoadPolicyRequest(string policyNumber)
        {
            this.PolicyNumber = policyNumber;
        }
        public LoadPolicyRequest(string policyNumber, T contextData)
        {
            this.PolicyNumber = policyNumber;
            this.ContextData = contextData;
        }
    }
}