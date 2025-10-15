using IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickQuote.CommonMethods;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.Common
{
    [TestClass]
    public class BillingInformationTests : VRQQLibBase
    {
        [TestMethod]
        public void BillingAddressTest()
        {
            QuickQuoteHelperClass qqHelper = new QuickQuoteHelperClass();

            string billTo_Other_id = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, "Other");
            string billTo_Insured_id = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, "Insured");
            string billTo_Agent_id = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, "Agent");

            string method_DirectBill_Id = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId, "Direct Bill");
            string method_AgencyBill_Id = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId, "Agency Bill");

            QuickQuote.CommonObjects.QuickQuoteObject q = new QuickQuote.CommonObjects.QuickQuoteObject();

            // Test all Empty
            q.BillToId = "";
            q.BillingPayPlanId = "";
            q.BillMethodId = "";
            //run validator
            var Validations = BillingInformationValidator.BillingInformationValidation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, "false");
            Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.Billto), "Failed Bill To");
            Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.PayPlan), "Failed Pay Plan");
            Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.Method), "Failed Method");

            //testing Valid
            q.BillToId = billTo_Insured_id;
            q.BillingPayPlanId = "1";
            q.BillMethodId = method_DirectBill_Id;
            //rerun validator
            Validations = BillingInformationValidator.BillingInformationValidation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate,"false");
            Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.Billto), "Failed Bill To");
            Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.PayPlan), "Failed Pay Plan");
            Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.Method), "Failed Method");

            //testing Invalid - must be numeric values
            q.BillToId = "a";
            q.BillingPayPlanId = "a";
            q.BillMethodId = "a";
            //rerun validator
            Validations = BillingInformationValidator.BillingInformationValidation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, "false");
            Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.Billto), "Failed Bill To");
            Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.PayPlan), "Failed Pay Plan");
            Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.Method), "Failed Method");

            // testing invalid combination
            q.BillToId = billTo_Insured_id;
            q.BillingPayPlanId = "20";
            q.BillMethodId = method_AgencyBill_Id;
            //rerun validator
            Validations = BillingInformationValidator.BillingInformationValidation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, "false");
            Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.Billto), "Failed Bill To");
            Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.PayPlan), "Failed Pay Plan");
            Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.Method), "Failed Method");
            Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.Billto_Method_Combination), "Failed Billto/Method Combination");

            // testing invalid combination
            q.BillToId = billTo_Agent_id;
            q.BillingPayPlanId = "1";
            q.BillMethodId = method_DirectBill_Id;
            //rerun validator
            Validations = BillingInformationValidator.BillingInformationValidation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, "false");
            Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.Billto), "Failed Bill To");
            Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.PayPlan), "Failed Pay Plan");
            Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.Method), "Failed Method");
            Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.Billto_Method_Combination), "Failed Billto/Method Combination");
        }

        [TestMethod]
        public void BillingMethod_Payplan_Ids()
        {
            QuickQuoteHelperClass qqHelper = new QuickQuoteHelperClass();

            string billTo_Other_id = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, "Other");
            string billTo_Insured_id = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, "Insured");
            string billTo_Agent_id = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, "Agent");

            string method_DirectBill_Id = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId, "Direct Bill");
            string method_AgencyBill_Id = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId, "Agency Bill");

            QuickQuote.CommonObjects.QuickQuoteObject q = new QuickQuote.CommonObjects.QuickQuoteObject();

            // with agency bill only the following payplans are valid 20,21,22            
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
            q.BillToId = billTo_Agent_id;
            q.BillingPayPlanId = "12";
            q.BillMethodId = method_DirectBill_Id;
            var Validations = BillingInformationValidator.BillingInformationValidation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, "false");
            Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.PayPlan), "Failed Pay Plan");

            q.BillToId = billTo_Agent_id;
            q.BillingPayPlanId = "13";
            q.BillMethodId = method_DirectBill_Id;
            Validations = BillingInformationValidator.BillingInformationValidation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, "false");
            Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.PayPlan), "Failed Pay Plan");

            q.BillToId = billTo_Agent_id;
            q.BillingPayPlanId = "14";
            q.BillMethodId = method_DirectBill_Id;
            Validations = BillingInformationValidator.BillingInformationValidation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, "false");
            Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.PayPlan), "Failed Pay Plan");

            q.BillToId = billTo_Agent_id;
            q.BillingPayPlanId = "20";
            q.BillMethodId = method_AgencyBill_Id;
            Validations = BillingInformationValidator.BillingInformationValidation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, "false");
            Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.PayPlan), "Failed Pay Plan");

            q.BillToId = billTo_Agent_id;
            q.BillingPayPlanId = "21";
            q.BillMethodId = method_AgencyBill_Id;
            Validations = BillingInformationValidator.BillingInformationValidation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, "false");
            Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.PayPlan), "Failed Pay Plan");

            q.BillToId = billTo_Agent_id;
            q.BillingPayPlanId = "22";
            q.BillMethodId = method_AgencyBill_Id;
            Validations = BillingInformationValidator.BillingInformationValidation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, "false");
            Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.PayPlan), "Failed Pay Plan");
        }

        [TestMethod]
        public void Billing_BillTo_Mortgagee()
        {
            QuickQuoteHelperClass qqHelper = new QuickQuoteHelperClass();

            string billTo_Mortgagee_id = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, "Mortgagee");
            string method_DirectBill_Id = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId, "Direct Bill");
            string payplan_AnnUalMTG = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillingPayPlanId, "Annual MTG");

            QuickQuote.CommonObjects.QuickQuoteObject q = new QuickQuote.CommonObjects.QuickQuoteObject();

            // should fail because it doesn't have a mortgagee type AI but has billto set to Mortgagee
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
            q.BillToId = billTo_Mortgagee_id;
            q.BillingPayPlanId = payplan_AnnUalMTG;
            q.BillMethodId = method_DirectBill_Id;
            var Validations = BillingInformationValidator.BillingInformationValidation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, "false");
            Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.Billto), "No AI");

            // should not fail because it has an AI of type Mortgagee
            string AIType_Mortgagee_id = "42";
            q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
            var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
            q.Locations.Add(l);
            l.AdditionalInterests = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteAdditionalInterest>();
            var ai = new QuickQuote.CommonObjects.QuickQuoteAdditionalInterest();
            ai.TypeId = AIType_Mortgagee_id;
            ai.BillTo = true;
            l.AdditionalInterests.Add(ai);
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
            q.BillToId = billTo_Mortgagee_id;
            q.BillingPayPlanId = payplan_AnnUalMTG;
            q.BillMethodId = method_DirectBill_Id;
            Validations = BillingInformationValidator.BillingInformationValidation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, "false");
            Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.Billto), "Has AI");

            // should fail because BillTo is set to false - must have a AI with type of Mortgagee and one BillTo set to true when Billing 'BillTo' is set to mortgagee
            q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
            ai.TypeId = AIType_Mortgagee_id;
            ai.BillTo = false;
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
            q.BillToId = billTo_Mortgagee_id;
            q.BillingPayPlanId = payplan_AnnUalMTG;
            q.BillMethodId = method_DirectBill_Id;
            Validations = BillingInformationValidator.BillingInformationValidation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, "false");
            Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.Billto), "Has AI but no BillTo set true");

            //To have payplan of AnnualMTG you must be using BillTo of Mortgagee
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
            q.BillToId = "1"; // Not Mortgagee
            q.BillingPayPlanId = payplan_AnnUalMTG;
            q.BillMethodId = method_DirectBill_Id;
            Validations = BillingInformationValidator.BillingInformationValidation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, "false");
            Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.PayPlan), "Annual MTG is not valid without BillTo of Mortagee");
        }

        [TestMethod]
        public void BillingAddressTest_BillTo_Other_Requires_Address()
        {
            QuickQuoteHelperClass qqHelper = new QuickQuoteHelperClass();

            string billTo_Other_id = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, "Other");
            string billTo_Insured_id = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, "Insured");
            string billTo_Agent_id = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, "Agent");

            string method_DirectBill_Id = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId, "Direct Bill");
            string method_AgencyBill_Id = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId, "Agency Bill");

            QuickQuote.CommonObjects.QuickQuoteObject q = new QuickQuote.CommonObjects.QuickQuoteObject();

            // billing type of other will require an address to be included
            q.BillToId = billTo_Other_id;

            //rerun validator
            var Validations = BillingInformationValidator.BillingInformationValidation(q, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate, "false");
            Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.BillingFirstName), "Failed First Name");
            Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.BillingLastName), "Failed Last Name");
            Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.BillingAddressStreetAndPoBoxEmpty), "Failed No Street or Po Box Info");
            // no need to test all the address logic
        }

        private readonly string[] BillingPayPlanIds = new string[] { "19", "12", "13", "14", "20", "21", "22" };

        [TestMethod]
        public void BillingMethod_Payplan_Test_EFT_Info_RoutingNumber()
        {
            QuickQuoteHelperClass qqHelper = new QuickQuoteHelperClass();

            string billTo_Other_id = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, "Other");
            string billTo_Insured_id = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, "Insured");
            string billTo_Agent_id = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, "Agent");

            string method_DirectBill_Id = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId, "Direct Bill");
            string method_AgencyBill_Id = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId, "Agency Bill");

            QuickQuote.CommonObjects.QuickQuoteObject q = new QuickQuote.CommonObjects.QuickQuoteObject();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;

            string payplan_EFTMonthly = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillingPayPlanId, "Renewal EFT Monthly 2");

            var valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate; // should create a list and test other val types because these should only happen on final rate

            foreach (var BillingPayPlanId in BillingPayPlanIds)
            {
                // keep in mind that on non EFT payplan any routing number is invalid - these tests below account for that

                // testing routing number - empty
                q.BillToId = billTo_Insured_id;
                q.BillingPayPlanId = BillingPayPlanId;
                q.BillMethodId = method_DirectBill_Id;
                q.EFT_BankRoutingNumber = "";
                q.EFT_BankAccountNumber = "";
                q.EFT_BankAccountTypeId = "";
                q.EFT_DeductionDay = "";
                var Validations = BillingInformationValidator.BillingInformationValidation(q, valType, "true");
                if (BillingPayPlanId == payplan_EFTMonthly)
                {
                    Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.BillingEftRouting), "");
                    Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.BillingEftAccount), "");
                    Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.BillingEftAccountType), "");
                    Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.BillingEftDeduction), "");
                }
                else
                {
                    Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.BillingEftRouting), "");
                    Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.BillingEftAccount), "");
                    Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.BillingEftAccountType), "");
                    Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.BillingEftDeduction), "");
                }

                #region RoutingNumber

                // testing routing number - not numeric
                q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
                q.BillToId = billTo_Insured_id;
                q.BillingPayPlanId = BillingPayPlanId;
                q.BillMethodId = method_DirectBill_Id;
                q.EFT_BankRoutingNumber = "a12345678";
                Validations = BillingInformationValidator.BillingInformationValidation(q, valType, "true");
                Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.BillingEftRouting), "");

                // testing routing number - not 9 digits
                q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
                q.BillToId = billTo_Insured_id;
                q.BillingPayPlanId = BillingPayPlanId;
                q.BillMethodId = method_DirectBill_Id;
                q.EFT_BankRoutingNumber = "12345678"; // not 9 digits
                Validations = BillingInformationValidator.BillingInformationValidation(q, valType, "true");
                Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.BillingEftRouting), "");

                // testing routing number - not a real routing number
                q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
                q.BillToId = billTo_Insured_id;
                q.BillingPayPlanId = BillingPayPlanId;
                q.BillMethodId = method_DirectBill_Id;
                q.EFT_BankRoutingNumber = "123456780";
                Validations = BillingInformationValidator.BillingInformationValidation(q, valType, "true");
                Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.BillingEftRouting), "");

                // testing routing number - should pass - expect of non EFT payplans
                q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;
                q.BillToId = billTo_Insured_id;
                q.BillingPayPlanId = BillingPayPlanId;
                q.BillMethodId = method_DirectBill_Id;
                q.EFT_BankRoutingNumber = "322271627";
                Validations = BillingInformationValidator.BillingInformationValidation(q, valType, "true");
                if (BillingPayPlanId == payplan_EFTMonthly)
                    Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.BillingEftRouting), "");
                else
                    Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.BillingEftRouting), ""); // not valid on all payplans

                #endregion RoutingNumber

                #region Account Number

                // testing Account Number - Not empty
                q.BillToId = billTo_Insured_id;
                q.BillingPayPlanId = BillingPayPlanId;
                q.BillMethodId = method_DirectBill_Id;
                q.EFT_BankAccountNumber = "1234";
                Validations = BillingInformationValidator.BillingInformationValidation(q, valType, "true");
                if (BillingPayPlanId == payplan_EFTMonthly)
                    Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.BillingEftAccount), "");
                else
                    Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.BillingEftAccount), "");

                #endregion Account Number

                #region Bank Account Type

                // testing EFT_BankAccountTypeId - non numeric chars
                q.BillToId = billTo_Insured_id;
                q.BillingPayPlanId = BillingPayPlanId;
                q.BillMethodId = method_DirectBill_Id;
                q.EFT_BankAccountTypeId = "abc";
                Validations = BillingInformationValidator.BillingInformationValidation(q, valType, "true");
                if (BillingPayPlanId == payplan_EFTMonthly)
                    Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.BillingEftAccountType), "");
                else
                    Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.BillingEftAccountType), "");

                // testing EFT_BankAccountTypeId - testing valid
                q.BillToId = billTo_Insured_id;
                q.BillingPayPlanId = BillingPayPlanId;
                q.BillMethodId = method_DirectBill_Id;
                q.EFT_BankAccountTypeId = "2";
                Validations = BillingInformationValidator.BillingInformationValidation(q, valType, "true");
                if (BillingPayPlanId == payplan_EFTMonthly)
                    Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.BillingEftAccountType), "");
                else
                    Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.BillingEftAccountType), "");

                #endregion Bank Account Type

                #region Deduction date

                // testing EFT_DeductionDay - non numeric chars
                q.BillToId = billTo_Insured_id;
                q.BillingPayPlanId = BillingPayPlanId;
                q.BillMethodId = method_DirectBill_Id;
                q.EFT_DeductionDay = "abc";
                Validations = BillingInformationValidator.BillingInformationValidation(q, valType, "true");
                if (BillingPayPlanId == payplan_EFTMonthly)
                    Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.BillingEftDeduction), "");
                else
                    Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.BillingEftDeduction), "");

                // testing EFT_DeductionDay - date greater than 31
                q.BillToId = billTo_Insured_id;
                q.BillingPayPlanId = BillingPayPlanId;
                q.BillMethodId = method_DirectBill_Id;
                q.EFT_DeductionDay = "32";
                Validations = BillingInformationValidator.BillingInformationValidation(q, valType, "true");
                if (BillingPayPlanId == payplan_EFTMonthly)
                    Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.BillingEftDeduction), "");
                else
                    Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.BillingEftDeduction), "");

                // testing EFT_DeductionDay - testing valid
                q.BillToId = billTo_Insured_id;
                q.BillingPayPlanId = BillingPayPlanId;
                q.BillMethodId = method_DirectBill_Id;
                q.EFT_DeductionDay = "29";
                Validations = BillingInformationValidator.BillingInformationValidation(q, valType, "true");
                if (BillingPayPlanId == payplan_EFTMonthly)
                    Assert.IsFalse(Validations.ListHasValidationId(BillingInformationValidator.BillingEftDeduction), "");
                else
                    Assert.IsTrue(Validations.ListHasValidationId(BillingInformationValidator.BillingEftDeduction), "");

                #endregion Deduction date
            }
        }
    }
}