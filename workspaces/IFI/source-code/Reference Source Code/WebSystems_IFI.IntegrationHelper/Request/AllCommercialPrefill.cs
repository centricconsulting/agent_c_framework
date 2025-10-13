using IFI.Integrations.Objects;
using IFI.Integrations.Request.Objects;
using IFI.Integrations.Response.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IFI.Integrations.Request
{
    public class AllCommercialPrefill : VendorRequest
    {
        public ComFirmographicsPrefill FirmographicsData { get; set; } = new ComFirmographicsPrefill();
        public List<Objects.ComPropertyPrefill> PropertyData { get; set; } = new List<Objects.ComPropertyPrefill>();

        public AllCommercialPrefill(string API_Address, string apiKey) : base(API_Address, apiKey, "rating", "allcomprefill") { }

        public AllCommercialPrefill(string API_Address, string apiKey, bool useLibraryValidation) : base(API_Address, apiKey, "rating", "allcomprefill", useLibraryValidation) { }

        private void ValidateRequest()
        {
            if (_useLibraryValidation)
            {
                if (_isPreload == false)
                {
                    //TestRequiredVariable(PolicyId, nameof(PolicyId));
                    //TestRequiredVariable(PolicyImageNum, nameof(PolicyImageNum));
                }
                TestRequiredVariable(FirmographicsData.BusinessName, nameof(FirmographicsData.BusinessName));
                TestRequiredVariable(FirmographicsData.Address1, nameof(FirmographicsData.Address1));
                TestRequiredVariable(FirmographicsData.City, nameof(FirmographicsData.City));
                TestTwoLetterVariable(FirmographicsData.State, nameof(FirmographicsData.State));
                TestZip(FirmographicsData.Zip, nameof(FirmographicsData.Zip));
                foreach (var propInfo in PropertyData)
                {
                    TestRequiredVariable_OneIsRequired(new string[] { propInfo.FirstName, propInfo.LastName }, new string[] { nameof(propInfo.FirstName), nameof(propInfo.LastName) });
                    TestRequiredVariable(propInfo.City, nameof(propInfo.City));
                    TestTwoLetterVariable(propInfo.State, nameof(propInfo.State));
                    TestZip(propInfo.Zip, nameof(propInfo.Zip));
                }
            }
        }

        public ServiceResult<Response.AllCommercialPrefill> GetVendorData()
        {
            ValidateRequest();

            return SendRequestForListResponse<IEnumerable<ServiceResult<Response.AllCommercialPrefill>>>(InternalEnums.TransType.Post, this)?.FirstOrDefault();
        }

        public async Task<ServiceResult<Response.AllCommercialPrefill>> GetVendorData_Async()
        {
            ValidateRequest();

            var response = await SendRequestForListResponse_Async<IEnumerable<ServiceResult<Response.AllCommercialPrefill>>>(InternalEnums.TransType.Post, this);
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

        public void AddPropertyLocation(string firstName = "", string middleName = "", string lastName = "", string streetNumber = "", string streetName = "", string unitNum = "", string city = "", string state = "", string zip = "")
        {
            Objects.ComPropertyPrefill location = new Objects.ComPropertyPrefill();
            if (string.IsNullOrEmpty(firstName) == false) { location.FirstName = firstName; }
            if (string.IsNullOrEmpty(middleName) == false) { location.MiddleName = middleName; }
            if (string.IsNullOrEmpty(lastName) == false) { location.LastName = lastName; }
            if (string.IsNullOrEmpty(streetNumber) == false) { location.StreetNumber = streetNumber; }
            if (string.IsNullOrEmpty(streetName) == false) { location.StreetName = streetName; }
            if (string.IsNullOrEmpty(unitNum) == false) { location.UnitNumber = unitNum; }
            if (string.IsNullOrEmpty(city) == false) { location.City = city; }
            if (string.IsNullOrEmpty(state) == false) { location.State = state; }
            if (string.IsNullOrEmpty(zip) == false) { location.Zip = zip; }
            PropertyData.Add(location);
        }
    }
}
