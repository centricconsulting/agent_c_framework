using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFM.PrimitiveExtensions;

namespace IFM.DataServicesCore.BusinessLogic.OMP
{
    public class AddressHelper : BusinessLogicBase
    {
        public static bool IsAddressOutOfState(IFM.DataServicesCore.CommonObjects.OMP.Address myAddress, string OriginStateAbbrev, out string ErrorMessage)
        {
            bool isOutOfState = false;
            ErrorMessage = "";
            if(myAddress != null && myAddress.Zip5.HasValue())
            {
                var myZipResults = global::IFM.VR.Common.Helpers.GetCityCountyFromZipCode.GetCityCountyFromZipCode(myAddress.Zip5);
                if (myZipResults?.Count > 0)
                {
                    var filteredResults = GetFilteredZipResults(myAddress, myZipResults);
                    if (filteredResults?.Count > 0)
                    {
                        isOutOfState = filteredResults[0].StateAbbrev.StringsAreNotEqual(OriginStateAbbrev);
                    }
                    else
                    {
                        ErrorMessage = $"Out of state garaging check - Unable to determine state from zipcode. {myAddress.Zip} returned {myZipResults.Count} results. After filtering by city name of {myAddress.City}, no results were left.";
                    }
                }
                else
                {
                    ErrorMessage = $"Out of state garaging check - Unable to determine state from zipcode. {myAddress.Zip} returned no results.";
                }
            }

            return isOutOfState;
        }

        public static List<global::IFM.VR.Common.Helpers.ZipLookupResult> GetFilteredZipResults(IFM.DataServicesCore.CommonObjects.OMP.Address myAddress, List<global::IFM.VR.Common.Helpers.ZipLookupResult> zipResults)
        {
            CommonHelperClass chc = new CommonHelperClass();
            bool settingExists = false;
            bool hasParseError = false;
            bool validateCounty = chc.GetApplicationXMLSettingForBoolean("Endorsements_GaragingOutOfState_ValidateCounty", ref settingExists, ref hasParseError, "Endorsements.xml");
            if(settingExists == false)
            {
                validateCounty = false;
            }
            if (zipResults?.Count > 0)
            {
                if (myAddress.City.NoneAreNullEmptyOrWhitespace(myAddress.County, myAddress.StateAbbrev))
                {
                    if (validateCounty)
                    {
                        return zipResults.FindAll(x => x.City.StringsAreEqual(myAddress.City) && x.County.StringsAreEqual(myAddress.County) && x.StateAbbrev.StringsAreEqual(myAddress.StateAbbrev));
                    }
                    else
                    {
                        return zipResults.FindAll(x => x.City.StringsAreEqual(myAddress.City) && x.StateAbbrev.StringsAreEqual(myAddress.StateAbbrev));
                    }
                }
                else if (myAddress.City.NoneAreNullEmptyOrWhitespace(myAddress.County))
                {
                    if (validateCounty)
                    {
                        return zipResults.FindAll(x => x.City.StringsAreEqual(myAddress.City));
                    }
                    else
                    {
                        return zipResults.FindAll(x => x.City.StringsAreEqual(myAddress.City) && x.County.StringsAreEqual(myAddress.County));
                    }

                }
                else if (myAddress.City.NoneAreNullEmptyOrWhitespace(myAddress.StateAbbrev))
                {
                    return zipResults.FindAll(x => x.City.StringsAreEqual(myAddress.City) && x.StateAbbrev.StringsAreEqual(myAddress.StateAbbrev));
                }
                else if (myAddress.County.NoneAreNullEmptyOrWhitespace(myAddress.StateAbbrev))
                {
                    if (validateCounty)
                    {
                        return zipResults.FindAll(x => x.County.StringsAreEqual(myAddress.County) && x.StateAbbrev.StringsAreEqual(myAddress.StateAbbrev));
                    }
                    else
                    {
                        return zipResults.FindAll(x => x.StateAbbrev.StringsAreEqual(myAddress.StateAbbrev));
                    }

                }
                else if (myAddress.City.NoneAreNullEmptyOrWhitespace())
                {
                    return zipResults.FindAll(x => x.City.StringsAreEqual(myAddress.City));
                }
                else if (myAddress.County.NoneAreNullEmptyOrWhitespace())
                {
                    if (validateCounty)
                    {
                        return zipResults.FindAll(x => x.County.StringsAreEqual(myAddress.County));
                    }
                }
                else if (myAddress.StateAbbrev.NoneAreNullEmptyOrWhitespace())
                {
                    return zipResults.FindAll(x => x.StateAbbrev.StringsAreEqual(myAddress.StateAbbrev));
                }
            }
            return zipResults;
        }
    }
}
