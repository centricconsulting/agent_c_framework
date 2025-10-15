using QuickQuote.CommonMethods;
using QuickQuote.CommonObjects;
using System;
using IFM.PrimativeExtensions;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA
{
    public class DriverValidator
    {
        //This is only used for PPA, so no multi state changes are needed 9/17/18 MLW
        public const string ValidationListID = "{4B5FCB3E-DE71-4983-8B62-524129FF2541}";

        public const string QuoteIsNull = "{A4CE93D0-5D3C-4572-A722-CFE33F9C717C}";
        public const string DriverNull = "{A977E82F-CB6B-42AE-B1A5-8CD59544B3A2}";

        public const string DriverNameIsNull = "{82355696-DABC-4260-89EF-9A07BCE9F1DD}";

        public const string DriverFirstname = "{F4233E91-8FDF-46AA-9C39-0FEC84FB1643}";
        public const string DriverLastname = "{815DF8D6-4A7C-44E5-A530-AFBD75AEFCB1}";
        public const string DriverBirthDate = "{6EA8C36D-9CA6-4B5C-AC4E-C0C4CD772396}";
        public const string DriverGender = "{07CE6D28-1D85-48B7-A7BA-ABF0B8EBA55B}";
        public const string DriverMaritalStatus = "{76BB9FE7-68C9-4711-A872-19B36A4CE982}";
        public const string DriverDLNumber = "{DB055DE8-6D55-4017-B11B-AC2691268319}";
        public const string DriverDLDate = "{E5BAFF4F-12EA-411C-A5DE-CF04F73592A2}";
        public const string DriverDLState = "{E1540270-9918-4248-9FF1-C18879D45AC1}";
        public const string DriverExcludedType = "{C6F73BAE-1333-441A-BE02-1A6C6A98CD63}";
        public const string DriverRelationship = "{EBBD5C58-725C-4B1D-B4EE-E882145447FD}";
        public const string DriverRelationshipPh1Duplicated = "{53448776-82F0-48F6-A559-0AEAB5E11C65}";
        public const string DriverRelationshipPh2Duplicated = "{7F7E377D-68C0-4FC7-B2FD-F2D8E6B7880A}";
        public const string DriverDefensiveDate = "{399517B7-3D88-4FC3-A1B1-D8075716ED80}";
        public const string DriverMatureDriverDate = "{C6B93056-C66B-40DE-A8AB-DCB45138AFDB}";
        public const string DriverGoodStudentAge = "{4DFFBB0E-4F98-4847-82CF-BEC8C8A5A9D2}";
        public const string DriverDistanceToSchool = "{E2E5019F-E35E-4C61-91B3-A43426350604}";
        public const string DriverMotorCycleTrainingDate = "{6A242853-5776-4653-A91C-1CA2DB8702B4}";
        public const string DriverMotorCycleYearsOfExperience = "{80382141-9964-47F8-AA5A-7B6AAEF41DD2}";

        public static Validation.ObjectValidation.ValidationItemList ValidateDriver(int driverIndex, QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            QuickQuoteHelperClass QQHelper = new QuickQuoteHelperClass();

            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.DriverIndex, driverIndex.ToString());

            if (quote != null)
            {
                QuickQuote.CommonObjects.QuickQuoteDriver driver = null;
                if (quote.Drivers != null && quote.Drivers.Count > driverIndex)
                    driver = quote.Drivers[driverIndex];

                if (quote.QuoteTransactionType == QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                {
                    valType = ValidationItem.ValidationType.endorsement;
                }

                if (driver != null)
                {
                    if (driver.Name != null)
                    {
                        string DriverExcludeTypeId_Rated = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuoteHelperClass.QuickQuotePropertyName.DriverExcludeTypeId, "Rated");
                        bool ratedDriver = driver.DriverExcludeTypeId == DriverExcludeTypeId_Rated;

                        VRGeneralValidations.Val_HasRequiredField_DD(driver.DriverExcludeTypeId, valList, DriverExcludedType, "Excluded Type");

                        VRGeneralValidations.Val_HasRequiredField(driver.Name.FirstName, valList, DriverFirstname, "First Name");
                        VRGeneralValidations.Val_HasRequiredField(driver.Name.LastName, valList, DriverLastname, "Last Name");

                        if (VRGeneralValidations.Val_HasRequiredField_Date(driver.Name.BirthDate, valList, DriverBirthDate, "Birth Date"))
                            VRGeneralValidations.Val_IsValidBirthDate(driver.Name.BirthDate, valList, DriverBirthDate, "Birth Date");

                        VRGeneralValidations.Val_HasRequiredField_DD(driver.Name.SexId, valList, DriverGender, "Gender");
                        VRGeneralValidations.Val_HasRequiredField(driver.Name.MaritalStatusId, valList, DriverMaritalStatus, "Marital Status");
                        switch (valType)
                        {
                            case ValidationItem.ValidationType.issuance:
                            case ValidationItem.ValidationType.appRate:
                            case ValidationItem.ValidationType.endorsement:
                                if (ratedDriver)
                                {
                                    driver.Name.DriversLicenseNumber = driver.Name.DriversLicenseNumber.Replace_NullSafe("-", "");
                                    string StateId_IN = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAddress, QuickQuoteHelperClass.QuickQuotePropertyName.StateId, "IN");
                                    if (VRGeneralValidations.Val_HasRequiredField(driver.Name.DriversLicenseNumber, valList, DriverDLNumber, "DL Number"))
                                    {
                                        if (VRGeneralValidations.Val_IsNotTextRepeated(driver.Name.DriversLicenseNumber, valList, DriverDLNumber, "DL Number"))
                                            if (VRGeneralValidations.Val_IsNotTextSequential(driver.Name.DriversLicenseNumber, valList, DriverDLNumber, "DL Number"))
                                                if (driver.Name.DriversLicenseStateId == StateId_IN)
                                                {
                                                    if (VRGeneralValidations.Val_IsValidAlphaNumeric(driver.Name.DriversLicenseNumber, valList, DriverDLNumber, "DL Number"))
                                                        VRGeneralValidations.Val_IsTextLengthInRange(driver.Name.DriversLicenseNumber, valList, DriverDLNumber, "DL Number", "10", "10");
                                                }
                                                else
                                                {
                                                    if (VRGeneralValidations.Val_IsValidAlphaNumeric(driver.Name.DriversLicenseNumber, valList, DriverDLNumber, "DL Number"))
                                                        VRGeneralValidations.Val_IsTextLengthInRange(driver.Name.DriversLicenseNumber, valList, DriverDLNumber, "DL Number", "1", "19");
                                                }
                                    }
                                    VRGeneralValidations.Val_HasRequiredField_DD(driver.Name.DriversLicenseStateId, valList, DriverDLState, "DL State");

                                    if (VRGeneralValidations.Val_HasRequiredField_Date(driver.Name.DriversLicenseDate, valList, DriverDLDate, "DL Date"))
                                        if (VRGeneralValidations.Val_IsValidDate(driver.Name.DriversLicenseDate, valList, DriverDLDate, "DL Date"))
                                            VRGeneralValidations.Val_IsDateInRange(driver.Name.DriversLicenseDate, valList, DriverDLDate, "DL Date", DateTime.Now.AddYears(-100).ToShortDateString(), DateTime.Now.ToShortDateString());
                                }

                                break;

                            case ValidationItem.ValidationType.quoteRate:
                                driver.Name.DriversLicenseNumber = driver.Name.DriversLicenseNumber.Replace_NullSafe("-", "");
                                if (VRGeneralValidations.Val_HasRequiredField(driver.Name.DriversLicenseNumber, valList, DriverDLNumber, "DL Number", true))
                                    VRGeneralValidations.Val_IsValidAlphaNumeric(driver.Name.DriversLicenseNumber, valList, DriverDLNumber, "DL Number");
                                VRGeneralValidations.Val_IsValidDate(driver.Name.DriversLicenseDate, valList, DriverDLDate, "DL Date");
                                break;

                            default:
                                break;
                        }

                        VRGeneralValidations.Val_HasRequiredField_DD(driver.RelationshipTypeId, valList, DriverRelationship, "Relationship to Driver");

                        // CHECK FOR DUPLICATE RELATIONSHIP TYPES FOR PH#1 & 2
                        int ph1Count = 0;
                        int ph2Count = 0;
                        string ph1ID = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuoteHelperClass.QuickQuotePropertyName.RelationshipTypeId, "Policyholder");
                        string ph2ID = QQHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuoteHelperClass.QuickQuotePropertyName.RelationshipTypeId, "Policyholder #2");
                        foreach (QuickQuote.CommonObjects.QuickQuoteDriver d in quote.Drivers)
                        {
                            if (d.RelationshipTypeId == ph1ID)
                                ph1Count += 1;

                            if (d.RelationshipTypeId == ph2ID)
                                ph2Count += 1;
                        }
                        if (ph1Count > 1)
                            valList.Add(new ObjectValidation.ValidationItem("Policyholder chosen twice.", DriverRelationshipPh1Duplicated));

                        if (ph2Count > 1)
                            valList.Add(new ObjectValidation.ValidationItem("Policyholder #2 chosen twice.", DriverRelationshipPh2Duplicated));
                        // END  -  CHECK FOR DUPLICATE RELATIONSHIP TYPES FOR PH#1 & 2

                        if (IFM.Common.InputValidation.CommonValidations.IsEmptyOrDefaultDiamond_Date(driver.DefDriverDate) == false)
                            VRGeneralValidations.Val_IsValidBirthDate(driver.DefDriverDate, valList, DriverDefensiveDate, "Defensive Driver Date"); // make sure it is sane date

                        if (IFM.Common.InputValidation.CommonValidations.IsEmptyOrDefaultDiamond_Date(driver.AccPreventionCourse) == false)
                            VRGeneralValidations.Val_IsValidBirthDate(driver.AccPreventionCourse, valList, DriverMatureDriverDate, "Mature Driver Course Date"); // make sure it is sane date

                        // GOOD STUDENT - Old Code
                        //if (driver.GoodStudent | driver.DistantStudent | driver.SchoolDistance != "0")
                        //    {
                        //    if (IFM.Common.InputValidation.CommonValidations.IsEmptyOrDefaultDiamond_Date(driver.Name.BirthDate) == false) // already checked above so do not add error
                        //    {
                        //        DateTime effectiveDate = DateTime.MinValue;
                        //        if (System.DateTime.TryParse(quote.EffectiveDate, out effectiveDate) == false)
                        //        {
                        //            effectiveDate = DateTime.Now;
                        //        }

                        //        // just checking age
                        //        if (VRGeneralValidations.Val_IsDateInRange(driver.Name.BirthDate, valList, "", "", effectiveDate.AddYears(-25).AddSeconds(1).ToShortDateString(), effectiveDate.ToShortDateString(), true))
                        //        {
                        //            // is less than 25
                        //            if (driver.DistantStudent)
                        //            {
                        //                if (VRGeneralValidations.Val_HasRequiredField_Int(driver.SchoolDistance, valList, DriverDistanceToSchool, "Distance to School"))
                        //                    VRGeneralValidations.Val_IsNumberInRange(driver.SchoolDistance, valList, DriverDistanceToSchool, "Distance to School", "100", "12000");
                        //            }
                        //            else
                        //            {
                        //                VRGeneralValidations.Val_HasIneligibleField(driver.SchoolDistance, valList, DriverDistanceToSchool, "Distance to School");
                        //            }
                        //        }
                        //        else
                        //        {
                        //            // not under 25 so should not have any of these values
                        //            if (driver.GoodStudent | driver.DistantStudent | string.IsNullOrWhiteSpace(driver.SchoolDistance) == false)
                        //                valList.Add(new ObjectValidation.ValidationItem("Good student is only available to drivers less than 25 years of age.", DriverGoodStudentAge));
                        //        }
                        //    }
                        //}
                        // END   -  GOOD STUDENT

                        // Good Student - New Code - CAH 9/3/2019
                        if (driver.GoodStudent || driver.DistantStudent || (string.IsNullOrWhiteSpace(driver.SchoolDistance) == false && driver.SchoolDistance != "0"))
                        {
                            if (IFM.Common.InputValidation.CommonValidations.IsEmptyOrDefaultDiamond_Date(driver.Name.BirthDate) == false) // already checked above so do not add error
                            {
                                DateTime effectiveDate = DateTime.MinValue;
                                if (System.DateTime.TryParse(quote.EffectiveDate, out effectiveDate) == false)
                                {
                                    effectiveDate = DateTime.Now;
                                }

                                // just checking age
                                if (VRGeneralValidations.Val_IsDateInRange(driver.Name.BirthDate, valList, "", "", effectiveDate.AddYears(-25).AddSeconds(1).ToShortDateString(), effectiveDate.ToShortDateString(), true))
                                {
                                    // is less than 25
                                    if (driver.DistantStudent)
                                    {
                                        if (VRGeneralValidations.Val_HasRequiredField_Int(driver.SchoolDistance, valList, DriverDistanceToSchool, "Distance to School"))
                                            VRGeneralValidations.Val_IsNumberInRange(driver.SchoolDistance, valList, DriverDistanceToSchool, "Distance to School", "100", "12000");
                                    }
                                    else
                                    {
                                        VRGeneralValidations.Val_HasIneligibleField_Int(driver.SchoolDistance, valList, DriverDistanceToSchool, "Distance to School");
                                    }
                                }
                                else
                                {
                                    // not under 25 so should not have any of these values
                                    if (driver.GoodStudent | driver.DistantStudent | string.IsNullOrWhiteSpace(driver.SchoolDistance) == false)
                                        valList.Add(new ObjectValidation.ValidationItem("Good student is only available to drivers less than 25 years of age.", DriverGoodStudentAge));
                                }
                            }
                        }
                        // End - Good Student












                        if (IFM.Common.InputValidation.CommonValidations.IsEmptyOrDefaultDiamond_Date(driver.MotorMembershipDate) == false) // just checking
                        {
                            VRGeneralValidations.Val_IsDateInRange(driver.MotorMembershipDate, valList, DriverMotorCycleTrainingDate, "Motorcycle Training Date", DateTime.Now.AddYears(-100).ToString(), DateTime.Now.ToString());
                        }

                        if (IFM.Common.InputValidation.CommonValidations.IsEmptyOrDefaultDiamond_Int(driver.MotorcycleYearsExperience) == false)
                        {
                            VRGeneralValidations.Val_IsNumberInRange(driver.MotorcycleYearsExperience, valList, DriverMotorCycleYearsOfExperience, "Years of Motorcycle Experience", "0", "100");
                        }
                    }
                    else
                    {
                        //driver name is null
                        valList.Add(new ObjectValidation.ValidationItem("Driver Name is null", DriverNameIsNull));
                    }
                }
                else
                {
                    valList.Add(new ObjectValidation.ValidationItem("Driver is null", DriverNull));
                }
            }
            else
            {
                // quote null
                valList.Add(new ObjectValidation.ValidationItem("Quote is null", QuoteIsNull));
            }
            return valList;
        }
    }
}