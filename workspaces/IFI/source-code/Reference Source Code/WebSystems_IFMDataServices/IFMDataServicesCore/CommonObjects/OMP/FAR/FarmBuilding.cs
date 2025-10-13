using DCO = Diamond.Common.Objects;
#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.CommonObjects.OMP.FAR
{
    [System.Serializable]
    public class FarmBuilding : Building
    {
        public int FarmStructureTypeId { get; set; }
        public string FarmStructureType { get; set; }

        public FarmBuilding() { }
        internal FarmBuilding(DCO.Policy.BarnBuilding dBuilding) : base(dBuilding)
        {
            if (dBuilding != null)
            {
                this.FarmStructureTypeId = dBuilding.FarmStructureTypeId;
                this.FarmStructureType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding,
                    QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.FarmStructureTypeId, this.FarmStructureTypeId.ToString());
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