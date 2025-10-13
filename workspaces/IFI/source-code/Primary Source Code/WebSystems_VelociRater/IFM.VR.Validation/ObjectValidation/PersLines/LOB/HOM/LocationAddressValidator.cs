using QuickQuote.CommonMethods;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class LocationAddressValidator
    {
        public const string ValidationListID = "{F2208DF4-28C6-4821-B47B-B19E9B05E25A}";

        public const string QuoteIsNull = "{6DFACE67-E7DE-44DD-9353-42679CB783C6}";
        public const string NoLocations = "{61170094-FA1D-47F3-B536-B5F8C7E22BA5}";

        public const string AddressStreetNumber = "{583245BB-2947-497E-922A-39FD95B71CAD}";
        public const string AddressStreetName = "{87171292-1977-454D-A07B-38C00132C39C}";
        public const string AddressPoBox = "{E3CD19BB-1AC5-4866-9A11-FFE2C939F59E}";
        public const string AddressZipCode = "{1C7B0D15-1C55-4767-871B-4810EAA5E04D}";
        public const string AddressCity = "{1E261C4F-4384-4FDD-90F3-95670850F454}";
        public const string AddressState = "{B2AA7997-4F0A-439A-817D-A76B7C40DEBB}";
        public const string AddressSatetNotIndiana = "{343DB64C-B73B-41B7-902B-C2984222C07C}";
        public const string AddressStreetAndPoBoxEmpty = "{30254507-9450-4240-90F8-2801AAB41F76}";
        public const string PolicyHolderCountyID = "{AA61F913-5122-48E1-A57D-ABD60C984856}";
        public const string AddressStreetAndPoBoxAreSet = "{B4F6C068-C605-4FB2-B7E4-218757A058C5}";
        public const string AddressIsEmpty = "{6177B65C-07D2-47F7-AD00-43A82F25A10F}";

        public const string Acreage_AcreageAmount = "{966FECB5-9E5F-4C51-888A-6B337770C31A}";
        public const string AcreageType = "{DCCC16DB-B3E0-4563-A5A5-4B8C66ED4A15}";
        public const string AcreageSection = "{70AB0898-0396-4C63-BF01-6B4600BBD7EA}";
        public const string AcreageTwp = "{4290C83F-B86C-460F-BE9C-3A048A7F3767}";
        public const string AcreageRange = "{F4B2F911-CD40-477D-8E08-6C20D2A38F1E}";
        public const string AcreageTownshipName = "{9782E48F-8260-4DF3-9420-9B95A5A1C91D}";
        public const string AcreageDescription = "{41FEBADD-AD4A-4AB4-85FB-06BF1700127D}";
        public const string AcreageStateId = "{F176B229-5C1D-4D2D-BDA2-AC7C30C8DB8B}";
        public const string AcreageCounty = "{B1FED337-AB3F-48F9-BD79-3891693CF2EC}";

        public static Validation.ObjectValidation.ValidationItemList ValidateHOMLocationAddress(QuickQuote.CommonObjects.QuickQuoteObject quote, int LocationIndex, bool residenceExist, ValidationItem.ValidationType valType)
        {
            //HOM, DFR uses
            QuickQuoteHelperClass QQHelper = new QuickQuoteHelperClass();

            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.LocationIndex, LocationIndex.ToString());

            if (quote != null)
            {
                if (quote.Locations != null && quote.Locations.Any() && quote.Locations.Count > LocationIndex)
                {
                    var MyLocation = quote.Locations[LocationIndex];

                    if (LocationIndex == 0 || residenceExist)
                    {
                        //check address
                        var addressValidations = AllLines.AddressValidator.AddressValidation(MyLocation.Address, valType);

                        foreach (var err in addressValidations)
                        {
                            //convert to address for this object
                            switch (err.FieldId)
                            {
                                case AllLines.AddressValidator.HouseNumberID:
                                    if (!(quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm && MyLocation.ProgramTypeId == "8" && err.Message.ToLower().Contains("missing")))
                                    {
                                        err.FieldId = AddressStreetNumber;
                                        valList.Add(err); // add validation item to current validation list
                                    }
                                    break;

                                case AllLines.AddressValidator.StreetNameID:
                                    if (!(quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm && MyLocation.ProgramTypeId == "8" && err.Message.ToLower().Contains("missing")))
                                    {
                                        err.FieldId = AddressStreetName;
                                        valList.Add(err); // add validation item to current validation list
                                    }
                                    break;

                                case AllLines.AddressValidator.POBOXID:
                                    if (!(quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm && MyLocation.ProgramTypeId == "8" && err.Message.ToLower().Contains("missing")))
                                    {
                                        err.FieldId = AddressPoBox;
                                        valList.Add(err); // add validation item to current validation list
                                    }
                                    break;

                                case AllLines.AddressValidator.ZipCodeID:
                                    err.FieldId = AddressZipCode;
                                    valList.Add(err); // add validation item to current validation list
                                    break;

                                case AllLines.AddressValidator.CityID:
                                    err.FieldId = AddressCity;
                                    valList.Add(err); // add validation item to current validation list
                                    break;

                                case AllLines.AddressValidator.StateID:
                                    err.FieldId = AddressState;
                                    valList.Add(err); // add validation item to current validation list
                                    break;

                                case AllLines.AddressValidator.StreetAndPoBoxEmpty:
                                    if (!(quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm && MyLocation.ProgramTypeId == "8" && err.Message.ToLower().Contains("required")))
                                    {
                                        err.FieldId = AddressStreetAndPoBoxEmpty;
                                        valList.Add(err);
                                    }
                                    break;

                                case AllLines.AddressValidator.StreetAndPoxBoxAreSet:
                                    err.FieldId = AddressStreetAndPoBoxAreSet;
                                    valList.Add(err);
                                    break;

                                case AllLines.AddressValidator.CountyID:
                                    err.FieldId = PolicyHolderCountyID;
                                    valList.Add(err); // add validation item to current validation list
                                    break;

                                case AllLines.AddressValidator.AddressIsEmpty:
                                    err.FieldId = AddressIsEmpty;
                                    valList.Add(err);
                                    break;

                                default:
                                    break;
                            }
                        }

                        if (MyLocation.Address != null && MyLocation.Address.StateId != "16")
                            valList.Add(new ObjectValidation.ValidationItem("Property must be located in Indiana", AddressSatetNotIndiana));
                    }

                    if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                    {
                        if (MyLocation != null)
                        {
                            if (MyLocation.Acreages != null)
                            {
                                int acreageNum = 1;
                                foreach (var a in MyLocation.Acreages)
                                {
                                    // Remember to create tests for this whole section
                                    //a.Section
                                    string postfix = string.Format(" - Acreage #{0}", acreageNum);
                                    if (acreageNum == 1)
                                        postfix = "";
                                    if (VRGeneralValidations.Val_HasRequiredField(a.Acreage, valList, Acreage_AcreageAmount, "Acreage" + postfix, true))
                                    {
                                        VRGeneralValidations.Val_IsNonNegativeNumber(a.Acreage, valList, Acreage_AcreageAmount, "Acreage" + postfix);
                                    }
                                    VRGeneralValidations.Val_HasRequiredField_DD(a.LocationAcreageTypeId, valList, AcreageType, "Type Id(Acreage Only)" + postfix);
                                    VRGeneralValidations.Val_HasRequiredField(a.Section, valList, AcreageSection, "Section" + postfix);
                                    VRGeneralValidations.Val_HasRequiredField(a.Twp, valList, AcreageTwp, "Township" + postfix);
                                    VRGeneralValidations.Val_HasRequiredField(a.Range, valList, AcreageRange, "Range" + postfix);
                                    VRGeneralValidations.Val_HasRequiredField_DD(a.TownshipCodeTypeId, valList, AcreageTownshipName, "Township Name" + postfix);
                                    VRGeneralValidations.Val_HasRequiredField(a.Description, valList, AcreageDescription, "Location Description" + postfix);

                                    VRGeneralValidations.Val_HasRequiredField_DD(a.StateId, valList, AcreageStateId, "State" + postfix);
                                    VRGeneralValidations.Val_HasRequiredField(a.County, valList, AcreageCounty, "County" + postfix);
                                    acreageNum += 1;
                                    if (valType == ValidationItem.ValidationType.quoteRate)
                                        break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //no location
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