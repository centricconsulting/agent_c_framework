using QuickQuote.CommonMethods;
using QuickQuote.CommonObjects;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class SectionIAddressValidator
    {
        public const string ValidationListID = "{2D4DD605-157A-4FED-8819-ECCDED2006A3}";

        public const string QuoteIsNull = "{CE9E8691-F2F6-4018-8F3E-9054E7F729B8}";
        public const string NoLocations = "{0E32B8C7-08D3-493A-A01F-41EDABA87772}";

        public const string AddressStreetNumber = "{BE6BF54A-356E-444C-B834-7FD54E1DF4DF}";
        public const string AddressStreetName = "{9A77872C-567C-4AA2-85D9-B36659E139FA}";
        public const string AddressZipCode = "{AC0499E8-E247-4B15-B5C3-E2E56EF83E9A}";
        public const string AddressCity = "{B33B5C75-4048-45CF-8A4E-061BE80E862F}";
        public const string AddressState = "{5F17F2F6-D7EC-4862-867D-1C6F117BC153}";
        public const string AddressSatetNotIndiana = "{9468AFA3-87D7-46C9-9D11-8FE8BE6BD587}";
        public const string AddressCountyID = "{C6B4EE25-620D-4FB6-885E-E83F53EE7BCC}";

        public static Validation.ObjectValidation.ValidationItemList ValidateHOMSectionIAddress(QuickQuote.CommonObjects.QuickQuoteObject quote, int RowIndex, ValidationItem.ValidationType valType)
        {
            //Do not believe this is used
            QuickQuoteHelperClass QQHelper = new QuickQuoteHelperClass();

            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.LocationIndex, "0");

            if (quote != null)
            {
                if (quote.Locations != null && quote.Locations.Any() && quote.Locations[0] != null)
                {
                    var MyLocation = quote.Locations[0].SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises);
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