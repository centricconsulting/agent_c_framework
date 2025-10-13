using DCO = Diamond.Common.Objects;
using System;

#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.CommonObjects.OMP.HOM
{
    [System.Serializable]
    public class HomCoverage : CoverageBase
    {
        public double ManualLimitIncluded { get; set; }
        public double ManualLimitIncreased { get; set; }
        public double ManualLimitAmount { get; set; }

        public HomCoverage() { }
        internal HomCoverage(DCO.Coverage dCoverage) : base(dCoverage)
        {
            if (dCoverage != null)
            {
                this.ManualLimitIncluded = Convert.ToDouble(dCoverage.ManualLimitIncluded);
                this.ManualLimitIncreased = Convert.ToDouble(dCoverage.ManualLimitIncreased);
                this.ManualLimitAmount = Convert.ToDouble(dCoverage.ManualLimitAmount);
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