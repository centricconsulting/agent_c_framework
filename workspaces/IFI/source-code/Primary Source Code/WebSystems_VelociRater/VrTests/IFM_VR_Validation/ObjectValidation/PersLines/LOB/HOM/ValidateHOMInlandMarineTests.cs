using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VrTests.IFM_VR_Validation.ObjectValidation.PersLines.LOB.HOM
{
    [TestClass]
    public class ValidateHOMInlandMarineTests : VRQQLibBase
    {
        //[TestMethod]
        //public void InlandMarine_Issuance_Tests()
        //{
        //    var q = GetNewQuickQuote();
        //    q.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal;

        //    q.Locations = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteLocation>();
        //    var l = new QuickQuote.CommonObjects.QuickQuoteLocation();
        //    q.Locations.Add(l);
        //    l.InlandMarines = new System.Collections.Generic.List<QuickQuote.CommonObjects.QuickQuoteInlandMarine>();

        //    var inland1 = new QuickQuote.CommonObjects.QuickQuoteInlandMarine();
        //    inland1.InlandMarineType = QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry;
        //    inland1.IncreasedLimit = "1000";
        //    l.InlandMarines.Add(inland1);

        //    var inland2 = new QuickQuote.CommonObjects.QuickQuoteInlandMarine();
        //    inland2.InlandMarineType = QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault;
        //    inland2.IncreasedLimit = "1000";
        //    l.InlandMarines.Add(inland2);

        //    var inland3 = new QuickQuote.CommonObjects.QuickQuoteInlandMarine();
        //    inland3.InlandMarineType = QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage;
        //    inland3.IncreasedLimit = "1000";
        //    l.InlandMarines.Add(inland3);

        //    var inland4 = new QuickQuote.CommonObjects.QuickQuoteInlandMarine();
        //    inland4.InlandMarineType = QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage;
        //    inland4.IncreasedLimit = "1000";
        //    l.InlandMarines.Add(inland4);

        //    var inland5 = new QuickQuote.CommonObjects.QuickQuoteInlandMarine();
        //    inland5.InlandMarineType = QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs;
        //    inland5.IncreasedLimit = "1000";
        //    l.InlandMarines.Add(inland5);

        //    IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.issuance;

        //    // less than 5000 so should get no error
        //    var vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.ValidateHOMInlandMarine(q, valType);
        //    Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.Single_Item_Jewelry_Arts_Furs_Exceeded_5000), "Jewelry is 5000");

        //    // $1 over 5000 so should get error
        //    inland5.IncreasedLimit = "5001";
        //    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.ValidateHOMInlandMarine(q, valType);
        //    Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.Single_Item_Jewelry_Arts_Furs_Exceeded_5000), "Jewelry is 5001");

        //    // testing collection limit should be fine at 30,000
        //    inland5.IncreasedLimit = "26000"; // already has 4,000 from the 4 other 1,000 items
        //    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.ValidateHOMInlandMarine(q, valType);
        //    Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.Combined_Item_Jewelry_Arts_Furs_Exceeded_30000), "Collection is 30000");

        //    // testing collection limit should get error at 30,001
        //    inland5.IncreasedLimit = "26001"; // already has 4,000 from the 4 other 1,000 items
        //    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.ValidateHOMInlandMarine(q, valType);
        //    Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.Combined_Item_Jewelry_Arts_Furs_Exceeded_30000), "Collection is 30001");

        //    // testing gun over 5,000
        //    inland5.IncreasedLimit = "5000";
        //    inland5.InlandMarineType = QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns;
        //    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.ValidateHOMInlandMarine(q, valType);
        //    Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.Single_Gun_Limit_Exceeded), "Single Gun at 5000");

        //    // testing gun over 5,000
        //    inland5.IncreasedLimit = "5001";
        //    inland5.InlandMarineType = QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns;
        //    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.ValidateHOMInlandMarine(q, valType);
        //    Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.Single_Gun_Limit_Exceeded), "Single Gun at 5001");

        //    // testing gun collection over 5,000
        //    inland4.InlandMarineType = QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns;
        //    inland4.IncreasedLimit = "1000";
        //    inland5.IncreasedLimit = "4000";
        //    inland5.InlandMarineType = QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns;
        //    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.ValidateHOMInlandMarine(q, valType);
        //    Assert.IsFalse(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.Combined_Guns_Limit_Exceeded), "Gun collection at 5,000");

        //    // testing gun collection over 5,000
        //    inland4.InlandMarineType = QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns;
        //    inland4.IncreasedLimit = "1001";
        //    inland5.IncreasedLimit = "4000";
        //    inland5.InlandMarineType = QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns;
        //    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.ValidateHOMInlandMarine(q, valType);
        //    Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.Combined_Guns_Limit_Exceeded), "Gun collection at 5,001");

        //    // assert that you are getting back errors when it is issuance valTYpe
        //    valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.issuance;
        //    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.ValidateHOMInlandMarine(q, valType);
        //    Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.Combined_Guns_Limit_Exceeded), "Gun collection at 5,001");
        //    Assert.IsTrue(vals.GetValidationItemById(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.Combined_Guns_Limit_Exceeded).IsWarning == false, "Should be error.");

        //    // assert that you are getting back warnings when it is NOT issuance valTYpe
        //    valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.finalRate;
        //    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.ValidateHOMInlandMarine(q, valType);
        //    Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.Combined_Guns_Limit_Exceeded), "Gun collection at 5,001");
        //    Assert.IsFalse(vals.GetValidationItemById(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.Combined_Guns_Limit_Exceeded).IsWarning == false, "Should be error.");

        //    // testing missing inland type
        //    valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
        //    inland4.IncreasedLimit = "1";
        //    inland5.InlandMarineType = QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.None;
        //    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.ValidateHOMInlandMarine(q, valType);
        //    Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.TypeIdNotDefined), "Type is 'none'.");

        //    // testing zero increased limits
        //    valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
        //    inland4.IncreasedLimit = "0";
        //    inland5.InlandMarineType = QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns;
        //    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.ValidateHOMInlandMarine(q, valType);
        //    Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.LimitValueNotGreaterThanZero), "A limit is a negative number.");

        //    // testing negative increased limits
        //    valType = IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate;
        //    inland4.IncreasedLimit = "-1";
        //    inland5.InlandMarineType = QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns;
        //    vals = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.ValidateHOMInlandMarine(q, valType);
        //    Assert.IsTrue(vals.ListHasValidationId(IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.InlandMarineListValidator.LimitValueNotGreaterThanZero), "A limit is a negative number.");
        //}
    }
}