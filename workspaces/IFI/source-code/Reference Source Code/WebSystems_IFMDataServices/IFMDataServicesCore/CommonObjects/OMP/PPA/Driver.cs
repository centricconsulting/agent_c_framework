using QuickQuote.CommonObjects;
using System;
using System.Collections.Generic;
using IFM.PrimitiveExtensions;
using IFM.DataServicesCore.BusinessLogic;
using DCO = Diamond.Common.Objects;

#if DEBUG

using System.Diagnostics;

#endif

using System.Linq;

namespace IFM.DataServicesCore.CommonObjects.OMP.PPA
{
    [System.Serializable]
    public class Driver : ModelBase
    {
        public Name Name { get; set; }
        public Int32 DriverNum { get; set; }
        public List<AccidentViolation> Violations { get; set; }
        public List<LossHistory> LossHistory { get; set; }
        public int RelationshipTypeId { get; set; }
        public string RelationshipType { get; set; }

        public bool DistantStudent { get; set; }
        public int DistanceToSchool { get; set; }

        public bool GoodStudent { get; set; }
        public DateTime MatureDriverTrainingDate { get; set; }

        public int RatedExcludedTypeId { get; set; }//1 = Rated, Excluded = 3
        public string RatedExcludedType { get; set; }
        private Int32 _licenseStatusId = 0;
        public Int32 LicenseStatusId {
            get
            {
                return _licenseStatusId == 0 ? 1 : _licenseStatusId; //It appears sometimes Diamond has a value of 0 for the license status id. Thinking we should default this to 1 (Valid) if this is the case.
            }
            set
            {
                _licenseStatusId = value;
            }
        }
        public String LicenseStatus { get; set; }

        public Driver() { }
        internal Driver(DCO.Policy.Driver dDriver)
        {
            if (dDriver != null)
            {
                this.Name = new Name(dDriver.Name, false);
                this.DriverNum = dDriver.DriverNum;
                if (dDriver.AccidentViolations != null && dDriver.AccidentViolations.Any())
                {
                    this.Violations = new List<AccidentViolation>();
                    foreach (var d in dDriver.AccidentViolations)
                    {
                        this.Violations.Add(new AccidentViolation(d));
                    }
                }
                if (dDriver.LossHistories != null && dDriver.LossHistories.Any())
                {
                    this.LossHistory = new List<LossHistory>();
                    foreach (var l in dDriver.LossHistories)
                    {
                        this.LossHistory.Add(new LossHistory(l));
                    }
                }
                this.RelationshipTypeId = dDriver.RelationshipTypeId;
                this.RelationshipType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.RelationshipTypeId, this.RelationshipTypeId.ToString());

                this.DistantStudent = dDriver.DistantStudent;
                this.DistanceToSchool = dDriver.EnolMiles;

                this.GoodStudent = dDriver.GoodStudent;
                this.MatureDriverTrainingDate = dDriver.SeniorDriverDate;
                this.RatedExcludedTypeId = dDriver.DriverExcludeTypeId;
                this.RatedExcludedType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.DriverExcludeTypeId, this.RatedExcludedTypeId.ToString());

                this.LicenseStatusId = dDriver.LicenseStatusId;
                this.LicenseStatus = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.LicenseStatusId, this.LicenseStatusId.ToString());
            }
#if DEBUG
            else
            {
                Debugger.Break();
            }
#endif
        }

        public void FillInIdInfo()
        {
            if (this.RelationshipTypeId.HasValue()) this.RelationshipType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.RelationshipTypeId, this.RelationshipTypeId.ToString());
            if (this.RatedExcludedTypeId.HasValue()) this.RatedExcludedType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.DriverExcludeTypeId, this.RatedExcludedTypeId.ToString());
            if (this.LicenseStatusId.HasValue()) this.LicenseStatus = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.LicenseStatusId, this.LicenseStatusId.ToString());
            if(this.RatedExcludedTypeId.HasValue()) this.RatedExcludedType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.DriverExcludeTypeId, this.RatedExcludedTypeId.ToString());
            this.Name.FillInIdInfo();
        }

        public QuickQuoteDriver UpdateQuickQuoteDriver(QuickQuoteDriver DriverToUpdate = null)
        {
            DriverToUpdate = DriverToUpdate.NewIfNull();
            if (this.Name != null) { DriverToUpdate.Name = Name.UpdateQuickQuoteName(DriverToUpdate.Name); }
            //DriverToUpdate.DriverNum = DriverNum.ToString();
            return MPObjToQQObj(DriverToUpdate);
        }

        private QuickQuoteDriver MPObjToQQObj(QuickQuoteDriver QQDriver)
        {
            if(this.RelationshipTypeId.HasValue()) QQDriver.RelationshipTypeId = RelationshipTypeId.ToString();
            if (this.DistantStudent) QQDriver.DistantStudent = DistantStudent;
            if (this.DistanceToSchool.HasValue()) QQDriver.SchoolDistance = DistanceToSchool.ToString();
            if (this.GoodStudent) QQDriver.GoodStudent = GoodStudent;
            if (this.RatedExcludedTypeId.HasValue()) QQDriver.DriverExcludeTypeId = RatedExcludedTypeId.ToString();
            if (this.LicenseStatusId.HasValue()) QQDriver.LicenseStatusId = LicenseStatusId.ToString();
            if (this.RatedExcludedTypeId.HasValue()) QQDriver.DriverExcludeTypeId = RatedExcludedTypeId.ToString();
            //FillInIdInfo();
            return QQDriver;
        }

        public override string ToString()
        {
            return this.Name != null ? Name.ToString() : "Name is NUll";
        }
    }
}