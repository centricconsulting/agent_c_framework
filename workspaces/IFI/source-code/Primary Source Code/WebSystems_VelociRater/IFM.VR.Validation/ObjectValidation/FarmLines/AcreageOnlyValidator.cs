using System;

namespace IFM.VR.Validation.ObjectValidation.FarmLines
{
    public class AcreageOnlyValidator
    {
        public const string ValidationListID = "{473e7535-ed3d-4817-abd9-3a3e5bbd880f}";

        public const string QuoteIsNull = "{5ef96e92-8a92-441c-ad6a-22a325a64506}";
        public const string LocationIsNull = "{d51cbbd0-fcdd-446f-aaf1-4aa8e16a1a7f}";
        public const string LocationAcreageListIsNull = "{e14b0456-370b-49b1-a468-9f4eb9e5fe3d}";

        public const string Acres = "{d9b568fe-2c27-4b48-9438-3c9f052fbbb2}";
        public const string Section = "{ed96601b-74e7-4a39-a01b-1c789dba5c50}";
        public const string TownShipNum = "{14cdd7ae-1ccd-4e78-a2c4-cc79a677d435}";
        public const string Range = "{9b1dc16d-7723-4f80-bea0-7f66d75aca25}";
        public const string County = "{a9f3af99-5b53-4a0b-a198-7849a140049a}";
        public const string Description = "{ac3f0705-6fe9-434f-bacb-66e3cab9c9db}";
        public const string TownshipName = "{ea2f9690-8b1b-4ce9-a335-e407ff16b944}";

        public static Validation.ObjectValidation.ValidationItemList ValidateAcreages(QuickQuote.CommonObjects.QuickQuoteObject quote, Int32 locationIndex, int acreageIndex, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                if (quote.Locations != null)
                {
                    if (quote.Locations.Count > locationIndex)
                    {
                        if (quote.Locations[locationIndex].Acreages != null)
                        {
                            if (quote.Locations[locationIndex].Acreages.Count > acreageIndex)
                            {
                                // do tests
                                var a = quote.Locations[locationIndex].Acreages[acreageIndex];
                                if (a != null)
                                {
                                    switch (valType)
                                    {
                                        case ValidationItem.ValidationType.appRate:
                                            if (VRGeneralValidations.Val_HasRequiredField(a.Section, valList, Section, "Section", true))
                                            {
                                                if (a.Section.Trim() == "0")
                                                    valList.Add(new ObjectValidation.ValidationItem("Invalid Section", Section));
                                            }

                                            if (VRGeneralValidations.Val_HasRequiredField(a.Twp, valList, TownShipNum, "Township", true))
                                            {
                                                if (a.Range.Trim() == "0")
                                                    valList.Add(new ObjectValidation.ValidationItem("Invalid Township", TownShipNum));
                                            }

                                            if (VRGeneralValidations.Val_HasRequiredField(a.Range, valList, Range, "Range", true))
                                            {
                                                if (a.Range.Trim() == "0")
                                                    valList.Add(new ObjectValidation.ValidationItem("Invalid Range", Range));
                                            }

                                            goto case ValidationItem.ValidationType.quoteRate;
                                        case ValidationItem.ValidationType.quoteRate:

                                            if (VRGeneralValidations.Val_HasRequiredField(a.Acreage, valList, Acres, "Acres"))
                                                if (VRGeneralValidations.Val_IsGreaterThanZeroNumber(a.Acreage, valList, Acres, "Acres", true) == false)
                                                    valList.Add(new ObjectValidation.ValidationItem("Must be at least 1 acre", Acres));

                                            VRGeneralValidations.Val_HasRequiredField(a.Section, valList, Section, "Section");

                                            VRGeneralValidations.Val_HasRequiredField(a.Twp, valList, TownShipNum, "Township");

                                            VRGeneralValidations.Val_HasRequiredField(a.Range, valList, Range, "Range");

                                            VRGeneralValidations.Val_HasRequiredField(a.Description, valList, Description, "Description");
                                            VRGeneralValidations.Val_HasRequiredField(a.County, valList, County, "County");

                                            VRGeneralValidations.Val_HasRequiredField(a.TownshipCodeTypeId, valList, TownshipName, "Township Name");

                                            break;
                                    }
                                }
                            }
                            else
                            {
                                valList.Add(new ObjectValidation.ValidationItem("Invalid acreage index for this location.", LocationAcreageListIsNull));
                            }
                        }
                        else
                        {
                            valList.Add(new ObjectValidation.ValidationItem("Locations acreage list is null.", LocationAcreageListIsNull));
                        }
                    }
                    else
                    {
                        valList.Add(new ObjectValidation.ValidationItem("Invalid location Index.", LocationIsNull));
                    }
                }
                else
                {
                    valList.Add(new ObjectValidation.ValidationItem("Location is null", LocationIsNull));
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