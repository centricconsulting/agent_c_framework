using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.Common
{
    [TestClass]
    public class ApplicantListValidatorTests : VRQQLibBase
    {
        [TestMethod]
        public void ApplicantListTest()
        {
            foreach (var lobType in GetLobTypeList())
            {
                var q = GetNewQuickQuote();
                q.LobType = lobType;

                if (lobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                {
                    // test no error with no applicants when policyholder is of personal type
                    q.Policyholder.Name.TypeId = "1";
                    var valItem = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.ApplicantListValidator.ValidateApplicantList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                    Assert.IsFalse(valItem.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.ApplicantListValidator.ApplicantsMissing));

                    // commercial policyholders require applicants - NOTE: personal do to but they are added automatically
                    q.Policyholder.Name.TypeId = "2";
                    valItem = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.ApplicantListValidator.ValidateApplicantList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                    Assert.IsTrue(valItem.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.ApplicantListValidator.ApplicantsMissing));

                    // make sure it doesn't complain if there are applicants
                    q.Policyholder.Name.TypeId = "2";
                    q.Applicants = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteApplicant>();
                    q.Applicants.Add(new QuickQuote.CommonObjects.QuickQuoteApplicant());
                    valItem = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.ApplicantListValidator.ValidateApplicantList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                    Assert.IsFalse(valItem.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.ApplicantListValidator.ApplicantsMissing));
                }
                else
                {
                    // test no error with no applicants when policyholder is of personal type
                    q.Policyholder.Name.TypeId = "1";
                    var valItem = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.ApplicantListValidator.ValidateApplicantList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                    Assert.IsFalse(valItem.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.ApplicantListValidator.ApplicantsMissing));

                    // commercial policyholders require applicants - NOTE: personal do to but they are added automatically
                    q.Policyholder.Name.TypeId = "2";
                    valItem = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.ApplicantListValidator.ValidateApplicantList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                    Assert.IsFalse(valItem.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.ApplicantListValidator.ApplicantsMissing));

                    // make sure it doesn't complain if there are applicants
                    q.Policyholder.Name.TypeId = "2";
                    q.Applicants = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteApplicant>();
                    q.Applicants.Add(new QuickQuote.CommonObjects.QuickQuoteApplicant());
                    valItem = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.ApplicantListValidator.ValidateApplicantList(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                    Assert.IsFalse(valItem.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.ApplicantListValidator.ApplicantsMissing));
                }
            }
        }
    }
}