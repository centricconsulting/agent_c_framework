using System;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class RVWatercraftOperatorsValidator
    {
        public const string ValidationListID = "{E5F26DD1-49CB-410D-A43B-F23C185A1799}";

        public const string OperatorIsNull = "{F789C9E3-346E-4209-8200-6E5550927C5D}";

        public const string FirstName = "{E8E0F95F-796E-44E2-AA1F-7EF5C324F1D4}";
        public const string LastName = "{FF46D679-FD2C-456A-B2A6-A9CBD94D46AC}";
        public const string BirthDate = "{E6FB27A7-0699-464C-AB27-6C2581CD82CD}";

        public static Validation.ObjectValidation.ValidationItemList ValidateRvWaterCraftOperator(QuickQuote.CommonObjects.QuickQuoteOperator op, ValidationItem.ValidationType valType)
        {
           
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (op != null)
            {
                switch (valType)
                {
                    case ValidationItem.ValidationType.quoteRate:
                        break;

                    case ValidationItem.ValidationType.appRate:
                    case ValidationItem.ValidationType.issuance:

                        if (op.Name.FirstName.ToLower().Trim() == "youthful")
                        {
                            valList.Add(new ValidationItem("Invalid First Name", FirstName));
                        }

                        if (op.Name.LastName.ToLower().Trim() == "operator")
                        {
                            valList.Add(new ValidationItem("Invalid Last Name", LastName));
                        }

                        if (Microsoft.VisualBasic.Information.IsDate(op.Name.BirthDate))
                        {
                            // must be under 25 years of age
                            if (DateTime.Parse(op.Name.BirthDate).AddYears(25) < DateTime.Now)
                            {
                                // older than 25
                                valList.Add(new ValidationItem("Invalid Birth Date", BirthDate));
                            }
                        }

                        // check operator
                        var vals = IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.ValidateNameObject(op.Name, valType);
                        foreach (var v in vals)
                        {
                            switch (v.FieldId)
                            {
                                case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.CommAndPersNameComponentsEmpty:
                                    valList.Add(new ValidationItem("Invalid FirstName", FirstName));
                                    valList.Add(new ValidationItem("Invalid LastName", LastName));
                                    break;

                                case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.FirstNameID:
                                    v.FieldId = FirstName;
                                    valList.Add(v);
                                    break;

                                case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.LastNameID:
                                    v.FieldId = LastName;
                                    valList.Add(v);
                                    break;

                                case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.BirthDate:
                                    v.FieldId = BirthDate;
                                    valList.Add(v);
                                    break;
                            }
                        }
                        break;
                    case ValidationItem.ValidationType.endorsement:
                        ValidateEndorsement(op,valList);
                        break;
                }
            }
            else
            {
                valList.Add(new ObjectValidation.ValidationItem("Operator is null", OperatorIsNull));
            }

            return valList;
        }

        private static void ValidateEndorsement(QuickQuote.CommonObjects.QuickQuoteOperator op, Validation.ObjectValidation.ValidationItemList valList)
        {
            if (op.Name.FirstName.ToLower().Trim() == "youthful")
            {
                valList.Add(new ValidationItem("Invalid First Name", FirstName));
            }

            if (op.Name.LastName.ToLower().Trim() == "operator")
            {
                valList.Add(new ValidationItem("Invalid Last Name", LastName));
            }           

            // check operator
            var vals = IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.ValidateNameObject(op.Name, ValidationItem.ValidationType.endorsement);
            foreach (var v in vals)
            {
                switch (v.FieldId)
                {
                    case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.CommAndPersNameComponentsEmpty:
                        valList.Add(new ValidationItem("Invalid FirstName", FirstName));
                        valList.Add(new ValidationItem("Invalid LastName", LastName));
                        break;

                    case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.FirstNameID:
                        v.FieldId = FirstName;
                        valList.Add(v);
                        break;

                    case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.LastNameID:
                        v.FieldId = LastName;
                        valList.Add(v);
                        break;

                    case IFM.VR.Validation.ObjectValidation.AllLines.NameValidator.BirthDate:
                        v.FieldId = BirthDate;
                        valList.Add(v);
                        break;
                }
            }
        }


    }
}