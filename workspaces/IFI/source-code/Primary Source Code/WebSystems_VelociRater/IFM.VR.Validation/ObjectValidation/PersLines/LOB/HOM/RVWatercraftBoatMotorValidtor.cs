namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class RVWatercraftBoatMotorValidtor
    {
        public const string ValidationListID = "{7F461032-D98C-4321-BECF-420A70DD6F6C}";

        public const string MotorIsNull = "{410ED311-ADD9-42B4-BC81-19DFF1F4DD00}";

        public const string SerialNumberMissing = "{53DA9095-23F5-4FFC-8AC8-5DA4A1FDE0F7}";
        public const string SerialNumberInvalid = "{0E9DD57D-6DC2-4C1A-B690-40D7DA1B108F}";

        public const string ManufacturerMissing = "{4225CA9A-043C-4A5B-87AD-B8AF61960CA8}";
        public const string ManufacturerInvalid = "{C295F4B2-A5BD-4A49-AE73-C4998C546DAC}";

        public const string ModelMissing = "{D1DA10FD-2A09-4A6D-9CCE-067D33BB812D}";
        public const string ModelInvalid = "{283DE713-0AC1-417B-B4B4-C4775CC170A0}";

        public static Validation.ObjectValidation.ValidationItemList ValidateRvWaterCraftMotor(QuickQuote.CommonObjects.QuickQuoteRvWatercraftMotor motor, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (motor != null)
            {
                if (VRGeneralValidations.Val_HasRequiredField(motor.SerialNumber, valList, SerialNumberMissing, "Serial Number"))
                    if (VRGeneralValidations.Val_IsTextLengthInRange(motor.SerialNumber, valList, SerialNumberInvalid, "Serial Number", "4", "50"))
                        VRGeneralValidations.Val_IsValidAlphaNumeric(motor.SerialNumber, valList, SerialNumberInvalid, "Serial Number");

                if (VRGeneralValidations.Val_HasRequiredField(motor.Manufacturer, valList, ManufacturerMissing, "Manufacturer"))
                    VRGeneralValidations.Val_IsValidAlphaNumeric(motor.Manufacturer, valList, ManufacturerInvalid, "Manufacturer");

                if (VRGeneralValidations.Val_HasRequiredField(motor.Model, valList, ModelMissing, "Model"))
                    VRGeneralValidations.Val_IsValidAlphaNumeric(motor.Model, valList, ModelInvalid, "Model");
            }
            else
            {
                valList.Add(new ObjectValidation.ValidationItem("Boat Motor is null", MotorIsNull));
            }

            return valList;
        }
    }
}