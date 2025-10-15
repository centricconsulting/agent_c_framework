using System;
using System.Collections.Generic;
using System.Linq;
using DCO = Diamond.Common.Objects;
using DCE = Diamond.Common.Enums;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class Location : ModelBase
    {
        public Address Address { get; set; }
        public List<AdditionalInterest> AdditionalInterests { get; set; }
        public List<HOM.HomCoverage> Coverages { get; set; }
        public Int32 FormTypeId { get; set; }
        public string FormType { get; set; }

        public Int32 ProgramTypeId { get; set; }
        public string ProgramType { get; set; }
        public Int32 ProtectionClassId { get; set; }
        public string ProtectionClass { get; set; }
        public int LocationNum { get; set; }

        public Location() { }

        internal Location(DCO.Policy.Location dLocation)
        {
            // if null let it fail
            this.Address = new Address(dLocation.Address);
            if (dLocation.AdditionalInterests != null && dLocation.AdditionalInterests.Any())
            {
                this.AdditionalInterests = new List<AdditionalInterest>();
                foreach (var a in dLocation.AdditionalInterests)
                {
                    this.AdditionalInterests.Add(new AdditionalInterest(a));
                }
            }
            if (dLocation.Coverages != null && dLocation.Coverages.Any())
            {
                this.Coverages = new List<HOM.HomCoverage>();
                foreach (var cov in dLocation.Coverages)
                {
                    this.Coverages.Add(new HOM.HomCoverage(cov));
                }
            }
            this.LocationNum = dLocation.LocationNum;
            this.FormTypeId = dLocation.FormTypeId;
            this.FormType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, this.FormTypeId.ToString());
            this.ProgramTypeId = dLocation.ProgramTypeId;
            this.ProgramType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ProgramTypeId, this.ProgramTypeId.ToString());
            this.ProtectionClassId = dLocation.ProtectionClassId;
            this.ProtectionClass = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ProtectionClassId, this.ProtectionClassId.ToString());
        }

        public override string ToString()
        {
            if (Address != null)
            {
                return Address.ToString();
            }
            return "Adress is null.";
        }
    }
    [System.Serializable]
    public class FNOLLocation
    {
        public string ApartmentNumber { get; set; }
        public string City { get; set; }
        public DCE.Country CountryID { get; set; }
        public DCE.StatusCode Status { get; set; }
        public string HouseNumber { get; set; }
        public DCE.NameAddressSource NameAddressSource { get; set; }
        public string POBox { get; set; }
        public CommonObjects.Enums.Enums.State State { get; set; }
        public string StreetName { get; set; }
        public string Zip { get; set; }
        public string Other { get; set; }

    }
}