using System.Linq;
using System;
using System.Diagnostics;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common
{
    public static class LossListValidator
    {
        public const string ValidationListID = "{F7F22CCD-96EC-443C-8EE2-AC0413037F3F}";
        public const string LossDateDuplicatedInList = "{42E0AB55-2E2B-4D01-BED5-46AC7B4EE464}";
        public const string MultipleLossesIn5Years = "{DB5C0835-9402-4833-8C2C-DF1A9BF41D5F}";
        //public const string ChoicePointProcessingStatusCodeIs2 = "{F6D0D3B6-5386-4525-AA05-0EDBE42C4985}"; 'BU Decided they did not want this validation - Daniel Gugenheim - 11/20/20105

        public static Validation.ObjectValidation.ValidationItemList ValidateLossList(int driverIndex, Diamond.Common.Objects.ThirdParty.ThirdPartyData clue, QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.DriverIndex, driverIndex.ToString());

            if (quote != null)
            {
                //updated 8/15/2018 for multi-state; original logic is in IF; applicable references to quote changed to governingStateQuote
                QuickQuote.CommonObjects.QuickQuoteObject governingStateQuote = IFM.VR.Common.Helpers.MultiState.General.GoverningStateQuote_SubQuotesOmitted(ref quote); //should always return something
                if (governingStateQuote != null)
                {
                    switch (quote.LobType)
                    {
                        case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal:
                            if (governingStateQuote.Drivers != null && governingStateQuote.Drivers.Count > driverIndex)
                            {
                                QuickQuote.CommonObjects.QuickQuoteDriver d = governingStateQuote.Drivers[driverIndex];
                                if (d != null && d.LossHistoryRecords != null && d.LossHistoryRecords.Any())
                                {
                                    switch (valType)
                                    {
                                        case ValidationItem.ValidationType.quoteRate:
                                            var lossesGrouped = from lh in d.LossHistoryRecords group lh by lh.LossDate;
                                            if ((from grp in lossesGrouped where grp.Count() > 1 select d).Any())
                                            {
                                                valList.Add(new ValidationItem("More than one loss is listed on the Loss History screen with the same loss date.", LossDateDuplicatedInList, true));
                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }
                            break;
                        //case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal:
                        //    if (valType == ValidationItem.ValidationType.appRate || valType == ValidationItem.ValidationType.issuance)
                        //    {
                        //        if (governingStateQuote.LossHistoryRecords != null && governingStateQuote.LossHistoryRecords.Count > 1)
                        //        {
                        //            int numLossesWithin5Years = 0;
                        //            governingStateQuote.LossHistoryRecords.ForEach(delegate (QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord loss) { if (DateTime.Parse(loss.LossDate) > DateTime.Now.AddYears(-5)) { numLossesWithin5Years++; } });
                        //            if (numLossesWithin5Years > 1)
                        //            {
                        //                valList.Add(new ValidationItem("More than 1 loss in the last 5 years. Refer to Underwriting", MultipleLossesIn5Years, valType != ValidationItem.ValidationType.issuance, true));
                        //            }
                        //        }
                        //        else if (clue != null && clue.LossHistory != null && clue.LossHistory.Count > 0)
                        //        {
                        //            int numLossesWithin5Years = 0;
                        //            clue.LossHistory.ToList().ForEach(delegate (Diamond.Common.Objects.Policy.LossHistory loss) { if (DateTime.Parse(loss.LossDate) > DateTime.Now.AddYears(-5)) { numLossesWithin5Years++; } });
                        //            if (numLossesWithin5Years > 1)
                        //            {
                        //                valList.Add(new ValidationItem("More than 1 loss in the last 5 years. Refer to Underwriting", MultipleLossesIn5Years, valType != ValidationItem.ValidationType.issuance, true));
                        //            }
                        //        }
                        //    }

                        //    //if (clue != null && clue.ChoicePointTransmissions != null && clue.ChoicePointTransmissions.Count > 0) 'BU Decided they did not want this validation - Daniel Gugenheim - 11/20/20105
                        //    //{
                        //    //    if (clue.ChoicePointTransmissions[0].ProcessingStatusCode == "5")
                        //    //    {
                        //    //        valList.Add(new ValidationItem("CLUE Property – Processing Complete. No Claims. No Inquiry History", ChoicePointProcessingStatusCodeIs2, valType != ValidationItem.ValidationType.issuance, true));
                        //    //    }
                        //    //}
                        //    break;
                        case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP:
                            if (valType == ValidationItem.ValidationType.appRate || valType == ValidationItem.ValidationType.issuance)
                            {
                                if (governingStateQuote.LossHistoryRecords != null && governingStateQuote.LossHistoryRecords.Count > 1)
                                {
                                    int numLossesWithin5Years = 0;
                                    // I rewrote this validation to only test dates that are valid because it was throwing an error.  MGB 7-17-2017 (BOP)
                                    foreach (QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord loss in governingStateQuote.LossHistoryRecords)
                                    {
                                        string dt = "";
                                        try
                                        {
                                            DateTime dat = DateTime.Parse(loss.LossDate);
                                            dt = loss.LossDate;
                                        }
                                        catch { }
                                        if (dt != "")
                                        {
                                            if (DateTime.Parse(dt) > DateTime.Now.AddYears(-5)) { numLossesWithin5Years++; }
                                        }
                                    }
                                    //quote.LossHistoryRecords.ForEach(delegate (QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord loss) { if (DateTime.Parse(loss.LossDate) > DateTime.Now.AddYears(-5)) { numLossesWithin5Years++; } });
                                    if (numLossesWithin5Years > 1)
                                    {
                                        valList.Add(new ValidationItem("More than 1 loss in the last 5 years. Refer to Underwriting", MultipleLossesIn5Years, valType != ValidationItem.ValidationType.issuance, true));
                                    }
                                }
                                else if (clue != null && clue.LossHistory != null && clue.LossHistory.Count > 0)
                                {
                                    int numLossesWithin5Years = 0;
                                    clue.LossHistory.ToList().ForEach(delegate (Diamond.Common.Objects.Policy.LossHistory loss) { if (DateTime.Parse(loss.LossDate) > DateTime.Now.AddYears(-5)) { numLossesWithin5Years++; } });
                                    if (numLossesWithin5Years > 1)
                                    {
                                        valList.Add(new ValidationItem("More than 1 loss in the last 5 years. Refer to Underwriting", MultipleLossesIn5Years, valType != ValidationItem.ValidationType.issuance, true));
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                                
            }

            return valList;
        }
    }
}