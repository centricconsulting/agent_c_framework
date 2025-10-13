using IFI.Integrations.Response.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IFI.Integrations.Objects
{
    public class VendorRequest : APIClass
    {
        /// <summary>
        /// If you know the policyId, set this so we can store it in our records.
        /// </summary>
        public int PolicyId { get; set; } = 1;

        /// <summary>
        /// If you know the policyImageNum, set this so we can store it in our records.
        /// </summary>
        public int PolicyImageNum { get; set; } = 1;

        /// <summary>
        /// Set to true if we should ignore cache and force a call to the vendor - will also replace current cache
        /// </summary>
        public bool isReorder { get; set; } = false;

        /// <summary>
        /// set to true to simulate a bad URL error message on the vendor's end
        /// </summary>
        public bool apiBadURL { get; set; } = false;

        /// <summary>
        /// set to true to simulate a bad API key when trying to connect to the vendor.
        /// </summary>
        public bool apiBadKey { get; set; } = false;

        private string _apiBaseName = "";
        protected bool _useLibraryValidation = true;
        protected bool _isPreload = false;

        protected VendorRequest(string apiAddress, string apiKey, string apiPath, string apiBaseName) : base(apiAddress)
        {
            Init(apiKey, apiPath, apiBaseName, true);
        }

        protected VendorRequest(string apiAddress, string apiKey, string apiPath, string apiBaseName, bool useLibraryValidation) : base(apiAddress)
        {
            Init(apiKey, apiPath, apiBaseName, useLibraryValidation);
        }

        protected void SetPreload()
        {
            _isPreload = true;
            API_Endpoint = _apiBaseName + "preload";
        }

        protected void Init(string apiKey, string apiPath, string apiBaseName, bool useLibraryValidation)
        {
            API_Path = apiPath;
            HeadersToAdd = new Dictionary<string, string>
            {
                { "x-api-key", apiKey }
            };
            _apiBaseName = apiBaseName;
            _useLibraryValidation = useLibraryValidation;
            API_Endpoint = _apiBaseName;
        }
    }
}
