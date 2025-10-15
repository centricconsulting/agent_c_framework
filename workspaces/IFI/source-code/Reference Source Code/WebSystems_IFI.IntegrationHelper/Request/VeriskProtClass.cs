using IFI.Integrations.Objects;
using IFI.Integrations.Response.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IFI.Integrations.Request
{
    public class ProtectionClass : VendorRequest
    {

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }

        public ProtectionClass(string API_Address, string apiKey) : base(API_Address, apiKey, "rating", "veriskppc") { }

        public ProtectionClass(string API_Address, string apiKey, bool useLibraryValidation) : base(API_Address, apiKey, "rating", "veriskppc", useLibraryValidation) { }

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
                        TestTwoLetterVariable(Country, nameof(Country));
                        TestTwoLetterVariable(State, nameof(State));
                        TestZip(Zip, nameof(Zip));
                }
        }

        public ServiceResult<Response.ProtectionClass> GetVendorData()
        {
            ValidateRequest();

            return SendRequestForListResponse<IEnumerable<ServiceResult<Response.ProtectionClass>>>(InternalEnums.TransType.Post, this)?.FirstOrDefault();
        }

        public async Task<ServiceResult<Response.ProtectionClass>> GetVendorData_Async()
        {
            ValidateRequest();

            var response = await SendRequestForListResponse_Async<IEnumerable<ServiceResult<Response.ProtectionClass>>>(InternalEnums.TransType.Post, this);
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
