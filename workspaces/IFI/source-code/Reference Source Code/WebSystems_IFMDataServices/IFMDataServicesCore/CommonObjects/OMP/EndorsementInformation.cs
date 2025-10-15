using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFM.DataServicesCore.BusinessLogic;
using IFM.PrimitiveExtensions;
using QuickQuote.CommonMethods;
using DCO = Diamond.Common.Objects;

#if DEBUG

using System.Diagnostics;


#endif

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class EndorsementInformation : ModelBase
    {

        public DateTime MinimumEffectiveDate;
        public DateTime MaximumEffectiveDate;
        public bool HasPendingEndorsement;
        public decimal CurrentRate;
        public decimal EndorsementRate;
        public decimal ChangeInRate;
        public QuickQuote.CommonObjects.QuickQuoteEndorsementForPolicyIdAndTransactionDateInput.EndorsementOriginTypes EndorsementOriginType;
        public DateTime TransactionEffectiveDate;
        public DateTime TransactionExpirationDate;
        public int EndorsementPolicyImageNum;
        public DateTime DateAdded;
        public DateTime DateModified;

        public bool ErrorGettingEndorsementInformation;
        public string ErrorMessageForGettingEndorsementInformation;
        public string ErrorSqlQuery;

        public EndorsementInformation() { }

        public EndorsementInformation(DCO.Policy.Image image)
        {
            if (image != null)
            {
                QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo qqPLI = null;

                CurrentRate = image.FullTermPremium;
                HasPendingEndorsement = QuickQuoteHelperClass.HasPendingEndorsementImage(image.PolicyNumber, image.PolicyId, ref qqPLI, ref ErrorGettingEndorsementInformation);

                if (qqPLI != null)
                {
                    ErrorMessageForGettingEndorsementInformation = qqPLI.ErrorMessage;
                    ErrorSqlQuery = qqPLI.SqlQuery;
                }
                else
                {
                    ErrorMessageForGettingEndorsementInformation = "Policy lookup object is null.";
                }

                CommonHelperClass chc = new CommonHelperClass();
                MinimumEffectiveDate = DateTime.Now.AddDays(-(chc.GetApplicationXMLSettingForInteger("Endorsements_TransactionDate_DaysBackAllowed", "Endorsements.xml")));
                MaximumEffectiveDate = DateTime.Now.AddDays(chc.GetApplicationXMLSettingForInteger("Endorsements_TransactionDate_DaysForwardAllowed", "Endorsements.xml"));
                if (image.EffectiveDate > MinimumEffectiveDate)
                {
                    MinimumEffectiveDate = image.EffectiveDate;
                }
                if (image.ExpirationDate < MaximumEffectiveDate)
                {
                    MaximumEffectiveDate = image.ExpirationDate.AddDays(-1);
                }

                if (HasPendingEndorsement)
                {
                    TransactionEffectiveDate = qqPLI.TransactionEffectiveDate.IsDate() == true ? qqPLI.TransactionEffectiveDate.ToDateTime() : DateTime.MinValue;
                    TransactionExpirationDate = qqPLI.TransactionExpirationDate.IsDate() == true ? qqPLI.TransactionExpirationDate.ToDateTime() : DateTime.MinValue;
                    DateAdded = qqPLI.DateAdded.IsDate() == true ? qqPLI.DateAdded.ToDateTime() : DateTime.MinValue;
                    DateModified = qqPLI.DateModified.IsDate() == true ? qqPLI.DateModified.ToDateTime() : DateTime.MinValue;
                    EndorsementPolicyImageNum = qqPLI.PolicyImageNum;
                    ChangeInRate = (decimal)qqPLI.ChangeInFullTermPremium.TryToGetDouble();
                    EndorsementRate = (decimal)qqPLI.FullTermPremium.TryToGetDouble();
                    if(Enum.IsDefined(typeof(QuickQuote.CommonObjects.QuickQuoteEndorsementForPolicyIdAndTransactionDateInput.EndorsementOriginTypes), qqPLI.EndorsementOriginTypeId) == true){
                        EndorsementOriginType = (QuickQuote.CommonObjects.QuickQuoteEndorsementForPolicyIdAndTransactionDateInput.EndorsementOriginTypes)qqPLI.EndorsementOriginTypeId;
                    }
                }
                else
                {
                    TransactionEffectiveDate = DateTime.MinValue;
                    TransactionExpirationDate = DateTime.MinValue;
                    DateAdded = DateTime.MinValue;
                    DateModified = DateTime.MinValue;
                    EndorsementPolicyImageNum = 0;
                    ChangeInRate = 0;
                    EndorsementRate = 0;
                }
            }
            else
            {
#if !DEBUG
                global::IFM.IFMErrorLogging.LogIssue($"Image was null.");
#else
                Debugger.Break();
#endif
            }
        }
    }
}
