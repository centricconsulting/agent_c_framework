namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA
{
    public class ScheduledItemValidator_PPA
    {
        public const string ValidationListID = "{D8F9C46C-C633-43A4-BCEB-9EEA2F5508A7}";

        public const string IsNull = "{FF494F20-6C87-4C0A-8CBB-86563D864FEC}";
        public const string EquipmentDescription = "{39D73F16-6166-4AC9-89D4-922E934FF17F}";
        public const string EquipmentAmount = "{CD9998A1-F22B-4664-B6EC-FC3F984219DE}";

        public static Validation.ObjectValidation.ValidationItemList ScheduledItemViolation(QuickQuote.CommonObjects.QuickQuoteScheduledItem item)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (item != null)
            {
                VRGeneralValidations.Val_HasRequiredField_DD(item.Description, valList, EquipmentDescription, "Description");

                if (VRGeneralValidations.Val_HasRequiredField(item.Amount, valList, EquipmentAmount, "Amount"))
                    VRGeneralValidations.Val_IsNonNegativeNumber(item.Amount, valList, EquipmentAmount, "Amount");
            }
            else
            {
                valList.Add(new ValidationItem("Scheduled item is null", IsNull));
            }
            return valList;
        }

        public static Validation.ObjectValidation.ValidationItemList ScheduledItemViolation(int vehicleIndex, int scheduledItemIndex, QuickQuote.CommonObjects.QuickQuoteObject quote)
        {
            QuickQuote.CommonObjects.QuickQuoteScheduledItem item = null;

            if (quote != null && quote.Vehicles != null && quote.Vehicles.Count > vehicleIndex && quote.Vehicles[vehicleIndex].ScheduledItems != null & quote.Vehicles[vehicleIndex].ScheduledItems.Count > scheduledItemIndex)
            {
                item = quote.Vehicles[vehicleIndex].ScheduledItems[scheduledItemIndex];
            }

            var valList = ScheduledItemViolation(item);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.VehicleIndex, vehicleIndex.ToString());
            valList.AddBreadCrum(ValidationBreadCrum.BCType.ScheduledItem, scheduledItemIndex.ToString());
            return valList;
        }
    }
}