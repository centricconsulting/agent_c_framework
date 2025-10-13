using System;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common
{
    public class LossValidator
    {
        public const string ValidationListID = "{96561CA9-C97B-4C0C-A8F5-CE80082321B9}";

        public const string LossType = "{0472E5B1-3F74-47D0-BDC4-1933666280F0}";
        public const string LossDate = "{A2B2E2ED-EC74-4C09-AAE5-D379442589D8}";
        public const string LossFault = "{5A1849B8-D1AE-4155-B6CA-61C3F2A1D817}";
        public const string LossAmount = "{ACCB3053-9D84-4B7F-8770-4529E18C0222}";
        public const string LossObjectNull = "{4A658472-048E-4483-889C-B8D1E0A5EA24}";

        public static Validation.ObjectValidation.ValidationItemList ValidateLoss(int driverIndex, int lossIndex, QuickQuote.CommonObjects.QuickQuoteObject quote)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.DriverIndex, driverIndex.ToString());
            valList.AddBreadCrum(ValidationBreadCrum.BCType.LossIndex, lossIndex.ToString());

            if (quote != null)
            {
                //updated 8/15/2018 for multi-state; original logic is in IF; applicable references to quote changed to governingStateQuote
                QuickQuote.CommonObjects.QuickQuoteObject governingStateQuote = IFM.VR.Common.Helpers.MultiState.General.GoverningStateQuote_SubQuotesOmitted(ref quote); //should always return something
                if (governingStateQuote != null)
                {
                    QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord loss = null;
                    switch (quote.LobType)
                    {
                        case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal:
                            if (governingStateQuote.Drivers != null && governingStateQuote.Drivers.Count > driverIndex)
                            {
                                if (governingStateQuote.Drivers[driverIndex].LossHistoryRecords != null && governingStateQuote.Drivers[driverIndex].LossHistoryRecords.Count > lossIndex)
                                {
                                    loss = governingStateQuote.Drivers[driverIndex].LossHistoryRecords[lossIndex];
                                    if (loss != null)
                                    {
                                        if (VRGeneralValidations.Val_HasRequiredField(loss.TypeOfLossId, valList, LossType, "Loss Type"))
                                        {
                                            // this is a special case because n/a is valid here and usually it is not
                                            if (loss.TypeOfLossId.Trim() == "-1")
                                            {
                                                valList.Add(new ValidationItem("Missing Loss Type", LossType, false));
                                            }
                                        }

                                        if (VRGeneralValidations.Val_HasRequiredField(loss.LossDate, valList, LossDate, "Loss Date"))
                                            VRGeneralValidations.Val_IsDateInRange(loss.LossDate, valList, LossDate, "Loss Date", DateTime.Now.AddYears(-100).ToString(), DateTime.Now.ToString());

                                        if (VRGeneralValidations.Val_HasRequiredField(loss.Amount, valList, LossAmount, "Loss Amount"))
                                            VRGeneralValidations.Val_IsNonNegativeNumber(loss.Amount, valList, LossAmount, "Loss Amount");

                                        VRGeneralValidations.Val_HasRequiredField_DD(loss.LossHistoryFaultId, valList, LossFault, "Fault Indicator");
                                    }
                                    else
                                    {
                                        valList.Add(new ValidationItem("Loss is empty", LossObjectNull, false));
                                    }
                                }
                                else
                                {
                                    valList.Add(new ValidationItem("Driver is empty", LossObjectNull, false));
                                }
                            }
                            else
                            {
                                valList.Add(new ValidationItem("Driver is empty", LossObjectNull, false));
                            }
                            break;

                        case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal: // just fall through to do the same as HOM - Matt A 10-23-2015
                        case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal:
                            var Losses = IFM.VR.Common.Helpers.HOM.LossHistoryHelper_HOM.GetAllHOMLosses(quote);
                            if (Losses != null && lossIndex < Losses.Count)
                            {
                                loss = Losses[lossIndex];

                                VRGeneralValidations.Val_HasRequiredField_DD(loss.TypeOfLossId, valList, LossType, "Loss Type");

                                if (VRGeneralValidations.Val_HasRequiredField(loss.LossDate, valList, LossDate, "Loss Date"))
                                    VRGeneralValidations.Val_IsDateInRange(loss.LossDate, valList, LossDate, "Loss Date", DateTime.Now.AddYears(-100).ToString(), DateTime.Now.ToString());

                                if (VRGeneralValidations.Val_HasRequiredField(loss.Amount, valList, LossAmount, "Loss Amount"))
                                    VRGeneralValidations.Val_IsNonNegativeNumber(loss.Amount, valList, LossAmount, "Loss Amount");
                            }
                            else
                            {
                                valList.Add(new ValidationItem("Loss is not in loss list", LossObjectNull, false));
                            }

                            break;
                    }
                }
                
            }
            else
            {
                valList.Add(new ValidationItem("Quote is empty", LossObjectNull, false));
            }
            return valList;
        }
    }
}