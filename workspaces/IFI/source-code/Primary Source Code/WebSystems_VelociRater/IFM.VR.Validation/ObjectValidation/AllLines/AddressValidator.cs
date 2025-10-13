using IFM.PrimativeExtensions;
using System;

namespace IFM.VR.Validation.ObjectValidation.AllLines
{
    public class AddressValidator
    {
        public const string ValidationListID = "{34DE6947-4B87-4127-8233-8F380FC55E38}";

        public const string StreetAndPoBoxEmpty = "{3FBBDA72-001F-4F65-8EFD-162A5A1FBB90}";
        public const string StreetAndPoxBoxAreSet = "{8CE2CFE6-C5B5-416E-B396-84252E2C41AA}";
        public const string AddressIsEmpty = "{658F59DD-2E95-4E24-967C-B5CD379302D3}";
        public const string HouseNumberID = "{50818066-17E9-4AAF-A124-D48E2F5FE52B}";

        public const string StreetNameID = "{2FE75939-1185-4DE0-A38B-A612673638E3}";
        public const string POBOXID = "{7CBE893F-DF16-41CE-8F65-11C3D219069A}";
        public const string CanNotHavePOBOX = "{4073699A-B9B4-4FF0-8257-A2696C19223A}";
        public const string ZipCodeID = "{33857A50-6D49-4A4A-BAB0-E72BF1375084}";
        public const string CityID = "{EB7272A1-41BC-4FA2-8EDD-B9BD9E711101}";
        public const string StateID = "{EE4B1915-61B4-4E51-A441-7089820F9CE2}";
        public const string AddressSatetNotIndiana = "{343DB64C-B73B-41B7-902B-C2984222C07C}";
        public const string CountyID = "{3BE1E668-8540-4AEE-A981-64D49FF4CD50}";
        public const string OtherEmpty = "{D82D75C7-644B-466C-A84D-E1FBDB5D1DA9}";

        public static string GetHomeVersion(QuickQuote.CommonObjects.QuickQuoteObject quote)
        {
            QuickQuote.CommonMethods.QuickQuoteHelperClass qqh = new QuickQuote.CommonMethods.QuickQuoteHelperClass();
            DateTime effectiveDate = DateTime.Today;
            string eDate = "";
            string HomeVersion = "";
            if (quote != null)
            {
                if (quote.EffectiveDate != null && quote.EffectiveDate != "")
                {
                    effectiveDate = Convert.ToDateTime(quote.EffectiveDate);
                }
                eDate = Convert.ToString(effectiveDate);
                if(qqh.doUseNewVersionOfLOB(quote, QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, Convert.ToDateTime("7/1/2018")))
                {
                    HomeVersion = "After20180701";
                }
                else
                {
                    HomeVersion = "Before20180701";
                }
            }
            return HomeVersion;
        }

        /// <summary>
        /// Validate partial house/street numbers.
        /// These are house/street numbers with a fraction, such as 33 1/2 or 123 1/3
        /// </summary>
        /// <param name="StreetNum"></param>
        /// <returns></returns>
        private static bool PartialStreetNumberOK(string StreetNum)
        {
            string pattern = null;
            char[] sep = { ' ' };
            System.Text.RegularExpressions.Regex R = null;

            // check the format of the passed string.  It must separate into two or more parts - the last part must be the fraction
            string[] parts = StreetNum.Split(sep);
            if (parts.Length < 2)
            {
                // Error - not enough parts.  Invalid format.
                return false;
            }
            else
            {
                // Last part must have the '/' in it
                if (!parts[parts.Length-1].Contains("/"))
                {
                    return false;
                }

                // Format OK
                string partial = parts[parts.Length-1].Trim();

                // Validate the non-fraction part of the house number
                foreach (string hnpart in parts)
                {
                    if (hnpart != partial) // Don't check the fractional part
                    {
                        // Whole house number must be alphanumeric, allow numbers, letters, spaces
                        pattern = "^[a-zA-Z0-9 ]";
                        R = new System.Text.RegularExpressions.Regex(pattern);
                        if (!R.IsMatch(hnpart)) { return false; }
                    }
                }

                // Validate the fractional (partial) part of the house number
                // Partial house number must be a "1", followed by '/', followed by another single digit.  Like 1/2 or 1/3 or 1/4 - up to 1/9.  Regex it.
                pattern = "[1]+[/]+[1-9]";
                if (!R.IsMatch(partial))
                {
                    // No Match = invalid partial address
                    return false;
                }
            }

            // If we made it here the house number is valid
            return true;
        }

