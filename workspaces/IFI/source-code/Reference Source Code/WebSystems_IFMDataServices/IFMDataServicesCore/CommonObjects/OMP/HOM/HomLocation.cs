using System.Collections.Generic;
using DCO = Diamond.Common.Objects;

#if DEBUG

using System.Diagnostics;

#endif

using System.Linq;

namespace IFM.DataServicesCore.CommonObjects.OMP.HOM
{
    [System.Serializable]
    public class HomLocation : Location
    {
        public int FoundationTypeId { get; set; }
        public string FoundationType { get; set; }
        public int NumberOfFamiliesId { get; set; }
        public string NumberOfFamilies { get; set; }
        public int OccupancyCodeId { get; set; }
        public string OccupancyCode { get; set; }
        public int OccupancyTypeId { get; set; }
        public string OccupancyType { get; set; }
        public bool IsPrimaryResidence { get; set; }
        public List<HOM.SectionCoverage> SectionCoverages { get; set; }

        public HomLocation() { }
        internal HomLocation(DCO.Policy.Location dLocation) : base(dLocation)
        {
            if (dLocation != null)
            {
                this.FoundationTypeId = dLocation.FoundationTypeId;
                //Me.FoundationType = qqhelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.FoundationTypeId, Me.FoundationTypeId.ToString())
                this.NumberOfFamiliesId = dLocation.NumberofFamiliesId;
                //Me.NumberOfFamilies = qqhelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.NumberOfFamiliesId, Me.NumberOfFamiliesId.ToString())
                this.OccupancyCodeId = dLocation.OccupancyCodeId;
                //Me.OccupancyCode = qqhelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.OccupancyCodeId, Me.OccupancyCodeId.ToString())
                this.OccupancyTypeId = dLocation.OccupancyTypeId;
                //Me.OccupancyType = qqhelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.OccupationTypeId, Me.OccupancyTypeId.ToString())

                this.IsPrimaryResidence = dLocation.PrimaryResidence;
                if (dLocation.SectionCoverages != null && dLocation.SectionCoverages.Any())
                {
                    this.SectionCoverages = new List<HOM.SectionCoverage>();
                    foreach (var cov in dLocation.SectionCoverages)
                    {
                        this.SectionCoverages.Add(new HOM.SectionCoverage(cov));
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