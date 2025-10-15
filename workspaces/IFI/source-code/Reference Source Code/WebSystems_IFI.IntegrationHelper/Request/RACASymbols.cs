using IFI.Integrations.Objects;
using IFI.Integrations.Response.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFI.Integrations.Request
{
    public class RACASymbols : VendorRequest
    {
        public List<String> Vin { get; set; } = new List<String>();

        public RACASymbols(string API_Address, string apiKey) : base(API_Address, apiKey, "rating", "raca") { }

        public RACASymbols(string API_Address, string apiKey, bool useLibraryValidation) : base(API_Address, apiKey, "rating", "raca", useLibraryValidation) { }

        public ServiceResult<Response.RACAVins> GetVendorData()
        {
            return SendRequestForListResponse<IEnumerable<ServiceResult<Response.RACAVins>>>(InternalEnums.TransType.Post, this)?.FirstOrDefault();
        }

        public async Task<ServiceResult<Response.RACAVins>> GetVendorData_Async()
        {
            var response = await SendRequestForListResponse_Async<IEnumerable<ServiceResult<Response.RACAVins>>>(InternalEnums.TransType.Post, this);
            return response?.FirstOrDefault();
        }
    }
}
