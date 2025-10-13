using QuickQuote.CommonObjects;
using System;
using System.Collections.Generic;
using IFM.PrimitiveExtensions;
using System.Web.UI.HtmlControls;
using IFM.DataServicesCore.CommonObjects.IFM.Auto;
using IFM.VR.Common;
using IFM.VR.Common.Helpers.PPA;
using DCE = Diamond.Common.Enums;
using DCO = Diamond.Common.Objects;
using IFM.DataServicesCore.BusinessLogic;
using VinLookupResult = IFM.DataServicesCore.CommonObjects.IFM.Auto.VinLookupResult;
using QuickQuote.CommonMethods;
using DevExpress.DataAccess.Native.EntityFramework;

#if DEBUG

using System.Diagnostics;

#endif

using System.Linq;

namespace IFM.DataServicesCore.CommonObjects.OMP.PPA
{
    [System.Serializable]
    public class Vehicle : ModelBase
    {
        public Address GaragingAddress { get; set; }
        public List<AdditionalInterest> AdditionalInterests { get; set; }
        public Int32 BodyTypeId { get; set; }
        public string BodyType { get; set; }
        public string ClassCode { get; set; }
        public Int32 RestraintTypeId { get; set; }
        public string RestraintType { get; set; }
        public string PerformanceType { get; set; }
        public Int32 PerformanceTypeId { get; set; }
        public string AntiLockBrakesType { get; set; }
        public Int32 AntiLockBrakesTypeId { get; set; }
        public string AntiTheftType { get; set; }
        public Int32 AntiTheftTypeId { get; set; }
        public Int32 PrincipalDriverNum { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public Int32 Year { get; set; }
        public double Premium { get; set; }
        public Int32 UseTypeId { get; set; }
        public string UseType { get; set; }
        public Int32 VehicleNum { get; set; }
        public string VIN { get; set; }
        public List<VehicleCoverage> Coverages { get; set; }
        public double CostNew { get; set; }
        public Int32 ValuationMethodTypeId { get; set; }
        public string ValuationMethodType { get; set; }
        public string CompSymbol { get; set; }
        public string CollSymbol { get; set; }
        public string LiabSymbol { get; set; }

        public string BodilyInjurySymbol { get; set; }
        public string MedPaySymbol { get; set; }
        public string PropertyDamageSymbol { get; set; }

        public bool HasLoanOrLease { get; set; }
        public bool HasLoanLeaseCoverage { get; set; }
        public bool MultiCarDiscount {get;set;}
        public bool OutOfStateSurcharge { get; set; }
        public string LoanLeaseCoveragePremium { get; set; }
        public int ComprehensiveDeductibleLimitId { get; set; }
        public int CollisionDeductibleLimitId { get; set; }
        public int ComprehensiveDeductibleId { get; set; }
        public int CollisionDeductibleId { get; set; }
        public int TowingAndLaborDeductibleLimitId { get; set; }
        public int TransportationExpenseLimitId { get; set; }
        public int AnnualMileage { get; set; }
        public string Style { get; set; }
        public string BodilyInjuryLimit;
        public string PropertyDamageLimit;
        public string MedicalPaymentsLimit;
        public string UMUIMBodilyInjuryLimit;
        public string UMPropertyDamageLimit;
        public string UMPropertyDamageDeductibleLimit;
        public string SingleLiabilityLimit;
        public string UMUIMCSL;

        public Vehicle() { }
        internal Vehicle(DCO.Policy.Vehicle dVehicle)
        {
            if (dVehicle != null)
            {
                if (dVehicle.AdditionalInterests != null && dVehicle.AdditionalInterests.Any())
                {
                    this.AdditionalInterests = new List<AdditionalInterest>();
                    foreach (var ai in dVehicle.AdditionalInterests)
                    {
                        this.AdditionalInterests.Add(new AdditionalInterest(ai));
                        if(ai.AdditionalInterestTypeId == 8 || ai.AdditionalInterestTypeId == 53)
                        {
                            this.HasLoanOrLease = true;
                        }
                    }
                }
                if(dVehicle.GaragingAddress?.Address != null)
                {
                    this.GaragingAddress = new Address(dVehicle.GaragingAddress.Address);
                }
                this.VehicleNum = dVehicle.VehicleNum;
                this.BodyTypeId = dVehicle.BodyTypeId;
                this.ClassCode  = dVehicle.ClassCode;
                this.BodyType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, this.BodyTypeId.ToString());
                this.PrincipalDriverNum = dVehicle.PrincipalDriverNum;
                this.Make = dVehicle.Make;
                this.Model = dVehicle.Model;
                this.Year = dVehicle.Year;
                this.Premium = Convert.ToDouble(dVehicle.PremiumFullterm);
                this.UseTypeId = dVehicle.VehicleUseTypeId;
                this.UseType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.VehicleUseTypeId, this.UseTypeId.ToString());
                this.CostNew = Convert.ToDouble(dVehicle.CostNew);
                this.ValuationMethodTypeId = dVehicle.ValuationMethodTypeId;
                this.ValuationMethodType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ValuationMethodTypeId, this.ValuationMethodTypeId.ToString());
                this.AnnualMileage = dVehicle.AnnualMiles;
                this.PerformanceTypeId = dVehicle.PerformanceTypeId;
                this.PerformanceType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PerformanceTypeId, this.PerformanceTypeId.ToString());
                this.MultiCarDiscount = dVehicle.MultiCar;
                this.OutOfStateSurcharge = dVehicle.DriverOutOfStateSurcharge;
                this.VIN = dVehicle.Vin;

