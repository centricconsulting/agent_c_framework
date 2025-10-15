using IFM.DataServicesCore.BusinessLogic.OMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCO = Diamond.Common.Objects;
using DCS = Diamond.Common.Services;
using IDS = Insuresoft.DiamondServices;

namespace IFM.DataServicesCore.BusinessLogic.Diamond
{
    public static class Print
    {
        //formcategorytype_id dscr
        //0	N/A
        //1	Declarations
        //2	Invoice
        //3	Advance Notice of Cancellation/Expiration
        //4	Final Cancellation/Expiration Notice
        //5	Letter Payment Option Letter Welcome Letter
        //6	Proxy Letter
        //7	Notice of Receipt of Payment
        //8	Quotes
        //9	Applications

        public static byte[] GetSpecificPrintFormBytes(string policyNumber, string formCategories)
        {
            DCO.InsCollection<DCO.Printing.PrintForm> myPrintForms = new DCO.InsCollection<DCO.Printing.PrintForm>();
            var DS = new DataServicesCore.BusinessLogic.Diamond.Policy.PolicyImage();
            var policy = DS.GetPolicyIDAndImageNumByPolicyNumber(policyNumber);

            if (policy?.PolicyId > 0 && policy?.PolicyImageNum > 0)
            {
                int policyImageNum = 0;
                policyImageNum = policy.PolicyImageNum;

                return GetSpecificPrintFormBytes(policy.PolicyId, ref policyImageNum, formCategories);
            }
            else
            {
                return null;
            }
        }

        public static byte[] GetSpecificPrintFormBytes(int policyID, ref int policyImageNum, string formCategories)
        {
            DCO.InsCollection<DCO.Printing.PrintForm> myPrintForms = new DCO.InsCollection<DCO.Printing.PrintForm>();
            byte[] printBytes = null;

            if (policyID > 0 && policyImageNum > 0)
            {
                myPrintForms = GetSpecificPrintForms(policyID, formCategories);
                if (myPrintForms?.Count > 0)
                {
                    if(myPrintForms[0].PolicyImageNum != policyImageNum)
                    {
                        policyImageNum = myPrintForms[0].PolicyImageNum;
                    }

                    printBytes = GetByteArrayForDiamondPrintForms(policyID, policyImageNum, myPrintForms);
                }
            }

            if (printBytes?.Length > 0)
            {
                return printBytes;
            }
            else
            {
                return null;
            }
        }

        public static DCO.InsCollection<DCO.Printing.PrintForm> GetSpecificPrintForms(int policyID, string formCategories)
        {
            DCO.InsCollection<DCO.Printing.PrintForm> myPrintForms = new DCO.InsCollection<DCO.Printing.PrintForm>();

            if (String.IsNullOrWhiteSpace(formCategories) == false && policyID > 0)
            {
                formCategories = formCategories.Replace(" ", "");
                string[] formCategoriesSplit = formCategories.Split(',');

                using (var DS = IDS.PrintingService.LoadPrintHistory())
                {
                    DS.RequestData.PolicyId = policyID;
                    DS.RequestData.PolicyImageNum = -1;
                    DS.RequestData.ReturnSpecificFormCategories = formCategories;
                    var ph = DS.Invoke()?.DiamondResponse?.ResponseData?.PrintHistory;
                    if (ph != null && ph.Count > 0)
                    {
                        foreach (DCO.Printing.PrintForm printForm in ph)
                        {
                            if (printForm.FormCategoryTypeId == 0)
                            {
                                if(formCategoriesSplit.Contains("6"))
                                {
                                    if(printForm.FormNumber.ToUpper().Contains("PROXY"))
                                    {
                                        myPrintForms.Add(printForm);
                                    }
                                }
                                if(formCategoriesSplit.Contains("9"))
                                {
                                    if ((printForm.FormNumber.ToUpper().Contains("ACORD") || printForm.FormNumber.ToUpper().Contains("APP")))
                                    {
                                        myPrintForms.Add(printForm);
                                    }
                                }
                            }
                            else
                            {
                                myPrintForms.Add(printForm);
                            }
                        }
                    }
                }
            }

            myPrintForms = RemoveDuplicatePrintForms(myPrintForms);

            if(myPrintForms?.Count > 0)
            {
                return myPrintForms;
            }
            else
            {
                return null;
            }
        }

        public static DCO.InsCollection<DCO.Printing.PrintForm> GetSpecificPrintForms(string policyNumber, string formCategories)
        {
            DCO.InsCollection<DCO.Printing.PrintForm> myPrintForms = new DCO.InsCollection<DCO.Printing.PrintForm>();

            if (String.IsNullOrWhiteSpace(formCategories) == false && String.IsNullOrWhiteSpace(policyNumber) == false)
            {
                var DS = new DataServicesCore.BusinessLogic.Diamond.Policy.PolicyImage();
                var policy = DS.GetPolicyIDAndImageNumByPolicyNumber(policyNumber);
                if (policy?.PolicyId > 0)
                {
                    myPrintForms = GetSpecificPrintForms(policy.PolicyId, formCategories);
                }
            }

            if(myPrintForms?.Count > 0)
            {
                return myPrintForms;
            }
            else
            {
                return null;
            }
        }

        public static byte[] GetByteArrayForDiamondPrintForms(int policyID, int policyImageNum, DCO.InsCollection<DCO.Printing.PrintForm> diamondPrintForms)
        {
            byte[] formData = null;
            if(policyID > 0 && policyImageNum > 0 && diamondPrintForms?.Count > 0)
            {
                using (var DS = IDS.PrintingService.ReprintJob())
                {
                    DS.RequestData.PolicyId = policyID;
                    DS.RequestData.PolicyImageNum = policyImageNum;
                    DS.RequestData.PrintForms = diamondPrintForms;
                    var responseData = DS.Invoke()?.DiamondResponse?.ResponseData;
                    formData = responseData?.Data;
                }
            }

            if(formData?.Length > 0)
            {
                return formData;
            }
            else
            {
                return null;
            }
        }

        private static DCO.InsCollection<DCO.Printing.PrintForm> RemoveDuplicatePrintForms(DCO.InsCollection<DCO.Printing.PrintForm> myPrintForms) //Ran into an issue where PROD policy had two APP90 forms and the Diamond Printing service failed when we sent both of them in. This should fix it.
        {
            DCO.InsCollection<DCO.Printing.PrintForm> FormsToKeep = new DCO.InsCollection<DCO.Printing.PrintForm>();
            List<string> testedFormNums = new List<string>();

            foreach (var pf in myPrintForms)
            {
                if (testedFormNums.Contains(pf.FormNumber) == false) //We don't want to re-test the same form number as we should have already figured out which one to keep.
                {
                    testedFormNums.Add(pf.FormNumber);
                    DCO.Printing.PrintForm formToKeep = null;
                    var forms = myPrintForms.Where(x => x.FormNumber == pf.FormNumber);
                    if (forms != null && forms.Count() > 1) //See if there are duplicates
                    {
                        formToKeep = forms.MaxBy(x => x.AddedDate);
                        FormsToKeep.Add(formToKeep);
                    }
                    else
                    {
                        if (FormsToKeep.Contains(pf) == false)
                        {
                            FormsToKeep.Add(pf);
                        }
                    }
                }
            }
            return FormsToKeep;
        }
    }
}
