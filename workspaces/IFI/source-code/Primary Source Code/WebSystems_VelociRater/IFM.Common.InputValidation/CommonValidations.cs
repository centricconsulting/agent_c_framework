using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IFM.Common.InputValidation
{
    public static class CommonValidations
    {
        // ******************************************************************************************************************************************************************************
        // ******************************************************************************************************************************************************************************
        //  DO NOT ADD ANY ADDITIONAL REFERENCES TO THIS LIBRARY - WE WANT IT TO BE PORTABLE BECAUSE ONCE YOU START TO USE THESE YOU WILL WANT THEM EVERYWHERE
        //  IN ORDER FOR IT TO BE PORTABLE DONT BIND IT TO THINGS LIKE DIAMOND EITHER DIRECTLY OR INDIRECTLY BY ADDING REFERENCE TO SOMETHING THAT HAS DIAMOND LIBS AS A DEPENDENCY
        // ******************************************************************************************************************************************************************************
        // ******************************************************************************************************************************************************************************

        /// <summary>
        /// Validates a string as a boolean
        /// Returns true if a valid boolean, false if not
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsBoolean(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return false;
            s = s.ToUpper();
            if (s == "TRUE" | s == "FALSE")
                return true;
            if (s == "0" | s == "1" | s == "-1")
                return true;
            return false;
        }

        /// <summary>
        /// Returns true only if the passed string has a number in it
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool StringHasNumericValue(string s)
        {
            if (string.IsNullOrWhiteSpace(s) || (!Information.IsNumeric(s)))
                return false;
            return true;
        }

        /// <summary>
        /// Returns true if the passed string has any value in it
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool StringHasAnyValue(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return false;
            return true;
        }

        public static bool IsValidAddress(string address)
        {
            return (RegExTester(address, RegexPatterns.ADDRESS) && address.Length <= 50);
        }

        public static bool IsValidAlphaNumeric(string alphaString)
        {
            return (RegExTester(alphaString, RegexPatterns.ALPHANUMERIC_MAX50));
        }

        public static bool IsAlphaNum(string strInputText, string exceptionListAsString = "")
        {
            bool IsAlpha = false;
            if (string.IsNullOrWhiteSpace(strInputText) == false)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(strInputText, "^[a-zA-Z0-9 " + ((string.IsNullOrWhiteSpace(exceptionListAsString) == false) ? exceptionListAsString : "") + "]+$"))
                {
                    IsAlpha = true;
                }
                else
                {
                    IsAlpha = false;
                }
            }
            return IsAlpha;
        }

        public static bool IsValidBizName(string businessName)
        {
            return (RegExTester(businessName, RegexPatterns.BUSINESSNAME) && businessName.Length <= 150);
        }

        public static bool IsValidCity(string city)
        {
            return RegExTester(city, RegexPatterns.CITY_WORLD);
        }

        public static bool IsValidEmail(string email)
        {
            return RegExTester(email, RegexPatterns.EMAIL);
        }

        /// <summary>
        /// Returns true on numbers greater than 0
        /// </summary>
        /// <param name="numberText"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsPositiveWholeNumber(string numberText)
        {
            numberText = CleanUpNumericStrings(numberText);
            if (!string.IsNullOrWhiteSpace(numberText))
            {
                if (Information.IsNumeric(numberText))
                {
                    try
                    {
                        long val = Convert.ToInt64(numberText);
                        // used for whole number test
                        if (val == Convert.ToDecimal(numberText) & val > 0)
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Is Greater than 0
        /// </summary>
        /// <param name="numberText"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsPositiveNumber(string numberText)
        {
            numberText = CleanUpNumericStrings(numberText);
            if (!string.IsNullOrWhiteSpace(numberText))
            {
                if (Information.IsNumeric(numberText))
                {
                    try
                    {
                        return decimal.Parse(numberText) > 0;
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Is 0 or greater
        /// </summary>
        /// <param name="numberText"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsNonNegativeNumber(string numberText)
        {
            if (!string.IsNullOrWhiteSpace(numberText))
            {
                numberText = numberText.Trim();
                if (numberText.FirstOrDefault() == '(' && numberText.LastOrDefault() == ')')
                {
                    numberText = "-" + CleanUpNumericStrings(numberText);
                }
                else
                {
                    numberText = CleanUpNumericStrings(numberText);
                }

                if (Information.IsNumeric(numberText))
                {
                    try
                    {
                        return decimal.Parse(numberText) >= 0;
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Is whole number 0 or greater
        /// </summary>
        /// <param name="numberText"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsNonNegativeWholeNumber(string numberText)
        {
            numberText = CleanUpNumericStrings(numberText);
            if (!string.IsNullOrWhiteSpace(numberText))
            {
                if (Information.IsNumeric(numberText))
                {
                    try
                    {
                        return decimal.Parse(numberText) >= 0 & decimal.Parse(numberText) == Convert.ToInt64(numberText);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return false;
        }

        public static string CleanUpNumericStrings(string inputText)
        {
            if (!string.IsNullOrWhiteSpace(inputText))
            {
                char[] removeMeArray = {
            Convert.ToChar("$"),
            Convert.ToChar("%"),
            Convert.ToChar(","),
            Convert.ToChar("#"),
            Convert.ToChar("("),
            Convert.ToChar(")"),
            Convert.ToChar("+"),
            Convert.ToChar("!"),
            Convert.ToChar("@"),
            Convert.ToChar("^"),
            Convert.ToChar("&"),
            Convert.ToChar("*"),
            Convert.ToChar("="),
            Convert.ToChar("`"),
            Convert.ToChar("~")
        };
                foreach (char trash in removeMeArray)
                {
                    inputText = inputText.Replace(trash.ToString(), "");
                }
            }
            return inputText;
        }

        public static bool ContainsProblematicChars(string inputText, string excludingChar = null)
        {
            char[] removeMeArray = {
        Convert.ToChar("$"),
        Convert.ToChar("%"),
        Convert.ToChar("#"),
        Convert.ToChar("("),
        Convert.ToChar(")"),
        Convert.ToChar("+"),
        Convert.ToChar("!"),
        Convert.ToChar("@"),
        Convert.ToChar("^"),
        Convert.ToChar("&"),
        Convert.ToChar("*"),
        Convert.ToChar("="),
        Convert.ToChar("`"),
        Convert.ToChar("~"),
        Convert.ToChar("/")
    };
            //CChar(",")
            foreach (char st in removeMeArray)
            {
                if (excludingChar != null)
                {
                    if (st.ToString() != excludingChar)
                    {
                        if (inputText.Contains(st))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (inputText.Contains(st))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsPositiveWholeNumberOrEmptyOrNullOrWhitespace(string inputText)
        {
            return string.IsNullOrWhiteSpace(inputText) | IsNonNegativeWholeNumber(inputText);
        }

        public static bool IsNumberInRange(string number, double minValue, double maxvalue)
        {
            if (string.IsNullOrWhiteSpace(number) == false)
                return InputHelpers.TryToGetDouble(number) >= minValue & InputHelpers.TryToGetDouble(number) <= maxvalue;
            return false;
        }

        public static bool IsDateInRange(string date, string minDate, string maxDate)
        {
            DateTime d, mindate, maxdate;
            if (DateTime.TryParse(date, out d) & DateTime.TryParse(minDate, out mindate) & DateTime.TryParse(maxDate, out maxdate))
            {
                // remove time component of datetimes and test
                d = DateTime.Parse(d.ToShortDateString());
                mindate = DateTime.Parse(mindate.ToShortDateString());
                maxdate = DateTime.Parse(maxdate.ToShortDateString());
                return d >= mindate & d <= maxdate;
            }
            return false;
        }

        public static bool IsTextLenghtInRange(string text, uint minLength, uint maxLegth)
        {
            if (string.IsNullOrWhiteSpace(text) == false)
            {
                return text.Length >= minLength & text.Length <= maxLegth;
            }
            else
            {
                return minLength == 0;
            }
        }

        public static bool IsValidName(string name)
        {
            return RegExTester(name, RegexPatterns.NAME_MAX50);
        }

        public static bool IsValidPhone(string phone)
        {
            bool isValid = RegExTester(phone, RegexPatterns.IFMPHONENUMBER);
            return isValid;
        }

        public static bool IsValidStateRegion(string state)
        {
            return RegExTester(state, RegexPatterns.ALPHABETIC_MAX5);
        }

        public static bool IsValidWebsite(string website)
        {
            return RegExTester(website, RegexPatterns.URL);
        }

        public static bool IsValidZipCode(string zipcode)
        {
            if (zipcode != "00000-0000" & zipcode != "00000")
            {
                return RegExTester(zipcode, RegexPatterns.POSTAL_CODE);
            }
            return false;
        }

        public static bool IsValidSSN(string SSN)
        {
            if (string.IsNullOrWhiteSpace(SSN) == false)
            {
                SSN = SSN.Replace("-", "").Trim();
                return SSN != "000000000" & SSN.Length == 9 & (RegExTester(SSN, "\\d{9}$"));// || RegExTester(SSN, "^(?!(000|666|9))\\d{3}-(?!00)\\d{2}-(?!0000)\\d{4}$"));
            }
            return false;
        }

        public static bool IsAlphabetOnly(string text)
        {
            if (string.IsNullOrWhiteSpace(text) == false)
            {
                bool isAlphabetic = RegExTester(text, "[a-zA-Z\\s]+");
                return isAlphabetic;
            }
            return false;
        }

        public static bool RegExTester(string valueToTest, string regExpPattern)
        {
            bool isValid = false;

            try
            {
                if (string.IsNullOrWhiteSpace(valueToTest) == false && string.IsNullOrWhiteSpace(regExpPattern) == false)
                {
                    Regex regEx = new Regex(regExpPattern);
                    isValid = regEx.IsMatch(valueToTest);
                }
            }
            catch
            {
            }

            return isValid;
        }

        public static bool IsRepeatedCharacters(string text)
        {
            if (string.IsNullOrWhiteSpace(text) == false && string.IsNullOrWhiteSpace(text.Trim()) == false)
            {
                // take first char and do replace on that char. If all chars are the same you will have an empty string.
                if (text.Replace(text[0].ToString(), "").Length == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Only works with positive numbers or alphabetic chars
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsAlhaNumericlySequential(string text)
        {
            //works to find sequential in things like
            //123456789
            //abcdefghi
            //mnopqrstu
            //ihgfedcba
            //987654321

            //Does not work for things like
            //1234567890 ' wrapped back to zero
            //xyzabcde ' wrapped back to A
            if (string.IsNullOrWhiteSpace(text) == false)
            {
                text = text.ToLower().Trim().Replace(",", "").Replace("-", "");
                if (text != null && text.Length > 1)
                {
                    List<Int32> listOfInts = new List<Int32>();
                    if (Information.IsNumeric(text))
                    {
                        // convert into a list of ints
                        int currentDigit = -1;
                        foreach (char c in text)
                        {
                            if (Int32.TryParse(c.ToString(), out currentDigit))
                            {
                                listOfInts.Add(currentDigit);
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        //alphabetic sequence
                        Int32 currentDigit = -1;
                        foreach (char c in text)
                        {
                            currentDigit = Convert.ToInt32(c);
                            listOfInts.Add(currentDigit);
                        }
                    }
                    return IsSequenceSequential(listOfInts);
                }
            }
            return false;
        }

        private static bool IsSequenceSequential(List<Int32> sequence)
        {
            // need to know direction to avoid
            // 1,2,3,2,1 if you only check that each is 1 apart that would look sequential

            bool charsAreSequential = true;
            Int32 digitFlowDirection = 0;
            // 0 = unknown, -1 = desc, 1 = asc
            Int32 lastDigit = Int32.MinValue;
            if (sequence != null && sequence.Count > 1)
            {
                foreach (Int32 currentDigit in sequence)
                {
                    int thisLoopDirection = 0;
                    //0 = unknown, -1 = desc, 1 = asc
                    if (true)
                    {
                        if (lastDigit != Int32.MinValue)
                        {
                            if (lastDigit > currentDigit)
                            {
                                thisLoopDirection = -1;
                            }
                            else
                            {
                                thisLoopDirection = 1;
                            }
                            if (digitFlowDirection != 0)
                            {
                                if (thisLoopDirection != digitFlowDirection)
                                {
                                    // not the same
                                    charsAreSequential = false;
                                    break; // TODO: might not be correct. Was : Exit For
                                }
                                else
                                {
                                    //same direction as last time
                                    if (Math.Abs(currentDigit - lastDigit) > 1)
                                    {
                                        charsAreSequential = false;
                                        break; // TODO: might not be correct. Was : Exit For
                                    }
                                }
                            }
                        }
                        else
                        {
                            //first loop no last digit
                            // first loop unknown direction
                            Int32 secondDigit = sequence[1];
                            if (currentDigit < secondDigit)
                            {
                                digitFlowDirection = 1;
                            }
                            else
                            {
                                digitFlowDirection = -1;
                            }
                        }
                        lastDigit = currentDigit;
                    }
                }
            }
            else
            {
                charsAreSequential = false;
            }

            return charsAreSequential;
        }

        public static bool IsEmptyOrDefaultDiamond_Date(string date)
        {
            if (string.IsNullOrWhiteSpace(date) == false)
            {
                try
                {
                    if (DateTime.Parse(IFM.Common.InputValidation.InputHelpers.DiamondDefaultDate_string).ToString() == DateTime.Parse(date).ToString())
                    {
                        return true;
                    }
                }
                catch
                { }
                return false;
            }
            return true;
        }

        public static bool IsEmptyOrDefaultDiamond_Int(string numericValue)
        {
            if (string.IsNullOrWhiteSpace(numericValue) == false)
            {
                return numericValue.Trim() == "0";
            }
            return true;
        }
    }

    public static class RegexPatterns
    {
        public const string ADDRESS = "^[a-zA-Z\\d]+(([\\'\\,\\.\\- #][a-zA-Z\\d ])?[a-zA-Z\\d]*[\\.]*)*$";
        public const string ALPHABETIC_MAX5 = "^[a-zA-Z]{1,5}$";
        public const string ALPHANUMERIC_MAX5 = "^[a-zA-Z0-9\\-]{1,5}$";
        public const string ALPHANUMERIC_MAX10 = "^[a-zA-Z0-9\\-]{1,10}$";
        public const string ALPHANUMERIC_MAX15 = "^[a-zA-Z0-9\\-]{1,15}$";
        public const string ALPHANUMERIC_MAX20 = "^[a-zA-Z0-9\\-]{1,20}$";
        public const string ALPHANUMERIC_MAX25 = "^[a-zA-Z0-9\\-]{1,25}$";
        public const string ALPHANUMERIC_MAX30 = "^[a-zA-Z0-9\\-]{1,30}$";
        public const string ALPHANUMERIC_MAX50 = "^[a-zA-Z0-9\\-]{1,50}$";
        public const string BUSINESSNAME = "^[a-zA-Z0-9]+[a-zA-Z0-9 &amp;\\'\\,\\.\\-]*[a-zA-Z\\.]$";
        public const string CITY = "^[a-zA-Z]{1}[a-zA-Z]{2,}$|^[a-zA-Z]{1}[a-zA-Z]{2,}[ ]{1}[a-zA-Z]{2,32}$";
        public const string CITY_WORLD = "^[a-zA-Z]{1}[a-zA-Z\\.\\s\\-]{2,49}$";
        public const string EMAIL = "^([0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";
        public const string GENERAL = "^[^&lt;&gt;`/@\\}:;)(^{&amp;*']$";
        public const string GENERAL_MAX5 = "^[^&lt;&gt;`/@\\}:;)(^{&amp;*']{1,5}$";
        public const string GENERAL_MAX10 = "^[^&lt;&gt;`/@\\}:;)(^{&amp;*']{1,10}$";
        public const string GENERAL_MAX15 = "^[^&lt;&gt;`/@\\}:;)(^{&amp;*']{1,15}$";
        public const string GENERAL_MAX20 = "^[^&lt;&gt;`/@\\}:;)(^{&amp;*']{1,20}$";
        public const string GENERAL_MAX25 = "^[^&lt;&gt;`/@\\}:;)(^{&amp;*']{1,25}$";
        public const string GENERAL_MAX30 = "^[^&lt;&gt;`/@\\}:;)(^{&amp;*']{1,30}$";
        public const string GENERAL_MAX50 = "^[^&lt;&gt;`/@\\}:;)(^{&amp;*']{1,50}$";
        public const string GENERAL_MAX100 = "^[^&lt;&gt;`/@\\}:;)(^{&amp;*']{1,100}$";
        public const string GENERAL_TEXTAREA = "([^&lt;&gt;`/@\\}:;)(^{&amp;*'](\\s+)?)+";
        public const string COMMON = "^[A-Za-z0-9]{1}[A-Za-z0-9\\'\\-\\. \\s]{0,250}$";
        public const string GUID = "^([A-Za-z0-9]{8}[-][A-Za-z0-9]{4}[-][A-Za-z0-9]{4}[-][A-Za-z0-9]{4}[-][A-Za-z0-9]{12})|([({]{1}[A-Za-z0-9]{8}[-][A-Za-z0-9]{4}[-][A-Za-z0-9]{4}[-][A-Za-z0-9]{4}[-][A-Za-z0-9]{12}[})]{1})|([A-Za-z0-9]{32})$";
        public const string INTEGER_POSITIVE = "^\\d$";
        public const string INTEGER_MAX5 = "^(-)?\\d{1,5}$";
        public const string INTEGER_MAX10 = "^(-)?\\d{1,10}$";
        public const string INTEGER_MAX15 = "^(-)?\\d{1,15}$";
        public const string INTEGER_MAX20 = "^(-)?\\d{1,20}$";
        public const string INTEGER_NOZERO_MAX5 = "^(-)?[1-9]{1}\\d{0,4}$";
        public const string INTEGER_NOZERO_MAX10 = "^(-)?[1-9]{1}\\d{0,9}$";
        public const string INTEGER_NOZERO_MAX15 = "^(-)?[1-9]{1}\\d{0,14}$";
        public const string NAME_MAX15 = "^[A-Za-z]{1}[A-Za-z\\'\\-\\. ]{1,15}$";
        public const string NAME_MAX25 = "^[A-Za-z]{1}[A-Za-z\\'\\-\\. ]{1,25}$";
        public const string NAME_MAX30 = "^[A-Za-z]{1}[A-Za-z\\'\\-\\. ]{1,30}$";
        public const string NAME_MAX50 = "^[A-Za-z]{1}[A-Za-z\\'\\-\\. ]{1,50}$";
        public const string NUMERIC_NOZERO = "^(([1-9]{1}[0-9]{0,11})|(([1-9]{1}[0-9]{0,2})(([\\,]{1}[0-9]{3}){0,3})))((\\.[0-9]{1,5})?)$";

        public const string PASSWORD = "(?-i)(?=^.{6,}$)((?!.*\\s)(?=.*[A-Z])(?=.*[a-z]))((?=(.*\\d){1,})|(?=(.*[a-zA-Z0-9]){1,}))^[a-zA-Z0-9]{6,20}$";

        // Matt A 10-22-14
        public const string IFMPHONENUMBER = "^\\([0-9]{3}\\)[0-9]{3}\\-[0-9]{4}$";

        //Public Const PHONENUMBER As String = "^(([2-9]{1}[0-9]{9})|([2-9]{1}[0-9]{2}[\-]{1}[0-9]{3}[\-]{1}[0-9]{4})|([2-9]{1}[0-9]{2}[\.]{1}[0-9]{3}[\.]{1}[0-9]{4})|(([\(]{1}[2-9]{1}[0-9]{2}[\)]{1}[\ ]?)([0-9]{3}(\-|\.)?[0-9]{4})))$"
        //Public Const PHONENUMBER_FILTERED As String = "^(([2-9]{1}[0-9]{2}[\-]{1}[0-9]{3}[\-]{1}[0-9]{4})|(___-___-____))$"
        //Public Const PHONENUMBER_WORLD As String = "^[+]?([0-9]*[\.\s\-\(\)]|[0-9]+){3,20}$"
        public const string POSTAL_CODE = "(^\\d{5}$)|(^\\d{5}-\\d{4}$)";

        public const string PRODUCT_TYPE_NAME = "^[^&lt;>!/@}%)(^{&amp;*=|+]{1,150}$";
        public const string STATE = "^[A-Za-z]{2}$";
        public const string URL = "^(((http://)|(https://)|(www))([A-Za-z0-9./&amp;?=-_]{3,140}))$";
        public const string USERNAME = "(^([0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$)|(^[^&lt;&gt;!/}%)(^{&amp;*=|+]{1,20}$)";
    }
}