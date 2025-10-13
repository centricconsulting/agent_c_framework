using System.Linq;
using IFM.PrimativeExtensions;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common
{
    public class ApplicantListValidator
    {
        public const string ValidationListID = "{9F91879C-2588-4A54-9F28-15D55A4779CE}";

        public const string ApplicantsMissing = "{534EAC18-0562-4D9B-80A5-B1448EF26C84}";

        public static Validation.ObjectValidation.ValidationItemList ValidateApplicantList(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                switch (quote.LobType)
                {
                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm:
                        if (quote.Policyholder != null && quote.Policyholder.Name != null && quote.Policyholder.Name.TypeId == "2")
                        {
                            // applicants are required

                            //Updated 9/18/18 for multi state MLW - quote to GoverningStateQuote
                            QuickQuote.CommonMethods.QuickQuoteHelperClass qqh = new QuickQuote.CommonMethods.QuickQuoteHelperClass();
                            var subQuotes = qqh.MultiStateQuickQuoteObjects(ref quote);
                            if (subQuotes != null)
                            {
                                var stateType = quote.QuickQuoteState;
                                var GoverningStateQuote = subQuotes.FirstOrDefault(x => x.QuickQuoteState == stateType);
                                if (GoverningStateQuote == null)
                                {
                                    GoverningStateQuote = subQuotes.GetItemAtIndex(0);
                                }
                                if (GoverningStateQuote.Applicants == null || GoverningStateQuote.Applicants.Any() == false)
                                {
                                    //This evals to GoverningStateQuote.Applicants.Count = 1 when quote.Applicants is null - therefore, this doesn't fire with the new code, but did with the old
                                    //TODO: Mary - How to get this to fire?
                                    //To test this create a commercial farm quote - needs to be a commercial farm quote to get the add applicant button and see applicants under policyholders
                                    valList.Add(new ValidationItem("Commercial Policyholders require at least one Applicant", ApplicantsMissing));
                                }
                            }

                            //if (quote.Applicants == null || quote.Applicants.Any() == false)
                            //{
                            //    valList.Add(new ValidationItem("Commercial Policyholders require at least one Applicant", ApplicantsMissing));
                            //}
                        }
                        break;

                    default:
                        break;
                }
            }

            return valList;
        }
    }
}