using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using IFM.VR.Common.Underwriting;
using IFM.VR.Common.Underwriting.LOB;

using QuickQuote.CommonMethods;
using QuickQuote.CommonObjects;

using UnderwritingQuestion = IFM.VR.Common.UWQuestions.VRUWQuestion;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.DFR
{
    public class UnderwritingQuestionValidator
    {
        public const string ValidationListID = "{D4197454-0316-4D96-B829-1CD11152BEC5}";
        public const string QuoteIsNull = "{87D7B94F-36D4-4AFD-8B1F-9408C9CD0616}";
        public const string NoLocations = "{48265284-EFF1-4BEF-A98A-7A551558ED3E}";

        public const string QUESTION27_NO_ROUTE_TO_UW = "{F61BEC69-8C0E-4B1A-829F-38110E802B02}";
        public const string QUESTION28_YES_ROUTE_TO_UW = "{8CD9D24D-7574-498B-BBB8-D9596DEFD397}";
        public const string QUESTION29_YES_ROUTE_TO_UW = "{D053752E-3F34-4497-AE2C-4C078B13C4B4}";

        private static IUnderwritingQuestionsService _questionService;

        static UnderwritingQuestionValidator()
        {
            _questionService = UnderwritingQuestionServiceFactory.BuildFor(QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal);
        }

        public static Validation.ObjectValidation.ValidationItemList ValidateRouteToUnderwriting(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote is null)
            {
                valList.Add(new ObjectValidation.ValidationItem("Quote is null", QuoteIsNull));
            }
            else if (quote.Locations?.Any() == false)
            {
                // no location
                valList.Add(new ObjectValidation.ValidationItem("No locations", NoLocations));
            }
            else if (valType == ValidationItem.ValidationType.appRate || valType == ValidationItem.ValidationType.issuance)
            {//can be changed to a switch to take advantage of pattern matching if logic changes require nesting

                var searchCriteria = new UnderwritingQuestionRequest()
                {
                    LobType = quote.LobType,
                    GoverningState = quote.QuickQuoteState,
                    Quote = quote,
                    TypeFilter = UnderwritingQuestionRequest.QuestionTypeFilter.ExcludeUnmapped |
                                 UnderwritingQuestionRequest.QuestionTypeFilter.MinimumEffectiveDate |
                                 UnderwritingQuestionRequest.QuestionTypeFilter.Subset,
                    MinimumEffectiveDate = Convert.ToDateTime(quote.EffectiveDate),
                    SubsetToFind_QuestionNumbers = new int[] { 27, 28, 29 }
                };

                var answers = _questionService.GetQuestions(searchCriteria);

                if (answers?.Any() == true)
                {
                    UnderwritingQuestion uw27 = answers.FirstOrDefault(q => q.QuestionNumber == 27),
                                         uw28 = answers.FirstOrDefault(q => q.QuestionNumber == 28),
                                         uw29 = answers.FirstOrDefault(q => q.QuestionNumber == 29);

                    if(uw27?.QuestionAnswerNo == true)
                        valList.Add(new ObjectValidation.ValidationItem("UW Question #27 requires underwriting approval.", QUESTION27_NO_ROUTE_TO_UW));
                    if (uw28?.QuestionAnswerYes == true)
                        valList.Add(new ObjectValidation.ValidationItem("UW Question #28 requires underwriting approval.", QUESTION28_YES_ROUTE_TO_UW));
                    if (uw29?.QuestionAnswerYes == true)
                        valList.Add(new ObjectValidation.ValidationItem("UW Question #29 requires underwriting approval.", QUESTION29_YES_ROUTE_TO_UW));
                    
                    //if ever desired ...
                    //foreach (var item in answers)
                    //{
                    //    if (item.ReferToUnderwritingOnNo && item.QuestionAnswerNo)
                    //        valList.Add(new ObjectValidation.ValidationItem($"UW Question #{item.QuestionNumber} requires underwriting approval.", QUESTION27_NO_ROUTE_TO_UW));
                    //    if (item.ReferToUnderwritingOnYes && item.QuestionAnswerYes)
                    //        valList.Add(new ObjectValidation.ValidationItem($"UW Question #{item.QuestionNumber} requires underwriting approval.", QUESTION27_NO_ROUTE_TO_UW));
                    //}
                }
            }

            return valList;
        }
    }
}