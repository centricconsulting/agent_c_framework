using System;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA
{
    public static class PolicyDriverLossesAndViolationValidator
    {
        // This is only used for PPA, so no multi state changes are needed 9/17/18 MLW
        public const string ValidationListID = "{B94E35F7-E89A-4DF7-9FC7-DFE85F60171C}";

        public const string PolicyAtFaultLossesInPast3YearsExceeeded = "{E7705B1D-934A-488B-95FA-819176511AA8}";
        public const string PolicyAtFaultLossesAndOrViolationsInPast5YearsExceeeded = "{E445CB3D-C1DD-4534-9875-ECB2F5491ACD}";
        public const string PolicyMinorViolationsCountInPast3YearsExceeded = "{753AAA4F-9C96-421C-80A6-D7506D298B5A}";
        public const string anyDriverOver20Has3Minorsin3YearsExceeded = "{39CDB918-102C-465C-814E-46CA0E14414F}";
        public const string anyDriverUNder20Has2Minorsin3YearsExceeded = "{BB5A0A99-5BEA-40FD-8EBB-13C28B722B32}";
        public const string anyDriverHas1AtFaulAnd1MajorInPast3YearsExceeded = "{E7169CB7-8ABF-40F5-B6EA-EBA29F92BFDB}";
        public const string anyDriverHas2OrMoreMajorAnd1OrMoreUnacceptableInPast3YearsExceeded = "{3C9F21BA-1E50-44D7-B069-E358DCF27245}";
        public const string anyDriverHas2OrMoreMajorViolationsInPast3YearsExceeded = "{CC0F5EF3-4672-4F1A-8EDB-139B7A5DC336}";
        public const string PolicyMajorViolationsCountInPast3YearsExceeded2 = "{01631DA0-3665-40AA-85BA-E9DFEEF57AF2}";
        public const string PolicyUnacceptableViolationsCountInPast5YearsExceeded = "{A7DAAE75-7F85-4A47-89F9-F3A131560A73}";

        public const string AnyDriverHasMoreThan1LossesInPast3Years = "{b705e612-5e88-4131-9632-29261701e31c}";

        public static readonly string[] minorViolations = new string[] { "3", "2", "21", "1", "4" };
        public static readonly string[] majorViolations = new string[] { "14", "15", "16", "9", "10", "13", "47", "11", "12", "7", "8", "20", "19" };
        public static readonly string[] unacceptableViolations = new string[] { "32", "33", "28", "31", "26", "34", "27", "25", "29", "30", "24", "23" };
        public static readonly string[] violationsToIgnore = new string[] { "36" }; //added 11/3/2017 for TFS Bug 22702 (36 = MVR Record Clear)

        public static Validation.ObjectValidation.ValidationItemList ValidatePolicyDriversLossesAndViolations(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            //valList.AddBreadCrum(ValidationBreadCrum.BCType.DriverIndex, driverIndex.ToString());
            if (valType == ValidationItem.ValidationType.quoteRate)
            {
                Int32 TotalViolationCountInPast5Years = 0;

                bool anyDriverHasMoreThan1LossesIn3Years = false;

                bool anyDriverOver20Has3Minorsin3Years = false;
                bool anyDriverUNder20Has2Minorsin3Years = false;
                bool anyDriverHas1AtFaulAnd1MajorInPast3Years = false;
                bool anyDriverHas2OrMoreMajorAnd1OrMoreUnacceptableInPast3Years = false;
                bool anyDriverHas2OrMoreMajorViolationsInPast3Years = false;

                Int32 Policy_Total_Losses_CountInPas3Years = 0;
                Int32 PolicyAtFault_Losses_CountInPas3Years = 0;
                Int32 PolicyAtFault_Losses_CountInPas5Years = 0;

                Int32 PolicyMajorViolationsCountInPast3Years = 0;
                Int32 PolicyMinorViolationsCountInPast3Years = 0;
                Int32 PolicyUnacceptableViolationsCountInPast3Years = 0;

                Int32 PolicyMajorViolationsCountInPast5Years = 0;
                Int32 PolicyMinorViolationsCountInPast5Years = 0;
                Int32 PolicyUnacceptableViolationsCountInPast5Years = 0;

                //added 7/26/2019
                QuickQuote.CommonMethods.QuickQuoteHelperClass qqHelper = new QuickQuote.CommonMethods.QuickQuoteHelperClass();
                bool isEndorsement = false;
                bool hasNewEndorsementDriverViolations = false;
                bool hasNewEndorsementDriverLosses = false;
                bool anyNewEndorsementDriverHasMoreThan1LossesIn3Years = false;
                bool anyNewEndorsementDriverOver20Has3Minorsin3Years = false;
                bool anyNewEndorsementDriverUNder20Has2Minorsin3Years = false;
                bool anyNewEndorsementDriverHas1AtFaulAnd1MajorInPast3Years = false;
                bool anyNewEndorsementDriverHas2OrMoreMajorAnd1OrMoreUnacceptableInPast3Years = false;
                bool anyNewEndorsementDriverHas2OrMoreMajorViolationsInPast3Years = false;
                Int32 TotalViolationCountInPast5Years_Endorsement_NewDriver = 0;
                Int32 Policy_Total_Losses_CountInPas3Years_Endorsement_NewDriver = 0;
                Int32 PolicyAtFault_Losses_CountInPas3Years_Endorsement_NewDriver = 0;
                Int32 PolicyAtFault_Losses_CountInPas5Years_Endorsement_NewDriver = 0;
                Int32 PolicyMajorViolationsCountInPast3Years_Endorsement_NewDriver = 0;
                Int32 PolicyMinorViolationsCountInPast3Years_Endorsement_NewDriver = 0;
                Int32 PolicyUnacceptableViolationsCountInPast3Years_Endorsement_NewDriver = 0;
                Int32 PolicyMajorViolationsCountInPast5Years_Endorsement_NewDriver = 0;
                Int32 PolicyMinorViolationsCountInPast5Years_Endorsement_NewDriver = 0;
                Int32 PolicyUnacceptableViolationsCountInPast5Years_Endorsement_NewDriver = 0;
                Int32 TotalViolationCountInPast5Years_Endorsement_ExistingDriver_New = 0;
                Int32 Policy_Total_Losses_CountInPas3Years_Endorsement_ExistingDriver_New = 0;
                Int32 PolicyAtFault_Losses_CountInPas3Years_Endorsement_ExistingDriver_New = 0;
                Int32 PolicyAtFault_Losses_CountInPas5Years_Endorsement_ExistingDriver_New = 0;
                Int32 PolicyMajorViolationsCountInPast3Years_Endorsement_ExistingDriver_New = 0;
                Int32 PolicyMinorViolationsCountInPast3Years_Endorsement_ExistingDriver_New = 0;
                Int32 PolicyUnacceptableViolationsCountInPast3Years_Endorsement_ExistingDriver_New = 0;
                Int32 PolicyMajorViolationsCountInPast5Years_Endorsement_ExistingDriver_New = 0;
                Int32 PolicyMinorViolationsCountInPast5Years_Endorsement_ExistingDriver_New = 0;
                Int32 PolicyUnacceptableViolationsCountInPast5Years_Endorsement_ExistingDriver_New = 0;
                Int32 TotalViolationCountInPast5Years_Endorsement_ExistingDriver_PreExisting = 0;
                Int32 Policy_Total_Losses_CountInPas3Years_Endorsement_ExistingDriver_PreExisting = 0;
                Int32 PolicyAtFault_Losses_CountInPas3Years_Endorsement_ExistingDriver_PreExisting = 0;
                Int32 PolicyAtFault_Losses_CountInPas5Years_Endorsement_ExistingDriver_PreExisting = 0;
                Int32 PolicyMajorViolationsCountInPast3Years_Endorsement_ExistingDriver_PreExisting = 0;
                Int32 PolicyMinorViolationsCountInPast3Years_Endorsement_ExistingDriver_PreExisting = 0;
                Int32 PolicyUnacceptableViolationsCountInPast3Years_Endorsement_ExistingDriver_PreExisting = 0;
                Int32 PolicyMajorViolationsCountInPast5Years_Endorsement_ExistingDriver_PreExisting = 0;
                Int32 PolicyMinorViolationsCountInPast5Years_Endorsement_ExistingDriver_PreExisting = 0;
                Int32 PolicyUnacceptableViolationsCountInPast5Years_Endorsement_ExistingDriver_PreExisting = 0;

                if (quote != null)
                {
                    DateTime effectiveDate = DateTime.MinValue;
                    if (DateTime.TryParse(quote.EffectiveDate, out effectiveDate) == false)
                    {
                        effectiveDate = DateTime.Now; // no effective date so just use today
                    }

                    //added 7/26/2019
                    if (quote.QuoteTransactionType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                    {
                        isEndorsement = true;
                    }                    

                    if (quote.Drivers != null)
                    {
                        foreach (QuickQuote.CommonObjects.QuickQuoteDriver d in quote.Drivers)
                        {
                            if (d.DriverExcludeTypeId == "1")
                            {
                                int Driver_Total_Losses_CountInPast3Years = 0;
                                int DriverAtFault_Losses_CountInPast3Years = 0;
                                Int32 DriverMajorViolationCountInPast3Years = 0;
                                Int32 DriverMinorViolationCountInPast3Years = 0;
                                Int32 DriverUnacceptableViolationCountInPast3Years = 0;

                                Int32 DriverAtFault_Losses_CountInPast5Years = 0;
                                Int32 DriverMajorViolationCountInPast5Years = 0;
                                Int32 DriverMinorViolationCountInPast5Years = 0;
                                Int32 DriverUnacceptableViolationCountInPast5Years = 0;

                                //added 7/26/2019
                                bool newEndorsementDriverHasViolations = false;
                                bool newEndorsementDriverHasLosses = false;
                                int Driver_Total_Losses_CountInPast3Years_Endorsement_NewDriver = 0;
                                int DriverAtFault_Losses_CountInPast3Years_Endorsement_NewDriver = 0;
                                Int32 DriverMajorViolationCountInPast3Years_Endorsement_NewDriver = 0;
                                Int32 DriverMinorViolationCountInPast3Years_Endorsement_NewDriver = 0;
                                Int32 DriverUnacceptableViolationCountInPast3Years_Endorsement_NewDriver = 0;
                                Int32 DriverAtFault_Losses_CountInPast5Years_Endorsement_NewDriver = 0;
                                Int32 DriverMajorViolationCountInPast5Years_Endorsement_NewDriver = 0;
                                Int32 DriverMinorViolationCountInPast5Years_Endorsement_NewDriver = 0;
                                Int32 DriverUnacceptableViolationCountInPast5Years_Endorsement_NewDriver = 0;
                                int Driver_Total_Losses_CountInPast3Years_Endorsement_ExistingDriver_New = 0;
                                int DriverAtFault_Losses_CountInPast3Years_Endorsement_ExistingDriver_New = 0;
                                Int32 DriverMajorViolationCountInPast3Years_Endorsement_ExistingDriver_New = 0;
                                Int32 DriverMinorViolationCountInPast3Years_Endorsement_ExistingDriver_New = 0;
                                Int32 DriverUnacceptableViolationCountInPast3Years_Endorsement_ExistingDriver_New = 0;
                                Int32 DriverAtFault_Losses_CountInPast5Years_Endorsement_ExistingDriver_New = 0;
                                Int32 DriverMajorViolationCountInPast5Years_Endorsement_ExistingDriver_New = 0;
                                Int32 DriverMinorViolationCountInPast5Years_Endorsement_ExistingDriver_New = 0;
                                Int32 DriverUnacceptableViolationCountInPast5Years_Endorsement_ExistingDriver_New = 0;
                                int Driver_Total_Losses_CountInPast3Years_Endorsement_ExistingDriver_PreExisting = 0;
                                int DriverAtFault_Losses_CountInPast3Years_Endorsement_ExistingDriver_PreExisting = 0;
                                Int32 DriverMajorViolationCountInPast3Years_Endorsement_ExistingDriver_PreExisting = 0;
                                Int32 DriverMinorViolationCountInPast3Years_Endorsement_ExistingDriver_PreExisting = 0;
                                Int32 DriverUnacceptableViolationCountInPast3Years_Endorsement_ExistingDriver_PreExisting = 0;
                                Int32 DriverAtFault_Losses_CountInPast5Years_Endorsement_ExistingDriver_PreExisting = 0;
                                Int32 DriverMajorViolationCountInPast5Years_Endorsement_ExistingDriver_PreExisting = 0;
                                Int32 DriverMinorViolationCountInPast5Years_Endorsement_ExistingDriver_PreExisting = 0;
                                Int32 DriverUnacceptableViolationCountInPast5Years_Endorsement_ExistingDriver_PreExisting = 0;
                                bool newEndorsementDriver = false; //only need to check if there are accsViols or lossHists
                                if (isEndorsement)
                                {
                                    if (d.AccidentViolations != null && d.AccidentViolations.Count > 0)
                                    {
                                        newEndorsementDriverHasViolations = true;
                                        hasNewEndorsementDriverViolations = true;
                                    }
                                    if (d.LossHistoryRecords != null && d.LossHistoryRecords.Count > 0)
                                    {
                                        newEndorsementDriverHasLosses = true;
                                        hasNewEndorsementDriverLosses = true;
                                    }
                                    if (newEndorsementDriverHasViolations || newEndorsementDriverHasLosses)
                                    {                                        
                                        newEndorsementDriver = qqHelper.IsQuickQuoteDriverNewToImage(d, quote);
                                    }
                                }                                

                                if (d.AccidentViolations != null)
                                {
                                    foreach (QuickQuote.CommonObjects.QuickQuoteAccidentViolation v in d.AccidentViolations)
                                    {
                                        if (string.IsNullOrWhiteSpace(v.AccidentsViolationsTypeId) == false && violationsToIgnore.Contains(v.AccidentsViolationsTypeId) == false) //updated 11/3/2017 for TFS Bug 22702
                                        {
                                            System.DateTime vDate = DateTime.MinValue;
                                            if (DateTime.TryParse(v.AvDate, out vDate))
                                            {
                                                //added 7/26/2019
                                                bool isNewViolForDriver = false;
                                                if (newEndorsementDriver)
                                                {
                                                    isNewViolForDriver = true;
                                                }
                                                else
                                                {
                                                    isNewViolForDriver = qqHelper.IsQuickQuoteAccidentViolationNewToImage(v, quote);
                                                }
                                                
                                                // 5 years
                                                if (vDate > effectiveDate.AddYears(-5))
                                                {
                                                    TotalViolationCountInPast5Years += 1;
                                                    if (isEndorsement) //added 7/26/2019
                                                    {
                                                        if (newEndorsementDriver)
                                                        {
                                                            TotalViolationCountInPast5Years_Endorsement_NewDriver += 1;
                                                        }
                                                        else
                                                        {
                                                            if (isNewViolForDriver)
                                                            {
                                                                TotalViolationCountInPast5Years_Endorsement_ExistingDriver_New += 1;
                                                            }
                                                            else
                                                            {
                                                                TotalViolationCountInPast5Years_Endorsement_ExistingDriver_PreExisting += 1;
                                                            }
                                                        }
                                                    }

                                                    if (unacceptableViolations.Contains(v.AccidentsViolationsTypeId))
                                                    {
                                                        DriverUnacceptableViolationCountInPast5Years += 1;
                                                        if (isEndorsement) //added 7/26/2019
                                                        {
                                                            if (newEndorsementDriver)
                                                            {
                                                                DriverUnacceptableViolationCountInPast5Years_Endorsement_NewDriver += 1;
                                                            }
                                                            else
                                                            {
                                                                if (isNewViolForDriver)
                                                                {
                                                                    DriverUnacceptableViolationCountInPast5Years_Endorsement_ExistingDriver_New += 1;
                                                                }
                                                                else
                                                                {
                                                                    DriverUnacceptableViolationCountInPast5Years_Endorsement_ExistingDriver_PreExisting += 1;
                                                                }
                                                            }
                                                        }
                                                    }                                                        

                                                    if (majorViolations.Contains(v.AccidentsViolationsTypeId))
                                                    {
                                                        DriverMajorViolationCountInPast5Years += 1;
                                                        if (isEndorsement) //added 7/26/2019
                                                        {
                                                            if (newEndorsementDriver)
                                                            {
                                                                DriverMajorViolationCountInPast5Years_Endorsement_NewDriver += 1;
                                                            }
                                                            else
                                                            {
                                                                if (isNewViolForDriver)
                                                                {
                                                                    DriverMajorViolationCountInPast5Years_Endorsement_ExistingDriver_New += 1;
                                                                }
                                                                else
                                                                {
                                                                    DriverMajorViolationCountInPast5Years_Endorsement_ExistingDriver_PreExisting += 1;
                                                                }
                                                            }
                                                        }
                                                    }                                                        

                                                    if (minorViolations.Contains(v.AccidentsViolationsTypeId))
                                                    {
                                                        DriverMinorViolationCountInPast5Years += 1;
                                                        if (isEndorsement) //added 7/26/2019
                                                        {
                                                            if (newEndorsementDriver)
                                                            {
                                                                DriverMinorViolationCountInPast5Years_Endorsement_NewDriver += 1;
                                                            }
                                                            else
                                                            {
                                                                if (isNewViolForDriver)
                                                                {
                                                                    DriverMinorViolationCountInPast5Years_Endorsement_ExistingDriver_New += 1;
                                                                }
                                                                else
                                                                {
                                                                    DriverMinorViolationCountInPast5Years_Endorsement_ExistingDriver_PreExisting += 1;
                                                                }
                                                            }
                                                        }
                                                    }                                                        
                                                }

                                                // 3 years
                                                if (vDate > effectiveDate.AddYears(-3))
                                                {
                                                    if (unacceptableViolations.Contains(v.AccidentsViolationsTypeId))
                                                    {
                                                        DriverUnacceptableViolationCountInPast3Years += 1;
                                                        if (isEndorsement) //added 7/26/2019
                                                        {
                                                            if (newEndorsementDriver)
                                                            {
                                                                DriverUnacceptableViolationCountInPast3Years_Endorsement_NewDriver += 1;
                                                            }
                                                            else
                                                            {
                                                                if (isNewViolForDriver)
                                                                {
                                                                    DriverUnacceptableViolationCountInPast3Years_Endorsement_ExistingDriver_New += 1;
                                                                }
                                                                else
                                                                {
                                                                    DriverUnacceptableViolationCountInPast3Years_Endorsement_ExistingDriver_PreExisting += 1;
                                                                }
                                                            }
                                                        }
                                                    }                                                        

                                                    if (majorViolations.Contains(v.AccidentsViolationsTypeId))
                                                    {
                                                        DriverMajorViolationCountInPast3Years += 1;
                                                        if (isEndorsement) //added 7/26/2019
                                                        {
                                                            if (newEndorsementDriver)
                                                            {
                                                                DriverMajorViolationCountInPast3Years_Endorsement_NewDriver += 1;
                                                            }
                                                            else
                                                            {
                                                                if (isNewViolForDriver)
                                                                {
                                                                    DriverMajorViolationCountInPast3Years_Endorsement_ExistingDriver_New += 1;
                                                                }
                                                                else
                                                                {
                                                                    DriverMajorViolationCountInPast3Years_Endorsement_ExistingDriver_PreExisting += 1;
                                                                }
                                                            }
                                                        }
                                                    }                                                        

                                                    if (minorViolations.Contains(v.AccidentsViolationsTypeId))
                                                    {
                                                        DriverMinorViolationCountInPast3Years += 1;
                                                        if (isEndorsement) //added 7/26/2019
                                                        {
                                                            if (newEndorsementDriver)
                                                            {
                                                                DriverMinorViolationCountInPast3Years_Endorsement_NewDriver += 1;
                                                            }
                                                            else
                                                            {
                                                                if (isNewViolForDriver)
                                                                {
                                                                    DriverMinorViolationCountInPast3Years_Endorsement_ExistingDriver_New += 1;
                                                                }
                                                                else
                                                                {
                                                                    DriverMinorViolationCountInPast3Years_Endorsement_ExistingDriver_PreExisting += 1;
                                                                }
                                                            }
                                                        }
                                                    }                                                        
                                                }
                                            }
                                        }
                                    }
                                }

                                string atFaultID = "1";
                                if (d.LossHistoryRecords != null)
                                {
                                    foreach (QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord loss in d.LossHistoryRecords)
                                    {
                                        System.DateTime vDate = DateTime.MinValue;
                                        if (DateTime.TryParse(loss.LossDate, out vDate))
                                        {
                                            //added 7/26/2019
                                            bool isNewLossForDriver = false;
                                            if (newEndorsementDriver)
                                            {
                                                isNewLossForDriver = true;
                                            }
                                            else
                                            {
                                                isNewLossForDriver = qqHelper.IsQuickQuoteLossHistoryNewToImage(loss, quote);
                                            }

                                            // 5 years
                                            if (vDate > effectiveDate.AddYears(-5))
                                            {
                                                if (loss.LossHistoryFaultId == atFaultID)
                                                {
                                                    DriverAtFault_Losses_CountInPast5Years += 1;
                                                    if (isEndorsement) //added 7/26/2019
                                                    {
                                                        if (newEndorsementDriver)
                                                        {
                                                            DriverAtFault_Losses_CountInPast5Years_Endorsement_NewDriver += 1;
                                                        }
                                                        else
                                                        {
                                                            if (isNewLossForDriver)
                                                            {
                                                                DriverAtFault_Losses_CountInPast5Years_Endorsement_ExistingDriver_New += 1;
                                                            }
                                                            else
                                                            {
                                                                DriverAtFault_Losses_CountInPast5Years_Endorsement_ExistingDriver_PreExisting += 1;
                                                            }
                                                        }
                                                    }
                                                }                                                    
                                            }

                                            //3 years At Fault last 3 years
                                            if (vDate > effectiveDate.AddYears(-3))
                                            {
                                                Driver_Total_Losses_CountInPast3Years += 1;
                                                if (isEndorsement) //added 7/26/2019
                                                {
                                                    if (newEndorsementDriver)
                                                    {
                                                        Driver_Total_Losses_CountInPast3Years_Endorsement_NewDriver += 1;
                                                    }
                                                    else
                                                    {
                                                        if (isNewLossForDriver)
                                                        {
                                                            Driver_Total_Losses_CountInPast3Years_Endorsement_ExistingDriver_New += 1;
                                                        }
                                                        else
                                                        {
                                                            Driver_Total_Losses_CountInPast3Years_Endorsement_ExistingDriver_PreExisting += 1;
                                                        }
                                                    }
                                                }
                                                if (loss.LossHistoryFaultId == atFaultID)
                                                {
                                                    DriverAtFault_Losses_CountInPast3Years += 1;
                                                    if (isEndorsement) //added 7/26/2019
                                                    {
                                                        if (newEndorsementDriver)
                                                        {
                                                            DriverAtFault_Losses_CountInPast3Years_Endorsement_NewDriver += 1;
                                                        }
                                                        else
                                                        {
                                                            if (isNewLossForDriver)
                                                            {
                                                                DriverAtFault_Losses_CountInPast3Years_Endorsement_ExistingDriver_New += 1;
                                                            }
                                                            else
                                                            {
                                                                DriverAtFault_Losses_CountInPast3Years_Endorsement_ExistingDriver_PreExisting += 1;
                                                            }
                                                        }
                                                    }
                                                }                                                    
                                            }
                                        }
                                    }
                                }

                                // do driver checks or add driver totals to policy totals
                                if (Driver_Total_Losses_CountInPast3Years > 1)
                                {
                                    anyDriverHasMoreThan1LossesIn3Years = true;
                                    if (isEndorsement) //added 7/26/2019
                                    {
                                        if (newEndorsementDriver)
                                        {
                                            anyNewEndorsementDriverHasMoreThan1LossesIn3Years = true;
                                        }
                                    }
                                }                                    

                                Policy_Total_Losses_CountInPas3Years += Driver_Total_Losses_CountInPast3Years;
                                PolicyAtFault_Losses_CountInPas3Years += DriverAtFault_Losses_CountInPast3Years;
                                PolicyAtFault_Losses_CountInPas5Years += DriverAtFault_Losses_CountInPast5Years;

                                PolicyMajorViolationsCountInPast3Years += DriverMajorViolationCountInPast3Years;
                                PolicyMinorViolationsCountInPast3Years += DriverMinorViolationCountInPast3Years;
                                PolicyUnacceptableViolationsCountInPast3Years += DriverUnacceptableViolationCountInPast3Years;

                                PolicyMajorViolationsCountInPast5Years += DriverMajorViolationCountInPast5Years;
                                PolicyMinorViolationsCountInPast5Years += DriverMinorViolationCountInPast5Years;
                                PolicyUnacceptableViolationsCountInPast5Years += DriverUnacceptableViolationCountInPast5Years;

                                //added 7/26/2019 for Endorsements
                                if (isEndorsement)
                                {
                                    Policy_Total_Losses_CountInPas3Years_Endorsement_NewDriver += Driver_Total_Losses_CountInPast3Years_Endorsement_NewDriver;
                                    PolicyAtFault_Losses_CountInPas3Years_Endorsement_NewDriver += DriverAtFault_Losses_CountInPast3Years_Endorsement_NewDriver;
                                    PolicyAtFault_Losses_CountInPas5Years_Endorsement_NewDriver += DriverAtFault_Losses_CountInPast5Years_Endorsement_NewDriver;
                                    PolicyMajorViolationsCountInPast3Years_Endorsement_NewDriver += DriverMajorViolationCountInPast3Years_Endorsement_NewDriver;
                                    PolicyMinorViolationsCountInPast3Years_Endorsement_NewDriver += DriverMinorViolationCountInPast3Years_Endorsement_NewDriver;
                                    PolicyUnacceptableViolationsCountInPast3Years_Endorsement_NewDriver += DriverUnacceptableViolationCountInPast3Years_Endorsement_NewDriver;
                                    PolicyMajorViolationsCountInPast5Years_Endorsement_NewDriver += DriverMajorViolationCountInPast5Years_Endorsement_NewDriver;
                                    PolicyMinorViolationsCountInPast5Years_Endorsement_NewDriver += DriverMinorViolationCountInPast5Years_Endorsement_NewDriver;
                                    PolicyUnacceptableViolationsCountInPast5Years_Endorsement_NewDriver += DriverUnacceptableViolationCountInPast5Years_Endorsement_NewDriver;
                                    Policy_Total_Losses_CountInPas3Years_Endorsement_ExistingDriver_New += Driver_Total_Losses_CountInPast3Years_Endorsement_ExistingDriver_New;
                                    PolicyAtFault_Losses_CountInPas3Years_Endorsement_ExistingDriver_New += DriverAtFault_Losses_CountInPast3Years_Endorsement_ExistingDriver_New;
                                    PolicyAtFault_Losses_CountInPas5Years_Endorsement_ExistingDriver_New += DriverAtFault_Losses_CountInPast5Years_Endorsement_ExistingDriver_New;
                                    PolicyMajorViolationsCountInPast3Years_Endorsement_ExistingDriver_New += DriverMajorViolationCountInPast3Years_Endorsement_ExistingDriver_New;
                                    PolicyMinorViolationsCountInPast3Years_Endorsement_ExistingDriver_New += DriverMinorViolationCountInPast3Years_Endorsement_ExistingDriver_New;
                                    PolicyUnacceptableViolationsCountInPast3Years_Endorsement_ExistingDriver_New += DriverUnacceptableViolationCountInPast3Years_Endorsement_ExistingDriver_New;
                                    PolicyMajorViolationsCountInPast5Years_Endorsement_ExistingDriver_New += DriverMajorViolationCountInPast5Years_Endorsement_ExistingDriver_New;
                                    PolicyMinorViolationsCountInPast5Years_Endorsement_ExistingDriver_New += DriverMinorViolationCountInPast5Years_Endorsement_ExistingDriver_New;
                                    PolicyUnacceptableViolationsCountInPast5Years_Endorsement_ExistingDriver_New += DriverUnacceptableViolationCountInPast5Years_Endorsement_ExistingDriver_New;
                                    Policy_Total_Losses_CountInPas3Years_Endorsement_ExistingDriver_PreExisting += Driver_Total_Losses_CountInPast3Years_Endorsement_ExistingDriver_PreExisting;
                                    PolicyAtFault_Losses_CountInPas3Years_Endorsement_ExistingDriver_PreExisting += DriverAtFault_Losses_CountInPast3Years_Endorsement_ExistingDriver_PreExisting;
                                    PolicyAtFault_Losses_CountInPas5Years_Endorsement_ExistingDriver_PreExisting += DriverAtFault_Losses_CountInPast5Years_Endorsement_ExistingDriver_PreExisting;
                                    PolicyMajorViolationsCountInPast3Years_Endorsement_ExistingDriver_PreExisting += DriverMajorViolationCountInPast3Years_Endorsement_ExistingDriver_PreExisting;
                                    PolicyMinorViolationsCountInPast3Years_Endorsement_ExistingDriver_PreExisting += DriverMinorViolationCountInPast3Years_Endorsement_ExistingDriver_PreExisting;
                                    PolicyUnacceptableViolationsCountInPast3Years_Endorsement_ExistingDriver_PreExisting += DriverUnacceptableViolationCountInPast3Years_Endorsement_ExistingDriver_PreExisting;
                                    PolicyMajorViolationsCountInPast5Years_Endorsement_ExistingDriver_PreExisting += DriverMajorViolationCountInPast5Years_Endorsement_ExistingDriver_PreExisting;
                                    PolicyMinorViolationsCountInPast5Years_Endorsement_ExistingDriver_PreExisting += DriverMinorViolationCountInPast5Years_Endorsement_ExistingDriver_PreExisting;
                                    PolicyUnacceptableViolationsCountInPast5Years_Endorsement_ExistingDriver_PreExisting += DriverUnacceptableViolationCountInPast5Years_Endorsement_ExistingDriver_PreExisting;
                                }

                                //driver may not have a birth date
                                try
                                {
                                    //driver over 20
                                    if (DriverMinorViolationCountInPast3Years > 2 && (effectiveDate.Year - DateTime.Parse(d.Name.BirthDate).Year) >= 20)
                                    {
                                        anyDriverOver20Has3Minorsin3Years = true;
                                        if (isEndorsement) //added 7/26/2019
                                        {
                                            if (newEndorsementDriver)
                                            {
                                                anyNewEndorsementDriverOver20Has3Minorsin3Years = true;
                                            }
                                        }
                                    }                                        

                                    // 4-8-2014 Bug# 2922
                                    // driver under 20 - changed from DriverMinorViolationCountInPast3Years > 1 to
                                    // DriverMinorViolationCountInPast3Years > 2
                                    if (DriverMinorViolationCountInPast3Years > 2 && (effectiveDate.Year - DateTime.Parse(d.Name.BirthDate).Year) < 20)
                                    {
                                        anyDriverUNder20Has2Minorsin3Years = true;
                                        if (isEndorsement) //added 7/26/2019
                                        {
                                            if (newEndorsementDriver)
                                            {
                                                anyNewEndorsementDriverUNder20Has2Minorsin3Years = true;
                                            }
                                        }
                                    }                                        
                                }
                                catch (Exception ex)
                                {
                                }

                                // has an at fault and major violation
                                if (DriverAtFault_Losses_CountInPast3Years > 0 && DriverMajorViolationCountInPast3Years > 0)
                                {
                                    anyDriverHas1AtFaulAnd1MajorInPast3Years = true;
                                    if (isEndorsement) //added 7/26/2019
                                    {
                                        if (newEndorsementDriver)
                                        {
                                            anyNewEndorsementDriverHas1AtFaulAnd1MajorInPast3Years = true;
                                        }
                                    }
                                }                                    

                                //has more than one major and any unacceptable
                                if (DriverMajorViolationCountInPast3Years > 1 && DriverUnacceptableViolationCountInPast3Years > 0)
                                {
                                    anyDriverHas2OrMoreMajorAnd1OrMoreUnacceptableInPast3Years = true;
                                    if (isEndorsement) //added 7/26/2019
                                    {
                                        if (newEndorsementDriver)
                                        {
                                            anyNewEndorsementDriverHas2OrMoreMajorAnd1OrMoreUnacceptableInPast3Years = true;
                                        }
                                    }
                                }                                    

                                //has more than one major
                                if (DriverMajorViolationCountInPast3Years > 1)
                                {
                                    anyDriverHas2OrMoreMajorViolationsInPast3Years = true;
                                    if (isEndorsement) //added 7/26/2019
                                    {
                                        if (newEndorsementDriver)
                                        {
                                            anyNewEndorsementDriverHas2OrMoreMajorViolationsInPast3Years = true;
                                        }
                                    }
                                }                                    
                            }
                        }
                    }
                }

                if (isEndorsement) //added IF 7/26/2019 for Endorsements; original logic in ELSE
                {
                    if (hasNewEndorsementDriverViolations || hasNewEndorsementDriverLosses) //don't validate any pre-existing info if there are no new driver violations or losses
                    {
                        //note: also don't trigger any validations if new drivers didn't contribute to the condition total
                        Int32 currentCounter = PolicyAtFault_Losses_CountInPas3Years_Endorsement_NewDriver + PolicyAtFault_Losses_CountInPas3Years_Endorsement_ExistingDriver_PreExisting; //need to reset accordingly before each new condition check
                        if (PolicyAtFault_Losses_CountInPas3Years_Endorsement_NewDriver > 0 && currentCounter > 2)
                        {
                            string msg = "Policy exceeds the number of allowable at fault accidents. Contact Underwriting for possible exception approval.";
                            valList.Add(new ObjectValidation.ValidationItem(msg, PolicyAtFaultLossesInPast3YearsExceeeded));
                        }

                        currentCounter = PolicyAtFault_Losses_CountInPas5Years_Endorsement_NewDriver + TotalViolationCountInPast5Years_Endorsement_NewDriver + PolicyAtFault_Losses_CountInPas5Years_Endorsement_ExistingDriver_PreExisting + TotalViolationCountInPast5Years_Endorsement_ExistingDriver_PreExisting; //need to reset accordingly before each new condition check
                        if (PolicyAtFault_Losses_CountInPas5Years_Endorsement_NewDriver + TotalViolationCountInPast5Years_Endorsement_NewDriver > 0 && currentCounter > 4)
                        {
                            string msg = "Drivers exceed the number of allowable violations and/or accidents. Contact Underwriting for possible exception approval.";
                            valList.Add(new ObjectValidation.ValidationItem(msg, PolicyAtFaultLossesAndOrViolationsInPast5YearsExceeeded));
                        }

                        currentCounter = PolicyMinorViolationsCountInPast3Years_Endorsement_NewDriver + PolicyMinorViolationsCountInPast3Years_Endorsement_ExistingDriver_PreExisting; //need to reset accordingly before each new condition check
                        if (PolicyMinorViolationsCountInPast3Years_Endorsement_NewDriver > 0 && currentCounter > 3)
                        {
                            string msg = "Policy exceed the number of allowable minor violations. Contact Underwriting for possible exception approval.";
                            valList.Add(new ObjectValidation.ValidationItem(msg, PolicyMinorViolationsCountInPast3YearsExceeded));
                        }

                        if (anyNewEndorsementDriverOver20Has3Minorsin3Years)
                        {
                            string msg = "Driver exceeds the number of allowable minor violations. Contact Underwriting for possible exception approval.";
                            valList.Add(new ObjectValidation.ValidationItem(msg, anyDriverOver20Has3Minorsin3YearsExceeded));
                        }

                        if (anyNewEndorsementDriverUNder20Has2Minorsin3Years)
                        {
                            string msg = "Driver exceeds the number of allowable minor violations. Contact Underwriting for possible exception approval.";
                            valList.Add(new ObjectValidation.ValidationItem(msg, anyDriverUNder20Has2Minorsin3YearsExceeded));
                        }

                        if (anyNewEndorsementDriverHas1AtFaulAnd1MajorInPast3Years)
                        {
                            string msg = "Driver exceeds the number of allowable at fault accidents and major violations. Contact Underwriting for possible exception approval.";
                            valList.Add(new ObjectValidation.ValidationItem(msg, anyDriverHas1AtFaulAnd1MajorInPast3YearsExceeded));
                        }

                        if (anyNewEndorsementDriverHas2OrMoreMajorAnd1OrMoreUnacceptableInPast3Years)
                        {
                            string msg = "Driver exceeds the number of allowable major and unacceptable violations. Contact Underwriting for possible exception approval.";
                            valList.Add(new ObjectValidation.ValidationItem(msg, anyDriverHas2OrMoreMajorAnd1OrMoreUnacceptableInPast3YearsExceeded));
                        }

                        if (anyNewEndorsementDriverHas2OrMoreMajorViolationsInPast3Years)
                        {
                            string msg = "Driver exceeds the number of allowable major violations. Contact Underwriting for possible exception approval.";
                            valList.Add(new ObjectValidation.ValidationItem(msg, anyDriverHas2OrMoreMajorViolationsInPast3YearsExceeded));
                        }

                        currentCounter = PolicyMajorViolationsCountInPast3Years_Endorsement_NewDriver + PolicyMajorViolationsCountInPast3Years_Endorsement_ExistingDriver_PreExisting; //need to reset accordingly before each new condition check
                        if (PolicyMajorViolationsCountInPast3Years_Endorsement_NewDriver > 0 && currentCounter > 2)
                        {
                            string msg = "Policy exceeds the number of allowable major violations. Contact Underwriting for possible exception approval.";
                            valList.Add(new ObjectValidation.ValidationItem(msg, PolicyMajorViolationsCountInPast3YearsExceeded2));
                        }

                        //note: validation below could just check newDriver stuff since it's looking for anything greater than 0, but written the same as other validations for consistency in case a change is needed
                        currentCounter = PolicyUnacceptableViolationsCountInPast5Years_Endorsement_NewDriver + PolicyUnacceptableViolationsCountInPast5Years_Endorsement_ExistingDriver_PreExisting; //need to reset accordingly before each new condition check
                        if (PolicyUnacceptableViolationsCountInPast5Years_Endorsement_NewDriver > 0 && currentCounter > 0)
                        {
                            string msg = "Policy exceeds the number of allowable unacceptable violations. Contact Underwriting for possible exception approval.";
                            valList.Add(new ObjectValidation.ValidationItem(msg, PolicyUnacceptableViolationsCountInPast5YearsExceeded));
                        }
                    }
                }
                else
                {
                    if (PolicyAtFault_Losses_CountInPas3Years > 2) // Bug 5584 changed from 1 to 2 Matt A - 9-17-2015
                    {
                        string msg = "Policy exceeds the number of allowable at fault accidents. Contact Underwriting for possible exception approval.";
                        valList.Add(new ObjectValidation.ValidationItem(msg, PolicyAtFaultLossesInPast3YearsExceeeded));
                    }

                    if (PolicyAtFault_Losses_CountInPas5Years + TotalViolationCountInPast5Years > 4)
                    {
                        string msg = "Drivers exceed the number of allowable violations and/or accidents. Contact Underwriting for possible exception approval.";
                        valList.Add(new ObjectValidation.ValidationItem(msg, PolicyAtFaultLossesAndOrViolationsInPast5YearsExceeeded));
                    }

                    if (PolicyMinorViolationsCountInPast3Years > 3)
                    {
                        string msg = "Policy exceed the number of allowable minor violations. Contact Underwriting for possible exception approval.";
                        valList.Add(new ObjectValidation.ValidationItem(msg, PolicyMinorViolationsCountInPast3YearsExceeded));
                    }

                    if (anyDriverOver20Has3Minorsin3Years)
                    {
                        string msg = "Driver exceeds the number of allowable minor violations. Contact Underwriting for possible exception approval.";
                        valList.Add(new ObjectValidation.ValidationItem(msg, anyDriverOver20Has3Minorsin3YearsExceeded));
                    }

                    if (anyDriverUNder20Has2Minorsin3Years)
                    {
                        string msg = "Driver exceeds the number of allowable minor violations. Contact Underwriting for possible exception approval.";
                        valList.Add(new ObjectValidation.ValidationItem(msg, anyDriverUNder20Has2Minorsin3YearsExceeded));
                    }

                    if (anyDriverHas1AtFaulAnd1MajorInPast3Years)
                    {
                        string msg = "Driver exceeds the number of allowable at fault accidents and major violations. Contact Underwriting for possible exception approval.";
                        valList.Add(new ObjectValidation.ValidationItem(msg, anyDriverHas1AtFaulAnd1MajorInPast3YearsExceeded));
                    }

                    if (anyDriverHas2OrMoreMajorAnd1OrMoreUnacceptableInPast3Years)
                    {
                        string msg = "Driver exceeds the number of allowable major and unacceptable violations. Contact Underwriting for possible exception approval.";
                        valList.Add(new ObjectValidation.ValidationItem(msg, anyDriverHas2OrMoreMajorAnd1OrMoreUnacceptableInPast3YearsExceeded));
                    }

                    if (anyDriverHas2OrMoreMajorViolationsInPast3Years)
                    {
                        string msg = "Driver exceeds the number of allowable major violations. Contact Underwriting for possible exception approval.";
                        valList.Add(new ObjectValidation.ValidationItem(msg, anyDriverHas2OrMoreMajorViolationsInPast3YearsExceeded));
                    }

                    if (PolicyMajorViolationsCountInPast3Years > 2)
                    {
                        string msg = "Policy exceeds the number of allowable major violations. Contact Underwriting for possible exception approval.";
                        valList.Add(new ObjectValidation.ValidationItem(msg, PolicyMajorViolationsCountInPast3YearsExceeded2));
                    }

                    if (PolicyUnacceptableViolationsCountInPast5Years > 0)
                    {
                        string msg = "Policy exceeds the number of allowable unacceptable violations. Contact Underwriting for possible exception approval.";
                        valList.Add(new ObjectValidation.ValidationItem(msg, PolicyUnacceptableViolationsCountInPast5YearsExceeded));
                    }
                }                
            }
            return valList;
        }
    }
}