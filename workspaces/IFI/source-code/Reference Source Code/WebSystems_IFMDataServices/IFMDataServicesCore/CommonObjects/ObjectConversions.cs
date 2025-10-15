using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFM.PrimitiveExtensions;
using QuickQuote.CommonObjects;
using IFM.DataServicesCore.CommonObjects.OMP;
using IFM.DataServicesCore.BusinessLogic;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.CommonObjects
{
    public static class ObjectConversions
    {
        public static QuickQuoteAddress DiamondAddressToQQAddress(DCO.Address diaAddress)
        {
            var qqAddress = new QuickQuoteAddress();
            qqAddress.AddressId = diaAddress.AddressId;
            qqAddress.AddressNum = diaAddress.AddressNum;
            qqAddress.ApartmentNumber = diaAddress.ApartmentNumber;
            qqAddress.AttemptedVerify = diaAddress.AttemptedVerify;
            qqAddress.City = diaAddress.City;
            qqAddress.County = diaAddress.County;
            qqAddress.DetailStatusCode = diaAddress.DetailStatusCode.ToString();
            qqAddress.DisplayAddress = diaAddress.DisplayAddress;
            qqAddress.POBox = diaAddress.POBox;
            qqAddress.PolicyId = diaAddress.PolicyId.ToString();
            qqAddress.PolicyImageNum = diaAddress.PolicyImageNum.ToString();
            qqAddress.StateId = diaAddress.StateId.ToString();
            qqAddress.State = diaAddress.StateName;
            qqAddress.StreetName = diaAddress.StreetName;
            qqAddress.Township = diaAddress.Township;
            qqAddress.territorycode = diaAddress.TerritoryCode.ToString();
            qqAddress.Zip = diaAddress.Zip;
            qqAddress.Verified = diaAddress.Verified;
            return qqAddress;
        }

        public static QuickQuoteName DiamondNameToQQName(DCO.Name diaName)
        {
            var qqName = new QuickQuoteName();
            qqName.BirthDate = diaName.BirthDate;
            //qqName.CommercialDBAname = diaName.CommercialName1;
            qqName.CommercialName1 = diaName.CommercialName1;
            //qqName.CommercialIRSname = diaName.CommercialName1;
            qqName.CommercialName2 = diaName.CommercialName2;
            qqName.DateBusinessStarted = diaName.DateBusinessStarted;
            qqName.DescriptionOfOperations = diaName.DescriptionOfOperations;
            qqName.DetailStatusCode = diaName.DetailStatusCode.ToString();
            qqName.DisplayName = diaName.DisplayName;
            //qqName.DisplayNameForWeb =
            qqName.DriversLicenseDate = diaName.DLDate;
            qqName.DriversLicenseNumber = diaName.DLN;
            qqName.DriversLicenseStateId = diaName.DLStateId.ToString();
            qqName.EntityTypeId = diaName.EntityTypeId.ToString();
            qqName.FirstName = diaName.FirstName;
            qqName.LastName = diaName.LastName;
            qqName.MaritalStatusId = diaName.MaritalStatusId.ToString();
            qqName.MiddleName = diaName.MiddleName;
            qqName.NameAddressSourceId = diaName.NameAddressSourceId.ToString();
            qqName.NameId = diaName.NameId;
            qqName.NameNum = diaName.NameNum;
            qqName.PolicyId = diaName.PolicyId.ToString();
            qqName.PolicyImageNum = diaName.PolicyImageNum.ToString();
            qqName.PositionTitle = diaName.PositionTitle;
            qqName.PrefixName = diaName.PrefixName;
            qqName.Salutation = diaName.Salutation;
            qqName.SexId = diaName.SexId.ToString();
            qqName.SortName = diaName.SortName;
            qqName.SuffixName = diaName.SuffixName;
            qqName.TaxNumber = diaName.TaxNumber;
            qqName.TaxTypeId = diaName.TaxTypeId.ToString();
            qqName.ThirdPartyEntityId = diaName.ThirdPartyEntityId.ToString();
            qqName.ThirdPartyGroupId = diaName.ThirdPartyGroupId.ToString();
            qqName.TypeId = diaName.TypeId.ToString();
            qqName.YearsOfExperience = diaName.YearsOfExperience.ToString();
            return qqName;
        }

        public static List<QuickQuotePhone> DiamondPhonesToQQPhones(DCO.InsCollection<DCO.Phone> diaPhones)
        {
            var qqList = new List<QuickQuotePhone>();
            if (diaPhones?.Count > 0)
            {
                foreach (var phone in diaPhones)
                {
                    qqList.Add(DiamondPhoneToQQPhone(phone));
                }
            }
            return diaPhones?.Count > 0 ? qqList : null;
        }

        public static QuickQuotePhone DiamondPhoneToQQPhone(DCO.Phone diaPhone)
        {
            var qqPhone = new QuickQuotePhone();
            qqPhone.DetailStatusCode = diaPhone.DetailStatusCode.ToString();
            qqPhone.Extension = diaPhone.Extension.ToString();
            qqPhone.NameAddressSourceId = diaPhone.NameAddressSourceId.ToString();
            qqPhone.Number = diaPhone.Number;
            qqPhone.PhoneId = diaPhone.PhoneId;
            qqPhone.PolicyId = diaPhone.PolicyId.ToString();
            qqPhone.PolicyImageNum = diaPhone.PolicyImageNum.ToString();
            qqPhone.TypeId = diaPhone.TypeId.ToString();
            qqPhone.Type = diaPhone.TypeDescription;
            return qqPhone;
        }

        public static List<QuickQuoteEmail> DiamondEmailsToQQEmails(DCO.InsCollection<DCO.Email> diaEmails)
        {
            var qqList = new List<QuickQuoteEmail>();
            if(diaEmails?.Count > 0)
            {
                foreach(var email in diaEmails)
                {
                    qqList.Add(DiamondEmailToQQEmail(email));
                }
            }
            return qqList?.Count > 0 ? qqList : null;
        }

        public static QuickQuoteEmail DiamondEmailToQQEmail(DCO.Email diaEmail)
        {
            var qqEmail = new QuickQuoteEmail();
            qqEmail.Address = diaEmail.Address;
            qqEmail.DetailStatusCode = diaEmail.DetailStatusCode.ToString();
            qqEmail.EmailId = diaEmail.EmailId;
            qqEmail.NameAddressSourceId = diaEmail.NameAddressSourceId.ToString();
            qqEmail.PolicyId = diaEmail.PolicyId.ToString();
            qqEmail.PolicyImageNum = diaEmail.PolicyImageNum.ToString();
            qqEmail.Type = diaEmail.TypeDescription;
            qqEmail.TypeId = diaEmail.TypeId.ToString();
            return qqEmail;
        }

        public static List<QuickQuoteAdditionalInterest> DiamondAdditionalInterestListsToQQAdditionalInterests(List<DCO.Policy.AdditionalInterestList> dAIs)
        {
            var mpList = new List<QuickQuoteAdditionalInterest>();
            if (dAIs?.Count > 0)
            {
                foreach (var dAI in dAIs)
                {
                    mpList.Add(DiamondAdditionalInterestListToQQAdditionalInterest(dAI));
                }
            }
            return mpList?.Count > 0 ? mpList : null;
        }

        public static QuickQuoteAdditionalInterest DiamondAdditionalInterestListToQQAdditionalInterest(DCO.Policy.AdditionalInterestList dAI)
        {
            var newAI = new QuickQuoteAdditionalInterest();
            newAI.Address = DiamondAddressToQQAddress(dAI.Address);
            newAI.AgencyId = dAI.AgencyId.ToString();
            newAI.Emails = DiamondEmailsToQQEmails(dAI.Emails);
            newAI.GroupTypeId = dAI.AdditionalInterestGroupTypeId.ToString();
            newAI.SingleEntry = dAI.SingleEntry;
            newAI.StatusCode = dAI.StatusCode.ToString();
            newAI.ListId = dAI.AdditionalInterestListId;
            newAI.Name = DiamondNameToQQName(dAI.Name);
            newAI.Phones = DiamondPhonesToQQPhones(dAI.Phones);
            return newAI;
        }

        public static List<AdditionalInterest> DiamondAdditionalInterestListsToMPAdditionalInterests(List<DCO.Policy.AdditionalInterestList> dAIs)
        {
            var mpList = new List<AdditionalInterest>();
            if(dAIs?.Count > 0)
            {
                foreach(var dAI in dAIs)
                {
                    mpList.Add(DiamondAdditionalInterestListToMPAdditionalInterest(dAI));
                }
            }
            return mpList?.Count > 0 ? mpList : null;
        }

        public static AdditionalInterest DiamondAdditionalInterestListToMPAdditionalInterest(DCO.Policy.AdditionalInterestList dAI)
        {
            var newAI = new DataServicesCore.CommonObjects.OMP.AdditionalInterest(dAI);
            return newAI;
        }
    }
}
