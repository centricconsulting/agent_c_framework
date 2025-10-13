using QuickQuote.CommonMethods;
using System;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class HOMEmailValidator
    {
        public const string ValidationListID = "{4B617EED-1585-4E1B-8764-D91E1B5D772D}";

        public const string QuoteIsNull = "{DD70B503-B41B-5E73-A906-BE96A3779FC0}";
        public const string NoLocations = "{CA3806F5-B7A6-5D88-A89D-48CAC3EFFED0}";

        public const string EmailRequiredWithWoodburningStove = "{8D72D362-BDA0-41F0-BD89-4DC6C7B824B3}";
        public const string EmailEmpty = "{7D72D472-BDB0-41F0-BD77-4DC7C7B824B1}";
        public const string EmailInvalid = "{5D72D332-CDA0-41F0-BD55-4DC8C7B824B2}";
        public const string EmailListIsNull = "{3D72D361-BCA0-42F0-BD33-4DC6C7B124B4}";

        public static Validation.ObjectValidation.ValidationItemList ValidateHOMPolicyholderEmails(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            QuickQuoteHelperClass QQHelper = new QuickQuoteHelperClass();

            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.LocationIndex, "0");

            if (quote != null)
            {
                bool emailFound = false;
                string email = "";
                bool WoodburningIsNewToImage = false;

                if (IFM.VR.Common.Helpers.AllLines.RequiredEmailHelper.IsRequiredEmailAvailable(quote) == true)
                {
                    if (quote.Policyholder.Emails != null && quote.Policyholder.Emails.Any())
                    {
                        // PH1
                        if (quote.Policyholder.PrimaryEmail != "")
                        {
                            email = quote.Policyholder.PrimaryEmail;
                            emailFound = true;
                        }
                    }

                    if (!emailFound && quote.Policyholder2.Emails != null && quote.Policyholder2.Emails.Any())
                    {
                        // PH2
                        if (quote.Policyholder2.PrimaryEmail != "")
                        {
                            email = quote.Policyholder2.PrimaryEmail;
                            emailFound = true;
                        }
                    }

                    if (emailFound)
                    {
                        // Validate the email we found
                        if (VRGeneralValidations.Val_HasRequiredField(email, valList, EmailEmpty, "Email"))
                            VRGeneralValidations.Val_IsValidEmailAddress(email, valList, EmailInvalid, "Email");
                    }
                    else
                    {
                        VRGeneralValidations.Val_HasRequiredField(email, valList, EmailEmpty, "Email");
                    }
                }
                else
                {
                    if (quote.Locations[0].WoodOrFuelBurningApplianceSurcharge)
                    {

                        if (quote.GetDevDictionaryItem("", "HadWoodburningSurchargeOnPreviousImage") != "")
                        {
                            WoodburningIsNewToImage = !Convert.ToBoolean(quote.GetDevDictionaryItem("", "HadWoodburningSurchargeOnPreviousImage"));
                        }
                        if (WoodburningIsNewToImage && (quote.Policyholder.Emails != null && quote.Policyholder.Emails.Any()))
                        {
                            // PH1
                            if (quote.Policyholder.PrimaryEmail != "")
                            {
                                email = quote.Policyholder.PrimaryEmail;
                                emailFound = true;
                            }
                        }

                        if (WoodburningIsNewToImage && !emailFound && quote.Policyholder2.Emails != null && quote.Policyholder2.Emails.Any())
                        {
                            // PH2
                            if (quote.Policyholder2.PrimaryEmail != "")
                            {
                                email = quote.Policyholder2.PrimaryEmail;
                                emailFound = true;
                            }
                        }

                        if (emailFound)
                        {
                            // Validate the email we found
                            if (VRGeneralValidations.Val_HasRequiredField(email, valList, EmailEmpty, "Email"))
                                VRGeneralValidations.Val_IsValidEmailAddress(email, valList, EmailInvalid, "Email");
                        }
                        else
                        {
                            // No email found - only flag this if the wood burning is new to the image
                            if (WoodburningIsNewToImage)
                            {
                                valList.Add(new ObjectValidation.ValidationItem("An email is required with wood burning stove.", EmailRequiredWithWoodburningStove));
                            }
                        }
                    }
                }
            }
            else
            {
                valList.Add(new ObjectValidation.ValidationItem("Quote is null", QuoteIsNull));
            }

            return valList;
        }
    }
}