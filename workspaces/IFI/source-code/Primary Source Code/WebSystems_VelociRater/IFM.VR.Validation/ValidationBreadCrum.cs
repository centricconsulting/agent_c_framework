namespace IFM.VR.Validation
{
    public class ValidationBreadCrum
    {
        public enum BCType : int
        {
            Undefined = 0,
            PolicyholderIndex = 1,
            ClientIndex = 2,
            ApplicantIndex = 3,
            DriverIndex = 4,
            LossIndex = 5,
            ViolationIndex = 6,
            VehicleIndex = 7,
            AutoSymbolIndex = 8,
            LocationIndex = 9,
            AdditionalInterest = 10,
            ScheduledItem = 11
        }

        private BCType breadCrumIndicator = BCType.Undefined;

        public BCType BreadCrumIndicatorType
        {
            get
            {
                return breadCrumIndicator;
            }
        }

        private string bcValue = "";

        public string BreadCrumValue { get { return bcValue; } }

        public ValidationBreadCrum(BCType type, string value)
        {
            this.breadCrumIndicator = type;
            this.bcValue = value;
        }
    }
}