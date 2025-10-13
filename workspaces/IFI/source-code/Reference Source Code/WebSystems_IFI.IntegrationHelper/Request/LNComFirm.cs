using IFI.Integrations.Objects;
using IFI.Integrations.Response.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFI.Integrations.Request
{
    //LexisNexis Commercial Firmographics
    public class LNComFirm : VendorRequest
    {
        public string BusinessName { get; set; }
        public string Address1 { get; set; }
        public string OtherInfo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public LNComFirm(string API_Address, string apiKey) : base(API_Address, apiKey, "rating", "lnfirm") { }
        public LNComFirm(string API_Address, string apiKey, bool useLibraryValidation) : base(API_Address, apiKey, "rating", "lnfirm", useLibraryValidation) { }

        private void ValidateRequest()
        {
            if (_useLibraryValidation)
            {
                //base.ValidateRequest();
                if (_isPreload == false)
                {
                    //TestRequiredVariable(PolicyId, nameof(PolicyId));
                    //TestRequiredVariable(PolicyImageNum, nameof(PolicyImageNum));
                }
                TestRequiredVariable_OneIsRequired(new string[] { Address1, OtherInfo }, new string[] { nameof(Address1), nameof(OtherInfo) });
                TestRequiredVariable(City, nameof(City));
                TestTwoLetterVariable(State, nameof(State));
                TestZip(Zip, nameof(Zip));
            }
        }

        public ServiceResult<Response.LNComFirm> GetVendorData()
        {
            ValidateRequest();

            return SendRequestForListResponse<IEnumerable<ServiceResult<Response.LNComFirm>>>(InternalEnums.TransType.Post, this)?.FirstOrDefault();
        }

        public async Task<ServiceResult<Response.LNComFirm>> GetVendorData_Async()
        {
            ValidateRequest();

            var response = await SendRequestForListResponse_Async<IEnumerable<ServiceResult<Response.LNComFirm>>>(InternalEnums.TransType.Post, this);
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
