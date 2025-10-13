Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store section I (Property) coverage information
    ''' </summary>
    ''' <remarks>could potentially be under several different objects; currently part of Location</remarks>
    <Serializable()> _
    Public Class QuickQuoteSectionICoverage 'added 8/13/2013 for HOM
        Inherits QuickQuoteBaseGenericObject(Of QuickQuoteLocation) 'added 8/4/2014
        Implements IDisposable

        ''' <summary>
        ''' valid types for section I coverages
        ''' </summary>
        ''' <remarks>value previously corresponded to coveragecode_id in Diamond; now see static data xml file; coded for HOM and DFR w/ text matching HOM when overlaps exist; should use enum types specific to LOB instead</remarks>
        Enum SectionICoverageType '8/15/2013 note: these were written specifically for HOM but DFR can also have section I covs (some are different and some overlap)
            'None = 0
            '12/3/2013: without values
            None
            'ActualCashValueLossSettlement_HO_0481 = 40173 'Actual Cash Value Loss Settlement
            'ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing_HO_0493 = 40142 'Actual Cash Value Loss Settlement / Windstorm or Hail Losses to Roof Surfacing
            'BackupofSewerOrDrain_92_173 = 144 'Back up Sewers and Drains
            'BuildingAdditionsAndAlterations_HO_51 = 20075 'Building Additions and Alterations
            'BusinessPropertyIncreasedLimits_HO_312 = 20098 'Business Property Increased
            'ConsenttoMoveMobileHome_ML_25 = 70106 'Farm_Consent_to_Move_Mobile_Home
            'Cov_A_SpecifiedAdditionalAmountofInsurance_29_034 = 877 'Home - Coverage A Special Coverage
            'CreditCardFundTransferCardForgeryAndCounterfeitMoneyCoverage_HO_53 = 20126 'Credit Card/Fund Trans/Forgery/Etc
            'DebrisRemoval_92_267 = 40116 'Debris Removal
            'Earthquake_HO_315B = 40091 'Earthquake
            'EquipmentBreakdownCoverage_92_132HO = 80059 'Equipment_Breakdown_Coverage
            'FireDepartmentServiceCharge_ = 70142 'Farm_Fire_Department_Service_Charge
            'Firearms_Cov_CIncreasedSpecialLimitsofLiability_HO_65HO_221 = 20115 'Firearms
            'HomeownerEnhancementEndorsement_92_267 = 80057 'Homeowner Enhancement Endorsement
            'IncreasedLimitsMotorizedVehicles_ML_65 = 80058 'Increased Limits Motorized Vehicles
            'InflationGuard = 20000 'Inflation_Guard
            'JewelryWatchesAndFurs_Cov_CIncreasedSpecialLimitsofLiability_Limitedto1000peritem_HO_61 = 20116 'Jewelry, Watches & Furs
            'LossAssessment_HO_35 = 70259 'Loss Assessment
            'LossAssessment_Earthquake_HO_35B = 70260 'Loss Assessment - Earthquake
            'MineSubsidenceCovAAndB_92_074 = 80103 'Mine Subsidence Cov A & B
            'MineSubsidenceCovA_92_074 = 80102 'Mine Subsidence Cov A
            'Money_Cov_CIncreasedSpecialLimitsofLiability_HO_65HO_221 = 40117 'Money
            'OrdinanceOrLaw_HO_277 = 20125 'Ordinance or Law
            'OutdoorAntennas_ML_49 = 70143 'Farm_Outdoor_Antenna_Satellite_Dish
            'PersonalPropertyIncreaseLimit_OtherResidence_HO_50 = 70304 'Personal Property at Other Residence Increase Limit
            'PersonalPropertyReplacementCost_92_02392_195 = 20103 'Personal Property Replacement
            'RefrigeratedFoodProducts_92_267 = 20127 'Refrigerated Property
            'Securities_Cov_CIncreasedSpecialLimitsofLiability_HO_65HO_221 = 40118 'Securities
            'SilverwareGoldwarePewterware_Cov_CIncreasedSpecialLimitsofLiability_HO_61 = 20119 'Silverware, Goldware, Pewterware
            'SinkholeCollapse_HO_99 = 20105 'Sinkhole Collapse
            'SpecialComputerCoverage_HO_314 = 20106 'Special Computer Coverage
            'SpecifiedOtherStructures_OnPremises_92_049 = 70064 'Cov_B_Related_Private_Structures
            'SpecifiedOtherStructuresOffPremises_92_147 = 873 'Home - Related Private Strucutures Away From Premises
            'TheftofBuildingMaterials_92_367 = 50095 'Theft of Building Material
            'TripCollision_ML_26 = 80049 'Trip Collision
            'Unit_OwnersCoverageA_HO_32 = 20038 'Unit Owners Coverage A
            'VendorsSingleInterest_ML_27 = 20081 'Mobile Home Lienholder's Single Interest
            'FunctionalReplacementCostLossSettlement_HO0530 = 40172 'Functional Replacement Cost Loss Assessment 'added 11/26/2014
            ''8/16/2013 - added more for DFR; commented out the ones that were already there for HOM
            ''ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing = 40142 'Actual Cash Value Loss Settlement / Windstorm or Hail Losses to Roof Surfacing
            ''AwningssignsAndoutdoorradioAndTVEquipment = 70143 'Farm_Outdoor_Antenna_Satellite_Dish
            ''Earthquake = 40091 'Earthquake
            ''MineSubsidenceCovA = 80102 'Mine Subsidence Cov A
            ''MineSubsidenceCovAAndB = 80103 'Mine Subsidence Cov A & B
            'ModifiedLossSettlement = 20102 'Loss Settlement
            ''SinkholeCollapse = 20105 'Sinkhole Collapse
            ''SpecifiedOtherStructures_OnPremises = 70064 'Cov_B_Related_Private_Structures
            ''SpecifiedOtherStructuresOffPremises = 873 'Home - Related Private Strucutures Away From Premises
            'TheftCoverageIncreaseOffPremise = 20111 'Theft Coverage Increase - Off-Premises
            'TheftCoverageIncreaseOnPremise = 20110 'Theft Coverage Increase - On-Premises
            ''FunctionalReplacementCostLossSettlement = 40172 'Functional Replacement Cost Loss Assessment 'added 11/26/2014

            '12/2/2013... in case we ever want to use caption instead of coverage code desc
            'ActualCashValueLossSettlement_HO_0481 = 40173 'Actual Cash Value Loss Settlement; HOM
            'ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing_HO_0493 = 40142 'Actual Cash Value Loss Settlement / Windstorm or Hail Losses to Roof Surfacing; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BackupofSewerOrDrain_92_173 = 144 'Back up Sewers and Drains; HOM
            'BuildingAdditionsAndAlterations_HO_51 = 20075 'Building Additions and Alterations; HOM
            'BusinessPropertyIncreasedLimits_HO_312 = 20098 'Business Property Increased; HOM
            'ConsenttoMoveMobileHome_ML_25 = 70106 'Farm_Consent_to_Move_Mobile_Home; HOM
            'Cov_A_SpecifiedAdditionalAmountofInsurance_29_034 = 877 'Home - Coverage A Special Coverage; HOM
            'CreditCardFundTransferCardForgeryAndCounterfeitMoneyCoverage_HO_53 = 20126 'Credit Card/Fund Trans/Forgery/Etc; HOM
            'DebrisRemoval_92_267 = 40116 'Debris Removal; HOM
            'Earthquake_HO_315B = 40091 'Earthquake; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'EquipmentBreakdownCoverage_92_132HO = 80059 'Equipment_Breakdown_Coverage; HOM
            'FireDepartmentServiceCharge_ = 70142 'Farm_Fire_Department_Service_Charge; HOM
            'Firearms_Cov_CIncreasedSpecialLimitsofLiability_HO_65HO_221 = 20115 'Firearms; HOM
            'HomeownerEnhancementEndorsement_92_267 = 80057 'Homeowner Enhancement Endorsement; HOM
            'IncreasedLimitsMotorizedVehicles_ML_65 = 80058 'Increased Limits Motorized Vehicles; HOM
            'InflationGuard = 20000 'Inflation_Guard; HOM
            'JewelryWatchesAndFurs_Cov_CIncreasedSpecialLimitsofLiability_Limitedto1000peritem_HO_61 = 20116 'Jewelry, Watches & Furs; HOM
            'LossAssessment_HO_35 = 70259 'Loss Assessment; HOM
            'LossAssessment_Earthquake_HO_35B = 70260 'Loss Assessment - Earthquake; HOM
            'MineSubsidenceCovAAndB_92_074 = 80103 'Mine Subsidence Cov A & B; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'MineSubsidenceCovA_92_074 = 80102 'Mine Subsidence Cov A; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'ModifiedLossSettlement = 20102 'Loss Settlement; DFR
            'Money_Cov_CIncreasedSpecialLimitsofLiability_HO_65HO_221 = 40117 'Money; HOM
            'OrdinanceOrLaw_HO_277 = 20125 'Ordinance or Law; HOM
            'OutdoorAntennas_ML_49 = 70143 'Farm_Outdoor_Antenna_Satellite_Dish; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'PersonalPropertyIncreaseLimit_OtherResidence_HO_50 = 70304 'Personal Property at Other Residence Increase Limit; HOM
            'PersonalPropertyReplacementCost_92_02392_195 = 20103 'Personal Property Replacement; HOM
            'RefrigeratedFoodProducts_92_267 = 20127 'Refrigerated Property; HOM
            'Securities_Cov_CIncreasedSpecialLimitsofLiability_HO_65HO_221 = 40118 'Securities; HOM
            'SilverwareGoldwarePewterware_Cov_CIncreasedSpecialLimitsofLiability_HO_61 = 20119 'Silverware, Goldware, Pewterware; HOM
            'SinkholeCollapse_HO_99 = 20105 'Sinkhole Collapse; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'SpecialComputerCoverage_HO_314 = 20106 'Special Computer Coverage; HOM
            'SpecifiedOtherStructures_OnPremises_92_049 = 70064 'Cov_B_Related_Private_Structures; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'SpecifiedOtherStructuresOffPremises_92_147 = 873 'Home - Related Private Strucutures Away From Premises; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'TheftCoverageIncreaseOffPremise = 20111 'Theft Coverage Increase - Off-Premises; DFR
            'TheftCoverageIncreaseOnPremise = 20110 'Theft Coverage Increase - On-Premises; DFR
            'TheftofBuildingMaterials_92_367 = 50095 'Theft of Building Material; HOM
            'TripCollision_ML_26 = 80049 'Trip Collision; HOM
            'Unit_OwnersCoverageA_HO_32 = 20038 'Unit Owners Coverage A; HOM
            'VendorsSingleInterest_ML_27 = 20081 'Mobile Home Lienholder's Single Interest; HOM
            'FunctionalReplacementCostLossSettlement_HO0530 = 40172 'Functional Replacement Cost Loss Assessment; HOM and DFR (caption/CoverageCodeDscr from HOM) 'added 11/26/2014
            '12/3/2013: without values
            'ActualCashValueLossSettlement_HO_0481 '40173; Actual Cash Value Loss Settlement; HOM
            'ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing_HO_0493 '40142; Actual Cash Value Loss Settlement / Windstorm or Hail Losses to Roof Surfacing; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BackupofSewerOrDrain_92_173 '144; Back up Sewers and Drains; HOM
            'BuildingAdditionsAndAlterations_HO_51 '20075; Building Additions and Alterations; HOM
            'BusinessPropertyIncreasedLimits_HO_312 '20098; Business Property Increased; HOM
            'ConsenttoMoveMobileHome_ML_25 '70106; Farm_Consent_to_Move_Mobile_Home; HOM
            'Cov_A_SpecifiedAdditionalAmountofInsurance_29_034 '877; Home - Coverage A Special Coverage; HOM
            'CreditCardFundTransferCardForgeryAndCounterfeitMoneyCoverage_HO_53 '20126; Credit Card/Fund Trans/Forgery/Etc; HOM
            'DebrisRemoval_92_267 '40116; Debris Removal; HOM
            'Earthquake_HO_315B '40091; Earthquake; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'EquipmentBreakdownCoverage_92_132HO '80059; Equipment_Breakdown_Coverage; HOM
            'FireDepartmentServiceCharge_ '70142; Farm_Fire_Department_Service_Charge; HOM
            'Firearms_Cov_CIncreasedSpecialLimitsofLiability_HO_65HO_221 '20115; Firearms; HOM
            'HomeownerEnhancementEndorsement_92_267 '80057; Homeowner Enhancement Endorsement; HOM
            'IncreasedLimitsMotorizedVehicles_ML_65 '80058; Increased Limits Motorized Vehicles; HOM
            'InflationGuard '20000; Inflation_Guard; HOM
            'JewelryWatchesAndFurs_Cov_CIncreasedSpecialLimitsofLiability_Limitedto1000peritem_HO_61 '20116; Jewelry, Watches & Furs; HOM
            'LossAssessment_HO_35 '70259; Loss Assessment; HOM
            'LossAssessment_Earthquake_HO_35B '70260; Loss Assessment - Earthquake; HOM
            'MineSubsidenceCovAAndB_92_074 '80103; Mine Subsidence Cov A & B; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'MineSubsidenceCovA_92_074 '80102; Mine Subsidence Cov A; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'ModifiedLossSettlement '20102; Loss Settlement; DFR
            'Money_Cov_CIncreasedSpecialLimitsofLiability_HO_65HO_221 '40117; Money; HOM
            'OrdinanceOrLaw_HO_277 '20125; Ordinance or Law; HOM
            'OutdoorAntennas_ML_49 '70143; Farm_Outdoor_Antenna_Satellite_Dish; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'PersonalPropertyIncreaseLimit_OtherResidence_HO_50 '70304; Personal Property at Other Residence Increase Limit; HOM
            'PersonalPropertyReplacementCost_92_02392_195 '20103; Personal Property Replacement; HOM
            'RefrigeratedFoodProducts_92_267 '20127; Refrigerated Property; HOM
            'Securities_Cov_CIncreasedSpecialLimitsofLiability_HO_65HO_221 '40118; Securities; HOM
            'SilverwareGoldwarePewterware_Cov_CIncreasedSpecialLimitsofLiability_HO_61 '20119; Silverware, Goldware, Pewterware; HOM
            'SinkholeCollapse_HO_99 '20105; Sinkhole Collapse; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'SpecialComputerCoverage_HO_314 '20106; Special Computer Coverage; HOM
            'SpecifiedOtherStructures_OnPremises_92_049 '70064; Cov_B_Related_Private_Structures; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'SpecifiedOtherStructuresOffPremises_92_147 '873; Home - Related Private Strucutures Away From Premises; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'TheftCoverageIncreaseOffPremise '20111; Theft Coverage Increase - Off-Premises; DFR
            'TheftCoverageIncreaseOnPremise '20110; Theft Coverage Increase - On-Premises; DFR
            'TheftofBuildingMaterials_92_367 '50095; Theft of Building Material; HOM
            'TripCollision_ML_26 '80049; Trip Collision; HOM
            'Unit_OwnersCoverageA_HO_32 '20038; Unit Owners Coverage A; HOM
            'VendorsSingleInterest_ML_27 '20081; Mobile Home Lienholder's Single Interest; HOM
            'FunctionalReplacementCostLossSettlement_HO0530 '40172; Functional Replacement Cost Loss Assessment; HOM and DFR (caption/CoverageCodeDscr from HOM) 'added 11/26/2014
            'added 5/18/2015 for Farm
            'BusinessProperty_BusinessOccupancyonPremises '70102; Farm_Business_Property_Business_Occupancy_on_Premises
            'BusinessPropertyIncreasedLimits '70101; Farm_Business_Property_Increased_Limits
            'CollisionOrUpset '70103; Farm_Collision_or_Upset
            'Cov_BRelatedPrivateStructures '554; Farm. Private Structures - Increased Limits for Specific Structures
            'DwellingUnderConstruction_Theft '70108; Farm_Dwelling_Under_Construction_Theft
            'Earthquake '80137; Earthquake_Location
            'ExpandedReplacementCost '70110; Farm_Expanded_Replacement_Cost
            'FarmSprinklerLeakage '80142; Location_Farm_Sprinkler_Leakage
            'GunsIncreasedLimits '70111; Farm_Guns_Increased_Limits
            'IdentityRecoveryExpense '70213; Identity Fraud Expense
            'JewelryIncreasedLimits '70112; Farm_Jewelry_Increased_Limits
            'MoneyIncreasedLimits '70114; Farm_Money_Increased_Limits
            'MotorizedVehiclesIncreasedLimits '70115; Farm_Motorized_Vehicles_Increased_Limits
            'ReplacementValue_NumberofWellPumps '70118; Farm_Replacement_Value_Number_of_Well_Pumps
            'ReplacementValuePersonalPropertyCovC_ '70117; Farm_Replacement_Value_Personal_Property_Cov_C_
            'SecuredPartysInterest '70119; Farm_Secured_Party_Interest
            'SecuritiesIncreasedLimits '70120; Farm_Securities_Increased_Limits
            'SilverwareGoldwareAndPewterwareIncreasedLimits '70121; Farm_Silverware_Goldware_and_Pewterware_Increased_Limits

            'updated 12/2/2013 to use coverage code desc instead of caption
            'ActualCashValueLossSettlement = 40173 'Actual Cash Value Loss Settlement (HO-04 81); HOM
            'ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing = 40142 'Actual Cash Value Loss Settlement / Windstorm or Hail Losses to Roof Surfacing (HO-04 93); HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BackupSewersAndDrains = 144 'Backup of Sewer or Drain (92-173); HOM
            'BuildingAdditionsAndAlterations = 20075 'Building Additions and Alterations (HO-51); HOM
            'BusinessPropertyIncreased = 20098 'Business Property Increased Limits (HO-312); HOM
            'Cov_B_Related_Private_Structures = 70064 'Specified Other Structures - On Premises (92-049); HOM and DFR (caption/CoverageCodeDscr from HOM)
            'CreditCardFundTransForgeryEtc = 20126 'Credit Card, Fund Transfer Card, Forgery and Counterfeit Money Coverage (HO-53); HOM
            'DebrisRemoval = 40116 'Debris Removal (92-267); HOM
            'Earthquake = 40091 'Earthquake (HO-315B); HOM and DFR (caption/CoverageCodeDscr from HOM)
            'Equipment_Breakdown_Coverage = 80059 'Equipment Breakdown Coverage (92-132 HO); HOM
            'Farm_Consent_to_Move_Mobile_Home = 70106 'Consent to Move Mobile Home (ML-25); HOM
            'Farm_Fire_Department_Service_Charge = 70142 'Fire Department Service Charge (); HOM
            'Farm_Outdoor_Antenna_Satellite_Dish = 70143 'Outdoor Antennas (ML-49); HOM and DFR (caption/CoverageCodeDscr from HOM)
            'Firearms = 20115 'Firearms - Cov. C Increased Special Limits of Liability (HO-65 / HO-221); HOM
            'Home_CoverageASpecialCoverage = 877 'Cov. A - Specified Additional Amount of Insurance (29-034); HOM
            'Home_RelatedPrivateStrucuturesAwayFromPremises = 873 'Specified Other Structures Off Premises (92-147); HOM and DFR (caption/CoverageCodeDscr from HOM)
            'HomeownerEnhancementEndorsement = 80057 'Homeowner Enhancement Endorsement (92-267); HOM
            'IncreasedLimitsMotorizedVehicles = 80058 'Increased Limits Motorized Vehicles (ML-65); HOM
            'Inflation_Guard = 20000 'Inflation Guard; HOM
            'JewelryWatchesAndFurs = 20116 'Jewelry, Watches & Furs - Cov. C Increased Special Limits of Liability - Limited to $1000 per item (HO-61); HOM
            'LossAssessment = 70259 'Loss Assessment (HO-35); HOM
            'LossAssessment_Earthquake = 70260 'Loss Assessment - Earthquake (HO-35B); HOM
            'LossSettlement = 20102 'Modified Loss Settlement; DFR
            'MineSubsidenceCovA = 80102 'Mine Subsidence Cov A (92-074); HOM and DFR (caption/CoverageCodeDscr from HOM)
            'MineSubsidenceCovAAndB = 80103 'Mine Subsidence Cov A & B (92-074); HOM and DFR (caption/CoverageCodeDscr from HOM)
            'MobileHomeLienholdersSingleInterest = 20081 'Vendor's Single Interest (ML-27); HOM
            'Money = 40117 'Money - Cov. C Increased Special Limits of Liability (HO-65 / HO-221); HOM
            'OrdinanceOrLaw = 20125 'Ordinance or Law (HO-277); HOM
            'PersonalPropertyatOtherResidenceIncreaseLimit = 70304 'Personal Property Increase Limit- Other Residence (HO-50); HOM
            'PersonalPropertyReplacement = 20103 'Personal Property Replacement Cost (92-023 / 92-195); HOM
            'RefrigeratedProperty = 20127 'Refrigerated Food Products (92-267); HOM
            'Securities = 40118 'Securities - Cov. C Increased Special Limits of Liability (HO-65 / HO-221); HOM
            'SilverwareGoldwarePewterware = 20119 'Silverware, Goldware, Pewterware - Cov. C Increased Special Limits of Liability (HO-61); HOM
            'SinkholeCollapse = 20105 'Sinkhole Collapse (HO-99); HOM and DFR (caption/CoverageCodeDscr from HOM)
            'SpecialComputerCoverage = 20106 'Special Computer Coverage (HO-314); HOM
            'TheftCoverageIncrease_Off_Premises = 20111 'Theft Coverage Increase Off Premise; DFR
            'TheftCoverageIncrease_On_Premises = 20110 'Theft Coverage Increase On Premise; DFR
            'TheftofBuildingMaterial = 50095 'Theft of Building Materials (92-367); HOM
            'TripCollision = 80049 'Trip Collision (ML-26); HOM
            'UnitOwnersCoverageA = 20038 'Unit-Owners Coverage A (HO-32); HOM
            'FunctionalReplacementCostLossAssessment = 40172 'Functional Replacement Cost Loss Settlement (HO 05 30); HOM and DFR (caption/CoverageCodeDscr from HOM) 'added 11/26/2014
            '12/3/2013: without values
            ActualCashValueLossSettlement '40173; Actual Cash Value Loss Settlement (HO-04 81); HOM
            ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing '40142; Actual Cash Value Loss Settlement / Windstorm or Hail Losses to Roof Surfacing (HO-04 93); HOM and DFR (caption/CoverageCodeDscr from HOM)
            BackupSewersAndDrains '144; Backup of Sewer or Drain (92-173); HOM
            BuildingAdditionsAndAlterations '20075; Building Additions and Alterations (HO-51); HOM
            BusinessPropertyIncreased '20098; Business Property Increased Limits (HO-312); HOM
            Cov_B_Related_Private_Structures '70064; Specified Other Structures - On Premises (92-049); HOM and DFR (caption/CoverageCodeDscr from HOM)
            CreditCardFundTransForgeryEtc '20126; Credit Card, Fund Transfer Card, Forgery and Counterfeit Money Coverage (HO-53); HOM
            DebrisRemoval '40116; Debris Removal (92-267); HOM
            Earthquake '40091; Earthquake (HO-315B); HOM and DFR (caption/CoverageCodeDscr from HOM)
            Equipment_Breakdown_Coverage '80059; Equipment Breakdown Coverage (92-132 HO); HOM
            Farm_Consent_to_Move_Mobile_Home '70106; Consent to Move Mobile Home (ML-25); HOM
            Farm_Fire_Department_Service_Charge '70142; Fire Department Service Charge (); HOM
            Farm_Outdoor_Antenna_Satellite_Dish '70143; Outdoor Antennas (ML-49); HOM and DFR (caption/CoverageCodeDscr from HOM)
            Firearms '20115; Firearms - Cov. C Increased Special Limits of Liability (HO-65 / HO-221); HOM
            Home_CoverageASpecialCoverage '877; Cov. A - Specified Additional Amount of Insurance (29-034); HOM
            Home_RelatedPrivateStrucuturesAwayFromPremises '873; Specified Other Structures Off Premises (92-147); HOM and DFR (caption/CoverageCodeDscr from HOM)
            HomeownerEnhancementEndorsement '80057; Homeowner Enhancement Endorsement (92-267); HOM
            IncreasedLimitsMotorizedVehicles '80058; Increased Limits Motorized Vehicles (ML-65); HOM
            Inflation_Guard '20000; Inflation Guard; HOM
            JewelryWatchesAndFurs '20116; Jewelry, Watches & Furs - Cov. C Increased Special Limits of Liability - Limited to $1000 per item (HO-61); HOM
            LossAssessment '70259; Loss Assessment (HO-35); HOM
            LossAssessment_Earthquake '70260; Loss Assessment - Earthquake (HO-35B); HOM
            LossSettlement '20102; Modified Loss Settlement; DFR
            MineSubsidenceCovA '80102; Mine Subsidence Cov A (92-074); HOM and DFR (caption/CoverageCodeDscr from HOM)
            MineSubsidenceCovAAndB '80103; Mine Subsidence Cov A & B (92-074); HOM and DFR (caption/CoverageCodeDscr from HOM)
            MobileHomeLienholdersSingleInterest '20081; Vendor's Single Interest (ML-27); HOM
            Money '40117; Money - Cov. C Increased Special Limits of Liability (HO-65 / HO-221); HOM
            OrdinanceOrLaw '20125; Ordinance or Law (HO-277); HOM
            PersonalPropertyatOtherResidenceIncreaseLimit '70304; Personal Property Increase Limit- Other Residence (HO-50); HOM
            PersonalPropertyReplacement '20103; Personal Property Replacement Cost (92-023 / 92-195); HOM
            RefrigeratedProperty '20127; Refrigerated Food Products (92-267); HOM
            Securities '40118; Securities - Cov. C Increased Special Limits of Liability (HO-65 / HO-221); HOM
            SilverwareGoldwarePewterware '20119; Silverware, Goldware, Pewterware - Cov. C Increased Special Limits of Liability (HO-61); HOM
            SinkholeCollapse '20105; Sinkhole Collapse (HO-99); HOM and DFR (caption/CoverageCodeDscr from HOM)
            SpecialComputerCoverage '20106; Special Computer Coverage (HO-314); HOM
            TheftCoverageIncrease_Off_Premises '20111; Theft Coverage Increase Off Premise; DFR
            TheftCoverageIncrease_On_Premises '20110; Theft Coverage Increase On Premise; DFR
            TheftofBuildingMaterial '50095; Theft of Building Materials (92-367); HOM
            TripCollision '80049; Trip Collision (ML-26); HOM
            UnitOwnersCoverageA '20038; Unit-Owners Coverage A (HO-32); HOM

            'added 11/26/2014
            FunctionalReplacementCostLossAssessment '40172; Functional Replacement Cost Loss Settlement (HO 05 30); HOM and DFR (caption/CoverageCodeDscr from HOM)

            'added 5/18/2015 for Farm
            Earthquake_Location '80137; Earthquake; just FAR (no HOM or DFR)
            Farm_PrivateStructures_IncreasedLimitsForSpecificStructures '554; Cov. B Related Private Structures; just FAR (no HOM or DFR)
            Farm_Business_Property_Business_Occupancy_on_Premises '70102; Business Property - Business Occupancy on Premises; just FAR (no HOM or DFR)
            Farm_Business_Property_Increased_Limits '70101; Business Property Increased Limits; just FAR (no HOM or DFR)
            Farm_Collision_or_Upset '70103; Collision or Upset; just FAR (no HOM or DFR)
            Farm_Dwelling_Under_Construction_Theft '70108; Dwelling Under Construction - Theft; just FAR (no HOM or DFR)
            Farm_Expanded_Replacement_Cost '70110; Expanded Replacement Cost; just FAR (no HOM or DFR)
            Farm_Guns_Increased_Limits '70111; Guns Increased Limits; just FAR (no HOM or DFR)
            Farm_Jewelry_Increased_Limits '70112; Jewelry Increased Limits; just FAR (no HOM or DFR)
            Farm_Money_Increased_Limits '70114; Money Increased Limits; just FAR (no HOM or DFR)
            Farm_Motorized_Vehicles_Increased_Limits '70115; Motorized Vehicles Increased Limits; just FAR (no HOM or DFR)
            Farm_Replacement_Value_Number_of_Well_Pumps '70118; Replacement Value - Number of Well Pumps; just FAR (no HOM or DFR)
            Farm_Replacement_Value_Personal_Property_Cov_C_ '70117; Replacement Value Personal Property Cov C.; just FAR (no HOM or DFR)
            Farm_Secured_Party_Interest '70119; Secured Party's Interest; just FAR (no HOM or DFR)
            Farm_Securities_Increased_Limits '70120; Securities Increased Limits; just FAR (no HOM or DFR)
            Farm_Silverware_Goldware_and_Pewterware_Increased_Limits '70121; Silverware, Gold ware and Pewterware Increased Limits; just FAR (no HOM or DFR)
            IdentityFraudExpense '70213; Identity Recovery Expense; just FAR (no HOM or DFR)
            Location_Farm_Sprinkler_Leakage '80142; Farm Sprinkler Leakage; just FAR (no HOM or DFR)

            'HOM2018 Upgrade
            BroadenedResidencePremisesDefinition '80511
            CovBOtherStructuresAwayFromTheResidencePremises '80257
            GreenUpgrades '80389
            IdentityFraudExpenseHOM0455 '20029
            'PersonalPropertyReplacementCost
            ReplacementCostForNonBuildingStructures '80264
            UndergroundServiceLine '80507
            HomeownersPlusEnhancementEndorsement '80508
            WaterDamage '80520
            PersonalPropertySelfStorageFacilities '80260
            SpecialPersonalProperty '20107
            TheftOfPersonalPropertyInDwellingUnderConstruction '80517
            'SpecificStructuresAwayFromResidencePremises '70308 - Diamond Switched to using Home_RelatedPrivateStrucuturesAwayFromPremises (873)
            'CovASpecifiedAdditionalAmountOfInsurance '20043 - Diamond switched to using Home_CoverageASpecialCoverage (877)

            GraveMarkers '40104 (Included - added by Diamond, not user)
            TreesPlantsAndShrubs '40105 (Included - added by Diamond, not user)
            LandlordsFurnishing '80265 (Included - added by Diamond, not user)
            AntennasTapesWireRecordersDisksAndMediaInAMotorVehicle '80516 (Included - added by Diamond, not user)
            Trailers_NonWatercraft '80518 (Included - added by Diamond, not user)
            Watercraft '80519 (Included - added by Diamond, not user)
            'DamageToPropertyOfOthers '80522 (Included - added by Diamond, not user) - Moved to Section II
            PortableElectronicsInAMotorVehicle '80259
            OtherStructuresOnTheResidencePremises '70303 - Diamond switched to this from Cov_B_Related_Private_Structures (70064)

            'added 06/26/2020 for Ohio Farm
            Cosmetic_Damage_Exclusion '80555
            Family_Cyber_Protection '80572
            HomeownerEnhancementEndorsement1019 '80570; Homeowner Enhancement Endorsement (1019)
            HomeownersPlusEnhancementEndorsement1020 '80571
            UnitOwnersCoverageCSpecialCoverage '70309 Unit Owners Coverages C Special Coverage (HO 1731)

        End Enum
        ''' <summary>
        ''' valid types for HOM section I coverages
        ''' </summary>
        ''' <remarks>value previously corresponded to coveragecode_id in Diamond; now see static data xml file; this enum type should be used for HOM instead of general one for HOM and DFR</remarks>
        Enum HOM_SectionICoverageType 'added 8/16/2013 to differentiate HOM from DFR
            'None = 0
            '12/3/2013: without values
            None
            'ActualCashValueLossSettlement_HO_0481 = 40173 'Actual Cash Value Loss Settlement
            'ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing_HO_0493 = 40142 'Actual Cash Value Loss Settlement / Windstorm or Hail Losses to Roof Surfacing
            'BackupofSewerOrDrain_92_173 = 144 'Back up Sewers and Drains
            'BuildingAdditionsAndAlterations_HO_51 = 20075 'Building Additions and Alterations
            'BusinessPropertyIncreasedLimits_HO_312 = 20098 'Business Property Increased
            'ConsenttoMoveMobileHome_ML_25 = 70106 'Farm_Consent_to_Move_Mobile_Home
            'Cov_A_SpecifiedAdditionalAmountofInsurance_29_034 = 877 'Home - Coverage A Special Coverage
            'CreditCardFundTransferCardForgeryAndCounterfeitMoneyCoverage_HO_53 = 20126 'Credit Card/Fund Trans/Forgery/Etc
            'DebrisRemoval_92_267 = 40116 'Debris Removal
            'Earthquake_HO_315B = 40091 'Earthquake
            'EquipmentBreakdownCoverage_92_132HO = 80059 'Equipment_Breakdown_Coverage
            'FireDepartmentServiceCharge_ = 70142 'Farm_Fire_Department_Service_Charge
            'Firearms_Cov_CIncreasedSpecialLimitsofLiability_HO_65HO_221 = 20115 'Firearms
            'HomeownerEnhancementEndorsement_92_267 = 80057 'Homeowner Enhancement Endorsement
            'IncreasedLimitsMotorizedVehicles_ML_65 = 80058 'Increased Limits Motorized Vehicles
            'InflationGuard = 20000 'Inflation_Guard
            'JewelryWatchesAndFurs_Cov_CIncreasedSpecialLimitsofLiability_Limitedto1000peritem_HO_61 = 20116 'Jewelry, Watches & Furs
            'LossAssessment_HO_35 = 70259 'Loss Assessment
            'LossAssessment_Earthquake_HO_35B = 70260 'Loss Assessment - Earthquake
            'MineSubsidenceCovAAndB_92_074 = 80103 'Mine Subsidence Cov A & B
            'MineSubsidenceCovA_92_074 = 80102 'Mine Subsidence Cov A
            'Money_Cov_CIncreasedSpecialLimitsofLiability_HO_65HO_221 = 40117 'Money
            'OrdinanceOrLaw_HO_277 = 20125 'Ordinance or Law
            'OutdoorAntennas_ML_49 = 70143 'Farm_Outdoor_Antenna_Satellite_Dish
            'PersonalPropertyIncreaseLimit_OtherResidence_HO_50 = 70304 'Personal Property at Other Residence Increase Limit
            'PersonalPropertyReplacementCost_92_02392_195 = 20103 'Personal Property Replacement
            'RefrigeratedFoodProducts_92_267 = 20127 'Refrigerated Property
            'Securities_Cov_CIncreasedSpecialLimitsofLiability_HO_65HO_221 = 40118 'Securities
            'SilverwareGoldwarePewterware_Cov_CIncreasedSpecialLimitsofLiability_HO_61 = 20119 'Silverware, Goldware, Pewterware
            'SinkholeCollapse_HO_99 = 20105 'Sinkhole Collapse
            'SpecialComputerCoverage_HO_314 = 20106 'Special Computer Coverage
            'SpecifiedOtherStructures_OnPremises_92_049 = 70064 'Cov_B_Related_Private_Structures
            'SpecifiedOtherStructuresOffPremises_92_147 = 873 'Home - Related Private Strucutures Away From Premises
            'TheftofBuildingMaterials_92_367 = 50095 'Theft of Building Material
            'TripCollision_ML_26 = 80049 'Trip Collision
            'Unit_OwnersCoverageA_HO_32 = 20038 'Unit Owners Coverage A
            'VendorsSingleInterest_ML_27 = 20081 'Mobile Home Lienholder's Single Interest
            'FunctionalReplacementCostLossSettlement_HO0530 = 40172 'Functional Replacement Cost Loss Assessment 'added 11/26/2014
            '12/3/2013: without values
            'ActualCashValueLossSettlement_HO_0481 '40173; Actual Cash Value Loss Settlement
            'ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing_HO_0493 '40142; Actual Cash Value Loss Settlement / Windstorm or Hail Losses to Roof Surfacing
            'BackupofSewerOrDrain_92_173 '144; Back up Sewers and Drains
            'BuildingAdditionsAndAlterations_HO_51 '20075; Building Additions and Alterations
            'BusinessPropertyIncreasedLimits_HO_312 '20098; Business Property Increased
            'ConsenttoMoveMobileHome_ML_25 '70106; Farm_Consent_to_Move_Mobile_Home
            'Cov_A_SpecifiedAdditionalAmountofInsurance_29_034 '877; Home - Coverage A Special Coverage
            'CreditCardFundTransferCardForgeryAndCounterfeitMoneyCoverage_HO_53 '20126; Credit Card/Fund Trans/Forgery/Etc
            'DebrisRemoval_92_267 '40116; Debris Removal
            'Earthquake_HO_315B '40091; Earthquake
            'EquipmentBreakdownCoverage_92_132HO '80059; Equipment_Breakdown_Coverage
            'FireDepartmentServiceCharge_ '70142; Farm_Fire_Department_Service_Charge
            'Firearms_Cov_CIncreasedSpecialLimitsofLiability_HO_65HO_221 '20115; Firearms
            'HomeownerEnhancementEndorsement_92_267 '80057; Homeowner Enhancement Endorsement
            'IncreasedLimitsMotorizedVehicles_ML_65 '80058; Increased Limits Motorized Vehicles
            'InflationGuard '20000; Inflation_Guard
            'JewelryWatchesAndFurs_Cov_CIncreasedSpecialLimitsofLiability_Limitedto1000peritem_HO_61 '20116; Jewelry, Watches & Furs
            'LossAssessment_HO_35 '70259; Loss Assessment
            'LossAssessment_Earthquake_HO_35B '70260; Loss Assessment - Earthquake
            'MineSubsidenceCovAAndB_92_074 '80103; Mine Subsidence Cov A & B
            'MineSubsidenceCovA_92_074 '80102; Mine Subsidence Cov A
            'Money_Cov_CIncreasedSpecialLimitsofLiability_HO_65HO_221 '40117; Money
            'OrdinanceOrLaw_HO_277 '20125; Ordinance or Law
            'OutdoorAntennas_ML_49 '70143; Farm_Outdoor_Antenna_Satellite_Dish
            'PersonalPropertyIncreaseLimit_OtherResidence_HO_50 '70304; Personal Property at Other Residence Increase Limit
            'PersonalPropertyReplacementCost_92_02392_195 '20103; Personal Property Replacement
            'RefrigeratedFoodProducts_92_267 '20127; Refrigerated Property
            'Securities_Cov_CIncreasedSpecialLimitsofLiability_HO_65HO_221 '40118; Securities
            'SilverwareGoldwarePewterware_Cov_CIncreasedSpecialLimitsofLiability_HO_61 '20119; Silverware, Goldware, Pewterware
            'SinkholeCollapse_HO_99 '20105; Sinkhole Collapse
            'SpecialComputerCoverage_HO_314 '20106; Special Computer Coverage
            'SpecifiedOtherStructures_OnPremises_92_049 '70064; Cov_B_Related_Private_Structures
            'SpecifiedOtherStructuresOffPremises_92_147 '873; Home - Related Private Strucutures Away From Premises
            'TheftofBuildingMaterials_92_367 '50095; Theft of Building Material
            'TripCollision_ML_26 '80049; Trip Collision
            'Unit_OwnersCoverageA_HO_32 '20038; Unit Owners Coverage A
            'VendorsSingleInterest_ML_27 '20081; Mobile Home Lienholder's Single Interest
            'FunctionalReplacementCostLossSettlement_HO0530 '40172; Functional Replacement Cost Loss Assessment 'added 11/26/2014

            'updated 12/2/2013 to use coverage code desc instead of caption
            'ActualCashValueLossSettlement = 40173 'Actual Cash Value Loss Settlement (HO-04 81)
            'ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing = 40142 'Actual Cash Value Loss Settlement / Windstorm or Hail Losses to Roof Surfacing (HO-04 93)
            'BackupSewersAndDrains = 144 'Backup of Sewer or Drain (92-173)
            'BuildingAdditionsAndAlterations = 20075 'Building Additions and Alterations (HO-51)
            'BusinessPropertyIncreased = 20098 'Business Property Increased Limits (HO-312)
            'Cov_B_Related_Private_Structures = 70064 'Specified Other Structures - On Premises (92-049)
            'CreditCardFundTransForgeryEtc = 20126 'Credit Card, Fund Transfer Card, Forgery and Counterfeit Money Coverage (HO-53)
            'DebrisRemoval = 40116 'Debris Removal (92-267)
            'Earthquake = 40091 'Earthquake (HO-315B)
            'Equipment_Breakdown_Coverage = 80059 'Equipment Breakdown Coverage (92-132 HO)
            'Farm_Consent_to_Move_Mobile_Home = 70106 'Consent to Move Mobile Home (ML-25)
            'Farm_Fire_Department_Service_Charge = 70142 'Fire Department Service Charge ()
            'Farm_Outdoor_Antenna_Satellite_Dish = 70143 'Outdoor Antennas (ML-49)
            'Firearms = 20115 'Firearms - Cov. C Increased Special Limits of Liability (HO-65 / HO-221)
            'Home_CoverageASpecialCoverage = 877 'Cov. A - Specified Additional Amount of Insurance (29-034)
            'Home_RelatedPrivateStrucuturesAwayFromPremises = 873 'Specified Other Structures Off Premises (92-147)
            'HomeownerEnhancementEndorsement = 80057 'Homeowner Enhancement Endorsement (92-267)
            'IncreasedLimitsMotorizedVehicles = 80058 'Increased Limits Motorized Vehicles (ML-65)
            'Inflation_Guard = 20000 'Inflation Guard
            'JewelryWatchesAndFurs = 20116 'Jewelry, Watches & Furs - Cov. C Increased Special Limits of Liability - Limited to $1000 per item (HO-61)
            'LossAssessment = 70259 'Loss Assessment (HO-35)
            'LossAssessment_Earthquake = 70260 'Loss Assessment - Earthquake (HO-35B)
            'MineSubsidenceCovA = 80102 'Mine Subsidence Cov A (92-074)
            'MineSubsidenceCovAAndB = 80103 'Mine Subsidence Cov A & B (92-074)
            'MobileHomeLienholdersSingleInterest = 20081 'Vendor's Single Interest (ML-27)
            'Money = 40117 'Money - Cov. C Increased Special Limits of Liability (HO-65 / HO-221)
            'OrdinanceOrLaw = 20125 'Ordinance or Law (HO-277)
            'PersonalPropertyatOtherResidenceIncreaseLimit = 70304 'Personal Property Increase Limit- Other Residence (HO-50)
            'PersonalPropertyReplacement = 20103 'Personal Property Replacement Cost (92-023 / 92-195)
            'RefrigeratedProperty = 20127 'Refrigerated Food Products (92-267)
            'Securities = 40118 'Securities - Cov. C Increased Special Limits of Liability (HO-65 / HO-221)
            'SilverwareGoldwarePewterware = 20119 'Silverware, Goldware, Pewterware - Cov. C Increased Special Limits of Liability (HO-61)
            'SinkholeCollapse = 20105 'Sinkhole Collapse (HO-99)
            'SpecialComputerCoverage = 20106 'Special Computer Coverage (HO-314)
            'TheftofBuildingMaterial = 50095 'Theft of Building Materials (92-367)
            'TripCollision = 80049 'Trip Collision (ML-26)
            'UnitOwnersCoverageA = 20038 'Unit-Owners Coverage A (HO-32)
            'FunctionalReplacementCostLossAssessment = 40172 'Functional Replacement Cost Loss Settlement (HO 05 30) 'added 11/26/2014
            '12/3/2013: without values
            ActualCashValueLossSettlement '40173; Actual Cash Value Loss Settlement (HO-04 81)
            ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing '40142; Actual Cash Value Loss Settlement / Windstorm or Hail Losses to Roof Surfacing (HO-04 93)
            BackupSewersAndDrains '144; Backup of Sewer or Drain (92-173)
            BuildingAdditionsAndAlterations '20075; Building Additions and Alterations (HO-51)
            BusinessPropertyIncreased '20098; Business Property Increased Limits (HO-312)
            Cov_B_Related_Private_Structures '70064; Specified Other Structures - On Premises (92-049)
            CreditCardFundTransForgeryEtc '20126; Credit Card, Fund Transfer Card, Forgery and Counterfeit Money Coverage (HO-53)
            DebrisRemoval '40116; Debris Removal (92-267)
            Earthquake '40091; Earthquake (HO-315B)
            Equipment_Breakdown_Coverage '80059; Equipment Breakdown Coverage (92-132 HO)
            Farm_Consent_to_Move_Mobile_Home '70106; Consent to Move Mobile Home (ML-25)
            Farm_Fire_Department_Service_Charge '70142; Fire Department Service Charge ()
            Farm_Outdoor_Antenna_Satellite_Dish '70143; Outdoor Antennas (ML-49)
            Firearms '20115; Firearms - Cov. C Increased Special Limits of Liability (HO-65 / HO-221)
            Home_CoverageASpecialCoverage '877; Cov. A - Specified Additional Amount of Insurance (29-034)
            Home_RelatedPrivateStrucuturesAwayFromPremises '873; Specified Other Structures Off Premises (92-147)
            HomeownerEnhancementEndorsement '80057; Homeowner Enhancement Endorsement (92-267)
            IncreasedLimitsMotorizedVehicles '80058; Increased Limits Motorized Vehicles (ML-65)
            Inflation_Guard '20000; Inflation Guard
            JewelryWatchesAndFurs '20116; Jewelry, Watches & Furs - Cov. C Increased Special Limits of Liability - Limited to $1000 per item (HO-61)
            LossAssessment '70259; Loss Assessment (HO-35)
            LossAssessment_Earthquake '70260; Loss Assessment - Earthquake (HO-35B)
            MineSubsidenceCovA '80102; Mine Subsidence Cov A (92-074)
            MineSubsidenceCovAAndB '80103; Mine Subsidence Cov A & B (92-074)
            MobileHomeLienholdersSingleInterest '20081; Vendor's Single Interest (ML-27)
            Money '40117; Money - Cov. C Increased Special Limits of Liability (HO-65 / HO-221)
            OrdinanceOrLaw '20125; Ordinance or Law (HO-277)
            PersonalPropertyatOtherResidenceIncreaseLimit '70304; Personal Property Increase Limit- Other Residence (HO-50)
            PersonalPropertyReplacement '20103; Personal Property Replacement Cost (92-023 / 92-195)
            RefrigeratedProperty '20127; Refrigerated Food Products (92-267)
            Securities '40118; Securities - Cov. C Increased Special Limits of Liability (HO-65 / HO-221)
            SilverwareGoldwarePewterware '20119; Silverware, Goldware, Pewterware - Cov. C Increased Special Limits of Liability (HO-61)
            SinkholeCollapse '20105; Sinkhole Collapse (HO-99)
            SpecialComputerCoverage '20106; Special Computer Coverage (HO-314)
            TheftofBuildingMaterial '50095; Theft of Building Materials (92-367)
            TripCollision '80049; Trip Collision (ML-26)
            UnitOwnersCoverageA '20038; Unit-Owners Coverage A (HO-32)

            'added 11/26/2014
            FunctionalReplacementCostLossAssessment '40172; Functional Replacement Cost Loss Settlement (HO 05 30)

            'HOM2018 Upgrade
            BroadenedResidencePremisesDefinition '80511
            CovBOtherStructuresAwayFromTheResidencePremises '80257
            GreenUpgrades '80389
            IdentityFraudExpenseHOM0455 '20029
            'PersonalPropertyReplacementCost
            ReplacementCostForNonBuildingStructures '80264
            UndergroundServiceLine '80507
            HomeownersPlusEnhancementEndorsement '80508
            WaterDamage '80520
            PersonalPropertySelfStorageFacilities '80260
            SpecialPersonalProperty '20107
            TheftOfPersonalPropertyInDwellingUnderConstruction '80517
            'SpecificStructuresAwayFromResidencePremises '70308 - Diamond Switched to using Home_RelatedPrivateStrucuturesAwayFromPremises (873)
            'CovASpecifiedAdditionalAmountOfInsurance '20043 - Diamond switched to using Home_CoverageASpecialCoverage (877)
            GraveMarkers '40104 (Included - added by Diamond, not user)
            TreesPlantsAndShrubs '40105 (Included - added by Diamond, not user)
            LandlordsFurnishing '80265 (Included - added by Diamond, not user)
            AntennasTapesWireRecordersDisksAndMediaInAMotorVehicle '80516 (Included - added by Diamond, not user)
            Trailers_NonWatercraft '80518 (Included - added by Diamond, not user)
            Watercraft '80519 (Included - added by Diamond, not user)
            'DamageToPropertyOfOthers '80522 Moved to Section II
            PortableElectronicsInAMotorVehicle '80259
            OtherStructuresOnTheResidencePremises '70303 - Diamond switched to this from Cov_B_Related_Private_Structures (70064)
            Family_Cyber_Protection '80572
            HomeownerEnhancementEndorsement1019 '80570; Homeowner Enhancement Endorsement (1019)
            HomeownersPlusEnhancementEndorsement1020 '80571
            UnitOwnersCoverageCSpecialCoverage ' 70309 Unit Owners Coverages C Special Coverage (HO 1731)

        End Enum
        ''' <summary>
        ''' valid types for DFR section I coverages
        ''' </summary>
        ''' <remarks>value previously corresponded to coveragecode_id in Diamond; now see static data xml file; this enum type should be used for DFR instead of general one for HOM and DFR</remarks>
        Enum DFR_SectionICoverageType 'added 8/16/2013 to differentiate DFR from HOM
            'None = 0
            '12/3/2013: without values
            None
            'ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing = 40142 'Actual Cash Value Loss Settlement / Windstorm or Hail Losses to Roof Surfacing
            'AwningssignsAndoutdoorradioAndTVEquipment = 70143 'Farm_Outdoor_Antenna_Satellite_Dish
            'Earthquake = 40091 'Earthquake
            'MineSubsidenceCovA = 80102 'Mine Subsidence Cov A
            'MineSubsidenceCovAAndB = 80103 'Mine Subsidence Cov A & B
            'ModifiedLossSettlement = 20102 'Loss Settlement
            'SinkholeCollapse = 20105 'Sinkhole Collapse
            'SpecifiedOtherStructures_OnPremises = 70064 'Cov_B_Related_Private_Structures
            'SpecifiedOtherStructuresOffPremises = 873 'Home - Related Private Strucutures Away From Premises
            'TheftCoverageIncreaseOffPremise = 20111 'Theft Coverage Increase - Off-Premises
            'TheftCoverageIncreaseOnPremise = 20110 'Theft Coverage Increase - On-Premises
            'FunctionalReplacementCostLossSettlement = 40172 'Functional Replacement Cost Loss Assessment 'added 11/26/2014
            '12/3/2013: without values
            'ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing '40142; Actual Cash Value Loss Settlement / Windstorm or Hail Losses to Roof Surfacing
            'AwningssignsAndoutdoorradioAndTVEquipment '70143; Farm_Outdoor_Antenna_Satellite_Dish
            'Earthquake '40091; Earthquake
            'MineSubsidenceCovA '80102; Mine Subsidence Cov A
            'MineSubsidenceCovAAndB '80103; Mine Subsidence Cov A & B
            'ModifiedLossSettlement '20102; Loss Settlement
            'SinkholeCollapse '20105; Sinkhole Collapse
            'SpecifiedOtherStructures_OnPremises '70064; Cov_B_Related_Private_Structures
            'SpecifiedOtherStructuresOffPremises '873; Home - Related Private Strucutures Away From Premises
            'TheftCoverageIncreaseOffPremise '20111; Theft Coverage Increase - Off-Premises
            'TheftCoverageIncreaseOnPremise '20110; Theft Coverage Increase - On-Premises
            'FunctionalReplacementCostLossSettlement '40172; Functional Replacement Cost Loss Assessment 'added 11/26/2014

            'updated 12/2/2013 to use coverage code desc instead of caption
            'ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing = 40142 'Actual Cash Value Loss Settlement / Windstorm or Hail Losses to Roof Surfacing
            'Cov_B_Related_Private_Structures = 70064 'Specified Other Structures - On Premises
            'Earthquake = 40091 'Earthquake
            'Farm_Outdoor_Antenna_Satellite_Dish = 70143 'Awnings, signs and outdoor radio and TV Equipment
            'Home_RelatedPrivateStrucuturesAwayFromPremises = 873 'Specified Other Structures Off Premises
            'LossSettlement = 20102 'Modified Loss Settlement
            'MineSubsidenceCovA = 80102 'Mine Subsidence Cov A
            'MineSubsidenceCovAAndB = 80103 'Mine Subsidence Cov A & B
            'SinkholeCollapse = 20105 'Sinkhole Collapse
            'TheftCoverageIncrease_Off_Premises = 20111 'Theft Coverage Increase Off Premise
            'TheftCoverageIncrease_On_Premises = 20110 'Theft Coverage Increase On Premise
            'FunctionalReplacementCostLossAssessment = 40172 'Functional Replacement Cost Loss Settlement 'added 11/26/2014
            '12/3/2013: without values
            ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing '40142; Actual Cash Value Loss Settlement / Windstorm or Hail Losses to Roof Surfacing
            Cov_B_Related_Private_Structures '70064; Specified Other Structures - On Premises
            Earthquake '40091; Earthquake
            Farm_Outdoor_Antenna_Satellite_Dish '70143; Awnings, signs and outdoor radio and TV Equipment
            Home_RelatedPrivateStrucuturesAwayFromPremises '873; Specified Other Structures Off Premises
            LossSettlement '20102; Modified Loss Settlement
            MineSubsidenceCovA '80102; Mine Subsidence Cov A
            MineSubsidenceCovAAndB '80103; Mine Subsidence Cov A & B
            SinkholeCollapse '20105; Sinkhole Collapse
            TheftCoverageIncrease_Off_Premises '20111; Theft Coverage Increase Off Premise
            TheftCoverageIncrease_On_Premises '20110; Theft Coverage Increase On Premise

            'added 11/26/2014
            FunctionalReplacementCostLossAssessment '40172; Functional Replacement Cost Loss Settlement
        End Enum
        'added 5/15/2015 for Farm
        ''' <summary>
        ''' valid types for FAR section I coverages
        ''' </summary>
        ''' <remarks>value previously corresponded to coveragecode_id in Diamond; now see static data xml file; this enum type should be used for FAR instead of general one for HOM, DFR, and FAR</remarks>
        Enum FAR_SectionICoverageType 'added 8/16/2013 to differentiate DFR from HOM
            None

            'using coverage code desc
            ActualCashValueLossSettlement '40173; Actual Cash Value Loss Settlement; also HOM and DFR... 5/18/2015 note: on valid for old DFR version... won't use here
            ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing '40142; Actual Cash Value Loss Settlement / Windstorm or Hail Losses to Roof Surfacing; also HOM and DFR
            Cosmetic_Damage_Exclusion '80555 'added 06/26/2020 for Ohio Farm
            Earthquake_Location '80137; Earthquake; just FAR (no HOM or DFR)
            Farm_PrivateStructures_IncreasedLimitsForSpecificStructures '554; Cov. B Related Private Structures; just FAR (no HOM or DFR)
            Farm_Business_Property_Business_Occupancy_on_Premises '70102; Business Property - Business Occupancy on Premises; just FAR (no HOM or DFR)
            Farm_Business_Property_Increased_Limits '70101; Business Property Increased Limits; just FAR (no HOM or DFR)
            Farm_Collision_or_Upset '70103; Collision or Upset; just FAR (no HOM or DFR)
            Farm_Consent_to_Move_Mobile_Home '70106; Consent to Move Mobile Home; also HOM (no DFR)
            Farm_Dwelling_Under_Construction_Theft '70108; Dwelling Under Construction - Theft; just FAR (no HOM or DFR)
            Farm_Expanded_Replacement_Cost '70110; Expanded Replacement Cost; just FAR (no HOM or DFR)
            Farm_Guns_Increased_Limits '70111; Guns Increased Limits; just FAR (no HOM or DFR)
            Farm_Jewelry_Increased_Limits '70112; Jewelry Increased Limits; just FAR (no HOM or DFR)
            Farm_Money_Increased_Limits '70114; Money Increased Limits; just FAR (no HOM or DFR)
            Farm_Motorized_Vehicles_Increased_Limits '70115; Motorized Vehicles Increased Limits; just FAR (no HOM or DFR)
            Farm_Replacement_Value_Number_of_Well_Pumps '70118; Replacement Value - Number of Well Pumps; just FAR (no HOM or DFR)
            Farm_Replacement_Value_Personal_Property_Cov_C_ '70117; Replacement Value Personal Property Cov C.; just FAR (no HOM or DFR)
            Farm_Secured_Party_Interest '70119; Secured Party's Interest; just FAR (no HOM or DFR)
            Farm_Securities_Increased_Limits '70120; Securities Increased Limits; just FAR (no HOM or DFR)
            Farm_Silverware_Goldware_and_Pewterware_Increased_Limits '70121; Silverware, Gold ware and Pewterware Increased Limits; just FAR (no HOM or DFR)
            FunctionalReplacementCostLossAssessment '40172; Functional Replacement Cost Loss Settlement; also HOM and DFR
            IdentityFraudExpense '70213; Identity Recovery Expense; just FAR (no HOM or DFR)
            Location_Farm_Sprinkler_Leakage '80142; Farm Sprinkler Leakage; just FAR (no HOM or DFR)
            MineSubsidenceCovA '80102; Mine Subsidence Cov A; also HOM and DFR
            MineSubsidenceCovAAndB '80103; Mine Subsidence Cov A & Cov B; also HOM and DFR

            'using caption
            'ActualCashValueLossSettlement '40173; Actual Cash Value Loss Settlement
            'ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing '40142; Actual Cash Value Loss Settlement / Windstorm or Hail Losses to Roof Surfacing
            'BusinessProperty_BusinessOccupancyonPremises '70102; Farm_Business_Property_Business_Occupancy_on_Premises
            'BusinessPropertyIncreasedLimits '70101; Farm_Business_Property_Increased_Limits
            'CollisionOrUpset '70103; Farm_Collision_or_Upset
            'ConsenttoMoveMobileHome '70106; Farm_Consent_to_Move_Mobile_Home
            'Cov_BRelatedPrivateStructures '554; Farm. Private Structures - Increased Limits for Specific Structures
            'DwellingUnderConstruction_Theft '70108; Farm_Dwelling_Under_Construction_Theft
            'Earthquake '80137; Earthquake_Location
            'ExpandedReplacementCost '70110; Farm_Expanded_Replacement_Cost
            'FarmSprinklerLeakage '80142; Location_Farm_Sprinkler_Leakage
            'FunctionalReplacementCostLossSettlement '40172; Functional Replacement Cost Loss Assessment
            'GunsIncreasedLimits '70111; Farm_Guns_Increased_Limits
            'IdentityRecoveryExpense '70213; Identity Fraud Expense
            'JewelryIncreasedLimits '70112; Farm_Jewelry_Increased_Limits
            'MineSubsidenceCovA '80102; Mine Subsidence Cov A
            'MineSubsidenceCovAAndCovB '80103; Mine Subsidence Cov A & B
            'MoneyIncreasedLimits '70114; Farm_Money_Increased_Limits
            'MotorizedVehiclesIncreasedLimits '70115; Farm_Motorized_Vehicles_Increased_Limits
            'ReplacementValue_NumberofWellPumps '70118; Farm_Replacement_Value_Number_of_Well_Pumps
            'ReplacementValuePersonalPropertyCovC_ '70117; Farm_Replacement_Value_Personal_Property_Cov_C_
            'SecuredPartysInterest '70119; Farm_Secured_Party_Interest
            'SecuritiesIncreasedLimits '70120; Farm_Securities_Increased_Limits
            'SilverwareGoldwareAndPewterwareIncreasedLimits '70121; Farm_Silverware_Goldware_and_Pewterware_Increased_Limits
        End Enum

        Dim qqHelper As New QuickQuoteHelperClass

        Private _CoverageType As SectionICoverageType
        Private _CoverageCodeId As String
        Private _Premium As String
        Private _IncreasedLimitId As String 'may need matching IncreasedLimit variable/property
        Private _IncreasedLimit As String
        Private _IncludedLimit As String 'added 9/10/2014
        Private _TotalLimit As String 'added 9/10/2014
        Private _Description As String
        Private _Address As QuickQuoteAddress
        Private _EffectiveDate As String
        Private _ConstructionTypeId As String 'may need matching ConstructionType variable/property
        Private _DescribedLocation As Boolean
        Private _TheftExtension As Boolean
        Private _AdditionalInterests As List(Of QuickQuoteAdditionalInterest)

        'added 8/16/2013 for DFR
        Private _NumberOfFamilies As String
        Private _OccupancyCodeId As String 'may need matching OccupancyCode variable/property
        Private _ProtectionClassId As String 'may need matching ProtectionClass variable/property
        Private _UsageTypeId As String 'may need matching UsageType variable/property
        'added 8/16/2013 to differentiate HOM and DFR
        Private _HOM_CoverageType As HOM_SectionICoverageType
        Private _DFR_CoverageType As DFR_SectionICoverageType
        Private _FAR_CoverageType As FAR_SectionICoverageType 'added 5/19/2015

        Private _CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean 'added 4/29/2014

        'Private _SectionCoverageNum As String 'added 10/14/2014 for reconciliation; removed 10/29/2018
        Private _SectionCoverageNumGroup As QuickQuoteDiamondNumGroup 'added 10/29/2018

        'added 6/8/2015 for Farm (Section I Covs; Property)
        Private _NumberOfDaysVacant As String
        Private _NumberOfWells As String

        'added for HOM2018Upgrade
        Private _VegetatedRoof As Boolean
        Private _IncreasedCostOfLossId As String
        Private _IncreasedCostOfLoss As String
        Private _EventEffDate As String
        Private _EventExpDate As String
        Private _RelatedExpenseLimit As String

        ''' <summary>
        ''' general coverage type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>should use property specific to LOB instead</remarks>
        Public Property CoverageType As SectionICoverageType
            Get
                Return _CoverageType
            End Get
            Set(value As SectionICoverageType)
                _CoverageType = value
                If _CoverageType <> Nothing AndAlso _CoverageType <> SectionICoverageType.None Then
                    '_CoverageCodeId = CInt(_CoverageType).ToString
                    ''added 8/16/2013
                    'If System.Enum.IsDefined(GetType(HOM_SectionICoverageType), CInt(_CoverageCodeId)) = True Then
                    '    _HOM_CoverageType = CInt(_CoverageCodeId)
                    'End If
                    'If System.Enum.IsDefined(GetType(DFR_SectionICoverageType), CInt(_CoverageCodeId)) = True Then
                    '    _DFR_CoverageType = CInt(_CoverageCodeId)
                    'End If
                    'updated 12/4/2013
                    '_CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, _CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
                    'updated 12/23/2013 to send enum text
                    _CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, System.Enum.GetName(GetType(SectionICoverageType), _CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
                    'If System.Enum.TryParse(Of HOM_SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, _CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType), _HOM_CoverageType) = False Then
                    'updated 12/23/2013 to send enum text
                    If System.Enum.TryParse(Of HOM_SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, System.Enum.GetName(GetType(SectionICoverageType), _CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType), _HOM_CoverageType) = False Then
                        _HOM_CoverageType = HOM_SectionICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(HOM_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, _CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType)) = True Then
                    '    _HOM_CoverageType = System.Enum.Parse(GetType(HOM_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, _CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType))
                    'End If

                    'If System.Enum.TryParse(Of DFR_SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, _CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType), _DFR_CoverageType) = False Then
                    'updated 12/23/2013 to send enum text
                    If System.Enum.TryParse(Of DFR_SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, System.Enum.GetName(GetType(SectionICoverageType), _CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType), _DFR_CoverageType) = False Then
                        _DFR_CoverageType = DFR_SectionICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(DFR_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, _CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType)) = True Then
                    '    _DFR_CoverageType = System.Enum.Parse(GetType(DFR_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, _CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType))
                    'End If

                    'added 5/20/2015 for FAR
                    If System.Enum.TryParse(Of FAR_SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, System.Enum.GetName(GetType(SectionICoverageType), _CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType), _FAR_CoverageType) = False Then
                        _FAR_CoverageType = FAR_SectionICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(FAR_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, _CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType)) = True Then
                    '    _FAR_CoverageType = System.Enum.Parse(GetType(FAR_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, _CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType))
                    'End If
                End If
            End Set
        End Property
        ''' <summary>
        ''' HOM coverage type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>this property should be used for HOM instead of general one for HOM and DFR</remarks>
        Public Property HOM_CoverageType As HOM_SectionICoverageType 'added 8/16/2013 to differentiate HOM from DFR
            Get
                Return _HOM_CoverageType
            End Get
            Set(value As HOM_SectionICoverageType)
                _HOM_CoverageType = value
                If _HOM_CoverageType <> Nothing AndAlso _HOM_CoverageType <> HOM_SectionICoverageType.None Then
                    '_CoverageCodeId = CInt(_HOM_CoverageType).ToString
                    'If System.Enum.IsDefined(GetType(SectionICoverageType), CInt(_CoverageCodeId)) = True Then
                    '    _CoverageType = CInt(_CoverageCodeId)
                    'End If
                    'updated 12/4/2013
                    '_CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, _HOM_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
                    'updated 12/23/2013 to send enum text
                    _CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, System.Enum.GetName(GetType(HOM_SectionICoverageType), _HOM_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
                    'If System.Enum.TryParse(Of SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, _HOM_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType), _CoverageType) = False Then
                    'updated 12/23/2013 to send enum text
                    If System.Enum.TryParse(Of SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, System.Enum.GetName(GetType(HOM_SectionICoverageType), _HOM_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType), _CoverageType) = False Then
                        '_CoverageType = HOM_SectionICoverageType.None
                        'updated 5/20/2015
                        _CoverageType = SectionICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, _HOM_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType)) = True Then
                    '    _CoverageType = System.Enum.Parse(GetType(SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, _HOM_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType))
                    'End If
                    'previous logic wasn't setting this...
                    'If System.Enum.TryParse(Of DFR_SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, _HOM_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType), _DFR_CoverageType) = False Then
                    'updated 12/23/2013 to send enum text
                    If System.Enum.TryParse(Of DFR_SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, System.Enum.GetName(GetType(HOM_SectionICoverageType), _HOM_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType), _DFR_CoverageType) = False Then
                        _DFR_CoverageType = DFR_SectionICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(DFR_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, _HOM_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType)) = True Then
                    '    _DFR_CoverageType = System.Enum.Parse(GetType(DFR_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, _HOM_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType))
                    'End If

                    'added 5/20/2015 for FAR
                    If System.Enum.TryParse(Of FAR_SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, System.Enum.GetName(GetType(HOM_SectionICoverageType), _HOM_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType), _FAR_CoverageType) = False Then
                        _FAR_CoverageType = FAR_SectionICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(FAR_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, _HOM_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType)) = True Then
                    '    _FAR_CoverageType = System.Enum.Parse(GetType(FAR_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, _HOM_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType))
                    'End If
                End If
            End Set
        End Property
        ''' <summary>
        ''' DFR coverage type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>this property should be used for DFR instead of general one for HOM and DFR</remarks>
        Public Property DFR_CoverageType As DFR_SectionICoverageType 'added 8/16/2013 to differentiate DFR from HOM
            Get
                Return _DFR_CoverageType
            End Get
            Set(value As DFR_SectionICoverageType)
                _DFR_CoverageType = value
                If _DFR_CoverageType <> Nothing AndAlso _DFR_CoverageType <> DFR_SectionICoverageType.None Then
                    '_CoverageCodeId = CInt(_DFR_CoverageType).ToString
                    'If System.Enum.IsDefined(GetType(SectionICoverageType), CInt(_CoverageCodeId)) = True Then
                    '    _CoverageType = CInt(_CoverageCodeId)
                    'End If
                    'updated 12/4/2013
                    '_CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, _DFR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
                    'updated 12/23/2013 to send enum text
                    _CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, System.Enum.GetName(GetType(DFR_SectionICoverageType), _DFR_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
                    'If System.Enum.TryParse(Of SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, _DFR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType), _CoverageType) = False Then
                    'updated 12/23/2013 to send enum text
                    If System.Enum.TryParse(Of SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, System.Enum.GetName(GetType(DFR_SectionICoverageType), _DFR_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType), _CoverageType) = False Then
                        ' _CoverageType = HOM_SectionICoverageType.None
                        'updated 5/20/2015
                        _CoverageType = SectionICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, _DFR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType)) = True Then
                    '    _CoverageType = System.Enum.Parse(GetType(SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, _DFR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType))
                    'End If
                    'previous logic wasn't setting this...
                    'If System.Enum.TryParse(Of HOM_SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, _DFR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType), _HOM_CoverageType) = False Then
                    'updated 12/23/2013 to send enum text
                    If System.Enum.TryParse(Of HOM_SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, System.Enum.GetName(GetType(DFR_SectionICoverageType), _DFR_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType), _HOM_CoverageType) = False Then
                        _HOM_CoverageType = HOM_SectionICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(HOM_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, _DFR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType)) = True Then
                    '    _HOM_CoverageType = System.Enum.Parse(GetType(HOM_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, _DFR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType))
                    'End If

                    'added 5/20/2015 for FAR
                    If System.Enum.TryParse(Of FAR_SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, System.Enum.GetName(GetType(DFR_SectionICoverageType), _DFR_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType), _FAR_CoverageType) = False Then
                        _FAR_CoverageType = FAR_SectionICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(FAR_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, _DFR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType)) = True Then
                    '    _FAR_CoverageType = System.Enum.Parse(GetType(FAR_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, _DFR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType))
                    'End If
                End If
            End Set
        End Property
        ''' <summary>
        ''' FAR coverage type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>this property should be used for FAR instead of general one for HOM, DFR, or FAR</remarks>
        Public Property FAR_CoverageType As FAR_SectionICoverageType 'added 5/20/2015; 2/6/2020: corrected from DFR_SectionICoverageType
            Get
                Return _FAR_CoverageType
            End Get
            Set(value As FAR_SectionICoverageType) '2/6/2020: corrected from DFR_SectionICoverageType
                _FAR_CoverageType = value
                If _FAR_CoverageType <> Nothing AndAlso _FAR_CoverageType <> FAR_SectionICoverageType.None Then
                    '_CoverageCodeId = CInt(_FAR_CoverageType).ToString
                    'If System.Enum.IsDefined(GetType(SectionICoverageType), CInt(_CoverageCodeId)) = True Then
                    '    _CoverageType = CInt(_CoverageCodeId)
                    'End If
                    'updated 12/4/2013
                    '_CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, _FAR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
                    'updated 12/23/2013 to send enum text
                    _CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, System.Enum.GetName(GetType(FAR_SectionICoverageType), _FAR_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
                    'If System.Enum.TryParse(Of SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, _FAR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType), _CoverageType) = False Then
                    'updated 12/23/2013 to send enum text
                    If System.Enum.TryParse(Of SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, System.Enum.GetName(GetType(FAR_SectionICoverageType), _FAR_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType), _CoverageType) = False Then
                        ' _CoverageType = HOM_SectionICoverageType.None
                        'updated 5/20/2015
                        _CoverageType = SectionICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, _FAR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType)) = True Then
                    '    _CoverageType = System.Enum.Parse(GetType(SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, _FAR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType))
                    'End If
                    'previous logic wasn't setting this...
                    'If System.Enum.TryParse(Of HOM_SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, _FAR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType), _HOM_CoverageType) = False Then
                    'updated 12/23/2013 to send enum text
                    If System.Enum.TryParse(Of HOM_SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, System.Enum.GetName(GetType(FAR_SectionICoverageType), _FAR_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType), _HOM_CoverageType) = False Then
                        _HOM_CoverageType = HOM_SectionICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(HOM_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, _FAR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType)) = True Then
                    '    _HOM_CoverageType = System.Enum.Parse(GetType(HOM_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, _FAR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType))
                    'End If

                    'added 5/20/2015
                    If System.Enum.TryParse(Of DFR_SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, System.Enum.GetName(GetType(FAR_SectionICoverageType), _FAR_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType), _DFR_CoverageType) = False Then
                        _DFR_CoverageType = DFR_SectionICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(DFR_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, _FAR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType)) = True Then
                    '    _DFR_CoverageType = System.Enum.Parse(GetType(DFR_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, _FAR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType))
                    'End If
                End If
            End Set
        End Property
        Public Property CoverageCodeId As String
            Get
                Return _CoverageCodeId
            End Get
            Set(value As String)
                _CoverageCodeId = value
                If IsNumeric(_CoverageCodeId) = True AndAlso _CoverageCodeId <> "0" Then
                    'If System.Enum.IsDefined(GetType(SectionICoverageType), CInt(_CoverageCodeId)) = True Then
                    '    _CoverageType = CInt(_CoverageCodeId)
                    '    'added 8/16/2013
                    '    If System.Enum.IsDefined(GetType(HOM_SectionICoverageType), CInt(_CoverageCodeId)) = True Then
                    '        _HOM_CoverageType = CInt(_CoverageCodeId)
                    '    End If
                    '    If System.Enum.IsDefined(GetType(DFR_SectionICoverageType), CInt(_CoverageCodeId)) = True Then
                    '        _DFR_CoverageType = CInt(_CoverageCodeId)
                    '    End If
                    'End If
                    'updated 12/4/2013
                    If System.Enum.TryParse(Of SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType), _CoverageType) = False Then
                        _CoverageType = HOM_SectionICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType)) = True Then
                    '    _CoverageType = System.Enum.Parse(GetType(SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType))
                    'End If
                    If System.Enum.TryParse(Of HOM_SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType), _HOM_CoverageType) = False Then
                        _HOM_CoverageType = HOM_SectionICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(HOM_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType)) = True Then
                    '    _HOM_CoverageType = System.Enum.Parse(GetType(HOM_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType))
                    'End If
                    If System.Enum.TryParse(Of DFR_SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType), _DFR_CoverageType) = False Then
                        _DFR_CoverageType = DFR_SectionICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(DFR_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType)) = True Then
                    '    _DFR_CoverageType = System.Enum.Parse(GetType(DFR_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType))
                    'End If

                    'added 5/20/2015 for FAR
                    If System.Enum.TryParse(Of FAR_SectionICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType), _FAR_CoverageType) = False Then
                        _FAR_CoverageType = FAR_SectionICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(FAR_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType)) = True Then
                    '    _FAR_CoverageType = System.Enum.Parse(GetType(FAR_SectionICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType))
                    'End If
                End If
            End Set
        End Property
        Public Property Premium As String
            Get
                'Return _Premium
                'updated 8/26/2014
                Return qqHelper.QuotedPremiumFormat(_Premium)
            End Get
            Set(value As String)
                _Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Premium)
            End Set
        End Property
        Public Property IncreasedLimitId As String
            Get
                Return _IncreasedLimitId
            End Get
            Set(value As String)
                _IncreasedLimitId = value
            End Set
        End Property
        Public Property DeductibleLimitId As String 'added 8/16/2013 (some coverages use CoverageLimitId for deductible)
            Get
                Return _IncreasedLimitId
            End Get
            Set(value As String)
                _IncreasedLimitId = value
            End Set
        End Property
        Public Property IncreasedLimit As String
            Get
                Return _IncreasedLimit
            End Get
            Set(value As String)
                _IncreasedLimit = value
                qqHelper.ConvertToLimitFormat(_IncreasedLimit)
            End Set
        End Property
        Public Property IncludedLimit As String 'added 9/10/2014
            Get
                Return _IncludedLimit
            End Get
            Set(value As String)
                _IncludedLimit = value
                qqHelper.ConvertToLimitFormat(_IncludedLimit)
            End Set
        End Property
        Public Property TotalLimit As String 'added 9/10/2014
            Get
                Return _TotalLimit
            End Get
            Set(value As String)
                _TotalLimit = value
                qqHelper.ConvertToLimitFormat(_TotalLimit)
            End Set
        End Property
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property
        Public Property Address As QuickQuoteAddress
            Get
                SetObjectsParent(_Address)
                Return _Address
            End Get
            Set(value As QuickQuoteAddress)
                _Address = value
                SetObjectsParent(_Address)
            End Set
        End Property
        Public Property EffectiveDate As String
            Get
                Return _EffectiveDate
            End Get
            Set(value As String)
                _EffectiveDate = value
                qqHelper.ConvertToShortDate(_EffectiveDate)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's ConstructionType table</remarks>
        Public Property ConstructionTypeId As String
            Get
                Return _ConstructionTypeId
            End Get
            Set(value As String)
                _ConstructionTypeId = value
            End Set
        End Property
        Public Property DescribedLocation As Boolean
            Get
                Return _DescribedLocation
            End Get
            Set(value As Boolean)
                _DescribedLocation = value
            End Set
        End Property
        Public Property TheftExtension As Boolean
            Get
                Return _TheftExtension
            End Get
            Set(value As Boolean)
                _TheftExtension = value
            End Set
        End Property
        Public Property AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05632}")
                Return _AdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _AdditionalInterests = value
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05632}")
            End Set
        End Property
        Public Property NumberOfFamilies As String
            Get
                Return _NumberOfFamilies
            End Get
            Set(value As String)
                _NumberOfFamilies = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's OccupancyCode table</remarks>
        Public Property OccupancyCodeId As String '
            Get
                Return _OccupancyCodeId
            End Get
            Set(value As String)
                _OccupancyCodeId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's ProtectionClass table (-1=N/A, 0=None, 1=1, 2=2, 3=3, 4=4, 5=5, 6=6, 7=7, 8=8, 9=9, 10=10, 11=11, 12=01, 13=02, 14=03, 15=04, 16=05, 17=06, 18=07, 19=08, 20=8B, 21=09, 22=10); Personal uses ids 1-11, Commercial uses ids 12-22</remarks>
        Public Property ProtectionClassId As String '
            Get
                Return _ProtectionClassId
            End Get
            Set(value As String)
                _ProtectionClassId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's UsageType table (-1=N/A, 0=None, 1=Seasonal, 2=Non-Seasonal)</remarks>
        Public Property UsageTypeId As String '
            Get
                Return _UsageTypeId
            End Get
            Set(value As String)
                _UsageTypeId = value
            End Set
        End Property

        Public Property CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean 'added 4/29/2014
            Get
                Return _CanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property

        Public Property SectionCoverageNum As String 'added 10/14/2014 for reconciliation
            Get
                'Return _SectionCoverageNum
                'updated 10/29/2018
                Return SectionCoverageNumGroup.DiamondNum
            End Get
            Set(value As String)
                '_SectionCoverageNum = value
                'updated 10/29/2018
                SectionCoverageNumGroup.DiamondNum = value
            End Set
        End Property

        Public Property SectionCoverageNumGroup As QuickQuoteDiamondNumGroup 'added 10/29/2018
            Get
                If _SectionCoverageNumGroup Is Nothing Then
                    _SectionCoverageNumGroup = New QuickQuoteDiamondNumGroup(Me)
                Else
                    SetObjectsParent(_SectionCoverageNumGroup)
                End If
                Return _SectionCoverageNumGroup
            End Get
            Set(value As QuickQuoteDiamondNumGroup)
                _SectionCoverageNumGroup = value
                SetObjectsParent(_SectionCoverageNumGroup)
            End Set
        End Property

        'added 6/8/2015 for Farm (Section I Covs; Property)
        Public Property NumberOfDaysVacant As String
            Get
                Return _NumberOfDaysVacant
            End Get
            Set(value As String)
                _NumberOfDaysVacant = value
            End Set
        End Property
        Public Property NumberOfWells As String
            Get
                Return _NumberOfWells
            End Get
            Set(value As String)
                _NumberOfWells = value
            End Set
        End Property

        Public Property EventEffDate As String
            Get
                Return _EventEffDate
            End Get
            Set(value As String)
                _EventEffDate = value
                qqHelper.ConvertToShortDate(_EventEffDate)
            End Set
        End Property

        Public Property EventExpDate As String
            Get
                Return _EventExpDate
            End Get
            Set(value As String)
                _EventExpDate = value
                qqHelper.ConvertToShortDate(_EventExpDate)
            End Set
        End Property

        Public Property VegetatedRoof As Boolean
            Get
                Return _VegetatedRoof
            End Get
            Set(value As Boolean)
                _VegetatedRoof = value
            End Set
        End Property

        Public Property IncreasedCostOfLoss As String
            Get
                Return _IncreasedCostOfLoss
            End Get
            Set(value As String)
                _IncreasedCostOfLoss = value
                _IncreasedCostOfLossId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedCostOfLossId, _IncreasedCostOfLoss, QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteHelperClass.PersOrComm.Pers)
            End Set
        End Property

        Public Property IncreasedCostOfLossId As String
            Get
                Return _IncreasedCostOfLossId
            End Get
            Set(value As String)
                _IncreasedCostOfLossId = value
                _IncreasedCostOfLoss = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedCostOfLossId, _IncreasedCostOfLossId, QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteHelperClass.PersOrComm.Pers)
            End Set
        End Property
        Public Property RelatedExpenseLimit As String
            Get
                Return _RelatedExpenseLimit
            End Get
            Set(value As String)
                _RelatedExpenseLimit = value
                qqHelper.ConvertToLimitFormat(_RelatedExpenseLimit)
            End Set
        End Property
        Public Property ExteriorDoorWindowSurfacing As Boolean 'added 06/26/2020 for Ohio Farm
        Public Property ExteriorWallSurfacing As Boolean 'added 06/26/2020 for Ohio Farm
        Public Property RoofSurfacing As Boolean 'added 06/26/2020 for Ohio Farm

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub

        Public Sub New(Parent As QuickQuoteLocation)
            MyBase.New()
            SetDefaults()
            Me.SetParent = Parent
        End Sub

        Private Sub SetDefaults()
            _CoverageType = SectionICoverageType.None
            _CoverageCodeId = ""
            _Premium = ""
            _IncreasedLimitId = ""
            _IncreasedLimit = ""
            _IncludedLimit = "" 'added 9/10/2014
            _TotalLimit = "" 'added 9/10/2014
            _Description = ""
            _Address = New QuickQuoteAddress
            _EffectiveDate = ""
            _ConstructionTypeId = ""
            _DescribedLocation = False
            _TheftExtension = False
            '_AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            _AdditionalInterests = Nothing 'added 8/4/2014
            _NumberOfFamilies = ""
            _OccupancyCodeId = ""
            _ProtectionClassId = ""
            _UsageTypeId = ""
            _HOM_CoverageType = HOM_SectionICoverageType.None
            _DFR_CoverageType = DFR_SectionICoverageType.None
            _FAR_CoverageType = FAR_SectionICoverageType.None 'added 5/20/2015

            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False 'added 4/29/2014

            '_SectionCoverageNum = "" 'added 10/14/2014 for reconciliation; removed 10/29/2018
            _SectionCoverageNumGroup = New QuickQuoteDiamondNumGroup(Me) 'added 10/29/2018

            'added 6/8/2015 for Farm (Section I Covs; Property)
            _NumberOfDaysVacant = ""
            _NumberOfWells = ""

            _EventEffDate = ""
            _EventExpDate = ""
            _VegetatedRoof = False
            _IncreasedCostOfLossId = ""
            _IncreasedCostOfLoss = ""
            _RelatedExpenseLimit = ""
        End Sub
        'added 4/29/2014 for additionalInterests reconciliation
        Public Sub ParseThruAdditionalInterests()
            If _AdditionalInterests IsNot Nothing AndAlso _AdditionalInterests.Count > 0 Then
                For Each ai As QuickQuoteAdditionalInterest In _AdditionalInterests
                    'note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False Then
                        If ai.HasValidAdditionalInterestNum = True Then
                            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = True
                            Exit For
                        End If
                    End If
                Next
            End If
        End Sub
        Public Function HasValidSectionCoverageNum() As Boolean 'added 10/14/2014 for reconciliation purposes
            'Return qqHelper.IsValidQuickQuoteIdOrNum(_SectionCoverageNum)
            'updated 10/29/2018
            Return SectionCoverageNumGroup.HasValidDiamondNum()
        End Function

        'Private Sub ReEvaluateProperties() Handles Me.ConnectedToQuoteObject
        '    If CoverageType <> Nothing Then
        '        CoverageType = _CoverageType
        '    End If
        'End Sub

        Public Overrides Function ToString() As String 'added 6/29/2015
            Dim str As String = ""
            Dim myList As New List(Of String)
            If Me IsNot Nothing Then
                If String.IsNullOrWhiteSpace(CoverageCodeId) = False Then
                    str = "CoverageCodeId: " & CoverageCodeId & " (" & CoverageType.ToString() & ")"
                End If
            Else
                str = "Nothing"
            End If
            Return str
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _CoverageType <> Nothing Then
                        _CoverageType = Nothing
                    End If
                    If _CoverageCodeId IsNot Nothing Then
                        _CoverageCodeId = Nothing
                    End If
                    If _Premium IsNot Nothing Then
                        _Premium = Nothing
                    End If
                    If _IncreasedLimitId IsNot Nothing Then
                        _IncreasedLimitId = Nothing
                    End If
                    If _IncreasedLimit IsNot Nothing Then
                        _IncreasedLimit = Nothing
                    End If
                    If _IncludedLimit IsNot Nothing Then 'added 9/10/2014
                        _IncludedLimit = Nothing
                    End If
                    If _TotalLimit IsNot Nothing Then 'added 9/10/2014
                        _TotalLimit = Nothing
                    End If
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _Address IsNot Nothing Then
                        _Address.Dispose()
                        _Address = Nothing
                    End If
                    If _EffectiveDate IsNot Nothing Then
                        _EffectiveDate = Nothing
                    End If
                    If _ConstructionTypeId IsNot Nothing Then
                        _ConstructionTypeId = Nothing
                    End If
                    If _DescribedLocation <> Nothing Then
                        _DescribedLocation = Nothing
                    End If
                    If _TheftExtension <> Nothing Then
                        _TheftExtension = Nothing
                    End If
                    If _AdditionalInterests IsNot Nothing Then
                        If _AdditionalInterests.Count > 0 Then
                            For Each ai As QuickQuoteAdditionalInterest In _AdditionalInterests
                                ai.Dispose()
                                ai = Nothing
                            Next
                            _AdditionalInterests.Clear()
                        End If
                        _AdditionalInterests = Nothing
                    End If
                    If _NumberOfFamilies IsNot Nothing Then
                        _NumberOfFamilies = Nothing
                    End If
                    If _OccupancyCodeId IsNot Nothing Then
                        _OccupancyCodeId = Nothing
                    End If
                    If _ProtectionClassId IsNot Nothing Then
                        _ProtectionClassId = Nothing
                    End If
                    If _UsageTypeId IsNot Nothing Then
                        _UsageTypeId = Nothing
                    End If
                    If _HOM_CoverageType <> Nothing Then
                        _HOM_CoverageType = Nothing
                    End If
                    If _DFR_CoverageType <> Nothing Then
                        _DFR_CoverageType = Nothing
                    End If
                    _FAR_CoverageType = Nothing 'added 5/20/2015

                    If _CanUseAdditionalInterestNumForAdditionalInterestReconciliation <> Nothing Then 'added 4/29/2014
                        _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    End If

                    'If _SectionCoverageNum IsNot Nothing Then 'added 10/14/2014 for reconciliation; removed 10/29/2018
                    '    _SectionCoverageNum = Nothing
                    'End If
                    qqHelper.DisposeQuickQuoteDiamondNumGroup(_SectionCoverageNumGroup) 'added 10/29/2018

                    'added 6/8/2015 for Farm (Section I Covs; Property)
                    qqHelper.DisposeString(_NumberOfDaysVacant)
                    qqHelper.DisposeString(_NumberOfWells)

                    'HOM2018 Upgrade
                    qqHelper.DisposeString(_IncreasedCostOfLossId)
                    qqHelper.DisposeString(_IncreasedCostOfLoss)
                    qqHelper.DisposeString(_EventEffDate)
                    qqHelper.DisposeString(_EventExpDate)

                    If _VegetatedRoof <> Nothing Then
                        _VegetatedRoof = Nothing
                    End If

                    MyBase.Dispose() 'added 8/4/2014
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        'Public Sub Dispose() Implements IDisposable.Dispose
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Public Overrides Sub Dispose() 'Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