        public static Validation.ObjectValidation.ValidationItemList AddressValidation(QuickQuote.CommonObjects.QuickQuoteAddress Address, ValidationItem.ValidationType valType, bool mustBeInIndiana = false, bool mustNotHavePoxBox = false, QuickQuote.CommonObjects.QuickQuoteObject quote = null, bool countyRequired = true)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            //Updated 1/23/18 for HOM Upgrade MLW - was just else statement after quote != null
            if (quote != null)
            {
                var MyLocation = quote.Locations[0];
                string HomeVersion = GetHomeVersion(quote);
                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                {
                    if (Address != null)
                    {
                        //Updated 2/26/18 for HOM Upgrade MLW
                        if (((string.IsNullOrWhiteSpace(Address.HouseNum) == false && Address.HouseNum.Trim().ToUpper() != "NEED STREET #") || (string.IsNullOrWhiteSpace(Address.StreetName) == false && Address.StreetName.Trim().ToUpper() != "NEED STREET NAME")) && string.IsNullOrWhiteSpace(Address.POBox) == false)
                        {
                            // house num or street name and po box are set
                            valList.Add(new ObjectValidation.ValidationItem("Street Number and Name or PO Box Required", StreetAndPoxBoxAreSet));
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(Address.HouseNum) && string.IsNullOrWhiteSpace(Address.StreetName) && string.IsNullOrWhiteSpace(Address.POBox))
                            {
                                // all are missing
                                valList.Add(new ObjectValidation.ValidationItem("Street Number and Name or PO Box Required", StreetAndPoBoxEmpty));
                            }
                            else
                            {
                                // test street num and street name or PO box
                                //Updated 2/26/18 for HOM Upgrade MLW
                                if ((string.IsNullOrWhiteSpace(Address.HouseNum) == false || Address.HouseNum.Trim().ToUpper() != "NEED STREET #") | (string.IsNullOrWhiteSpace(Address.StreetName) == false || Address.StreetName.Trim().ToUpper() != "NEED STREET NAME"))
                                {
                                    // STREET NUMBER
                                    if (VRGeneralValidations.Val_HasRequiredField(Address.HouseNum, valList, HouseNumberID, "Street Number"))
                                    {
                                        // House number validation must allow partial house numbers such as 1/2 or 1/3
                                        if (Address.HouseNum.Contains("/"))
                                        {
                                            if (!PartialStreetNumberOK(Address.HouseNum))
                                            {
                                                valList.Add(new ObjectValidation.ValidationItem("Street Number is in an invalid format.", HouseNumberID));
                                            }
                                        }
                                        else
                                        {
                                            // There is no '/' in house number, validate as simple numeric
                                            VRGeneralValidations.Val_IsValidAlphaNumeric(Address.HouseNum, valList, HouseNumberID, "Street Number");
                                        }
                                    }
                                    // STREET NAME
                                    VRGeneralValidations.Val_HasRequiredField(Address.StreetName, valList, StreetNameID, "Street Name");
                                }
                                else
                                {
                                    // test po Box
                                    if (VRGeneralValidations.Val_HasRequiredField(Address.POBox, valList, POBOXID, "PO Box"))
                                    {
                                        if (mustNotHavePoxBox)
                                            valList.Add(new ObjectValidation.ValidationItem("Address must not have a PO Box", CanNotHavePOBOX));
                                    }
                                }
                            }
                        }

                        //Updated 2/26/18 for HOM Upgrade MLW
                        if (Address.Zip.Trim().ToUpper() != "00001" && Address.Zip.Trim().ToUpper() != "00001-0000") {
                            if (VRGeneralValidations.Val_HasRequiredField(Address.Zip, valList, ZipCodeID, "Zip Code"))
                                VRGeneralValidations.Val_IsValidZipCode(Address.Zip, valList, ZipCodeID, "Zip Code");
                        }

                        VRGeneralValidations.Val_HasRequiredField(Address.City, valList, CityID, "City");

                        VRGeneralValidations.Val_HasRequiredField(Address.Other, valList, OtherEmpty, "Other");

                        //Updated 2/26/18 for HOM Upgrade MLW
                        if (Address.StateId.Trim()!= "999")
                            //&& Address.StateId.Trim() != "99")
                        {
                            if (VRGeneralValidations.Val_HasRequiredField_DD(Address.StateId, valList, StateID, "State"))
                            {
                                VRGeneralValidations.Val_IsNonNegativeWholeNumber(Address.StateId, valList, StateID, "State");
                                if (mustBeInIndiana)
                                {
                                    if (Address.StateId != "16")
                                        valList.Add(new ObjectValidation.ValidationItem("Property must be located in Indiana", AddressSatetNotIndiana));
                                }
                            }
                        }
                        //Updated 02/17/2020 for Home Endorsements task 44249 MLW
                        if (quote.QuoteTransactionType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                        {
                            if (countyRequired == true)
                            {
                                VRGeneralValidations.Val_HasRequiredField(Address.County, valList, CountyID, "County");
                            }                           
                        } else
                        {
                            VRGeneralValidations.Val_HasRequiredField(Address.County, valList, CountyID, "County");
                        }                                                      
                    }
                    else
                    {
                        valList.Add(new ObjectValidation.ValidationItem("Address Information is null", AddressIsEmpty));
                    }
                }
                else
                {
                    //go back and validate without the quote object passed
                    AddressValidation(Address, valType, mustBeInIndiana, mustNotHavePoxBox);
                }
            }
            else
            {
                if (Address != null)
                {
                    if ((string.IsNullOrWhiteSpace(Address.HouseNum) == false || string.IsNullOrWhiteSpace(Address.StreetName) == false) && string.IsNullOrWhiteSpace(Address.POBox) == false)
                    {
                        // house num or street name and po box are set
                        valList.Add(new ObjectValidation.ValidationItem("Street Number and Name or PO Box Required", StreetAndPoxBoxAreSet));
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(Address.HouseNum) && string.IsNullOrWhiteSpace(Address.StreetName) && string.IsNullOrWhiteSpace(Address.POBox))
                        {
                            // all are missing
                            valList.Add(new ObjectValidation.ValidationItem("Street Number and Name or PO Box Required", StreetAndPoBoxEmpty));
                        }
                        else
                        {
                            // test street num and street name or PO box
                            if (string.IsNullOrWhiteSpace(Address.HouseNum) == false | string.IsNullOrWhiteSpace(Address.StreetName) == false)
                            {
                                // check street num and street name
                                // STREET NUMBER
                                if (VRGeneralValidations.Val_HasRequiredField(Address.HouseNum, valList, HouseNumberID, "Street Number"))
                                {
                                    // House number validation must allow partial house numbers such as 1/2 or 1/3
                                    if (Address.HouseNum.Contains("/"))
                                    {
                                        if (!PartialStreetNumberOK(Address.HouseNum))
                                        {
                                            valList.Add(new ObjectValidation.ValidationItem("Street Number is in an invalid format.", HouseNumberID));
                                        }
                                    }
                                    else
                                    {
                                        // There is no '/' in house number, validate as simple numeric
                                        VRGeneralValidations.Val_IsValidAlphaNumeric(Address.HouseNum, valList, HouseNumberID, "Street Number");  
                                    }
                                }
                                // STREET NAME
                                VRGeneralValidations.Val_HasRequiredField(Address.StreetName, valList, StreetNameID, "Street Name");
                            }
                            else
                            {
                                // test po Box
                                if (VRGeneralValidations.Val_HasRequiredField(Address.POBox, valList, POBOXID, "PO Box"))
                                {
                                    if (mustNotHavePoxBox)
                                        valList.Add(new ObjectValidation.ValidationItem("Address must not have a PO Box", CanNotHavePOBOX));
                                }
                            }
                        }
                    }

                    if (VRGeneralValidations.Val_HasRequiredField(Address.Zip, valList, ZipCodeID, "Zip Code"))
                        VRGeneralValidations.Val_IsValidZipCode(Address.Zip, valList, ZipCodeID, "Zip Code");

                    VRGeneralValidations.Val_HasRequiredField(Address.City, valList, CityID, "City");

                    VRGeneralValidations.Val_HasRequiredField(Address.Other, valList, OtherEmpty, "Other");

                    if (VRGeneralValidations.Val_HasRequiredField_DD(Address.StateId, valList, StateID, "State"))
                    {
                        VRGeneralValidations.Val_IsNonNegativeWholeNumber(Address.StateId, valList, StateID, "State");
                        if (mustBeInIndiana)
                        {
                            if (Address.StateId != "16")
                                valList.Add(new ObjectValidation.ValidationItem("Property must be located in Indiana", AddressSatetNotIndiana));
                        }
                    }

                    if (countyRequired)
                    {
                        VRGeneralValidations.Val_HasRequiredField(Address.County, valList, CountyID, "County");
                    }
                }
                else
                {
                    valList.Add(new ObjectValidation.ValidationItem("Address Information is null", AddressIsEmpty));
                }
            }

            return valList;
        }
    }
}