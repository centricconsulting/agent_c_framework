using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFM.DataServices.API.ResponseObjects.Payments
{
    [System.Serializable]
    public class AuthAndSessionTokens
    {
        public string AuthToken { get; set; } //passed to iframe and to our payment service
        public string SessionToken { get; set; } //passed to iframe and to our payment service
        public int SessionId { get; set; } //only passed to our payment service
    }
}