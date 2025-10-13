using IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VrTests.IFM_VR_Validation.ObjectValidation.Farm
{
    [TestClass]
    public class FarmLocationAddressTests : VRQQLibBase
    {
        [TestMethod]
        public void TestFarmLocationAddress()
        {
            var q = GetNewQuickQuote();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm;
            if (q.Locations == null)
                q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
            q.Locations.Add(new QuickQuote.CommonObjects.QuickQuoteLocation());

            QuickQuote.CommonObjects.QuickQuoteAddress a = q.Locations[0].Address;

            string[] programTypes = new string[] { "6", "7", "8" };

            foreach (string programType in programTypes)
            {
                q.Locations[0].ProgramTypeId = programType;
                // test empty
                a.StreetName = "";
                a.HouseNum = "";
                a.POBox = "";
                a.City = "city";
                a.StateId = "16";
                a.Zip = "46041";
                a.County = "county";

                var Validations = LocationAddressValidator.ValidateHOMLocationAddress(q, 0, true, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                if (programType == "8")
                {
                    Assert.IsFalse(Validations.ListHasValidationId(LocationAddressValidator.AddressStreetAndPoBoxEmpty));
                    Assert.IsFalse(Validations.ListHasValidationId(LocationAddressValidator.AddressStreetNumber));
                    Assert.IsFalse(Validations.ListHasValidationId(LocationAddressValidator.AddressStreetName));
                }
                else
                {
                    Assert.IsTrue(Validations.ListHasValidationId(LocationAddressValidator.AddressStreetAndPoBoxEmpty));
                }

                // test HouseNum empty
                a.StreetName = "";
                a.HouseNum = "1";
                a.POBox = "";
                a.City = "city";
                a.StateId = "16";
                a.Zip = "46041";
                a.County = "county";

                Validations = LocationAddressValidator.ValidateHOMLocationAddress(q, 0, true, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                if (programType == "8")
                {
                    Assert.IsFalse(Validations.ListHasValidationId(LocationAddressValidator.AddressStreetAndPoBoxEmpty));
                    Assert.IsFalse(Validations.ListHasValidationId(LocationAddressValidator.AddressStreetNumber));
                    Assert.IsFalse(Validations.ListHasValidationId(LocationAddressValidator.AddressStreetName));
                }
                else
                {
                    Assert.IsTrue(Validations.ListHasValidationId(LocationAddressValidator.AddressStreetName));
                }

                // test StreetName empty
                a.StreetName = "Fake St";
                a.HouseNum = "";
                a.POBox = "";
                a.City = "city";
                a.StateId = "16";
                a.Zip = "46041";
                a.County = "county";

                Validations = LocationAddressValidator.ValidateHOMLocationAddress(q, 0, true, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                if (programType == "8")
                {
                    Assert.IsFalse(Validations.ListHasValidationId(LocationAddressValidator.AddressStreetAndPoBoxEmpty));
                    Assert.IsFalse(Validations.ListHasValidationId(LocationAddressValidator.AddressStreetNumber));
                    Assert.IsFalse(Validations.ListHasValidationId(LocationAddressValidator.AddressStreetName));
                }
                else
                {
                    Assert.IsTrue(Validations.ListHasValidationId(LocationAddressValidator.AddressStreetNumber));
                }

                // test empty City
                a.StreetName = "";
                a.HouseNum = "";
                a.POBox = "";
                a.City = "";
                a.StateId = "16";
                a.Zip = "46041";
                a.County = "county";

                Validations = LocationAddressValidator.ValidateHOMLocationAddress(q, 0, true, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate);
                if (programType == "8")
                {
                    Assert.IsFalse(Validations.ListHasValidationId(LocationAddressValidator.AddressStreetAndPoBoxEmpty));
                    Assert.IsFalse(Validations.ListHasValidationId(LocationAddressValidator.AddressStreetNumber));
                    Assert.IsFalse(Validations.ListHasValidationId(LocationAddressValidator.AddressStreetName));
                }
                else
                {
                    Assert.IsTrue(Validations.ListHasValidationId(LocationAddressValidator.AddressStreetAndPoBoxEmpty));
                }

                Assert.IsTrue(Validations.ListHasValidationId(LocationAddressValidator.AddressCity));
            }
        }
    }
}