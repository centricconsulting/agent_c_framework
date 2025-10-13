using System;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA
{
    public class ViolationValidator
    {
        //This is only used for PPA, so no multi state changes are needed 9/17/18 MLW
        public const string ValidationListID = "{CDC56F16-BAB8-44FF-AF9D-01FBC9FE5474}";

        public const string ViolationType = "{B01FDBC0-D0B8-4156-A701-7F4B84BCF115}";
        public const string ViolationDate = "{05B417DC-7C01-4EE0-AAD3-E03C7250A869}";

        public static Validation.ObjectValidation.ValidationItemList ValidateViolation(int driverIndex, int violationIndex, QuickQuote.CommonObjects.QuickQuoteObject quote)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.DriverIndex, driverIndex.ToString());
            valList.AddBreadCrum(ValidationBreadCrum.BCType.ViolationIndex, violationIndex.ToString());

            if (quote != null && quote.Drivers != null && quote.Drivers.Count > driverIndex)
            {
                if (quote.Drivers[driverIndex].AccidentViolations != null && quote.Drivers[driverIndex].AccidentViolations.Count > violationIndex)
                {
                    QuickQuote.CommonObjects.QuickQuoteAccidentViolation violation = quote.Drivers[driverIndex].AccidentViolations[violationIndex];
                    if (violation != null)
                    {
                        if (VRGeneralValidations.Val_HasRequiredField_DD(violation.AccidentsViolationsTypeId, valList, ViolationType, "Violation Type"))
                            VRGeneralValidations.Val_IsNonNegativeWholeNumber(violation.AccidentsViolationsTypeId, valList, ViolationType, "Violation Type");

                        if (VRGeneralValidations.Val_HasRequiredField_Date(violation.AvDate, valList, ViolationDate, "Violation Date"))
                            VRGeneralValidations.Val_IsDateInRange(violation.AvDate, valList, ViolationDate, "Violation Date", DateTime.Now.AddYears(-100).ToString(), DateTime.Now.ToString());
                    }
                }
            }
            return valList;
        }
    }
}