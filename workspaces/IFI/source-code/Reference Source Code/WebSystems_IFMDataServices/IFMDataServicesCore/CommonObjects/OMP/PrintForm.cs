using IFM.PrimitiveExtensions;
using System;
using System.Linq;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class PrintForm : ModelBase
    {
        public Int32 PrintRecipientId { get; set; }// 4 = insured
        public DateTime PrintDate { get; set; }

        //AddedDate
        public string Description { get; set; }
        public Int32 PrintXmlId { get; set; }
        public string FormNumber { get; set; }
        public Int32 PolicyFormNumber { get; set; }
        public Int32 PolicyId { get; set; }
        public Int32 PolicyImageNum { get; set; }
        public Int32 PrintJobId { get; set; }
        public string UnitDescription { get; set; }
        public Int32 VehicleNum { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public Int32 Year { get; set; }

        public string PrintUrl
        {
            get
            {
                if (PrintRecipientId == 4 && Description.IsNullEmptyOrWhitespace() == false)
                {
                    //return $"/omp/print/document?policyId={PolicyId}&printxmlid={PrintXmlId}&description={System.Net.WebUtility.UrlEncode(Description)}&printformnumber={System.Net.WebUtility.UrlEncode(PolicyFormNumber.ToString())}";'
                    var urlDescription = Description;
                    var wildcard = "_-_";
                    CommonHelperClass chc = new CommonHelperClass();
                    var charactersToReplaceWithWildcard = chc.GetApplicationXMLSetting("PrintFormDescriptions_CharactersToMakeAsWildcards", "PrintFormsSettings.xml")?.Split(',');
                    foreach (string thisChar in charactersToReplaceWithWildcard)
                    {
                        urlDescription = urlDescription.Replace(thisChar, wildcard);
                    }

                    if (PrintXmlId > 0)
                    {
                        return $"/omp/print/document/{PolicyId}/{PrintXmlId}/{System.Net.WebUtility.UrlEncode(urlDescription)}/{System.Net.WebUtility.UrlEncode(PolicyFormNumber.ToString())}";
                    }
                    else
                    {
                        return $"/omp/print/documentbyprintjobid/{PolicyId}/{PrintJobId}/{System.Net.WebUtility.UrlEncode(urlDescription)}/{System.Net.WebUtility.UrlEncode(PolicyFormNumber.ToString())}";
                    }
                }
                return null;
            }
            set { }
        }
        public PrintForm() { }

        internal PrintForm(DCO.Printing.PrintForm dForm)
        {
            if (dForm != null)
            {
                this.PrintRecipientId = dForm.PrintRecipients != null && dForm.PrintRecipients.Any() ? dForm.PrintRecipients[0].PrintRecipientId : -1;
                this.PrintDate = dForm.AddedDate.Value;
                this.Description = dForm.Description.TrimEnd();
                this.PrintXmlId = dForm.PrintXmlId;
                this.FormNumber = dForm.FormNumber;
                this.PolicyFormNumber = dForm.PolicyFormNum;
                this.PolicyId = dForm.PolicyId;
                this.PolicyImageNum = dForm.PolicyImageNum;
                this.PrintJobId = dForm.PrintJobId;
                this.UnitDescription = dForm.UnitDescription;
                if (this.Description.Contains("Auto ID Card") || this.Description.ToLower().Contains("identification card"))
                    this.VehicleNum = dForm.UnitDescription.BruteForceInt32(); // Convert.ToInt32(GetNumbers(dForm.UnitDescription));
            }
        }
        //private string GetNumbers(String InputString)
        //{
        //    string Result="";
        //    string Numbers = "0123456789";
        //    int i = 0;

        //    for (i = 0; i < InputString.Length; i++)
        //    {
        //        if (Numbers.Contains(InputString.ElementAt(i)))
        //        {
        //            Result += InputString.ElementAt(i);
        //        }
        //    }
        //    return Result;
        //}
        public override string ToString()
        {
            return Description;
        }
    }
}