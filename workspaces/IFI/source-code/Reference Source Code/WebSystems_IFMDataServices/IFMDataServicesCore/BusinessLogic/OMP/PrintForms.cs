using IFM.DataServicesCore.CommonObjects.OMP;
using System;
using System.Collections.Generic;
using System.Linq;
using DCO = Diamond.Common.Objects;
using IFM.PrimitiveExtensions;
using System.Text.RegularExpressions;
using Diamond.Business.ThirdParty.AssureSignAPI;

namespace IFM.DataServicesCore.BusinessLogic.OMP
{
    public static class PrintForms
    {
        private static DateTime FiservBillingFormsCutoverDate = AppConfig.FiservBillingFormsStartDate;

        private static Dictionary<string, string> FiservForms = new Dictionary<string, string>()
        {
            {"PREMIUM AUDIT NOTICE - POLICY CANCELLED AUDPREM-INS_AP", "AUDPREM-INS_AP"},
            {"CREDIT CARD REFUND LETTERS CCREF", "CCREF"},
            {"COMMERCIAL MORTGAGEE INVOICE CONMINV-MT", "CONMINV-MT"},
            {"COMMERCIAL MORTGAGEE INVOICE CONMINV-MT-AI", "CONMINV-MT-AI"},
            {"SIMPLE PAY NOTIFICATION EFTNOT", "EFTNOT"},
            {"ADVANCE NOTICE OF CANCELLATION - (ACH RETURN) EFTNSF-INS", "EFTNSF-INS"},
            {"FINAL CANCELLATION - NON PAYMENT COPY FNNP-AI", "FNNP-AI"},
            {"FINAL CANCELLATION - NON PAYMENT COPY FNNP-INS", "FNNP-INS"},
            {"FINAL CANCELLATION - NON PAYMENT MORTGAGEE COPY FNNP-MTG", "FNNP-MTG"},
            {"FINAL EXPIRATION - NON PAYMENT COPY FNRNW-INS", "FNRNW-INS"},
            {"FINAL EXPIRATION - NON PAYMENT MORTGAGEE COPY FNRNW-MTG", "FNRNW-MTG"},
            {"INVOICE -INSURED COPY INV-INS", "INV-INS"},
            {"INVOICE - MORTGAGEE COPY INV-MTG", "INV-MTG"},
            {"ADVANCE NOTICE OF CANCELLATION -MORTGAGEE COPY (LEGAL NOTICE) LN-MTG", "LN-MTG"},
            {"ADVANCE NOTICE OF CANCELLATION - NSF LNNSF-INS", "LNNSF-INS"},
            {"ADVANCE NOTICE OF CANCELLATION - NSF LNNSF-MTG", "LNNSF-MTG"},
            {"ADVANCE NOTICE OF CANCELLATION - REOCCURING CREDIT CARD RETURN LNRCCRET-INS", "LNRCCRET-INS"},
            {"INSURED COLLECTION LETTER LTR COLLECTION INSURED 0100", "LTR COLLECTION INSURED 0100"},
            {"INSURED COLLECTION LETTER LTR COLLECTION AGENCY COPY 0001", "LTR COLLECTION AGENCY COPY 0001"},
            {"ADVANCE NOTICE OF CANCELLATION - INSURED (LEGAL NOTICE) MLN-INS", "MLN-INS"},
            {"RECURRING CREDIT CARD NOTIFICATION RCCNOT", "RCCNOT"},
            {"RECEIPT NOTICE / RESCISSION NOTICE RCP", "RCP"},
            {"EXTENSION OF LEGAL NOTICE OF CANCELLATION DATE RCP-EXT", "RCP-EXT"},
            {"RENEWAL EXPIRATION NOTICE - INSURED RNW-INS", "RNW-INS"},
            {"RENEWAL EXPIRATION NOTICE - MORTGAGEE BILLED RNW-MTG", "RNW-MTG"},
            {"NOTICE OF CANCELLATION - UNDERWRITING UW-NOT-1", "UW-NOT-1"},
            {"NOTICE OF NON-RENEWAL - UNDERWRITING UW-NOT-3", "UW-NOT-3"},
            {"NOTICE OF CANCELLATION - UNDERWRITING UW-NOT-IL-01", "UW-NOT-IL-01"},
            {"NOTICE OF NON-RENEWAL - UNDERWRITING UW-NOT-IL-03", "UW-NOT-IL-03"},
            {"NOTICE OF CANCELLATION - UNDERWRITING UW-NOT-IN-01", "UW-NOT-IN-01"},
            {"NOTICE OF CANCELLATION - UNDERWRITING (MVR OR CLUE) UW-NOT-IN-02", "UW-NOT-IN-02"},
            {"NOTICE OF NON-RENEWAL - UNDERWRITING UW-NOT-IN-03", "UW-NOT-IN-03"},
            {"NOTICE OF NON-RENEWAL - UNDERWRITING (MVR) UW-NOT-IN-04", "UW-NOT-IN-04"},
            {"NOTICE OF CANCELLATION - UNDERWRITING REQUIREMENTS - EARS UW-NOT-IN-06", "UW-NOT-IN-06"},
            {"NOTICE OF NON-RENEWAL - UNDERWRITING (EARS) UW-NOT-IN-07", "UW-NOT-IN-07"},
            {"NOTICE OF CANCELLATION - UNDERWRITING (MVR OR CLUE) UW-NOT-IL-02", "UW-NOT-IL-02"},
            {"NOTICE OF NON-RENEWAL - UNDERWRITING (MVR) UW-NOT-IL-04", "UW-NOT-IL-04"},
            {"NOTICE OF CANCELLATION - UNDERWRITING REQUIREMENTS - EARS UW-NOT-IL-6", "UW-NOT-IL-6"},
            {"NOTICE OF NON-RENEWAL - UNDERWRITING (EARS) UW-NOT-IL-7", "UW-NOT-IL-7"},
            {"NOTICE OF CANCELLATION - UNDERWRITING UW-NOT-OH-01", "UW-NOT-OH-01"},
            {"NOTICE OF NON-RENEWAL - UNDERWRITING UW-NOT-OH-03", "UW-NOT-OH-03"},
            {"PPA New Business Documents", "PPA_NB"},
            {"PPA Renewals Documents", "PPA_REN"},
            {"PPA Endorsement Documents", "PPA_END"},
            {"Invoice -Insured Copy ABINV-INS", "ABINV-INS"},
            {"Electronic Funds Transfer Notification ABEFTNOT", "ABEFTNOT"},
            {"Recurring Credit Card Notification ABRCCNOT", "ABRCCNOT"}
        };

