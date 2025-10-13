using IFI.Integrations.Objects;
using IFI.Integrations.Response.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace IFI.Integrations.Request
{
    public class CapeCom : VendorRequest
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public CapeCom(string API_Address, string apiKey) : base(API_Address, apiKey, "rating", "capecom") { }
        public CapeCom(string API_Address, string apiKey, bool useLibraryValidation) : base(API_Address, apiKey, "rating", "capecom", useLibraryValidation) { }

        private void ValidateRequest()
        {
            if (_useLibraryValidation)
            {
                if (_isPreload == false)
                {
                    TestRequiredVariable(PolicyId, nameof(PolicyId));
                    TestRequiredVariable(PolicyImageNum, nameof(PolicyImageNum));
                }
                TestRequiredVariable_OneIsRequired(new string[] { Address1, Address2 }, new string[] { nameof(Address1), nameof(Address2) });
                TestRequiredVariable(City, nameof(City));
                TestTwoLetterVariable(State, nameof(State));
                TestZip(Zip, nameof(Zip));
            }
        }

        public ServiceResult<Response.CapeCom> GetVendorData()
        {
            ValidateRequest();

            return SendRequestForListResponse<IEnumerable<ServiceResult<Response.CapeCom>>>(InternalEnums.TransType.Post, this)?.FirstOrDefault();
        }

        public async Task<ServiceResult<Response.CapeCom>> GetVendorData_Async()
        {
            ValidateRequest();

            var response = await SendRequestForListResponse_Async<IEnumerable<ServiceResult<Response.CapeCom>>>(InternalEnums.TransType.Post, this);
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
