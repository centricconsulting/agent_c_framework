using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace VrTests.VR_Common.Helpers
{
    [TestClass]
    public class QuickQuoteObjectHelperTests : VRQQLibBase
    {
        [TestMethod]
        public void MakeModelListLookup_GetMakes()
        {
            Assert.IsTrue(IFM.VR.Common.Helpers.PPA.MakeModelListLookup.GetMakes().Any(), "No makes returned.");
        }

        [TestMethod]
        public void MakeModelListLookup_GetMakes_ByYear()
        {
            var makesForYear = IFM.VR.Common.Helpers.PPA.MakeModelListLookup.GetMakes(DateTime.Now.Year);
            Assert.IsTrue(makesForYear.Any(), "No makes returned.");
        }

        [TestMethod]
        public void MakeModelListLookup_GetModels()
        {
            Assert.IsTrue(IFM.VR.Common.Helpers.PPA.MakeModelListLookup.GetModels("ford").Any(), "No models returned.");
        }

        [TestMethod]
        public void MakeModelListLookup_GetModels_withYear()
        {
            Assert.IsTrue(IFM.VR.Common.Helpers.PPA.MakeModelListLookup.GetModels("ford", 2012).Any(), "No models returned for given year.");
        }

        [TestMethod]
        public void MakeModelListLookup_ModelIsKnownWrong()
        {
            Assert.IsFalse(IFM.VR.Common.Helpers.PPA.MakeModelListLookup.ModelIsKnownWrong("dodge", "neon"), "Dodge-Neon should be false");
            Assert.IsTrue(IFM.VR.Common.Helpers.PPA.MakeModelListLookup.ModelIsKnownWrong("ford", "neon"), "Ford-Neon should be true.");
        }

        //[TestMethod]
        public void QuickQuoteObjectHelper_GetMakeModelYearOrVinVehicleInfo_FindFrom_VIN()
        {
            var VIN_Results = IFM.VR.Common.Helpers.PPA.VinLookup.GetMakeModelYearOrVinVehicleInfo("1G1PC5SB5E7360793", "", "", 0, DateTime.Now,128);
            // Year = 2014
            // Make = "Chevrolet"
            // Model = "CRUZE LT/CRUZE LT RS"
            // Airbags = "Side Airbags"
            // Anti-Theft = "Passive Disabling Device"
            Assert.IsTrue(VIN_Results.Any(), "No result returned on VIN Lookup");
            Assert.IsTrue(VIN_Results.Count == 1, "Should have only one result.");
            Assert.IsTrue(VIN_Results[0].Year == 2014, "Year unexpected result.");
            Assert.IsTrue(VIN_Results[0].Make.ToLower().Trim() == "Chevrolet".ToLower().Trim(), "Make unexpected result.");
            Assert.IsTrue(VIN_Results[0].Model.ToLower().Trim() == "CRUZE LT/CRUZE LT RS".ToLower().Trim(), "Model unexpected result.");
            Assert.IsTrue(VIN_Results[0].RestraintDescription.ToLower().Trim() == "Side Airbags".ToLower().Trim(), "RestraintDescription unexpected result.");
        }

        //[TestMethod]
        public void QuickQuoteObjectHelper_GetMakeModelYearOrVinVehicleInfo_FindFromMakeModelYear()
        {
            var VIN_Results = IFM.VR.Common.Helpers.PPA.VinLookup.GetMakeModelYearOrVinVehicleInfo("", "Chevrolet", "CRUZE", 2014, DateTime.Now,128);
            // First Result
            // VIN = "1G1PA5SG&E"
            Assert.IsTrue(VIN_Results.Any(), "No result returned on VIN Lookup");
            Assert.IsFalse(VIN_Results.Count == 1, "Should have more than 1 result.");
            Assert.IsTrue(VIN_Results[0].Vin.ToLower().Trim() == "1G1PA5SG&E".ToLower().Trim(), "VIN unexpected result.");
        }

        [TestMethod]
        public void AdditionalInterest_GetAdditionalInterestByName()
        {
            var Ai_Results = IFM.VR.Common.Helpers.AdditionalInterest.GetAdditionalInterestByName(
                "Bank",
                string.Empty,
                string.Empty,
                string.Empty,
                Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["QuickQuoteTestAgencyId"]));

            Assert.IsTrue(Ai_Results.Any(), "Expected search results.");
        }

        [TestMethod]
        public void AdditionalInterest_HasIncompleteAi()
        {
            var q = new QuickQuote.CommonObjects.QuickQuoteObject();
            q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal;

            q.Vehicles = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteVehicle>();

            var v = new QuickQuote.CommonObjects.QuickQuoteVehicle();
            q.Vehicles.Add(v);

            // no ai's to be incomplete
            Assert.IsFalse(IFM.VR.Common.Helpers.AdditionalInterest.HasIncompleteAi(q), "No Ais failed");

            v.AdditionalInterests = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteAdditionalInterest>();
            var ai = new QuickQuote.CommonObjects.QuickQuoteAdditionalInterest();
            v.AdditionalInterests.Add(ai);

            // empty ai
            Assert.IsTrue(IFM.VR.Common.Helpers.AdditionalInterest.HasIncompleteAi(q), "Empty Ai failed");

            ai.Name.FirstName = "First Name";
            ai.Address.POBox = "po box";
            Assert.IsTrue(IFM.VR.Common.Helpers.AdditionalInterest.HasIncompleteAi(q), "No Address failed");

            // should fail - No zip
            ai.Name.FirstName = "";
            ai.Name.CommercialName1 = "Name";
            ai.Address.POBox = "po box";
            ai.Address.City = "city";
            ai.Address.State = "state";
            ai.Address.Zip = "";
            Assert.IsTrue(IFM.VR.Common.Helpers.AdditionalInterest.HasIncompleteAi(q), "Name with Address minus zipcode failed");

            // should pass
            ai.Name.FirstName = "";
            ai.Name.CommercialName1 = "Name";
            ai.Address.POBox = "po box";
            ai.Address.City = "city";
            ai.Address.State = "state";
            ai.Address.Zip = "46041";
            Assert.IsFalse(IFM.VR.Common.Helpers.AdditionalInterest.HasIncompleteAi(q), "Name with Address failed");
        }
    }
}