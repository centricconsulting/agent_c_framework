using System;
using DCO = Diamond.Common.Objects;
using global::IFM.PrimitiveExtensions;

#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class CoverageBase : ModelBase
    {
        public double WrittenPremium { get; set; }
        public CoverageCode CoverageCode { get; set; }
        public CoverageLimit CoverageLimit { get; set; }
        public Int32 Exposure { get; set; }
        public Int32 CoverageDeductible { get; set; }
        public bool Checkbox { get; set; }
        public CoverageBase() { }

        internal CoverageBase(DCO.Coverage dCov)
        {
            if (dCov != null)
            {


                this.WrittenPremium = Convert.ToDouble(dCov.WrittenPremium);
                this.CoverageCode = CoverageCode.Create(dCov.CoverageCodeId);
                this.CoverageLimit = CoverageLimit.Create(dCov.CoverageLimitId);
                this.Checkbox = dCov.Checkbox;
                //this.CoverageDeductible = CoverageLimit.Create(qqCov.DeductibleId)

                this.CoverageDeductible = dCov.DeductibleId;
                this.Exposure = dCov.Exposure;
            }
#if DEBUG
            else
            {
                Debugger.Break();
            }
#endif
        }

        internal CoverageBase(QuickQuote.CommonObjects.QuickQuoteCoverage qqCov)
        {
            if (qqCov != null)
            {
                this.WrittenPremium = Convert.ToDouble(qqCov.WrittenPremium.TryToGetInt32());
                this.CoverageCode = CoverageCode.Create(qqCov.CoverageCodeId.TryToGetInt32());
                this.CoverageLimit = CoverageLimit.Create(qqCov.CoverageLimitId.TryToGetInt32());
                this.Checkbox = qqCov.Checkbox;
                //this.CoverageDeductible = CoverageLimit.Create(qqCov.DeductibleId)
                this.Exposure = qqCov.Exposure.TryToGetInt32();
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