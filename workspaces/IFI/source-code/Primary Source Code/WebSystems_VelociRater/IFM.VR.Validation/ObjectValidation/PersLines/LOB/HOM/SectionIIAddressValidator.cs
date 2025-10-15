using QuickQuote.CommonMethods;
using QuickQuote.CommonObjects;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class SectionIIAddressValidator
    {
        public const string ValidationListID = "{4D19F249-7791-430E-8911-D8C33B58B581}";

        public const string QuoteIsNull = "{E4AB717B-6B8E-48CA-A9DA-957365397825}";
        public const string NoLocations = "{195C050B-3B69-4ED0-AA64-88864F172B77}";

        public const string AddressStreetNumber = "{D154A7D3-073D-4C23-9B52-643E4AE30458}";
        public const string AddressStreetName = "{20519BAD-AC61-4286-92C7-733FF2915A54}";
        public const string AddressZipCode = "{05EB6695-36B6-402B-8944-078353B2C67E}";
        public const string AddressCity = "{B84C4331-0142-406A-831D-D9387DADD404}";
        public const string AddressState = "{FCD3F5C9-07A5-4BC7-B613-76371733B6F4}";
        public const string AddressSatetNotIndiana = "{CE6D333C-7F1D-41F8-9836-48919B1E3E0F}";
        public const string AddressCountyID = "{850AF3C6-6C7E-45DE-9712-4F51BA08FA5C}";

        public static Validation.ObjectValidation.ValidationItemList ValidateHOMSectionIIAddress(QuickQuote.CommonObjects.QuickQuoteObject quote, int RowIndex, ValidationItem.ValidationType valType)
        {
            //Do not believe this is used
            QuickQuoteHelperClass QQHelper = new QuickQuoteHelperClass();

            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.LocationIndex, "0");

            if (quote != null)
            {
                if (quote.Locations != null && quote.Locations.Any() && quote.Locations[0] != null)
                {
                    var MyLocation = quote.Locations[0].SectionIICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionIICoverage.SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres);
                    // check address
                    var addressValidations = CoveragesAddressValidator.AddressValidation(MyLocation[RowIndex].Address, valType);

                    foreach (var err in addressValidations)
                    {
                        //convert to address for this object
                        switch (err.FieldId)
                        {
                            case CoveragesAddressValidator.HouseNumberID:
                                err.FieldId = AddressStreetNumber;
                                valList.Add(err); // add validation item to current validation list
                                break;

                            case CoveragesAddressValidator.StreetNameID:
                                err.FieldId = AddressStreetName;
                                valList.Add(err); // add validation item to current validation list
                                break;

                            case CoveragesAddressValidator.ZipCodeID:
                                err.FieldId = AddressZipCode;
                                valList.Add(err); // add validation item to current validation list
                                break;

                            case CoveragesAddressValidator.CityID:
                                err.FieldId = AddressCity;
                                valList.Add(err); // add validation item to current validation list
                                break;

                            case CoveragesAddressValidator.StateID:
                                err.FieldId = AddressState;
                                valList.Add(err); // add validation item to current validation list
                                break;

                            case CoveragesAddressValidator.CountyID:
                                err.FieldId = AddressCountyID;
                                valList.Add(err); // add validation item to current validation list
                                break;

                            default:
                                break;
                        }
                    }

                    if (MyLocation[RowIndex].Address != null && MyLocation[RowIndex].Address.StateId != "16")
                        valList.Add(new ObjectValidation.ValidationItem("Property must be located in Indiana", AddressSatetNotIndiana));
                }
                else
                {
                    // no location
                    valList.Add(new ObjectValidation.ValidationItem("No locations", NoLocations));
                }
            }
            else
            {
                valList.Add(new ObjectValidation.ValidationItem("Quote is null", QuoteIsNull));
            }
            return valList;
        }
    }
}