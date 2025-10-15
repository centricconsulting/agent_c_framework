using DCO = Diamond.Common.Objects;
using System;

#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class BuildingUpdates : ModelBase
    {
        public Int32 HVACYearUpdated { get; set; }
        public Int32 ElectricYearUpdated { get; set; }
        public Int32 PaintExteriorYearUpdated { get; set; }
        public Int32 PlumbingYearUpdated { get; set; }
        public Int32 RoofYearUpdated { get; set; }

        public BuildingUpdates() { }
        internal BuildingUpdates(DCO.Policy.Updates dUpdates)
        {
            if (dUpdates != null)
            {
                HVACYearUpdated = dUpdates.CentralHeatUpdateYear;
                this.ElectricYearUpdated = dUpdates.ElectricUpdateYear;
                this.PaintExteriorYearUpdated = dUpdates.PaintExteriorUpdateYear;
                this.PlumbingYearUpdated = dUpdates.PlumbingUpdateYear;
                this.RoofYearUpdated = dUpdates.RoofUpdateYear;
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