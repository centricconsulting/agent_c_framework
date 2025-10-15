using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace VrTests.IFM_VR_Validation.ObjectValidation.AllLines
{
    [TestClass]
    public class EmailValidatorTests : VRQQLibBase
    {
        [TestMethod]
        public void EmailListTests()
        {
            var emailList = new List<QuickQuote.CommonObjects.QuickQuoteEmail>();
            emailList.Add(new QuickQuote.CommonObjects.QuickQuoteEmail());

            // testing emtpy
            var vals = IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.ValidateEmailList(emailList, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.EmailEmpty), "Email Was Empty");

            //testing Invalid
            emailList[0].Address = "5367367";
            vals = IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.ValidateEmailList(emailList, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.EmailInvalid), "Email Was Invalid");

            //testing valid
            emailList[0].Address = "user@site.com";
            vals = IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.ValidateEmailList(emailList, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
            Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.AllLines.EmailValidator.EmailInvalid), "Email Was Invalid");
        }
    }
}