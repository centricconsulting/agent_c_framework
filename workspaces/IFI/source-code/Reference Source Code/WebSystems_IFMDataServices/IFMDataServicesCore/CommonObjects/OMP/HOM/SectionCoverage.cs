using DCO = Diamond.Common.Objects;
using System.Collections.Generic;

#if DEBUG

using System.Diagnostics;

#endif

using System.Linq;

namespace IFM.DataServicesCore.CommonObjects.OMP.HOM
{
    [System.Serializable]
    public class SectionCoverage : ModelBase
    {
        public  List<HomCoverage> Coverages { get; set; }

        public SectionCoverage() { }
        internal SectionCoverage(DCO.Policy.SectionCoverage dSectionCov)
        {
            if (dSectionCov != null && dSectionCov.Coverages != null && dSectionCov.Coverages.Any())
            {
                this.Coverages = new List<HomCoverage>();
                foreach (var cov in dSectionCov.Coverages)
                {
                    this.Coverages.Add(new HomCoverage(cov));
                }
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