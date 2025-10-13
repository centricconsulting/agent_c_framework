using IFI.Integrations.Objects;
using IFI.Integrations.Request.Objects;
using IFI.Integrations.Response.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFI.Integrations.Request
{
    public class ComPropertyPrefill : VendorRequest
    {
        public List<Objects.ComPropertyPrefill> PropertyData { get; set; } = new List<Objects.ComPropertyPrefill>();

        public ComPropertyPrefill(string API_Address, string apiKey) : base(API_Address, apiKey, "rating", "compropertyprefill") { }
        public ComPropertyPrefill(string API_Address, string apiKey, bool useLibraryValidation) : base(API_Address, apiKey, "rating", "compropertyprefill", useLibraryValidation) { }

        private void ValidateRequest()
        {
            if (_useLibraryValidation)
            {
                foreach (var item in PropertyData)
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

        public ServiceResult<List<ServiceResult<Response.ComPropertyPrefill>>> GetVendorData()
        {
            ValidateRequest();

            var response = SendRequestForListResponse<IEnumerable<ServiceResult<List<ServiceResult<Response.ComPropertyPrefill>>>>>(InternalEnums.TransType.Post, this);

            return SendRequestForListResponse<IEnumerable<ServiceResult<List<ServiceResult<Response.ComPropertyPrefill>>>>>(InternalEnums.TransType.Post, this)?.FirstOrDefault();
        }

        public async Task<ServiceResult<List<ServiceResult<Response.ComPropertyPrefill>>>> GetVendorData_Async()
        {
            ValidateRequest();

            var response = await SendRequestForListResponse_Async<IEnumerable<ServiceResult<List<ServiceResult<Response.ComPropertyPrefill>>>>>(InternalEnums.TransType.Post, this);

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
            if (string.IsNullOrEmpty(firstName) == false){ location.FirstName = firstName; }
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