                var comprehensiveDeductableList = new List<int>() { 0, 40, 1, 2, 4, 8, 9, 28, 29 };
                var collisioneDeductableList = new List<int>() { 0, 1, 2, 3, 4, 8, 9, 28, 29 };

                if (dVehicle.Coverages != null && dVehicle.Coverages.Any())
                {
                    this.Coverages = new List<VehicleCoverage>();
                    foreach (var cov in dVehicle.Coverages)
                    {
                        this.Coverages.Add(new VehicleCoverage(cov));
                    }

                    List<VehicleCoverage> foundCovs = null;
                    List<int> autoCoverages = new List<int>();
                    autoCoverages.Add(10044); //LoanOrLease Coverage
                    autoCoverages.Add(60008); //TowingAndLabor
                    autoCoverages.Add(3); //Comprehensive
                    autoCoverages.Add(5); //Collision
                    autoCoverages.Add(68); //TransportationExpense (a.k.a. Rental Reimbursement)
                    foundCovs = this.Coverages.FindAll(c => autoCoverages.Where(ac => ac == c.CoverageCode.CoverageCodeId) != null);

                    foreach (var cov in foundCovs)
                    {
                        switch (cov.CoverageCode.CoverageCodeId)
                        {
                            case 10044:
                                this.HasLoanLeaseCoverage = true;
                                break;
                            case 60008:
                                this.TowingAndLaborDeductibleLimitId = cov.CoverageLimit.CoverageLimitId;
                                break;
                            case 66:
                                this.TransportationExpenseLimitId = cov.CoverageLimit.CoverageLimitId;
                                break;
                            case 5:
                                this.CollisionDeductibleLimitId = cov.CoverageLimit.CoverageLimitId;
                                this.CollisionDeductibleId = collisioneDeductableList.Contains( cov.CoverageDeductible) ? cov.CoverageDeductible : 0 ;
                                break;
                            case 3:
                                this.ComprehensiveDeductibleLimitId = cov.CoverageLimit.CoverageLimitId;
                                this.ComprehensiveDeductibleId = comprehensiveDeductableList.Contains( cov.CoverageDeductible) ? cov.CoverageDeductible : 0;
                                break;

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
#if DEBUG
            else
            {
                Debugger.Break();
            }
#endif
        }

        internal Vehicle(QuickQuoteVehicle qqVehicle)
        {
            if (qqVehicle != null)
            {
                if (qqVehicle.AdditionalInterests != null && qqVehicle.AdditionalInterests.Any())
                {
                    this.AdditionalInterests = new List<AdditionalInterest>();
                    foreach (var ai in qqVehicle.AdditionalInterests)
                    {
                        this.AdditionalInterests.Add(new AdditionalInterest(ai));
                        if (ai.TypeId.TryToGetInt32() == 8 || ai.TypeId.TryToGetInt32() == 53)
                        {
                            this.HasLoanOrLease = true;
                        }
                    }
                }
                if (qqVehicle.GaragingAddress?.Address != null)
                {
                    this.GaragingAddress = new Address(qqVehicle.GaragingAddress.Address);
                }
                this.VehicleNum = qqVehicle.VehicleNum.TryToGetInt32();
                this.BodyTypeId = qqVehicle.BodyTypeId.TryToGetInt32();
                this.BodyType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, this.BodyTypeId.ToString());
                this.PrincipalDriverNum = qqVehicle.PrincipalDriverNum.TryToGetInt32();
                this.Make = qqVehicle.Make;
                this.Model = qqVehicle.Model;
                this.Year = qqVehicle.Year.TryToGetInt32();
                this.Premium = Convert.ToDouble(qqVehicle.PremiumFullTerm.TryToGetInt32());
                this.UseTypeId = qqVehicle.VehicleUseTypeId.TryToGetInt32();
                this.UseType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.VehicleUseTypeId, this.UseTypeId.ToString());
                this.CostNew = Convert.ToDouble(qqVehicle.CostNew.TryToGetInt32());
                //this.ValuationMethodTypeId = qqVehicle.ValuationMethodTypeId.TryToGetInt32();
                //this.ValuationMethodType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ValuationMethodTypeId, this.ValuationMethodTypeId);
                this.AnnualMileage = qqVehicle.AnnualMiles.TryToGetInt32();
                this.PerformanceTypeId = qqVehicle.PerformanceTypeId.TryToGetInt32();
                this.PerformanceType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PerformanceTypeId, this.PerformanceTypeId.ToString());
                this.MultiCarDiscount = qqVehicle.MultiCar;
                this.OutOfStateSurcharge = qqVehicle.DriverOutOfStateSurcharge;
                this.VIN = qqVehicle.Vin;
                this.BodilyInjuryLimit = qqVehicle.BodilyInjuryLiabilityLimitId.NoneAreNullEmptyOrWhitespace() ? GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BodilyInjuryLiabilityLimitId, qqVehicle.BodilyInjuryLiabilityLimitId) : "";
                this.PropertyDamageLimit = qqVehicle.PropertyDamageLimitId.NoneAreNullEmptyOrWhitespace() ? GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PropertyDamageLimitId, qqVehicle.PropertyDamageLimitId) : "";
                this.MedicalPaymentsLimit = qqVehicle.MedicalPaymentsLimitId.NoneAreNullEmptyOrWhitespace() ? GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, qqVehicle.MedicalPaymentsLimitId) : "";
                this.UMUIMBodilyInjuryLimit = qqVehicle.UninsuredMotoristLiabilityLimitId.NoneAreNullEmptyOrWhitespace() ? GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristLiabilityLimitId, qqVehicle.UninsuredMotoristLiabilityLimitId) : "";
                this.UMPropertyDamageLimit = qqVehicle.UninsuredMotoristPropertyDamageLimitId.NoneAreNullEmptyOrWhitespace() ? GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, qqVehicle.UninsuredMotoristPropertyDamageLimitId) : "";
                this.UMPropertyDamageDeductibleLimit = qqVehicle.UninsuredMotoristPropertyDamageDeductibleLimitId.NoneAreNullEmptyOrWhitespace() ? GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageDeductibleLimitId, qqVehicle.UninsuredMotoristPropertyDamageDeductibleLimitId) : "";
                this.SingleLiabilityLimit = qqVehicle.Liability_UM_UIM_LimitId.NoneAreNullEmptyOrWhitespace() ? GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.Liability_UM_UIM_LimitId, qqVehicle.Liability_UM_UIM_LimitId) : "";
                this.UMUIMCSL = qqVehicle.UninsuredCombinedSingleLimitId.NoneAreNullEmptyOrWhitespace() ? GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredCombinedSingleLimitId, qqVehicle.UninsuredCombinedSingleLimitId) : "";

                if (qqVehicle.Coverages != null && qqVehicle.Coverages.Any())
                {
                    this.Coverages = new List<VehicleCoverage>();
                    foreach (var cov in qqVehicle.Coverages)
                    {
                        this.Coverages.Add(new VehicleCoverage(cov));
                    }

                    List<VehicleCoverage> foundCovs = null;
                    List<int> autoCoverages = new List<int>();
                    autoCoverages.Add(10044); //LoanOrLease Coverage
                    autoCoverages.Add(60008); //TowingAndLabor
                    autoCoverages.Add(3); //Comprehensive
                    autoCoverages.Add(5); //Collision
                    autoCoverages.Add(68); //TransportationExpense (a.k.a. Rental Reimbursement)
                    foundCovs = this.Coverages.FindAll(c => autoCoverages.Where(ac => ac == c.CoverageCode.CoverageCodeId) != null);

                    foreach (var cov in foundCovs)
                    {
                        switch (cov.CoverageCode.CoverageCodeId)
                        {
                            case 10044:
                                this.HasLoanLeaseCoverage = true;
                                break;
                            case 60008:
                                this.TowingAndLaborDeductibleLimitId = cov.CoverageLimit.CoverageLimitId;
                                break;
                            case 66:
                                this.TransportationExpenseLimitId = cov.CoverageLimit.CoverageLimitId;
                                break;
                            case 5:
                                this.CollisionDeductibleLimitId = cov.CoverageLimit.CoverageLimitId;
                                break;
                            case 3:
                                this.ComprehensiveDeductibleLimitId = cov.CoverageLimit.CoverageLimitId;
                                break;

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
#if DEBUG
            else
            {
                Debugger.Break();
            }
#endif
        }

        public void FillInIdInfo()
        {
            if (this.BodyTypeId.HasValue()) this.BodyType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, this.BodyTypeId.ToString());
            if (this.UseTypeId.HasValue()) this.UseType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.VehicleUseTypeId, this.UseTypeId.ToString());
            if (this.ValuationMethodTypeId.HasValue()) this.ValuationMethodType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ValuationMethodTypeId, this.ValuationMethodTypeId.ToString());
            if (this.PerformanceTypeId.HasValue() && this.PerformanceType.HasValue() == false) this.PerformanceType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PerformanceTypeId, this.PerformanceTypeId.ToString());
            if (this.RestraintTypeId.HasValue() && this.RestraintType.HasValue() == false) this.RestraintType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.RestraintTypeId, this.RestraintTypeId.ToString());
            if (this.AntiLockBrakesTypeId.HasValue() && this.AntiLockBrakesType.HasValue() == false) this.AntiLockBrakesType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.AntiLockTypeId, this.AntiLockBrakesTypeId.ToString());
            if (this.AntiTheftTypeId.HasValue() && this.AntiTheftType.HasValue() == false) this.AntiTheftType = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.AntiTheftTypeId, this.AntiTheftTypeId.ToString());
            if (GaragingAddress != null) GaragingAddress.FillInIdInfo();
            if (AdditionalInterests != null && AdditionalInterests.Count > 0)
            {
                foreach (AdditionalInterest ai in AdditionalInterests)
                {
                    ai.FillInIdInfo();
                }
            }
        }

        public QuickQuoteVehicle UpdateQuickQuoteVehicle(QuickQuoteVehicle VehicleToUpdate = null, QuickQuoteObject qqe = null)
        {
            VehicleToUpdate = VehicleToUpdate.NewIfNull();
            if (this.AdditionalInterests?.Count > 0) { VehicleToUpdate.AdditionalInterests = this.ConvertAdditionalInterestsListToQuickQuoteAdditionalInterestsList(); }
            if (this.GaragingAddress != null) { VehicleToUpdate.GaragingAddress.Address = this.GaragingAddress.UpdateQuickQuoteAddress(VehicleToUpdate.GaragingAddress.Address); }
            ConvertCoveragesToQQCoverages(VehicleToUpdate);
            return MPObjToQQObj(VehicleToUpdate, qqe);
        }

        public void ConvertCoveragesToQQCoverages(QuickQuoteVehicle qqVehicle)
        {
            qqVehicle.ComprehensiveDeductibleLimitId = "0";
            qqVehicle.CollisionDeductibleLimitId = "0";
            qqVehicle.TransportationExpenseLimitId = "0";
            qqVehicle.TowingAndLaborDeductibleLimitId = "0";
            qqVehicle.HasAutoLoanOrLease = false;
            if (this.ComprehensiveDeductibleLimitId.HasValue())
            {
                qqVehicle.ComprehensiveDeductibleLimitId = this.ComprehensiveDeductibleLimitId.ToString();
                if (this.HasLoanLeaseCoverage) { qqVehicle.HasAutoLoanOrLease = true; }
                if (this.CollisionDeductibleLimitId.HasValue()) { qqVehicle.CollisionDeductibleLimitId = this.CollisionDeductibleLimitId.ToString(); }
                if (this.TowingAndLaborDeductibleLimitId.HasValue()) { qqVehicle.TowingAndLaborDeductibleLimitId = this.TowingAndLaborDeductibleLimitId.ToString(); }
                if (this.TransportationExpenseLimitId.HasValue()) { qqVehicle.TransportationExpenseLimitId = this.TransportationExpenseLimitId.ToString(); }
            }
        }

        public List<QuickQuoteAdditionalInterest> ConvertAdditionalInterestsListToQuickQuoteAdditionalInterestsList()
        {
            List<QuickQuoteAdditionalInterest> myList = new List<QuickQuoteAdditionalInterest>();
            foreach(AdditionalInterest ai in AdditionalInterests)
            {
                QuickQuoteAdditionalInterest qqAI = ai.UpdateQuickQuoteAdditionalInterest();
                myList.Add(qqAI);
            }
            return myList;
        }

        public void GetNeededPremiums(QuickQuoteVehicle qqV)
        {
            if(qqV != null)
            {
                if (qqV.AutoLoanOrLeaseQuotedPremium.HasValue()) { this.LoanLeaseCoveragePremium = qqV.AutoLoanOrLeaseQuotedPremium; }
            }
        }

        public void SetValuesFromVinResult(VinLookupResult vinResult)
        {
            this.Year = vinResult.Year;
            this.Make = vinResult.Make;
            this.Model = vinResult.Model;
            //this.BodyType = vinResult.ISOBodyStyle;
            this.BodyTypeId = vinResult.DiamondBodyTypeId.TryToGetInt32();
            this.PerformanceTypeId = vinResult.PerformanceTypeId.TryToGetInt32();
            //this.PerformanceType = vinResult.PerformanceTypeDescription;
            this.RestraintTypeId = vinResult.RestraintTypeId.TryToGetInt32();
            //this.RestraintType = vinResult.RestraintDescription;
            //this.AntiLockBrakesType = vinResult.AntiLockDescription;
            this.AntiLockBrakesTypeId = vinResult.AntiLockTypeId.TryToGetInt32();
            //this.AntiTheftType = vinResult.AntiTheftDescription;
            this.AntiTheftTypeId = vinResult.AntiTheftTypeId.TryToGetInt32();
            this.CollSymbol = vinResult.CollisionSymbol;
            this.CompSymbol = vinResult.CompSymbol;
            this.LiabSymbol = vinResult.LiabilitySymbol;

            this.BodilyInjurySymbol = vinResult.BodilyInjurySymbol;
            this.MedPaySymbol = vinResult.MedPaySymbol;
            this.PropertyDamageSymbol = vinResult.PropertyDamageSymbol;
        }

        private QuickQuoteVehicle MPObjToQQObj(QuickQuoteVehicle QQVehicle, QuickQuoteObject qqe)
        {
            if (this.BodyTypeId.HasValue()) QQVehicle.BodyTypeId = this.BodyTypeId.ToString();
            if (this.PrincipalDriverNum.HasValue()) QQVehicle.PrincipalDriverNum = this.PrincipalDriverNum.ToString();
            if (this.Make.HasValue()) QQVehicle.Make = this.Make;
            if (this.Model.HasValue()) QQVehicle.Model = this.Model;
            if (this.Year.HasValue()) QQVehicle.Year = this.Year.ToString();
            if (this.Premium.HasValue()) QQVehicle.PremiumFullTerm = this.Premium.ToString();
            if (this.UseTypeId.HasValue()) QQVehicle.VehicleUseTypeId = this.UseTypeId.ToString();
            if (this.CostNew.HasValue()) QQVehicle.CostNew = this.CostNew.ToString();
            if (this.AnnualMileage.HasValue()) QQVehicle.AnnualMiles = this.AnnualMileage.ToString();
            if (this.VIN.HasValue()) QQVehicle.Vin = this.VIN;
            if (this.PerformanceTypeId.HasValue()) QQVehicle.PerformanceTypeId = this.PerformanceTypeId.ToString();
            if (this.RestraintTypeId.HasValue()) QQVehicle.RestraintTypeId = this.RestraintTypeId.ToString();
            if (this.AntiLockBrakesTypeId.HasValue()) QQVehicle.AntiLockTypeId = this.AntiLockBrakesTypeId.ToString();
            if (this.AntiTheftTypeId.HasValue()) QQVehicle.AntiTheftTypeId = this.AntiTheftTypeId.ToString();
            QQVehicle.VehicleSymbols = QQVehicle.VehicleSymbols.NewIfNull();
            if(qqe != null) //We basically only use the QQ object to check if the quote is Parachute.... I believe at this point everytyhing is parachute... Would be nice to be able to avoid this.
            {
                if (this.CompSymbol.HasValue()) { AddUpdateAutoSymbols(qqe, QQVehicle, "1", this.CompSymbol); }
                if (this.CollSymbol.HasValue()) { AddUpdateAutoSymbols(qqe, QQVehicle, "2", this.CollSymbol); }
                if (this.LiabSymbol.HasValue()) { AddUpdateAutoSymbols(qqe, QQVehicle, "3", this.LiabSymbol); }
            }
            if (qqe != null && QuickQuoteHelperClass.IsNewRAPASymbolAvailable(Convert.ToInt32(qqe.VersionId), Convert.ToDateTime(qqe.EffectiveDate), (qqe.QuoteTransactionType == QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote ? false : true)))
            {
                if (this.BodilyInjurySymbol.HasValue()) { AddUpdateAutoSymbols(qqe, QQVehicle, "8", this.BodilyInjurySymbol); }
                if (this.MedPaySymbol.HasValue()) { AddUpdateAutoSymbols(qqe, QQVehicle, "9", this.MedPaySymbol); }
                if (this.PropertyDamageSymbol.HasValue()) { AddUpdateAutoSymbols(qqe, QQVehicle, "14", this.PropertyDamageSymbol); }
            }
            return QQVehicle;
        }

        public bool IsGaragingAddressOutOfState(out string ErrorMessage, string OriginStateAbbrev = "IN")
        {
            return global::IFM.DataServicesCore.BusinessLogic.OMP.AddressHelper.IsAddressOutOfState(this.GaragingAddress, OriginStateAbbrev, out ErrorMessage);
        }

        private bool AddUpdateAutoSymbols(QuickQuote.CommonObjects.QuickQuoteObject quote, QuickQuote.CommonObjects.QuickQuoteVehicle vehicle, string symbolTypeId, string symbolValue)
        {
            var symbol = (from s in vehicle.VehicleSymbols where s.VehicleSymbolCoverageTypeId == symbolTypeId select s).FirstOrDefault();
            // versionId > 128 is to check if the policy is using parachute... at this point, everything should be using it...
            //if ((symbolTypeId.TryToGetInt32() == 3 && quote.VersionId.TryToGetInt32() > 128) || symbolValue.IsNullEmptyOrWhitespace())
            //{
            //    if (symbol != null)
            //    {
            //        vehicle.VehicleSymbols.Remove(symbol);
            //    }
            //    return false;
            //}

            if (symbol == null)
            {
                symbol = vehicle.VehicleSymbols.AddNew();
            }

            symbol.SystemGeneratedSymbol = symbolValue;
            symbol.UserOverrideSymbol = symbolValue;
            symbol.VehicleSymbolCoverageTypeId = symbolTypeId;

            if (BusinessLogic.AppConfig.UseRAPAApiForVehicleSymbolLookup.TryToGetBoolean() && quote.VersionId.TryToGetInt32() >= 245)
            {
                symbol.SystemGeneratedSymbolVehicleInfoLookupTypeId = ((int)DCE.VehicleInfoLookupType.VehicleInfoLookupType.ModelIsoRapaApi).ToString();
            }
            else
            {
                symbol.SystemGeneratedSymbolVehicleInfoLookupTypeId = ((int)DCE.VehicleInfoLookupType.VehicleInfoLookupType.ModelISORAPA).ToString();
            }
            
            return true;
        }

        public QuickQuoteVehicle ClearUniqueVehicleItemsThenUpdateQuickQuoteVehicle(QuickQuoteVehicle QQVehicle, QuickQuoteObject qqe)
        {
            QQVehicle.BodyTypeId = "";
            //QQVehicle.PrincipalDriverNum = ""; //Keep this??
            QQVehicle.Make = "";
            QQVehicle.Model = "";
            QQVehicle.Year = "";
            QQVehicle.PremiumFullTerm = "";
            QQVehicle.OperatorUseTypeId = "";
            QQVehicle.CostNew = "";
            QQVehicle.AnnualMiles = "";
            QQVehicle.Vin = "";
            QQVehicle.PerformanceTypeId = "";
            QQVehicle.RestraintTypeId = "";
            QQVehicle.AntiLockTypeId = "";
            QQVehicle.AntiTheftTypeId = "";
            QQVehicle.VehicleSymbols = null;
            return this.UpdateQuickQuoteVehicle(QQVehicle, qqe);
        }

        public override string ToString()
        {
            return $"{Year} {(Make ?? String.Empty)} {(Model ?? String.Empty)}";
        }
    }
}