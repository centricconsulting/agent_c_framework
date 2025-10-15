using Microsoft.VisualBasic;
using System;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class PropertyUpdatesValidator
    {
        public static int PropertyAgeThatRequiresUpdates = 30;

        public const string ValidationListID = "{17A6D054-A555-49D1-994D-856A8DD340C3}";

        public const string QuoteNull = "{4289DACC-62E8-4CB7-8AA8-E1B951391157}";
        public const string UpdatesNoLocations = "{99285AC9-77D0-49E6-A855-33BD66136C49}";

        public const string UpdatesRoofYear = "{334D844E-A93C-4931-810C-78B27BBC12A2}";
        public const string UpadtesRoofUpdateType = "{15ACA459-D625-48F4-862D-53081ED4BB45}";
        public const string UpdatesRoofMaterialType = "{0811223D-B449-4915-A422-B1964E4F5B55}";

        public const string UpdatesHeatYear = "{49C0AB87-AF62-4B17-8CD0-FA70A6C00B15}";
        public const string UpdatesHeatUpdateType = "{FDDDF482-9213-4975-A01A-A4351DAF199A}";
        public const string UpdatesHeatMaterialType = "{10812A82-DC6A-4933-844B-9610A3E25A6A}";

        public const string UpdatesElectricYear = "{A48F89D2-17E6-4DC0-9746-A1AB70BCEB41}";
        public const string UpdatesElectricUpdateType = "{C8D28073-DE74-4C24-876F-F6D71EB45BA3}";
        public const string UpdatesElectricMaterialType = "{5163F0D3-BC30-490E-B688-557967671DDD}";

        public const string UpdatesPlumbingYear = "{E8E4CEA4-C73C-4719-83DE-AD98CFC05ED6}";
        public const string UpdatesPlumbingUpdateType = "{77256E3D-F441-4254-B157-A09D508ADBC8}";
        public const string UpdatesPlumbingMaterialType = "{8CCD9A0C-50D1-4FE1-B15E-5DF9696451E0}";

        public const string UpdatesInspectionDate = "{394177DB-BA0E-4A1B-942F-62D78FC6ADAA}";
        public const string UpdatesInspectionUpdateType = "{DC7E7057-A177-424C-9A3A-B970EEFAA4B7}";
        public const string UpdatesInspectionRemarks = "{B25B4265-CD8D-4761-96C2-C32955DBEEA1}";

        public const string MobileHomeWidth = "{BF0578D6-06C8-4621-99A3-898EA925F9C8}";
        public const string MobileHomeLength = "{87DEF07D-4061-47A6-8FCB-50CFA9AFF1A0}";

        public const string Occupant = "{31176bda-7488-4012-8bcc-6f6dc01d2445}";

        //public const string UpdatesInspectionMaterialType = "";

        private enum UpdateCategory : int
        {
            roof = 1,
            heat = 2,
            electric = 3,
            plumbing = 4,
            inspection = 5
        }

        public static Validation.ObjectValidation.ValidationItemList ValidateHOMPropertyUpdates(QuickQuote.CommonObjects.QuickQuoteObject quote, int locationIndex, ValidationItem.ValidationType valType)
        {
            //HOM, DFR uses this
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.LocationIndex, "0");

            Func<int> PropertyAge = delegate ()
            {
                int propertyAge = Int32.MinValue;
                if (quote != null && quote.Locations != null && quote.Locations.Any())
                {
                    if (Information.IsNumeric(quote.Locations[locationIndex].YearBuilt))
                    {
                        int yearBuilt = Convert.ToInt32(quote.Locations[locationIndex].YearBuilt);
                        propertyAge = (DateTime.Now.Year - yearBuilt < 0) ? 0 : DateTime.Now.Year - yearBuilt;
                    }
                }
                return propertyAge;
            };

            Func<bool> IsContentOnlyPolicy = delegate ()
            {
                if (quote != null && quote.Locations != null && quote.Locations.Any()) {
                    //Updated 12/5/17 for HOM Upgrade MLW
                    if (quote.Locations[locationIndex].FormTypeId == "4" || (quote.Locations[locationIndex].FormTypeId == "25" && quote.Locations[locationIndex].StructureTypeId != "2"))
                    {
                        return true;
                    } else
                    {
                        return false;
                    }
                } else {
                    return false;
                }
                //return quote.Locations[locationIndex].FormTypeId == "4"; //4 = HO-4 Contents policy
            };

            Action<bool[], UpdateCategory> TestMaterialList = (materialList, type) =>
            {
                string ValidationFieldId = "";
                switch (type)
                {
                    case UpdateCategory.roof:
                        ValidationFieldId = UpdatesRoofMaterialType;
                        break;

                    case UpdateCategory.heat:
                        ValidationFieldId = UpdatesHeatMaterialType;
                        break;

                    case UpdateCategory.electric:
                        ValidationFieldId = UpdatesElectricMaterialType;
                        break;

                    case UpdateCategory.plumbing:
                        ValidationFieldId = UpdatesPlumbingMaterialType;
                        break;

                    default:
                        break;
                }

                if (materialList != null)
                {
                    if (materialList.Contains(true))
                    {
                        // atleast one but need to make sure it is only one
                        var trueCount = (from v in materialList where v == true select v).Count();
                        if (trueCount > 1)
                        {
                            // too many types selected
                            valList.Add(new ObjectValidation.ValidationItem("Multiple Types Selected", ValidationFieldId));
                        }
                    }
                    else
                    {
                        // none selected
                        valList.Add(new ObjectValidation.ValidationItem("Missing Type Information", ValidationFieldId));
                    }
                }
            };

            Func<UpdateCategory, bool[]> GetMaterialList = delegate (UpdateCategory type)
            {
                if (quote != null && quote.Locations != null && quote.Locations.Any() && quote.Locations[locationIndex] != null && quote.Locations[locationIndex].Updates != null)
                {
                    var MyUpdates = quote.Locations[locationIndex].Updates;
                    switch (type)
                    {
                        case UpdateCategory.roof:
                            bool[] materialListRoof = new bool[5] { MyUpdates.RoofAsphaltShingle, MyUpdates.RoofMetal, MyUpdates.RoofOther, MyUpdates.RoofSlate, MyUpdates.RoofWood };
                            return materialListRoof;
                            break;

                        case UpdateCategory.heat:
                            bool[] materialListHeat = new bool[4] { MyUpdates.CentralHeatElectric, MyUpdates.CentralHeatGas, MyUpdates.CentralHeatOil, MyUpdates.CentralHeatOther };
                            return materialListHeat;
                            break;

                        case UpdateCategory.electric:
                            bool[] materialListElectric = new bool[5] { MyUpdates.ElectricCircuitBreaker, MyUpdates.ElectricFuses, MyUpdates.Electric60Amp, MyUpdates.Electric100Amp, MyUpdates.Electric200Amp };
                            return materialListElectric;
                            break;

                        case UpdateCategory.plumbing:
                            bool[] materialListPlumbing = new bool[3] { MyUpdates.PlumbingPlastic, MyUpdates.PlumbingGalvanized, MyUpdates.PlumbingCopper };
                            return materialListPlumbing;
                            break;

                        case UpdateCategory.inspection:
                            break;

                        default:
                            break;
                    }
                }
                return null;
            };

            Action<UpdateCategory> CheckUpdateMaterial = delegate (UpdateCategory type)
            {
                if (quote != null && quote.Locations != null && quote.Locations.Any() && quote.Locations[locationIndex] != null && quote.Locations[locationIndex].Updates != null)
                {
                    TestMaterialList(GetMaterialList(type), type);
                }
            };

            if (quote != null)
            {
                if (quote.Locations != null && quote.Locations.Any())
                {
                    if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal)
                    {
                        PropertyAgeThatRequiresUpdates = 25;
                    }
                    var MyLocation = quote.Locations[locationIndex];
                    if (!(IsContentOnlyPolicy()))
                    {
                        bool HasorNeedsSomeUpdateInformation = false;
                        // is required due to age or is required do to simply having information that needs to be validated
                        if (PropertyAge() >= PropertyAgeThatRequiresUpdates ||
                            (string.IsNullOrWhiteSpace(MyLocation.Updates.RoofUpdateYear) == false ||
                            string.IsNullOrWhiteSpace(MyLocation.Updates.RoofUpdateTypeId) == false ||
                            (from v in GetMaterialList(UpdateCategory.roof) where v == true select v).Any()))
                        {
                            HasorNeedsSomeUpdateInformation = true;
                            // on farm these are optional
                            if (VRGeneralValidations.Val_HasRequiredField(MyLocation.Updates.RoofUpdateYear, valList, UpdatesRoofYear, "Roof Year Updated", quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm))
                                VRGeneralValidations.Val_IsNumberInRange(MyLocation.Updates.RoofUpdateYear, valList, UpdatesRoofYear, "Roof Year Updated", MyLocation.YearBuilt, DateTime.Now.Year.ToString());
                            if (VRGeneralValidations.Val_HasRequiredField_DD(MyLocation.Updates.RoofUpdateTypeId, valList, UpadtesRoofUpdateType, "Update Information", quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm))
                                VRGeneralValidations.Val_IsNonNegativeWholeNumber(MyLocation.Updates.RoofUpdateTypeId, valList, UpadtesRoofUpdateType, "Update Information");
                            // on farm this is required always
                            CheckUpdateMaterial(UpdateCategory.roof);
                        }
                        else
                        {
                            // on farm this is required always
                            if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                                CheckUpdateMaterial(UpdateCategory.roof);
                        }

                        if (PropertyAge() >= PropertyAgeThatRequiresUpdates ||
                           (string.IsNullOrWhiteSpace(MyLocation.Updates.CentralHeatUpdateYear) == false ||
                           string.IsNullOrWhiteSpace(MyLocation.Updates.CentralHeatUpdateTypeId) == false ||
                           (from v in GetMaterialList(UpdateCategory.heat) where v == true select v).Any()))
                        {
                            HasorNeedsSomeUpdateInformation = true;
                            // on farm these are optional
                            if (VRGeneralValidations.Val_HasRequiredField(MyLocation.Updates.CentralHeatUpdateYear, valList, UpdatesHeatYear, "Central Heat Year Updated", quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm))
                                VRGeneralValidations.Val_IsNumberInRange(MyLocation.Updates.CentralHeatUpdateYear, valList, UpdatesHeatYear, "Central Heat Year Updated", MyLocation.YearBuilt, DateTime.Now.Year.ToString());
                            if (VRGeneralValidations.Val_HasRequiredField_DD(MyLocation.Updates.CentralHeatUpdateTypeId, valList, UpdatesHeatUpdateType, "Update Information", quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm))
                                VRGeneralValidations.Val_IsNonNegativeWholeNumber(MyLocation.Updates.CentralHeatUpdateTypeId, valList, UpdatesHeatUpdateType, "Update Information");
                            // on farm this is required always
                            CheckUpdateMaterial(UpdateCategory.heat);
                        }
                        else
                        {
                            // on farm this is required always
                            if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                                CheckUpdateMaterial(UpdateCategory.heat);
                        }

                        if (PropertyAge() >= PropertyAgeThatRequiresUpdates ||
                           (string.IsNullOrWhiteSpace(MyLocation.Updates.ElectricUpdateYear) == false ||
                           string.IsNullOrWhiteSpace(MyLocation.Updates.ElectricUpdateTypeId) == false ||
                           (from v in GetMaterialList(UpdateCategory.electric) where v == true select v).Any()))
                        {
                            HasorNeedsSomeUpdateInformation = true;
                            // on farm these are optional
                            if (VRGeneralValidations.Val_HasRequiredField(MyLocation.Updates.ElectricUpdateYear, valList, UpdatesElectricYear, "Electric Year Updated", quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm))
                                VRGeneralValidations.Val_IsNumberInRange(MyLocation.Updates.ElectricUpdateYear, valList, UpdatesElectricYear, "Electric Year Updated", MyLocation.YearBuilt, DateTime.Now.Year.ToString());
                            if (VRGeneralValidations.Val_HasRequiredField_DD(MyLocation.Updates.ElectricUpdateTypeId, valList, UpdatesElectricUpdateType, "Update Information", quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm))
                                VRGeneralValidations.Val_IsNonNegativeWholeNumber(MyLocation.Updates.ElectricUpdateTypeId, valList, UpdatesElectricUpdateType, "Update Information");
                            // on farm this is required always
                            CheckUpdateMaterial(UpdateCategory.electric);
                        }
                        else
                        {
                            // on farm this is required always
                            if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                                CheckUpdateMaterial(UpdateCategory.electric);
                        }

                        if (PropertyAge() >= PropertyAgeThatRequiresUpdates ||
                           (string.IsNullOrWhiteSpace(MyLocation.Updates.PlumbingUpdateYear) == false ||
                           string.IsNullOrWhiteSpace(MyLocation.Updates.PlumbingUpdateTypeId) == false ||
                           (from v in GetMaterialList(UpdateCategory.plumbing) where v == true select v).Any()))
                        {
                            HasorNeedsSomeUpdateInformation = true;
                            // on farm these are optional
                            if (VRGeneralValidations.Val_HasRequiredField(MyLocation.Updates.PlumbingUpdateYear, valList, UpdatesPlumbingYear, "Plumbing Year Updated", quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm))
                                VRGeneralValidations.Val_IsNumberInRange(MyLocation.Updates.PlumbingUpdateYear, valList, UpdatesPlumbingYear, "Plumbing Year Updated", MyLocation.YearBuilt, DateTime.Now.Year.ToString());
                            if (VRGeneralValidations.Val_HasRequiredField_DD(MyLocation.Updates.PlumbingUpdateTypeId, valList, UpdatesPlumbingUpdateType, "Update Information", quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm))
                                VRGeneralValidations.Val_IsNonNegativeWholeNumber(MyLocation.Updates.PlumbingUpdateTypeId, valList, UpdatesPlumbingUpdateType, "Update Information");
                            // on farm this is required always
                            CheckUpdateMaterial(UpdateCategory.plumbing);
                        }
                        else
                        {
                            // on farm this is required always
                            if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                                CheckUpdateMaterial(UpdateCategory.plumbing);
                        }

                        if (quote.LobType != QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                        {
                            if (HasorNeedsSomeUpdateInformation)
                            {
                                if (VRGeneralValidations.Val_HasRequiredField_Date(MyLocation.Updates.InspectionDate, valList, UpdatesInspectionDate, "Inspection Date"))
                                    VRGeneralValidations.Val_IsValidDate(MyLocation.Updates.InspectionDate, valList, UpdatesInspectionDate, "Inspection Date");
                                if (VRGeneralValidations.Val_HasRequiredField_DD(MyLocation.Updates.InspectionUpdateTypeId, valList, UpdatesInspectionUpdateType, "Update Information"))
                                    VRGeneralValidations.Val_IsNonNegativeWholeNumber(MyLocation.Updates.InspectionUpdateTypeId, valList, UpdatesInspectionUpdateType, "Update Information");
                                CheckUpdateMaterial(UpdateCategory.inspection);
                                //VRGeneralValidations.Val_HasRequiredField(MyLocation.Updates.InspectionRemarks, valList, UpdatesInspectionRemarks, "Inspection Remarks");
                            }
                        }
                    }

                    //11-26-14 Matt A
                    if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                    {
                        if (MyLocation.FormTypeId == "6" | MyLocation.FormTypeId == "7" | ((MyLocation.FormTypeId == "22" | MyLocation.FormTypeId == "25") & MyLocation.StructureTypeId == "2"))
                        {
                            if (VRGeneralValidations.Val_HasRequiredField(MyLocation.MobileHomeLength, valList, MobileHomeLength, "Mobile Home Length"))
                                if (VRGeneralValidations.Val_IsNonNegativeWholeNumber(MyLocation.MobileHomeLength, valList, MobileHomeLength, "Mobile Home Length"))
                                    VRGeneralValidations.Val_IsNumberInRange(MyLocation.MobileHomeLength, valList, MobileHomeLength, "Mobile Home Length", "1", "999");

                            if (VRGeneralValidations.Val_HasRequiredField(MyLocation.MobileHomeWidth, valList, MobileHomeWidth, "Mobile Home Width"))
                                if (VRGeneralValidations.Val_IsNonNegativeWholeNumber(MyLocation.MobileHomeWidth, valList, MobileHomeWidth, "Mobile Home Width"))
                                    VRGeneralValidations.Val_IsNumberInRange(MyLocation.MobileHomeWidth, valList, MobileHomeWidth, "Mobile Home Width", "1", "999");
                        }
                    }

                    if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                    {
                        VRGeneralValidations.Val_HasRequiredField(MyLocation.Remarks, valList, Occupant, "Who Occupies this Dwelling");
                    }
                }
                else
                {
                    valList.Add(new ObjectValidation.ValidationItem("No Locations", UpdatesNoLocations));
                }
            }
            else
            {
                valList.Add(new ObjectValidation.ValidationItem("Quote Null", QuoteNull));
            }

            return valList;
        }
    }
}