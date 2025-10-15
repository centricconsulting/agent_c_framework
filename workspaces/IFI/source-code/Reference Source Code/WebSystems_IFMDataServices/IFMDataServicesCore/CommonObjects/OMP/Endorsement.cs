using IFM.DataServicesCore.CommonObjects.OMP.PPA;
using QuickQuote.CommonObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFM.PrimitiveExtensions;
using System.Deployment.Internal;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class Endorsement : ModelBase
    {
        public string PolicyNumber { get; set; }
        public int PolicyId { get; set; }
        public int PolicyImageNum { get; set; }
        public string TransactionDate { get; set; }
        public string PolicyEffectiveDate { get; set; }
        public string PolicyExpirationDate { get; set; }
        public string OldRate
        {
            get
            {
                if (DifferenceInRate.IsNumeric() && NewRate.IsNumeric())
                {
                    return (NewRate.TryToGetInt32() - DifferenceInRate.TryToGetInt32()).ToString().TryToFormatAsCurrency();
                }
                return null;
            }
        }
        public string NewRate { get; set; }
        public string DifferenceInRate { get; set; }
        public int UserID { get; set; }
        public string Username { get; set; }
        public BillingInformation BillingInformation { get; set; } = new BillingInformation();
        public List<Policyholder> Policyholders { get; set; } = new List<Policyholder>();
        public List<Location> Locations { get; set; } = new List<Location>();
        public List<Driver> Drivers { get; set; } = new List<Driver>();
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public Vehicle ReplacedVehicle { get; set; } = null;
        public ValidationItems ValidationItems { get; set; } = new ValidationItems();
        public EndorsementStatus EndorsementStatus { get; set; } = new EndorsementStatus();
        public EndorsementPolicyCoverages PolicyCoverages { get; set; } = new EndorsementPolicyCoverages();
        public string ErrorMessage { get; set; }
        private EndorsementLOBType _lobType = EndorsementLOBType.NA;
        public EndorsementLOBType LOBType
        {
            get
            {
                if(_lobType == EndorsementLOBType.NA)
                {
                    if (PolicyNumber.IsNotNull())
                    {
                        string LOBAbbr = "";
                        if (PolicyNumber.StartsWith("Q"))
                        {
                            LOBAbbr = PolicyNumber.Substring(1, 3);
                        }
                        else
                        {
                            LOBAbbr = PolicyNumber.Substring(0, 3);
                        }
                        Enum.TryParse<EndorsementLOBType>(LOBAbbr, out _lobType);
                    }
                }
                return _lobType;
            }
        }
        public EndorsementActionType ActionType { get; set; }
        public EndorsementObjectType ObjectType { get; set; }
        public EndorsementTransactionType TransactionType { get; set; }


        public enum EndorsementActionType
        {
            NA,
            Add,
            Edit,
            Delete
        }

        public enum EndorsementObjectType
        {
            NA,
            Driver,
            Vehicle,
            Lienholder,
            MailingAddress,
            LoanLenderInfo,
            PayPlan
        }

        public enum EndorsementTransactionType
        {
            Rate,
            Finalize,
            Delete
        }

        public enum EndorsementLOBType
        {
            NA,
            PPA,
            HOM
        }

        public void SetPropertiesByReferenceVars()
        {
            if(EndorsementStatus.refPolicyImageNum > 0 && PolicyImageNum != EndorsementStatus.refPolicyImageNum)
            {
                PolicyImageNum = EndorsementStatus.refPolicyImageNum;
            }
            ErrorMessage = EndorsementStatus.refErrorMessage;
            EndorsementStatus.PromoteAttempted = EndorsementStatus.refPromoteAttempted;
            EndorsementStatus.PromoteSuccessful = EndorsementStatus.refPromoteSuccessful;
            EndorsementStatus.RateAttempted = EndorsementStatus.refRateAttempted;
            EndorsementStatus.RateSuccessful = EndorsementStatus.refRateSuccessful;
            EndorsementStatus.SaveSuccessful = EndorsementStatus.refSaveSuccessful;
            EndorsementStatus.FinalizeAttempted = EndorsementStatus.refFinalizeAttempted;
            EndorsementStatus.FinalizeSuccessful = EndorsementStatus.refFinalizeSuccessful;
            EndorsementStatus.DeleteEndorsementAttempted = EndorsementStatus.refDeleteEndorsementAttempted;
            EndorsementStatus.DeleteEndorsementSuccessful = EndorsementStatus.refDeleteEndorsementSuccessful;
        }
        public void SetErrorMessageByReferenceVar()
        {
            ErrorMessage = EndorsementStatus.refErrorMessage;
        }

        public void SetQQPolicyCoverages(QuickQuoteObject qqe)
        {
            if (qqe != null && this.PolicyCoverages != null && this.ActionType != EndorsementActionType.Delete)
            {
                switch (this.ObjectType)
                {
                    case EndorsementObjectType.Vehicle:
                        if (this.LOBType == EndorsementLOBType.PPA)
                        {
                            var covs = this.PolicyCoverages;
                            AutoEnhancementSelection(covs, qqe);
                        }
                        else
                        {
                            //Could be used for Commercial Auto if it ever becomes a thing for MP.
                        }
                        break;
                }
            }
        }

        private void AutoEnhancementSelection(EndorsementPolicyCoverages covs, QuickQuoteObject qqe)
        {
            //Most of these items should be blocked by the UI, but lets just make sure everything is good...
            if (covs.hasAutoEnhancement || covs.hasAutoPlusEnhancement)
            {
                if (qqe.HasAutoPlusEnhancement || qqe.HasBusinessMasterEnhancement)
                {
                    //If we already have AutoEnhancement, upgrade to AutoEnhancementPlus if it was selected. Otherwise, do nothing (don't think they can get rid of them in the Endorsement process)
                    if (covs.hasAutoPlusEnhancement && qqe.HasBusinessMasterEnhancement)
                    {
                        qqe.HasAutoPlusEnhancement = true;
                        qqe.HasBusinessMasterEnhancement = false;
                    }
                }
                else
                {
                    if (covs.hasAutoPlusEnhancement && covs.hasAutoEnhancement)
                    {
                        qqe.HasAutoPlusEnhancement = true;
                    }
                    else
                    {
                        if (covs.hasAutoPlusEnhancement) { qqe.HasAutoPlusEnhancement = true; }
                        if (covs.hasAutoEnhancement) { qqe.HasBusinessMasterEnhancement = true; }
                    }
                }
            }
        }

        public Endorsement() { }
        internal Endorsement(string errorMsg)
        {
            this.ErrorMessage = errorMsg;
        }
    }

    [System.Serializable]
    public class ValidationItem : ModelBase
    {
        public string Message { get; set; }
        public string ValidationSeverityType { get; set; }
        public int ValidationSeverityTypeId { get; set; }

        public ValidationItem() { }
        internal ValidationItem(QuickQuoteValidationItem qqVI)
        {
            if (qqVI != null)
            {
                Message = qqVI.Message;
                ValidationSeverityType = qqVI.ValidationSeverityType.ToString();
                if (qqVI.ValidationSeverityTypeId.IsNumeric()) ValidationSeverityTypeId = int.Parse(qqVI.ValidationSeverityTypeId);
            }
        }
    }
    [System.Serializable]
    public class ValidationItems : List<ValidationItem>
    {
        public ValidationItems() { }
        internal ValidationItems(List<QuickQuoteValidationItem> qqVIs)
        {
            foreach(QuickQuoteValidationItem item in qqVIs)
            {
                var vi = new ValidationItem(item);
                this.Add(vi);
            }
        }
    }

    [System.Serializable]
    public class EndorsementStatus
    {
        public int refPolicyImageNum;
        public string refErrorMessage;
        public bool PromoteAttempted { get; set; }
        public bool refPromoteAttempted;
        public bool PromoteSuccessful { get; set; }
        public bool refPromoteSuccessful;
        public bool RateAttempted { get; set; }
        public bool refRateAttempted;
        public bool RateSuccessful { get; set; }
        public bool refRateSuccessful;
        public bool SaveSuccessful { get; set; }
        public bool refSaveSuccessful;
        public bool SaveAttempted { get; set; }
        public bool FinalizeAttempted { get; set; }
        public bool refFinalizeAttempted;
        public bool FinalizeSuccessful { get; set; }
        public bool refFinalizeSuccessful;
        public bool DeleteEndorsementAttempted { get; set; }
        public bool refDeleteEndorsementAttempted;
        public bool DeleteEndorsementSuccessful { get; set; }
        public bool refDeleteEndorsementSuccessful;
        public bool IsNewEndorsement { get; set; }
        public bool NeedsObjectAdded { get; set; }
        public bool CheckForAbondonedObject { get; set; } = false;
    }

    [System.Serializable]
    public class EndorsementPolicyCoverages
    {
        public bool hasAutoEnhancement { get; set; }
        public string AutoEnhancementPremium { get; set; }
        public bool hasAutoPlusEnhancement { get; set; }
        public string AutoPlusEnhancementPremium { get; set; }
    }
}