        private static bool HasFiservReplacementForm(DCO.Printing.PrintForm DiamondForm)
        {
            if (FiservForms.ContainsValue(DiamondForm.FormNumber.ToUpper()))
            {
                return true;
            }
            return false;
        }

        private static bool IsFiservForm(DCO.Printing.PrintForm DiamondForm)
        {
            if (DiamondForm.UnitDescription == "" && DiamondForm.FormVersionId == 0) //most likely will stay blank
            {
                return true;
            }
            return false;
        }

        public static List<PrintForm> GetForms(Int32 PolicyId)
        {
            List<PrintForm> lst = new List<PrintForm>();
            if (DiamondLogin.OMPLogin())
            {
                using (var DS = Insuresoft.DiamondServices.PrintingService.LoadPrintHistory())
                {
                    DS.RequestData.PolicyId = PolicyId;
                    DS.RequestData.PolicyImageNum = -1;
                    using (var forms = DS.Invoke()?.DiamondResponse?.ResponseData?.PrintHistory)
                    {
                        if (forms != null && forms.Any())
                        {
                            var chc = new CommonHelperClass();
                            var formsToIgnore = chc.GetApplicationXMLSetting("PrintForms_FormsToIgnore", "PrintFormsSettings.xml").Split(',');
                            // PrintRecipientId  -  4 = Insured, 2 = Agency, 1 = Company
                            // PrintEventId - 3 = Endorsement Dec, 6 = Invoices, 7 = AutoId Cards 'AndAlso f.PrintEventId <> 6 AndAlso f.PrintEventId <> 7 AndAlso f.PrintEventId <> 3
                            // BatchStatus - 1 = Unprocessed, 2 = Processing, 3 = Error, 4 = Processed, 5 = Export Received, 6 = Error Processing Export, 7 = Completed Export Processing
                            //bool isOKToUseFiservForms = OKToUseFiservForms(); //store in variable so we don't need to calculate this each loop.
                            var myForms = from f in forms where f.PrintRecipients.GetItemAtIndex(0)?.PrintRecipientId == 4 && (f.BatchStatus > 1 || IsFiservForm(f)) select f;
                            foreach (var i in myForms)
                            {
                                if(formsToIgnore.Contains(i.FormNumber) == false)
                                {
                                    if (i.AddedDate.Value >= FiservBillingFormsCutoverDate) //Was form created after cutover date
                                    { //created after cutover date - hide Diamond forms that are now created by Fiserv - For now, we must add in the form number for Fiserv documents
                                        bool addForm = false;
                                        if (IsFiservForm(i)) //Fiserv Billing Form
                                        {
                                            //string myKey = FiservForms.Keys.ToList().Find(x => x.Contains(i.Description));
                                            var myKey = FiservForms.Keys.Where(x => x.ToUpper().Equals(i.Description.ToUpper()));
                                            if (myKey.IsLoaded())
                                            {
                                                if (myKey.Count() > 1)
                                                {
                                                    //Too many results!
                                                    //TODO: DJG - what to do with this?
                                                }
                                                else
                                                {
                                                    if (i.FormNumber.IsNullEmptyOrWhitespace()) //They may update this in the future so that we can send the form number in with the form, in which case we will use that instead of setting here.
                                                    {
                                                        i.FormNumber = FiservForms[myKey.GetItemAtIndex(0)];
                                                    }
                                                }
                                            }
                                            addForm = true;
                                        }
                                        else if (HasFiservReplacementForm(i) == false) //Non-Billing Diamond Form
                                        {
                                            addForm = true;
                                        }

                                        if (addForm)
                                        {
                                            lst.Add(new PrintForm(i));
                                            i.Dispose();
                                        }
                                    }
                                    else
                                    { //created before cutover date - hide fiserv forms
                                        if (IsFiservForm(i) == false)
                                        {
                                            lst.Add(new PrintForm(i));
                                            i.Dispose();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return lst.OrderByDescending(x => x.PrintDate).ToList();
        }

        private static DCO.InsCollection<DCO.Printing.PrintForm> GetDiamondForms(Int32 PolicyId)
        {
            DCO.InsCollection<DCO.Printing.PrintForm> lst = new DCO.InsCollection<DCO.Printing.PrintForm>();

            if (DiamondLogin.OMPLogin())
            {
                using (var DS = Insuresoft.DiamondServices.PrintingService.LoadPrintHistory())
                {
                    DS.RequestData.PolicyId = PolicyId;
                    DS.RequestData.PolicyImageNum = -1;
                    using (var forms = DS.Invoke()?.DiamondResponse?.ResponseData?.PrintHistory)
                    {
                        var sortedForms = forms.OrderByDescending(x => x.AddedDate);
                        //-1 for all forms for the policyId
                        if (sortedForms != null && sortedForms.Any())
                        {
                            // PrintRecipientId  -  4 = Insured, 2 = Agency
                            // PrintEventId - 3 = Endorsement Dec, 6 = Invoices, 7 = AutoId Cards 'AndAlso f.PrintEventId <> 6 AndAlso f.PrintEventId <> 7 AndAlso f.PrintEventId <> 3
                            foreach (var i in from f in sortedForms where f.PrintRecipients.GetItemAtIndex(0)?.PrintRecipientId == 4 select f)
                            {
                                lst.Add(i);
                                i.Dispose();
                            }
                        }
                    }
                }
            }
            return lst;
        }

        public static byte[] GetFormBytes(Int32 PolicyId, Int32 xmlId, string description, string printFormNumber)
        {
            return GetFormBytes(PolicyId, xmlId, description, printFormNumber, description);
        }

        public static byte[] GetFormBytes(Int32 PolicyId, Int32 xmlId, string description, string printFormNumber, string decodedDescription)
        {
            if (DiamondLogin.OMPLogin())
            {
                using (DCO.InsCollection<DCO.Printing.PrintForm> forms = GetDiamondForms(PolicyId))
                {
                    if (forms != null && forms.Any())
                    {
                        DCO.InsCollection<DCO.Printing.PrintForm> lst = new DCO.InsCollection<DCO.Printing.PrintForm>();
                        foreach (var i in from f in forms where f.PrintXmlId == xmlId && DoDescriptionsMatch(f.Description.TrimEnd(), description, decodedDescription) && f.PolicyFormNum == Convert.ToInt32(printFormNumber.Trim()) select f)
                        {
                            lst.Add(i);
                            // only doing one at a time
                            using (var DS = Insuresoft.DiamondServices.PrintingService.ReprintJob())
                            {
                                DS.RequestData.PolicyId = PolicyId;
                                DS.RequestData.PolicyImageNum = lst[0].PolicyImageNum; //get the imageNum from the form
                                DS.RequestData.PrintForms = lst;
                                return DS.Invoke()?.DiamondResponse?.ResponseData?.Data;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static byte[] GetFormBytesByPrintJobId(Int32 PolicyId, Int32 printJobId, string description, string printFormNumber, string decodedDescription)
        {
            if (DiamondLogin.OMPLogin())
            {
                using (DCO.InsCollection<DCO.Printing.PrintForm> forms = GetDiamondForms(PolicyId))
                {
                    if (forms != null && forms.Any())
                    {
                        DCO.InsCollection<DCO.Printing.PrintForm> lst = new DCO.InsCollection<DCO.Printing.PrintForm>();
                        foreach (var i in from f in forms where f.PrintJobId == printJobId && DoDescriptionsMatch(f.Description.TrimEnd(), description, decodedDescription) && f.PolicyFormNum == Convert.ToInt32(printFormNumber.Trim()) select f)
                        {
                            lst.Add(i);
                            // only doing one at a time
                            using (var DS = Insuresoft.DiamondServices.PrintingService.ReprintJob())
                            {
                                DS.RequestData.PolicyId = PolicyId;
                                DS.RequestData.PolicyImageNum = lst[0].PolicyImageNum; //get the imageNum from the form
                                DS.RequestData.PrintForms = lst;
                                return DS.Invoke()?.DiamondResponse?.ResponseData?.Data;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private static bool DoDescriptionsMatch(string formDescription, string userDescription, string userDecodedDescription)
        {
            formDescription = formDescription.Trim();
            userDescription = userDescription.Trim();
            userDecodedDescription = userDecodedDescription.Trim();

            userDescription = userDescription.Replace("_-_", "?");
            userDecodedDescription = userDecodedDescription.Replace("_-_", "?");

            Wildcard wildcard1 = new Wildcard(userDescription, RegexOptions.IgnoreCase);
            Wildcard wildcard2 = new Wildcard(userDecodedDescription, RegexOptions.IgnoreCase);

            if (wildcard1.IsMatch(formDescription) || wildcard2.IsMatch(formDescription))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}