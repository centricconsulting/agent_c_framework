using IFI.Integrations.Objects;
using IFI.Integrations.Response.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using IFI.Integrations.Request.Objects;
using System.ComponentModel;

namespace IFI.Integrations.Request
{
    //LexisNexis Commercial Property
    public class LNComProp : VendorRequest
    {
        public List<Objects.ComPropertyPrefill> LexisNexisCommercialPropertyRequests { get; set; } = new List<Objects.ComPropertyPrefill>();

        public LNComProp(string API_Address, string apiKey) : base(API_Address, apiKey, "rating", "lnprop") { }
        public LNComProp(string API_Address, string apiKey, bool useLibraryValidation) : base(API_Address, apiKey, "rating", "lnprop", useLibraryValidation) { }

        private void ValidateRequest()

        {
            if (_useLibraryValidation)
            {
                foreach (var item in LexisNexisCommercialPropertyRequests)
                {
                    if (_isPreload == false)
                    {
                        //TestRequiredVariable(PolicyId, nameof(PolicyId));
                        //TestRequiredVariable(PolicyImageNum, nameof(PolicyImageNum));
                    }
                    TestRequiredVariable_OneIsRequired(new string[] { item.FirstName, item.LastName }, new string[] { nameof(item.FirstName), nameof(item.LastName) });
                    TestRequiredVariable(item.City, nameof(item.City));
                    TestTwoLetterVariable(item.State, nameof(item.State));
                    TestZip(item.Zip, nameof(item.Zip));
                }
            }
        }

        public ServiceResult<Response.LNComProp> GetVendorData()
        {
            ValidateRequest();

            return SendRequestForListResponse<IEnumerable<ServiceResult<Response.LNComProp>>>(InternalEnums.TransType.Post, this)?.FirstOrDefault();
        }

        public async Task<ServiceResult<Response.LNComProp>> GetVendorData_Async()
        {
            ValidateRequest();

            var response = await SendRequestForListResponse_Async<IEnumerable<ServiceResult<Response.LNComProp>>>(InternalEnums.TransType.Post, this);
            return response?.FirstOrDefault();
        }

        public ServiceResultPreLoad PreLoadVendorData()
        {
            SetPreload();
            ValidateRequest();

            return SendRequestForListResponse<IEnumerable<ServiceResultPreLoad>>(InternalEnums.TransType.Post, this)?.FirstOrDefault();
        }

        public async Task<ServiceResultPreLoad> PreLoadVendorData_Async()
        {
            SetPreload();
            ValidateRequest();

            var response = await SendRequestForListResponse_Async<IEnumerable<ServiceResultPreLoad>>(InternalEnums.TransType.Post, this);
            return response?.FirstOrDefault();
        }
    }
}
