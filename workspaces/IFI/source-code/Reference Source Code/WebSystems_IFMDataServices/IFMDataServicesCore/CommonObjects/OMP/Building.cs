using System;
using System.Collections.Generic;
using System.Linq;
using DCO = Diamond.Common.Objects;

#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class Building : ModelBase
    {
        public string BuildingDescription{ get; set; }
        public string BuildingDimensions{ get; set; }
        public double PremiumWritten{ get; set; }
        public string ProtectionClass{ get; set; }
        public Int32 SquareFeet{ get; set; }
        public BuildingUpdates Updates{ get; set; }
        public Int32 YearBuilt{ get; set; }

        public List<AdditionalInterest> AdditionalInterests{ get; set; }

        public List<CoverageBase> Coverages{ get; set; }

        public Building() { }
        internal Building(DCO.Policy.BarnBuilding dBuilding)
        {
            if (dBuilding != null)
            {
                this.BuildingDescription = dBuilding.Description;
                this.BuildingDimensions = dBuilding.Dimensions;
                this.PremiumWritten = Convert.ToDouble(dBuilding.PremiumWritten);
                this.ProtectionClass = dBuilding.ProtectionClass;
                this.SquareFeet = dBuilding.SquareFeet;
                this.Updates = new BuildingUpdates(dBuilding.Updates);
                this.YearBuilt = dBuilding.YearBuilt;

                if (dBuilding.AdditionalInterests != null && dBuilding.AdditionalInterests.Any())
                {
                    this.AdditionalInterests = new List<AdditionalInterest>();
                    foreach (var a in dBuilding.AdditionalInterests)
                    {
                        this.AdditionalInterests.Add(new AdditionalInterest(a));
                    }
                }

                if (dBuilding.Coverages != null && dBuilding.Coverages.Any())
                {
                    this.Coverages = new List<CoverageBase>();
                    foreach (var c in dBuilding.Coverages)
                    {
                        this.Coverages.Add(new CoverageBase(c));
                    }
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