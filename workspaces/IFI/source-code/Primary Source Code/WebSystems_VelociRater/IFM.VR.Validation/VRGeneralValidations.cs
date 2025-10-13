using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

namespace IFM.VR.Validation
{
    public static class VRGeneralValidations
    {
        public static bool Val_HasRequiredField(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.requiredField, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        public static bool Val_HasRequiredField_DD(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.requiredField_dd, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        public static bool Val_HasIneligibleField(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.has_ineligibleField, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        public static bool Val_HasIneligibleField_DD(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.has_ineligibleField_dd, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        public static bool Val_HasIneligibleField_Int(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.has_ineligibleField_Int, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        public static bool Val_HasIneligibleField_Date(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.has_ineligibleField_Date, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        public static bool Val_IsValidSSN(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.isValidSSN, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        public static bool Val_IsValidBirthDate(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.isValidBirthdate, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        public static bool Val_IsValidEmailAddress(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.isValidEmailAddress, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        public static bool Val_IsValidPhoneNumber(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.isValidPhoneNumber, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        public static bool Val_IsValidZipCode(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.isValidZipCode, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        public static bool Val_IsValidPhoneExtension(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.isValidPhoneExtension, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        public static bool Val_IsValidDriverBirthDate(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.isValidDriverBirthDate, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        public static bool Val_IsValidAlphaNumeric(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.isAlphaNumeric, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        public static bool Val_IsValidDate(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.isDate, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        public static bool Val_IsNumeric(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.isNumeric, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        /// <summary>
        /// Returns true when text is a number >= Zero.
        /// </summary>
        /// <param name="testValue"></param>
        /// <param name="valList"></param>
        /// <param name="fieldId"></param>
        /// <param name="fieldName"></param>
        /// <param name="checkConditionButDontAddError"></param>
        /// <returns></returns>
        public static bool Val_IsNonNegativeNumber(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.isNonNegativeNumber, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        public static bool Val_IsGreaterThanZeroNumber(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.isGreaterThanZeroNumber, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        public static bool Val_IsDateInRange(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, string lowerDate, string upperDate, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.isDateInRange, testValue, valList, fieldId, fieldName, checkConditionButDontAddError, new List<object> {
                            lowerDate,
                            upperDate
                            });
        }

        public static bool Val_IsNumberInRange(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, string lowerNumber, string upperNumber, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.isNumberInRange, testValue, valList, fieldId, fieldName, checkConditionButDontAddError, new List<object> {
                            lowerNumber,
                            upperNumber
                            });
        }

        public static bool Val_IsTextLengthInRange(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, string lowerNumber, string upperNumber, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.isTextLengthInRange, testValue, valList, fieldId, fieldName, checkConditionButDontAddError, new List<object> {
                            lowerNumber,
                            upperNumber
                            });
        }

        public static bool Val_IsNotTextRepeated(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.isNotTextRepeated, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        public static bool Val_IsNotTextSequential(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.isNotTextSequential, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        /// <summary>
        /// Returns true when text is a number between 1000-9999.
        /// </summary>
        /// <param name="testValue"></param>
        /// <param name="valList"></param>
        /// <param name="fieldId"></param>
        /// <param name="fieldName"></param>
        /// <param name="checkConditionButDontAddError"></param>
        /// <returns></returns>
        public static bool Val_IsFourDigitYear(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.is_4digitYear, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        /// <summary>
        /// False if number is not whole or number is less than zero
        /// </summary>
        /// <param name="testValue"></param>
        /// <param name="valList"></param>
        /// <param name="fieldId"></param>
        /// <param name="fieldName"></param>
        /// <param name="checkConditionButDontAddError"></param>
        /// <returns></returns>
        public static bool Val_IsNonNegativeWholeNumber(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.is_nonNegativeWholeNumber, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        /// <summary>
        /// False if number is not whole or number is less than zero
        /// </summary>
        /// <param name="testValue"></param>
        /// <param name="valList"></param>
        /// <param name="fieldId"></param>
        /// <param name="fieldName"></param>
        /// <param name="checkConditionButDontAddError"></param>
        /// <returns></returns>
        public static bool Val_IsPositiveWholeNumber(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.isPositiveWholeNumber, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }


        /// <summary>
        /// False if the field is EMPTY or the Diamond Default Date.
        /// </summary>
        /// <param name="testValue"></param>
        /// <param name="valList"></param>
        /// <param name="fieldId"></param>
        /// <param name="fieldName"></param>
        /// <param name="checkConditionButDontAddError"></param>
        /// <returns></returns>
        public static bool Val_HasRequiredField_Date(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.requiredField_Date, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        /// <summary>
        /// False if the field is EMPTY or the Diamond Default Int of Zero.
        /// </summary>
        /// <param name="testValue"></param>
        /// <param name="valList"></param>
        /// <param name="fieldId"></param>
        /// <param name="fieldName"></param>
        /// <param name="checkConditionButDontAddError"></param>
        /// <returns></returns>
        public static bool Val_HasRequiredField_Int(string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false)
        {
            return Val_GeneralTests(GeneralValidationTestType.requiredField_Int, testValue, valList, fieldId, fieldName, checkConditionButDontAddError);
        }

        private enum GeneralValidationTestType
        {
            requiredField = 1,
            isValidSSN = 2,
            isValidBirthdate = 3,
            isValidEmailAddress = 4,
            isValidPhoneNumber = 5,
            isValidZipCode = 6,
            isValidPhoneExtension = 7,
            isValidDriverBirthDate = 8,
            isAlphaNumeric = 9,
            isDate = 10,
            isNumeric = 11,
            isNonNegativeNumber = 12,
            isDateInRange = 13,
            isGreaterThanZeroNumber = 14,
            isNumberInRange = 15,
            isTextLengthInRange = 16,
            isNotTextRepeated = 17,
            isNotTextSequential = 18,
            has_ineligibleField = 19,
            requiredField_dd = 20,
            has_ineligibleField_dd = 21,
            is_4digitYear = 22,
            is_nonNegativeWholeNumber = 23,
            requiredField_Date = 24,
            requiredField_Int = 25,
            has_ineligibleField_Date = 26,
            has_ineligibleField_Int = 27,
            isPositiveWholeNumber = 28
        }

        private static bool Val_GeneralTests(GeneralValidationTestType testype, string testValue, Validation.ObjectValidation.ValidationItemList valList, string fieldId, string fieldName, bool checkConditionButDontAddError = false, List<object> parms = null)
        {
            bool hasError = false;

            string text = testValue;
            switch (testype)
            {
                case GeneralValidationTestType.requiredField:
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.requiredField_Date:
                    if (IFM.Common.InputValidation.CommonValidations.IsEmptyOrDefaultDiamond_Date(text))
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.requiredField_Int:
                    if (string.IsNullOrWhiteSpace(text) | IFM.Common.InputValidation.InputHelpers.TryToGetDouble(text.Trim()) == 0)
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.requiredField_dd:                    
                    if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(text.Trim()) || text == "0" || text == "-1" || text.Trim().ToLower() == "n/a" || text.Trim().ToLower() == "na")
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.has_ineligibleField:
                    if (string.IsNullOrWhiteSpace(text) == false)
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.has_ineligibleField_Int:
                    if (string.IsNullOrWhiteSpace(text) == false & text.Trim() != "0")
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.has_ineligibleField_Date:
                    if (!(IFM.Common.InputValidation.CommonValidations.IsEmptyOrDefaultDiamond_Date(text)))
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.has_ineligibleField_dd:
                    if (!(string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(text.Trim()) || text == "0" || text == "-1" || text.Trim().ToLower() == "n/a" || text.Trim().ToLower() == "na"))
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.isValidSSN:
                    if (IFM.Common.InputValidation.CommonValidations.IsValidSSN(text) == false)
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.isValidBirthdate:
                    {
                        DateTime dob = default(DateTime);
                        if (DateTime.TryParse(text, out dob) && IFM.Common.InputValidation.CommonValidations.IsEmptyOrDefaultDiamond_Date(text) == false)
                        {
                            if (dob > DateTime.Now | dob.AddYears(200) < DateTime.Now)
                            {
                                hasError = true;
                            }
                        }
                        else
                        {
                            hasError = true;
                        }
                    }
                    break;

                case GeneralValidationTestType.isValidEmailAddress:
                    if (IFM.Common.InputValidation.CommonValidations.IsValidEmail(text) == false)
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.isValidPhoneNumber:
                    if (IFM.Common.InputValidation.CommonValidations.IsValidPhone(text) == false)
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.isValidZipCode:
                    if (IFM.Common.InputValidation.CommonValidations.IsValidZipCode(text) == false)
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.isValidPhoneExtension:
                    int phoneExt = 0;
                    if (int.TryParse(text, out phoneExt) == false | phoneExt == 0)
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.isValidDriverBirthDate:
                    {
                        DateTime dob = default(DateTime);
                        if (DateTime.TryParse(text, out dob) && IFM.Common.InputValidation.CommonValidations.IsEmptyOrDefaultDiamond_Date(text) == false)
                        {
                            if (dob > DateTime.Now.AddYears(-14) | dob.AddYears(200) < DateTime.Now)
                            {
                                hasError = true;
                            }
                        }
                        else
                        {
                            hasError = true;
                        }
                    }
                    break;

                case GeneralValidationTestType.isAlphaNumeric:
                    if (IFM.Common.InputValidation.CommonValidations.IsAlphaNum(text) == false)
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.isDate:
                    if (Information.IsDate(text) == false | IFM.Common.InputValidation.CommonValidations.IsEmptyOrDefaultDiamond_Date(text))
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.isNumeric:
                    if (Information.IsNumeric(text) == false)
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.isNonNegativeNumber:
                    if (IFM.Common.InputValidation.CommonValidations.IsNonNegativeNumber(text) == false)
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.isDateInRange:
                    if (parms != null)
                    {
                        if (Information.IsDate(text) & IFM.Common.InputValidation.CommonValidations.IsEmptyOrDefaultDiamond_Date(text) == false)
                        {
                            if (parms.Count == 2)
                            {
                                if (Information.IsDate(parms[0]) & Information.IsDate(parms[1]))
                                {
                                    DateTime valDate = Convert.ToDateTime(text);
                                    if (!(valDate >= Convert.ToDateTime(parms[0]) & valDate <= Convert.ToDateTime(parms[1])))
                                    {
                                        hasError = true;
                                    }
                                }
                                else
                                {
                                    hasError = true;
                                }
                            }
                            else
                            {
                                hasError = true;
                            }
                        }
                        else
                        {
                            hasError = true;
                        }
                    }
                    else
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.isGreaterThanZeroNumber:
                    if (IFM.Common.InputValidation.CommonValidations.IsPositiveNumber(text) == false)
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.isNumberInRange:
                    if (parms != null)
                    {
                        text = IFM.Common.InputValidation.CommonValidations.CleanUpNumericStrings(text);
                        if (Information.IsNumeric(text))
                        {
                            if (parms.Count == 2)
                            {
                                parms[0] = IFM.Common.InputValidation.CommonValidations.CleanUpNumericStrings(parms[0].ToString());
                                parms[1] = IFM.Common.InputValidation.CommonValidations.CleanUpNumericStrings(parms[1].ToString());
                                if (Information.IsNumeric(parms[0]) & Information.IsNumeric(parms[1]))
                                {
                                    decimal valNum = Convert.ToDecimal(text);
                                    if (!(valNum >= Convert.ToDecimal(parms[0]) & valNum <= Convert.ToDecimal(parms[1])))
                                    {
                                        hasError = true;
                                    }
                                }
                                else
                                {
                                    hasError = true;
                                }
                            }
                            else
                            {
                                hasError = true;
                            }
                        }
                        else
                        {
                            hasError = true;
                        }
                    }
                    else
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.isTextLengthInRange:
                    if (parms.Count == 2)
                    {
                        parms[0] = parms[0];
                        parms[1] = parms[1];
                        if (Information.IsNumeric(parms[0]) & Information.IsNumeric(parms[1]))
                        {
                            Int32 valNum = text.Trim().Length;
                            if (!(valNum >= Convert.ToInt32(parms[0]) & valNum <= Convert.ToInt32(parms[1])))
                            {
                                hasError = true;
                            }
                        }
                        else
                        {
                            hasError = true;
                        }
                    }
                    else
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.isNotTextRepeated:
                    if (IFM.Common.InputValidation.CommonValidations.IsRepeatedCharacters(text))
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.isNotTextSequential:
                    if (IFM.Common.InputValidation.CommonValidations.IsAlhaNumericlySequential(text))
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.is_4digitYear:
                    int year = int.MinValue;
                    if (int.TryParse(text, out year))
                    {
                        hasError = !(year > 999 & year < 10000);
                    }
                    else
                    {
                        hasError = true;
                    }
                    break;

                case GeneralValidationTestType.is_nonNegativeWholeNumber:
                    hasError = !IFM.Common.InputValidation.CommonValidations.IsNonNegativeWholeNumber(text);
                    break;

                case GeneralValidationTestType.isPositiveWholeNumber:
                    hasError = !IFM.Common.InputValidation.CommonValidations.IsPositiveWholeNumber(text);
                    break;
            }

            if (hasError && checkConditionButDontAddError == false)
            {
                switch (testype)
                {
                    case GeneralValidationTestType.requiredField: // fall through
                    case GeneralValidationTestType.requiredField_Date: // fall through
                    case GeneralValidationTestType.requiredField_Int:
                    case GeneralValidationTestType.requiredField_dd:
                        valList.Add(new ObjectValidation.ValidationItem("Missing " + fieldName, fieldId));
                        break;

                    case GeneralValidationTestType.has_ineligibleField: // fall through
                    case GeneralValidationTestType.has_ineligibleField_Date: // fall through
                    case GeneralValidationTestType.has_ineligibleField_Int:
                    case GeneralValidationTestType.has_ineligibleField_dd:
                    case GeneralValidationTestType.isValidSSN:
                    case GeneralValidationTestType.isValidBirthdate:
                    case GeneralValidationTestType.isValidEmailAddress:
                    case GeneralValidationTestType.isValidPhoneNumber:
                    case GeneralValidationTestType.isValidZipCode:
                    case GeneralValidationTestType.isValidPhoneExtension:
                    case GeneralValidationTestType.isValidDriverBirthDate:
                    case GeneralValidationTestType.isAlphaNumeric:
                    case GeneralValidationTestType.isDate:
                    case GeneralValidationTestType.isNumeric:
                    case GeneralValidationTestType.isNonNegativeNumber:
                    case GeneralValidationTestType.isDateInRange:
                    case GeneralValidationTestType.isGreaterThanZeroNumber:
                    case GeneralValidationTestType.isNumberInRange:
                    case GeneralValidationTestType.isTextLengthInRange:
                    case GeneralValidationTestType.isNotTextRepeated:
                    case GeneralValidationTestType.isNotTextSequential:
                    case GeneralValidationTestType.is_4digitYear:
                    case GeneralValidationTestType.is_nonNegativeWholeNumber:
                    case GeneralValidationTestType.isPositiveWholeNumber:
                        valList.Add(new ObjectValidation.ValidationItem("Invalid " + fieldName, fieldId));
                        break;
                }
            }
            return !hasError;
        }
    }
}