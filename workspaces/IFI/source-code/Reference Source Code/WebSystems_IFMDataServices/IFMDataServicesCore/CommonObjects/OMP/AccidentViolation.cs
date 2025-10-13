using System;
using DCO = Diamond.Common.Objects;

#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class AccidentViolation : ModelBase
    {
        public DateTime AvDate { get; set; }
        public string Description { get; set; }
        public int ViolationConvictionTypeId { get; set; }
        public string ViolationConvictionType { get; set; }

        public AccidentViolation() { }
        internal AccidentViolation(DCO.Policy.AccidentViolation dAv)
        {
            if (dAv != null)
            {
                this.AvDate = dAv.AvDate;
                this.Description = dAv.Description;
                this.ViolationConvictionTypeId = dAv.ViolationConvictionTypeId;
                this.ViolationConvictionType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAccidentViolation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ViolationConvictionTypeId, this.ViolationConvictionTypeId.ToString());
            }
#if DEBUG
            else
            {
                Debugger.Break();
            }
#endif
        }
    }
}