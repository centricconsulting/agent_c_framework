using System;
using System.Configuration;
using IFM.Configuration.Extensions;
using IFM.ControlFlags;

namespace IFM.VR.Flags.LOB
{
    public class PPA:IFeatureFlag 
    {
        public const string OHIO_ENABLED = "VR_PPA_OH_Enabled";
        public const string TASK68137_CAPITALIZEDLN = "Task68137_CapitalizeDLN";        
        public const string OH_EARLIEST_AVAILABLE_EFFECTIVE_DATE = "VR_PPA_OH_EarliestAllowedEffectiveDate";
        public PPA()
        {
             
        }
        public bool OhioEnabled => ConfigurationManager.AppSettings.As<bool>(OHIO_ENABLED);             
        public bool Task68137_CapitalizeDLN => ConfigurationManager.AppSettings.As<bool>(TASK68137_CAPITALIZEDLN);
        public CompareFlag<DateTime> Ohio_EffectiveDateAllowed => CompareFlagFactory.BuildFor<DateTime>(OH_EARLIEST_AVAILABLE_EFFECTIVE_DATE, CompareFlagComparisons.Date_AtOrLater);

    }
}
