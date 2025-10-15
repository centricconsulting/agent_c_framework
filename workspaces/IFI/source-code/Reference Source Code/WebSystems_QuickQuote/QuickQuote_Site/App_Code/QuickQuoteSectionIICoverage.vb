Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store section II (Liability) coverage information
    ''' </summary>
    ''' <remarks>could potentially be under several different objects; currently part of Location</remarks>
    <Serializable()> _
    Public Class QuickQuoteSectionIICoverage 'added 8/13/2013 for HOM
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        ''' <summary>
        ''' valid types for section II coverages
        ''' </summary>
        ''' <remarks>value previously corresponded to coveragecode_id in Diamond; now see static data xml file; coded for HOM and DFR w/ text matching HOM when overlaps exist; should use enum types specific to LOB instead</remarks>
        Enum SectionIICoverageType '8/15/2013 note: these were written specifically for HOM but DFR can also have section II covs (some are different and some overlap)
            'None = 0
            '12/3/2013: without values
            None
            '_3Or4FamilyLiability_HO_74 = 20157 '3 or 4 Family Liability
            'AdditionalResidence_OccupiedbyInsured_NA = 20154 'Other Location Occupied By Insured
            'AdditionalResidenceRentedtoOthers_HO_70 = 20156 'Additional Residence Rented To Other
            'AnimalCollision_HO_73 = 70200 'Animal_Collision
            'BusinessPursuits_Clerical_HO_71 = 20049 'Business Pursuits - Clerical
            'BusinessPursuits_SalesPersonExcludingInstallation_HO_71 = 20050 'Business Pursuits - Sales Person - Excluding Installation
            'BusinessPursuits_SalesPersonIncludingInstallation_HO_71 = 20051 'Business Pursuits - Sales Person - Including Installation
            'BusinessPursuits_TeacherLabEtc_ExcludingCorporalPunishment_HO_71 = 20052 'Business Pursuits - Teacher - Lab Etc. - Excluding Corporal Punishment
            'BusinessPursuits_TeacherLabEtc_IncludingCorporalPunishment_HO_71 = 20053 'Business Pursuits - Teacher - Lab Etc. - Including Corporal Punishment
            'BusinessPursuits_TeacherOtherExcludingCorporalPunishment_HO_71 = 20054 'Business Pursuits - Teacher - Other - Excluding Corporal Punishment
            'BusinessPursuits_TeacherOtherIncludingCorporalPunishment_HO_71 = 20055 'Business Pursuits - Teacher - Other - Including Corporal Punishment
            'FarmOwnedAndOperatedByInsured0_160Acres_HO_2446 = 20215 'Farm Owned and Operated By Insured: 0-160 Acres
            'FarmOwnedAndOperatedByInsured160_500Acres_HO_2446 = 20216 'Farm Owned and Operated By Insured: 160-500 Acres
            'FarmOwnedAndOperatedByInsuredOver500Acres_HO_2446 = 20217 'Farm Owned and Operated By Insured: Over 500 Acres
            'HomeDayCare_HO_323 = 20169 'Home Day Care Liability
            'IncidentalFarmingPersonalLiability_HO_72 = 20163 'Incidental Farmers Personal Liability
            'PermittedIncidentalOccupancies_ResidencePremises_HO_42 = 20035 'Permitted Incidental Occupancies - Residence Premises 
            'PermittedIncidentalOccupanciesOtherResidence_HO_43 = 20036 'Permitted Incidental Occupancies - Other Residence
            'PersonalInjury_HO_82 = 20159 'Personal Injury
            'SpecialEventCoverage_92_347 = 80061 'Special Event Coverage
            'WaterbedLiability_HO_85 = 312 'Waterbed Coverage
            ''8/16/2013 - added more for DFR; commented out the ones that were already there for HOM
            ''AdditionalResidence_OccupiedbyInsured = 20154 'Other Location Occupied By Insured
            ''AdditionalResidenceRentedtoOthers = 20156 'Additional Residence Rented To Other
            'BusinessPursuits_Unclassified = 80092 'Business Pursuits - Other
            ''BusinessPursuits_Clerical = 20049 'Business Pursuits - Clerical
            ''BusinessPursuits_SalesCollectorsOrMessengersExcludingInstallation = 20050 'Business Pursuits - Sales Person - Excluding Installation
            ''BusinessPursuits_SalesCollectorsOrMessengersIncludingInstallation = 20051 'Business Pursuits - Sales Person - Including Installation
            ''BusinessPursuits_TeacherLabEtc_ExcludingCorporalPunishment = 20052 'Business Pursuits - Teacher - Lab Etc. - Excluding Corporal Punishment
            ''BusinessPursuits_TeacherLabEtc_IncludingCorporalPunishment = 20053 'Business Pursuits - Teacher - Lab Etc. - Including Corporal Punishment
            ''BusinessPursuits_TeacherOtherExcludingCorporalPunishment = 20054 'Business Pursuits - Teacher - Other - Excluding Corporal Punishment
            ''BusinessPursuits_TeacherOtherIncludingCorporalPunishment = 20055 'Business Pursuits - Teacher - Other - Including Corporal Punishment
            'ContingentWorkersCompensation = 40097 'Contingent Workers Compensation
            'EmployersLiability = 80090 'Employer Liability
            'IncidentalMotorConveyances = 20160 'Incidental Motor Conveyance
            'LossAssessmentLiability = 80093 'Loss Assessment Liability
            'Non_OwnerOccupiedDwelling = 80091 'Non-Owner Occupied Dwelling
            'OfficeProfPriv_SchoolStudioOccupancy = 70136 'Location_Office_Prof_Priv_School_Studio_Occupancy
            ''PermittedIncidentalOccupanciesOtherResidence = 20036 'Permitted Incidental Occupancies - Other Residence
            ''PersonalInjury = 20159 'Personal Injury

            '12/2/2013... in case we ever want to use caption instead of coverage code desc
            '_3Or4FamilyLiability_HO_74 = 20157 '3 or 4 Family Liability; HOM
            'AdditionalResidence_OccupiedbyInsured_NA = 20154 'Other Location Occupied By Insured; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'AdditionalResidenceRentedtoOthers_HO_70 = 20156 'Additional Residence Rented To Other; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'AnimalCollision_HO_73 = 70200 'Animal_Collision; HOM
            'BusinessPursuits_Unclassified = 80092 'Business Pursuits - Other; DFR
            'BusinessPursuits_Clerical_HO_71 = 20049 'Business Pursuits - Clerical; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BusinessPursuits_SalesPersonExcludingInstallation_HO_71 = 20050 'Business Pursuits - Sales Person - Excluding Installation; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BusinessPursuits_SalesPersonIncludingInstallation_HO_71 = 20051 'Business Pursuits - Sales Person - Including Installation; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BusinessPursuits_TeacherLabEtc_ExcludingCorporalPunishment_HO_71 = 20052 'Business Pursuits - Teacher - Lab Etc. - Excluding Corporal Punishment; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BusinessPursuits_TeacherLabEtc_IncludingCorporalPunishment_HO_71 = 20053 'Business Pursuits - Teacher - Lab Etc. - Including Corporal Punishment; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BusinessPursuits_TeacherOtherExcludingCorporalPunishment_HO_71 = 20054 'Business Pursuits - Teacher - Other - Excluding Corporal Punishment; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BusinessPursuits_TeacherOtherIncludingCorporalPunishment_HO_71 = 20055 'Business Pursuits - Teacher - Other - Including Corporal Punishment; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'ContingentWorkersCompensation = 40097 'Contingent Workers Compensation; DFR
            'EmployersLiability = 80090 'Employer Liability; DFR
            'FarmOwnedAndOperatedByInsured0_160Acres_HO_2446 = 20215 'Farm Owned and Operated By Insured: 0-160 Acres; HOM
            'FarmOwnedAndOperatedByInsured160_500Acres_HO_2446 = 20216 'Farm Owned and Operated By Insured: 160-500 Acres; HOM
            'FarmOwnedAndOperatedByInsuredOver500Acres_HO_2446 = 20217 'Farm Owned and Operated By Insured: Over 500 Acres; HOM
            'HomeDayCare_HO_323 = 20169 'Home Day Care Liability; HOM
            'IncidentalFarmingPersonalLiability_HO_72 = 20163 'Incidental Farmers Personal Liability; HOM
            'IncidentalMotorConveyances = 20160 'Incidental Motor Conveyance; DFR
            'LossAssessmentLiability = 80093 'Loss Assessment Liability; DFR
            'Non_OwnerOccupiedDwelling = 80091 'Non-Owner Occupied Dwelling; DFR
            'OfficeProfPriv_SchoolStudioOccupancy = 70136 'Location_Office_Prof_Priv_School_Studio_Occupancy; DFR
            'PermittedIncidentalOccupancies_ResidencePremises_HO_42 = 20035 'Permitted Incidental Occupancies - Residence Premises ; HOM
            'PermittedIncidentalOccupanciesOtherResidence_HO_43 = 20036 'Permitted Incidental Occupancies - Other Residence; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'PersonalInjury_HO_82 = 20159 'Personal Injury; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'SpecialEventCoverage_92_347 = 80061 'Special Event Coverage; HOM
            'WaterbedLiability_HO_85 = 312 'Waterbed Coverage; HOM
            '12/3/2013: without values
            '_3Or4FamilyLiability_HO_74 '20157; 3 or 4 Family Liability; HOM
            'AdditionalResidence_OccupiedbyInsured_NA '20154; Other Location Occupied By Insured; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'AdditionalResidenceRentedtoOthers_HO_70 '20156; Additional Residence Rented To Other; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'AnimalCollision_HO_73 '70200; Animal_Collision; HOM
            'BusinessPursuits_Unclassified '80092; Business Pursuits - Other; DFR
            'BusinessPursuits_Clerical_HO_71 '20049; Business Pursuits - Clerical; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BusinessPursuits_SalesPersonExcludingInstallation_HO_71 '20050; Business Pursuits - Sales Person - Excluding Installation; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BusinessPursuits_SalesPersonIncludingInstallation_HO_71 '20051; Business Pursuits - Sales Person - Including Installation; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BusinessPursuits_TeacherLabEtc_ExcludingCorporalPunishment_HO_71 '20052; Business Pursuits - Teacher - Lab Etc. - Excluding Corporal Punishment; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BusinessPursuits_TeacherLabEtc_IncludingCorporalPunishment_HO_71 '20053; Business Pursuits - Teacher - Lab Etc. - Including Corporal Punishment; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BusinessPursuits_TeacherOtherExcludingCorporalPunishment_HO_71 '20054; Business Pursuits - Teacher - Other - Excluding Corporal Punishment; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BusinessPursuits_TeacherOtherIncludingCorporalPunishment_HO_71 '20055; Business Pursuits - Teacher - Other - Including Corporal Punishment; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'ContingentWorkersCompensation '40097; Contingent Workers Compensation; DFR
            'EmployersLiability '80090; Employer Liability; DFR
            'FarmOwnedAndOperatedByInsured0_160Acres_HO_2446 '20215; Farm Owned and Operated By Insured: 0-160 Acres; HOM
            'FarmOwnedAndOperatedByInsured160_500Acres_HO_2446 '20216; Farm Owned and Operated By Insured: 160-500 Acres; HOM
            'FarmOwnedAndOperatedByInsuredOver500Acres_HO_2446 '20217; Farm Owned and Operated By Insured: Over 500 Acres; HOM
            'HomeDayCare_HO_323 '20169; Home Day Care Liability; HOM
            'IncidentalFarmingPersonalLiability_HO_72 '20163; Incidental Farmers Personal Liability; HOM
            'IncidentalMotorConveyances '20160; Incidental Motor Conveyance; DFR
            'LossAssessmentLiability '80093; Loss Assessment Liability; DFR
            'Non_OwnerOccupiedDwelling '80091; Non-Owner Occupied Dwelling; DFR
            'OfficeProfPriv_SchoolStudioOccupancy '70136; Location_Office_Prof_Priv_School_Studio_Occupancy; DFR
            'PermittedIncidentalOccupancies_ResidencePremises_HO_42 '20035; Permitted Incidental Occupancies - Residence Premises ; HOM
            'PermittedIncidentalOccupanciesOtherResidence_HO_43 '20036; Permitted Incidental Occupancies - Other Residence; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'PersonalInjury_HO_82 '20159; Personal Injury; HOM and DFR (caption/CoverageCodeDscr from HOM)
            'SpecialEventCoverage_92_347 '80061; Special Event Coverage; HOM
            'WaterbedLiability_HO_85 '312; Waterbed Coverage; HOM
            'added 5/19/2015 for Farm
            'AdditionalInsured_PartnersCorporateOfficersOrCo_Owners '70167; Location_Occupying_Residence_on_Premises
            'AdditionalResidencePremisesOccupiedbyInsured '40044; Optional Liability - Additional Residence Premises Occupied by an Insured
            'CareProvidedForOthers '40047; Optional Liability - Care Provided for Others
            'CustomFarming_NoSpraying '70129; Location_Custom_Farming_No_Spraying
            'CustomFarming_withSpraying '80115; Location_Custom_Farming_With_Spraying
            'DomesticEmployees '80133; Domestic Employees
            'FamilyMedicalPayments '70201; Named_Persons_Medical_Payments
            'FarmPersonalLiabilityGL9 '70139; Location_Farm_Personal_Liability_GL9
            'FruitOrVegetablePick_Your_OwnOperations_NoOffGroundPicking '70133; Location_Fruit_or_Vegetable_Pick_Your_Own_Operations
            'FruitOrVegetablePick_Your_OwnOperations_WithOffGroundPicking '80110; Location_Fruit_or_Vegetable_Pick_Your_Own_Operations_No
            'HorseBoarding '70134; Location_Horse_Boarding
            'IncidentalBusinessExposures '70135; Location_Incidental_Business_Exposures
            'LiabilityEnhancementEndorsement '80094; Enhancement Endorsement
            'LimitedFarmPollutionLiability '40054; Optional Liability - Farm Pollution Liability
            'Non_RelativeResident '70169; Location_Non_Relative_Resident
            'OptionalLiability_AdditionalFarmPremisesRentedtoOthers '80113; Optional Liability - Additional Farm Premises Rented to Others
            'OptionalLiability_AdditionalResidencesRentedtoOthers '40045; Optional Liability - Additional Residences or Farms Rented to Others
            'PersonalInjury '70137; Location_Personal_Injury

            'updated 12/2/2013 to use coverage code desc instead of caption
            '_3Or4FamilyLiability = 20157 '3 or 4 Family Liability (HO-74); HOM
            'AdditionalResidenceRentedToOther = 20156 'Additional Residence Rented to Others (HO-70); HOM and DFR (caption/CoverageCodeDscr from HOM)
            'Animal_Collision = 70200 'Animal Collision (HO-73); HOM
            'BusinessPursuits_Clerical = 20049 'Business Pursuits - Clerical (HO-71); HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BusinessPursuits_Other = 80092 'Business Pursuits  - Unclassified; DFR
            'BusinessPursuits_SalesPerson_ExcludingInstallation = 20050 'Business Pursuits - Sales Person Excluding Installation (HO-71); HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BusinessPursuits_SalesPerson_IncludingInstallation = 20051 'Business Pursuits - Sales Person Including Installation (HO-71); HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BusinessPursuits_Teacher_LabEtc__ExcludingCorporalPunishment = 20052 'Business Pursuits - Teacher Lab Etc. Excluding Corporal Punishment (HO-71); HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BusinessPursuits_Teacher_LabEtc__IncludingCorporalPunishment = 20053 'Business Pursuits - Teacher Lab Etc. Including Corporal Punishment (HO-71); HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BusinessPursuits_Teacher_Other_ExcludingCorporalPunishment = 20054 'Business Pursuits - Teacher Other Excluding Corporal Punishment (HO-71); HOM and DFR (caption/CoverageCodeDscr from HOM)
            'BusinessPursuits_Teacher_Other_IncludingCorporalPunishment = 20055 'Business Pursuits - Teacher Other Including Corporal Punishment (HO-71); HOM and DFR (caption/CoverageCodeDscr from HOM)
            'ContingentWorkersCompensation = 40097 'Contingent Workers Compensation; DFR
            'EmployerLiability = 80090 'Employers Liability; DFR
            'FarmOwnedAndOperatedByInsured0_160Acres = 20215 'Farm Owned and Operated By Insured: 0-160 Acres (HO-24 46); HOM
            'FarmOwnedAndOperatedByInsured160_500Acres = 20216 'Farm Owned and Operated By Insured: 160-500 Acres (HO-24 46); HOM
            'FarmOwnedAndOperatedByInsuredOver500Acres = 20217 'Farm Owned and Operated By Insured: Over 500 Acres (HO-24 46); HOM
            'HomeDayCareLiability = 20169 'Home Day Care (HO-323); HOM
            'IncidentalFarmersPersonalLiability = 20163 'Incidental Farming Personal Liability (HO-72); HOM
            'IncidentalMotorConveyance = 20160 'Incidental Motor Conveyances; DFR
            'Location_Office_Prof_Priv_School_Studio_Occupancy = 70136 'Office, Prof, Priv. School/Studio Occupancy; DFR
            'LossAssessmentLiability = 80093 'Loss Assessment Liability; DFR
            'Non_OwnerOccupiedDwelling = 80091 'Non-Owner Occupied Dwelling; DFR
            'OtherLocationOccupiedByInsured = 20154 'Additional Residence - Occupied by Insured (N/A); HOM and DFR (caption/CoverageCodeDscr from HOM)
            'PermittedIncidentalOccupancies_OtherResidence = 20036 'Permitted Incidental Occupancies Other Residence (HO-43); HOM and DFR (caption/CoverageCodeDscr from HOM)
            'PermittedIncidentalOccupancies_ResidencePremises = 20035 'Permitted Incidental Occupancies - Residence Premises (HO-42); HOM
            'PersonalInjury = 20159 'Personal Injury (HO-82); HOM and DFR (caption/CoverageCodeDscr from HOM)
            'SpecialEventCoverage = 80061 'Special Event Coverage (92-347); HOM
            'WaterbedCoverage = 312 'Waterbed Liability (HO-85); HOM
            '12/3/2013: without values
            _3Or4FamilyLiability '20157; 3 or 4 Family Liability (HO-74); HOM
            AdditionalResidenceRentedToOther '20156; Additional Residence Rented to Others (HO-70); HOM and DFR (caption/CoverageCodeDscr from HOM)
            Animal_Collision '70200; Animal Collision (HO-73); HOM
            BusinessPursuits_Clerical '20049; Business Pursuits - Clerical (HO-71); HOM and DFR (caption/CoverageCodeDscr from HOM)
            BusinessPursuits_Other '80092; Business Pursuits  - Unclassified; DFR
            BusinessPursuits_SalesPerson_ExcludingInstallation '20050; Business Pursuits - Sales Person Excluding Installation (HO-71); HOM and DFR (caption/CoverageCodeDscr from HOM)
            BusinessPursuits_SalesPerson_IncludingInstallation '20051; Business Pursuits - Sales Person Including Installation (HO-71); HOM and DFR (caption/CoverageCodeDscr from HOM)
            BusinessPursuits_Teacher_LabEtc__ExcludingCorporalPunishment '20052; Business Pursuits - Teacher Lab Etc. Excluding Corporal Punishment (HO-71); HOM and DFR (caption/CoverageCodeDscr from HOM)
            BusinessPursuits_Teacher_LabEtc__IncludingCorporalPunishment '20053; Business Pursuits - Teacher Lab Etc. Including Corporal Punishment (HO-71); HOM and DFR (caption/CoverageCodeDscr from HOM)
            BusinessPursuits_Teacher_Other_ExcludingCorporalPunishment '20054; Business Pursuits - Teacher Other Excluding Corporal Punishment (HO-71); HOM and DFR (caption/CoverageCodeDscr from HOM)
            BusinessPursuits_Teacher_Other_IncludingCorporalPunishment '20055; Business Pursuits - Teacher Other Including Corporal Punishment (HO-71); HOM and DFR (caption/CoverageCodeDscr from HOM)
            ContingentWorkersCompensation '40097; Contingent Workers Compensation; DFR
            EmployerLiability '80090; Employers Liability; DFR
            FarmOwnedAndOperatedByInsured0_160Acres '20215; Farm Owned and Operated By Insured: 0-160 Acres (HO-24 46); HOM
            FarmOwnedAndOperatedByInsured160_500Acres '20216; Farm Owned and Operated By Insured: 160-500 Acres (HO-24 46); HOM
            FarmOwnedAndOperatedByInsuredOver500Acres '20217; Farm Owned and Operated By Insured: Over 500 Acres (HO-24 46); HOM
            HomeDayCareLiability '20169; Home Day Care (HO-323); HOM
            IncidentalFarmersPersonalLiability '20163; Incidental Farming Personal Liability (HO-72); HOM
            IncidentalMotorConveyance '20160; Incidental Motor Conveyances; DFR
            Location_Office_Prof_Priv_School_Studio_Occupancy '70136; Office, Prof, Priv. School/Studio Occupancy; DFR
            LossAssessmentLiability '80093; Loss Assessment Liability; DFR
            Non_OwnerOccupiedDwelling '80091; Non-Owner Occupied Dwelling; DFR
            OtherLocationOccupiedByInsured '20154; Additional Residence - Occupied by Insured (N/A); HOM and DFR (caption/CoverageCodeDscr from HOM)
            PermittedIncidentalOccupancies_OtherResidence '20036; Permitted Incidental Occupancies Other Residence (HO-43); HOM and DFR (caption/CoverageCodeDscr from HOM)
            PermittedIncidentalOccupancies_ResidencePremises '20035; Permitted Incidental Occupancies - Residence Premises (HO-42); HOM
            PersonalInjury '20159; Personal Injury (HO-82); HOM and DFR (caption/CoverageCodeDscr from HOM)
            SpecialEventCoverage '80061; Special Event Coverage (92-347); HOM
            WaterbedCoverage '312; Waterbed Liability (HO-85); HOM

            'added 5/19/2015 for Farm
            DomesticEmployees '80133; Domestic Employees; just FAR
            EnhancementEndorsement '80094; Liability Enhancement Endorsement; also PPA cov but not Section II cov
            Location_Custom_Farming_No_Spraying '70129; Custom Farming - No Spraying; just FAR
            Location_Custom_Farming_With_Spraying '80115; Custom Farming - with Spraying; just FAR
            Location_Farm_Personal_Liability_GL9 '70139; Farm Personal Liability GL9; just FAR
            Location_Fruit_or_Vegetable_Pick_Your_Own_Operations '70133; Fruit or Vegetable Pick-Your-Own Operations - No Off Ground Picking; just FAR
            Location_Fruit_or_Vegetable_Pick_Your_Own_Operations_No '80110; Fruit or Vegetable Pick-Your-Own Operations - With Off Ground Picking; just FAR
            Location_Horse_Boarding '70134; Horse Boarding; just FAR
            Location_Incidental_Business_Exposures '70135; Incidental Business Exposures; just FAR
            Location_Non_Relative_Resident '70169; Non-Relative Resident; just FAR
            Location_Occupying_Residence_on_Premises '70167; Additional Insured - Partners, Corporate Officers, or Co-Owners; just FAR
            Location_Personal_Injury '70137; Personal Injury; just FAR
            Named_Persons_Medical_Payments '70201; Family Medical Payments; just FAR
            OptionalLiability_AdditionalFarmPremisesRentedtoOthers '80113; Optional Liability - Additional Farm Premises Rented to Others; just FAR
            OptionalLiability_AdditionalResidencePremisesOccupiedbyanInsured '40044; Additional Residence Premises Occupied by Insured; just FAR
            OptionalLiability_AdditionalResidencesOrFarmsRentedtoOthers '40045; Optional Liability - Additional Residences Rented to Others; just FAR
            OptionalLiability_CareProvidedForOthers '40047; Care Provided for Others; just FAR
            OptionalLiability_FarmPollutionLiability '40054; Limited Farm Pollution Liability; just FAR

            'added 06/26/2020 for Ohio Farm
            Motorized_Vehicles_Ohio '80553

            'HOM2018 Upgrade
            CanineLiabilityExclusion '80308 ' 05/19/2022 CAH - added to Farm, too
            IncidentalFarmingPersonalLiability_OffPremises '80512
            LowPowerRecreationalMotorVehicleLiability '80509
            DamageToPropertyOfOthers '80522
        End Enum
        ''' <summary>
        ''' valid types for HOM section II coverages
        ''' </summary>
        ''' <remarks>value previously corresponded to coveragecode_id in Diamond; now see static data xml file; this enum type should be used for HOM instead of general one for HOM and DFR</remarks>
        Enum HOM_SectionIICoverageType 'added 8/16/2013 to differentiate HOM from DFR
            'None = 0
            '12/3/2013: without values
            None
            '_3Or4FamilyLiability_HO_74 = 20157 '3 or 4 Family Liability
            'AdditionalResidence_OccupiedbyInsured_NA = 20154 'Other Location Occupied By Insured
            'AdditionalResidenceRentedtoOthers_HO_70 = 20156 'Additional Residence Rented To Other
            'AnimalCollision_HO_73 = 70200 'Animal_Collision
            'BusinessPursuits_Clerical_HO_71 = 20049 'Business Pursuits - Clerical
            'BusinessPursuits_SalesPersonExcludingInstallation_HO_71 = 20050 'Business Pursuits - Sales Person - Excluding Installation
            'BusinessPursuits_SalesPersonIncludingInstallation_HO_71 = 20051 'Business Pursuits - Sales Person - Including Installation
            'BusinessPursuits_TeacherLabEtc_ExcludingCorporalPunishment_HO_71 = 20052 'Business Pursuits - Teacher - Lab Etc. - Excluding Corporal Punishment
            'BusinessPursuits_TeacherLabEtc_IncludingCorporalPunishment_HO_71 = 20053 'Business Pursuits - Teacher - Lab Etc. - Including Corporal Punishment
            'BusinessPursuits_TeacherOtherExcludingCorporalPunishment_HO_71 = 20054 'Business Pursuits - Teacher - Other - Excluding Corporal Punishment
            'BusinessPursuits_TeacherOtherIncludingCorporalPunishment_HO_71 = 20055 'Business Pursuits - Teacher - Other - Including Corporal Punishment
            'FarmOwnedAndOperatedByInsured0_160Acres_HO_2446 = 20215 'Farm Owned and Operated By Insured: 0-160 Acres
            'FarmOwnedAndOperatedByInsured160_500Acres_HO_2446 = 20216 'Farm Owned and Operated By Insured: 160-500 Acres
            'FarmOwnedAndOperatedByInsuredOver500Acres_HO_2446 = 20217 'Farm Owned and Operated By Insured: Over 500 Acres
            'HomeDayCare_HO_323 = 20169 'Home Day Care Liability
            'IncidentalFarmingPersonalLiability_HO_72 = 20163 'Incidental Farmers Personal Liability
            'PermittedIncidentalOccupancies_ResidencePremises_HO_42 = 20035 'Permitted Incidental Occupancies - Residence Premises 
            'PermittedIncidentalOccupanciesOtherResidence_HO_43 = 20036 'Permitted Incidental Occupancies - Other Residence
            'PersonalInjury_HO_82 = 20159 'Personal Injury
            'SpecialEventCoverage_92_347 = 80061 'Special Event Coverage
            'WaterbedLiability_HO_85 = 312 'Waterbed Coverage
            '12/3/2013: without values
            '_3Or4FamilyLiability_HO_74 '20157; 3 or 4 Family Liability
            'AdditionalResidence_OccupiedbyInsured_NA '20154; Other Location Occupied By Insured
            'AdditionalResidenceRentedtoOthers_HO_70 '20156; Additional Residence Rented To Other
            'AnimalCollision_HO_73 '70200; Animal_Collision
            'BusinessPursuits_Clerical_HO_71 '20049; Business Pursuits - Clerical
            'BusinessPursuits_SalesPersonExcludingInstallation_HO_71 '20050; Business Pursuits - Sales Person - Excluding Installation
            'BusinessPursuits_SalesPersonIncludingInstallation_HO_71 '20051; Business Pursuits - Sales Person - Including Installation
            'BusinessPursuits_TeacherLabEtc_ExcludingCorporalPunishment_HO_71 '20052; Business Pursuits - Teacher - Lab Etc. - Excluding Corporal Punishment
            'BusinessPursuits_TeacherLabEtc_IncludingCorporalPunishment_HO_71 '20053; Business Pursuits - Teacher - Lab Etc. - Including Corporal Punishment
            'BusinessPursuits_TeacherOtherExcludingCorporalPunishment_HO_71 '20054; Business Pursuits - Teacher - Other - Excluding Corporal Punishment
            'BusinessPursuits_TeacherOtherIncludingCorporalPunishment_HO_71 '20055; Business Pursuits - Teacher - Other - Including Corporal Punishment
            'FarmOwnedAndOperatedByInsured0_160Acres_HO_2446 '20215; Farm Owned and Operated By Insured: 0-160 Acres
            'FarmOwnedAndOperatedByInsured160_500Acres_HO_2446 '20216; Farm Owned and Operated By Insured: 160-500 Acres
            'FarmOwnedAndOperatedByInsuredOver500Acres_HO_2446 '20217; Farm Owned and Operated By Insured: Over 500 Acres
            'HomeDayCare_HO_323 '20169; Home Day Care Liability
            'IncidentalFarmingPersonalLiability_HO_72 '20163; Incidental Farmers Personal Liability
            'PermittedIncidentalOccupancies_ResidencePremises_HO_42 '20035; Permitted Incidental Occupancies - Residence Premises 
            'PermittedIncidentalOccupanciesOtherResidence_HO_43 '20036; Permitted Incidental Occupancies - Other Residence
            'PersonalInjury_HO_82 '20159; Personal Injury
            'SpecialEventCoverage_92_347 '80061; Special Event Coverage
            'WaterbedLiability_HO_85 '312; Waterbed Coverage

            'updated 12/2/2013 to use coverage code desc instead of caption
            '_3Or4FamilyLiability = 20157 '3 or 4 Family Liability (HO-74)
            'AdditionalResidenceRentedToOther = 20156 'Additional Residence Rented to Others (HO-70)
            'Animal_Collision = 70200 'Animal Collision (HO-73)
            'BusinessPursuits_Clerical = 20049 'Business Pursuits - Clerical (HO-71)
            'BusinessPursuits_SalesPerson_ExcludingInstallation = 20050 'Business Pursuits - Sales Person Excluding Installation (HO-71)
            'BusinessPursuits_SalesPerson_IncludingInstallation = 20051 'Business Pursuits - Sales Person Including Installation (HO-71)
            'BusinessPursuits_Teacher_LabEtc__ExcludingCorporalPunishment = 20052 'Business Pursuits - Teacher Lab Etc. Excluding Corporal Punishment (HO-71)
            'BusinessPursuits_Teacher_LabEtc__IncludingCorporalPunishment = 20053 'Business Pursuits - Teacher Lab Etc. Including Corporal Punishment (HO-71)
            'BusinessPursuits_Teacher_Other_ExcludingCorporalPunishment = 20054 'Business Pursuits - Teacher Other Excluding Corporal Punishment (HO-71)
            'BusinessPursuits_Teacher_Other_IncludingCorporalPunishment = 20055 'Business Pursuits - Teacher Other Including Corporal Punishment (HO-71)
            'FarmOwnedAndOperatedByInsured0_160Acres = 20215 'Farm Owned and Operated By Insured: 0-160 Acres (HO-24 46)
            'FarmOwnedAndOperatedByInsured160_500Acres = 20216 'Farm Owned and Operated By Insured: 160-500 Acres (HO-24 46)
            'FarmOwnedAndOperatedByInsuredOver500Acres = 20217 'Farm Owned and Operated By Insured: Over 500 Acres (HO-24 46)
            'HomeDayCareLiability = 20169 'Home Day Care (HO-323)
            'IncidentalFarmersPersonalLiability = 20163 'Incidental Farming Personal Liability (HO-72)
            'OtherLocationOccupiedByInsured = 20154 'Additional Residence - Occupied by Insured (N/A)
            'PermittedIncidentalOccupancies_OtherResidence = 20036 'Permitted Incidental Occupancies Other Residence (HO-43)
            'PermittedIncidentalOccupancies_ResidencePremises = 20035 'Permitted Incidental Occupancies - Residence Premises (HO-42)
            'PersonalInjury = 20159 'Personal Injury (HO-82)
            'SpecialEventCoverage = 80061 'Special Event Coverage (92-347)
            'WaterbedCoverage = 312 'Waterbed Liability (HO-85)
            '12/3/2013: without values
            _3Or4FamilyLiability '20157; 3 or 4 Family Liability (HO-74)
            AdditionalResidenceRentedToOther '20156; Additional Residence Rented to Others (HO-70)
            Animal_Collision '70200; Animal Collision (HO-73)
            BusinessPursuits_Clerical '20049; Business Pursuits - Clerical (HO-71)
            BusinessPursuits_SalesPerson_ExcludingInstallation '20050; Business Pursuits - Sales Person Excluding Installation (HO-71)
            BusinessPursuits_SalesPerson_IncludingInstallation '20051; Business Pursuits - Sales Person Including Installation (HO-71)
            BusinessPursuits_Teacher_LabEtc__ExcludingCorporalPunishment '20052; Business Pursuits - Teacher Lab Etc. Excluding Corporal Punishment (HO-71)
            BusinessPursuits_Teacher_LabEtc__IncludingCorporalPunishment '20053; Business Pursuits - Teacher Lab Etc. Including Corporal Punishment (HO-71)
            BusinessPursuits_Teacher_Other_ExcludingCorporalPunishment '20054; Business Pursuits - Teacher Other Excluding Corporal Punishment (HO-71)
            BusinessPursuits_Teacher_Other_IncludingCorporalPunishment '20055; Business Pursuits - Teacher Other Including Corporal Punishment (HO-71)
            FarmOwnedAndOperatedByInsured0_160Acres '20215; Farm Owned and Operated By Insured: 0-160 Acres (HO-24 46)
            FarmOwnedAndOperatedByInsured160_500Acres '20216; Farm Owned and Operated By Insured: 160-500 Acres (HO-24 46)
            FarmOwnedAndOperatedByInsuredOver500Acres '20217; Farm Owned and Operated By Insured: Over 500 Acres (HO-24 46)
            HomeDayCareLiability '20169; Home Day Care (HO-323)
            IncidentalFarmersPersonalLiability '20163; Incidental Farming Personal Liability (HO-72)
            OtherLocationOccupiedByInsured '20154; Additional Residence - Occupied by Insured (N/A)
            PermittedIncidentalOccupancies_OtherResidence '20036; Permitted Incidental Occupancies Other Residence (HO-43)
            PermittedIncidentalOccupancies_ResidencePremises '20035; Permitted Incidental Occupancies - Residence Premises (HO-42)
            PersonalInjury '20159; Personal Injury (HO-82)
            SpecialEventCoverage '80061; Special Event Coverage (92-347)
            WaterbedCoverage '312; Waterbed Liability (HO-85)

            'HOM2018 Upgrade
            CanineLiabilityExclusion '80308
            IncidentalFarmingPersonalLiability_OffPremises '80512
            LowPowerRecreationalMotorVehicleLiability '80509
            DamageToPropertyOfOthers '80522
        End Enum
        ''' <summary>
        ''' valid types for DFR section II coverages
        ''' </summary>
        ''' <remarks>value previously corresponded to coveragecode_id in Diamond; now see static data xml file; this enum type should be used for DFR instead of general one for HOM and DFR</remarks>
        Enum DFR_SectionIICoverageType 'added 8/16/2013 to differentiate DFR from HOM
            'None = 0
            '12/3/2013: without values
            None
            'AdditionalResidence_OccupiedbyInsured = 20154 'Other Location Occupied By Insured
            'AdditionalResidenceRentedtoOthers = 20156 'Additional Residence Rented To Other
            'BusinessPursuits_Unclassified = 80092 'Business Pursuits - Other
            'BusinessPursuits_Clerical = 20049 'Business Pursuits - Clerical
            'BusinessPursuits_SalesCollectorsOrMessengersExcludingInstallation = 20050 'Business Pursuits - Sales Person - Excluding Installation
            'BusinessPursuits_SalesCollectorsOrMessengersIncludingInstallation = 20051 'Business Pursuits - Sales Person - Including Installation
            'BusinessPursuits_TeacherLabEtc_ExcludingCorporalPunishment = 20052 'Business Pursuits - Teacher - Lab Etc. - Excluding Corporal Punishment
            'BusinessPursuits_TeacherLabEtc_IncludingCorporalPunishment = 20053 'Business Pursuits - Teacher - Lab Etc. - Including Corporal Punishment
            'BusinessPursuits_TeacherOtherExcludingCorporalPunishment = 20054 'Business Pursuits - Teacher - Other - Excluding Corporal Punishment
            'BusinessPursuits_TeacherOtherIncludingCorporalPunishment = 20055 'Business Pursuits - Teacher - Other - Including Corporal Punishment
            'ContingentWorkersCompensation = 40097 'Contingent Workers Compensation
            'EmployersLiability = 80090 'Employer Liability
            'IncidentalMotorConveyances = 20160 'Incidental Motor Conveyance
            'LossAssessmentLiability = 80093 'Loss Assessment Liability
            'Non_OwnerOccupiedDwelling = 80091 'Non-Owner Occupied Dwelling
            'OfficeProfPriv_SchoolStudioOccupancy = 70136 'Location_Office_Prof_Priv_School_Studio_Occupancy
            'PermittedIncidentalOccupanciesOtherResidence = 20036 'Permitted Incidental Occupancies - Other Residence
            'PersonalInjury = 20159 'Personal Injury
            '12/3/2013: without values
            'AdditionalResidence_OccupiedbyInsured '20154; Other Location Occupied By Insured
            'AdditionalResidenceRentedtoOthers '20156; Additional Residence Rented To Other
            'BusinessPursuits_Unclassified '80092; Business Pursuits - Other
            'BusinessPursuits_Clerical '20049; Business Pursuits - Clerical
            'BusinessPursuits_SalesCollectorsOrMessengersExcludingInstallation '20050; Business Pursuits - Sales Person - Excluding Installation
            'BusinessPursuits_SalesCollectorsOrMessengersIncludingInstallation '20051; Business Pursuits - Sales Person - Including Installation
            'BusinessPursuits_TeacherLabEtc_ExcludingCorporalPunishment '20052; Business Pursuits - Teacher - Lab Etc. - Excluding Corporal Punishment
            'BusinessPursuits_TeacherLabEtc_IncludingCorporalPunishment '20053; Business Pursuits - Teacher - Lab Etc. - Including Corporal Punishment
            'BusinessPursuits_TeacherOtherExcludingCorporalPunishment '20054; Business Pursuits - Teacher - Other - Excluding Corporal Punishment
            'BusinessPursuits_TeacherOtherIncludingCorporalPunishment '20055; Business Pursuits - Teacher - Other - Including Corporal Punishment
            'ContingentWorkersCompensation '40097; Contingent Workers Compensation
            'EmployersLiability '80090; Employer Liability
            'IncidentalMotorConveyances '20160; Incidental Motor Conveyance
            'LossAssessmentLiability '80093; Loss Assessment Liability
            'Non_OwnerOccupiedDwelling '80091; Non-Owner Occupied Dwelling
            'OfficeProfPriv_SchoolStudioOccupancy '70136; Location_Office_Prof_Priv_School_Studio_Occupancy
            'PermittedIncidentalOccupanciesOtherResidence '20036; Permitted Incidental Occupancies - Other Residence
            'PersonalInjury '20159; Personal Injury

            'updated 12/2/2013 to use coverage code desc instead of caption
            'AdditionalResidenceRentedToOther = 20156 'Additional Residence Rented to Others
            'BusinessPursuits_Clerical = 20049 'Business Pursuits - Clerical
            'BusinessPursuits_Other = 80092 'Business Pursuits  - Unclassified
            'BusinessPursuits_SalesPerson_ExcludingInstallation = 20050 'Business Pursuits - Sales, Collectors or Messengers Excluding Installation
            'BusinessPursuits_SalesPerson_IncludingInstallation = 20051 'Business Pursuits - Sales, Collectors or Messengers Including Installation
            'BusinessPursuits_Teacher_LabEtc__ExcludingCorporalPunishment = 20052 'Business Pursuits - Teacher Lab Etc. Excluding Corporal Punishment
            'BusinessPursuits_Teacher_LabEtc__IncludingCorporalPunishment = 20053 'Business Pursuits - Teacher Lab Etc. Including Corporal Punishment
            'BusinessPursuits_Teacher_Other_ExcludingCorporalPunishment = 20054 'Business Pursuits - Teacher Other Excluding Corporal Punishment
            'BusinessPursuits_Teacher_Other_IncludingCorporalPunishment = 20055 'Business Pursuits - Teacher Other Including Corporal Punishment
            'ContingentWorkersCompensation = 40097 'Contingent Workers Compensation
            'EmployerLiability = 80090 'Employers Liability
            'IncidentalMotorConveyance = 20160 'Incidental Motor Conveyances
            'Location_Office_Prof_Priv_School_Studio_Occupancy = 70136 'Office, Prof, Priv. School/Studio Occupancy
            'LossAssessmentLiability = 80093 'Loss Assessment Liability
            'Non_OwnerOccupiedDwelling = 80091 'Non-Owner Occupied Dwelling
            'OtherLocationOccupiedByInsured = 20154 'Additional Residence - Occupied by Insured
            'PermittedIncidentalOccupancies_OtherResidence = 20036 'Permitted Incidental Occupancies Other Residence
            'PersonalInjury = 20159 'Personal Injury
            '12/3/2013: without values
            AdditionalResidenceRentedToOther '20156; Additional Residence Rented to Others
            BusinessPursuits_Clerical '20049; Business Pursuits - Clerical
            BusinessPursuits_Other '80092; Business Pursuits  - Unclassified
            BusinessPursuits_SalesPerson_ExcludingInstallation '20050; Business Pursuits - Sales, Collectors or Messengers Excluding Installation
            BusinessPursuits_SalesPerson_IncludingInstallation '20051; Business Pursuits - Sales, Collectors or Messengers Including Installation
            BusinessPursuits_Teacher_LabEtc__ExcludingCorporalPunishment '20052; Business Pursuits - Teacher Lab Etc. Excluding Corporal Punishment
            BusinessPursuits_Teacher_LabEtc__IncludingCorporalPunishment '20053; Business Pursuits - Teacher Lab Etc. Including Corporal Punishment
            BusinessPursuits_Teacher_Other_ExcludingCorporalPunishment '20054; Business Pursuits - Teacher Other Excluding Corporal Punishment
            BusinessPursuits_Teacher_Other_IncludingCorporalPunishment '20055; Business Pursuits - Teacher Other Including Corporal Punishment
            ContingentWorkersCompensation '40097; Contingent Workers Compensation
            EmployerLiability '80090; Employers Liability
            IncidentalMotorConveyance '20160; Incidental Motor Conveyances
            Location_Office_Prof_Priv_School_Studio_Occupancy '70136; Office, Prof, Priv. School/Studio Occupancy
            LossAssessmentLiability '80093; Loss Assessment Liability
            Non_OwnerOccupiedDwelling '80091; Non-Owner Occupied Dwelling
            OtherLocationOccupiedByInsured '20154; Additional Residence - Occupied by Insured
            PermittedIncidentalOccupancies_OtherResidence '20036; Permitted Incidental Occupancies Other Residence
            PersonalInjury '20159; Personal Injury
        End Enum
        'added 5/19/2015 for Farm
        ''' <summary>
        ''' valid types for DFR section II coverages
        ''' </summary>
        ''' <remarks>value previously corresponded to coveragecode_id in Diamond; now see static data xml file; this enum type should be used for FAR instead of general one for HOM, DFR, and FAR</remarks>
        Enum FAR_SectionIICoverageType
            None

            'using coverage code desc
            BusinessPursuits_Clerical '20049; Business Pursuits - Clerical; also HOM and DFR
            BusinessPursuits_SalesPerson_ExcludingInstallation '20050; Business Pursuits - Sales Person Excluding Installation; also HOM and DFR
            BusinessPursuits_SalesPerson_IncludingInstallation '20051; Business Pursuits - Sales Person Including Installation; also HOM and DFR
            BusinessPursuits_Teacher_LabEtc__ExcludingCorporalPunishment '20052; Business Pursuits - Teacher Lab Etc. Excluding Corporal Punishment; also HOM and DFR
            BusinessPursuits_Teacher_LabEtc__IncludingCorporalPunishment '20053; Business Pursuits - Teacher Lab Etc. Including Corporal Punishment; also HOM and DFR
            BusinessPursuits_Teacher_Other_ExcludingCorporalPunishment '20054; Business Pursuits - Teacher Other Excluding Corporal Punishment; also HOM and DFR
            BusinessPursuits_Teacher_Other_IncludingCorporalPunishment '20055; Business Pursuits - Teacher Other Including Corporal Punishment; also HOM and DFR
            DomesticEmployees '80133; Domestic Employees; just FAR
            EnhancementEndorsement '80094; Liability Enhancement Endorsement; also PPA cov but not Section II cov
            Location_Custom_Farming_No_Spraying '70129; Custom Farming - No Spraying; just FAR
            Location_Custom_Farming_With_Spraying '80115; Custom Farming - with Spraying; just FAR
            Location_Farm_Personal_Liability_GL9 '70139; Farm Personal Liability GL9; just FAR
            Location_Fruit_or_Vegetable_Pick_Your_Own_Operations '70133; Fruit or Vegetable Pick-Your-Own Operations - No Off Ground Picking; just FAR
            Location_Fruit_or_Vegetable_Pick_Your_Own_Operations_No '80110; Fruit or Vegetable Pick-Your-Own Operations - With Off Ground Picking; just FAR
            Location_Horse_Boarding '70134; Horse Boarding; just FAR
            Location_Incidental_Business_Exposures '70135; Incidental Business Exposures; just FAR
            Location_Non_Relative_Resident '70169; Non-Relative Resident; just FAR
            Location_Occupying_Residence_on_Premises '70167; Additional Insured - Partners, Corporate Officers, or Co-Owners; just FAR
            Location_Office_Prof_Priv_School_Studio_Occupancy '70136; Office, Prof, Priv. School/Studio Occupancy; also DFR
            Location_Personal_Injury '70137; Personal Injury; just FAR
            Motorized_Vehicles_Ohio '80553
            Named_Persons_Medical_Payments '70201; Family Medical Payments; just FAR
            OptionalLiability_AdditionalFarmPremisesRentedtoOthers '80113; Optional Liability - Additional Farm Premises Rented to Others; just FAR
            OptionalLiability_AdditionalResidencePremisesOccupiedbyanInsured '40044; Additional Residence Premises Occupied by Insured; just FAR
            OptionalLiability_AdditionalResidencesOrFarmsRentedtoOthers '40045; Optional Liability - Additional Residences Rented to Others; just FAR
            OptionalLiability_CareProvidedForOthers '40047; Care Provided for Others; just FAR
            OptionalLiability_FarmPollutionLiability '40054; Limited Farm Pollution Liability; just FAR
            CanineLiabilityExclusion '80308; Exclusion - Canine (FAR 4003 01 22)

            'using caption
            'AdditionalInsured_PartnersCorporateOfficersOrCo_Owners '70167; Location_Occupying_Residence_on_Premises
            'AdditionalResidencePremisesOccupiedbyInsured '40044; Optional Liability - Additional Residence Premises Occupied by an Insured
            'BusinessPursuits_Clerical '20049; Business Pursuits - Clerical
            'BusinessPursuits_SalesPersonExcludingInstallation '20050; Business Pursuits - Sales Person - Excluding Installation
            'BusinessPursuits_SalesPersonIncludingInstallation '20051; Business Pursuits - Sales Person - Including Installation
            'BusinessPursuits_TeacherLabEtc_ExcludingCorporalPunishment '20052; Business Pursuits - Teacher - Lab Etc. - Excluding Corporal Punishment
            'BusinessPursuits_TeacherLabEtc_IncludingCorporalPunishment '20053; Business Pursuits - Teacher - Lab Etc. - Including Corporal Punishment
            'BusinessPursuits_TeacherOtherExcludingCorporalPunishment '20054; Business Pursuits - Teacher - Other - Excluding Corporal Punishment
            'BusinessPursuits_TeacherOtherIncludingCorporalPunishment '20055; Business Pursuits - Teacher - Other - Including Corporal Punishment
            'CareProvidedForOthers '40047; Optional Liability - Care Provided for Others
            'CustomFarming_NoSpraying '70129; Location_Custom_Farming_No_Spraying
            'CustomFarming_withSpraying '80115; Location_Custom_Farming_With_Spraying
            'DomesticEmployees '80133; Domestic Employees
            'FamilyMedicalPayments '70201; Named_Persons_Medical_Payments
            'FarmPersonalLiabilityGL9 '70139; Location_Farm_Personal_Liability_GL9
            'FruitOrVegetablePick_Your_OwnOperations_NoOffGroundPicking '70133; Location_Fruit_or_Vegetable_Pick_Your_Own_Operations
            'FruitOrVegetablePick_Your_OwnOperations_WithOffGroundPicking '80110; Location_Fruit_or_Vegetable_Pick_Your_Own_Operations_No
            'HorseBoarding '70134; Location_Horse_Boarding
            'IncidentalBusinessExposures '70135; Location_Incidental_Business_Exposures
            'LiabilityEnhancementEndorsement '80094; Enhancement Endorsement
            'LimitedFarmPollutionLiability '40054; Optional Liability - Farm Pollution Liability
            'Non_RelativeResident '70169; Location_Non_Relative_Resident
            'OfficeProfPriv_SchoolStudioOccupancy '70136; Location_Office_Prof_Priv_School_Studio_Occupancy
            'OptionalLiability_AdditionalFarmPremisesRentedtoOthers '80113; Optional Liability - Additional Farm Premises Rented to Others
            'OptionalLiability_AdditionalResidencesRentedtoOthers '40045; Optional Liability - Additional Residences or Farms Rented to Others
            'PersonalInjury '70137; Location_Personal_Injury
        End Enum

        Dim qqHelper As New QuickQuoteHelperClass

        Private _CoverageType As SectionIICoverageType
        Private _CoverageCodeId As String
        Private _Premium As String
        Private _IncreasedLimit As String 'doesn't seem to be enabled for any HOM types; uncommented for DFR 8/16/2013
        Private _IncludedLimit As String 'added 9/10/2014
        Private _TotalLimit As String 'added 9/10/2014
        Private _Description As String
        Private _Name As QuickQuoteName
        Private _Address As QuickQuoteAddress
        Private _NumberOfPersonsReceivingCare As String
        Private _NumberOfFamilies As String
        Private _NumberOfFullTimeEmployees_180plus_days As String
        Private _NumberOfPartTimeEmployees_41_to_180_days As String
        Private _NumberOfPartTimeEmployees_40_or_less_days As String
        Private _EstimatedNumberOfHead As String '6/8/2015 note: NumberOfLivestock on QuickQuoteSectionCoverage
        Private _BusinessType As String
        Private _InitialFarmPremises As Boolean
        Private _EventFrom As String
        Private _EventTo As String
        Private _BusinessName As String
        Private _AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        'added 8/16/2013 for DFR
        Private _NavigationPeriodEffDate As String
        Private _NavigationPeriodExpDate As String
        Private _NumberOfEmployees As String
        'added 8/16/2013 to differentiate HOM and DFR
        Private _HOM_CoverageType As HOM_SectionIICoverageType
        Private _DFR_CoverageType As DFR_SectionIICoverageType
        Private _FAR_CoverageType As FAR_SectionIICoverageType 'added 5/19/2015

        Private _CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean 'added 4/29/2014

        'Private _SectionCoverageNum As String 'added 10/14/2014 for reconciliation; removed 10/29/2018
        Private _SectionCoverageNumGroup As QuickQuoteDiamondNumGroup 'added 10/29/2018

        'added 6/8/2015 for Farm
        Private _EstimatedReceipts As String
        Private _NumberOfDomesticEmployees As String
        Private _NumberOfEvents As String
        Private _NumberOfStalls As String
        'added 6/12/2015 for Farm
        Private _BusinessPursuitTypeId As String 'static data
        'added 10/8/2015 for Farm
        Private _IncreasedLimitId As String

        'added 11/1/2021
        Private _AddedDate As String
        Private _LastModifiedDate As String
        Private _PCAdded_Date As String
        Private _AddedImageNum As String

        ' Use the other private variables (EventFrom, EventTo)
        'Private _EventEffDate As String
        'Private _EventExpDate As String

        ''' <summary>
        ''' general coverage type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>should use property specific to LOB instead</remarks>
        Public Property CoverageType As SectionIICoverageType
            Get
                Return _CoverageType
            End Get
            Set(value As SectionIICoverageType)
                _CoverageType = value
                If _CoverageType <> Nothing AndAlso _CoverageType <> SectionIICoverageType.None Then
                    '_CoverageCodeId = CInt(_CoverageType).ToString
                    ''added 8/16/2013
                    'If System.Enum.IsDefined(GetType(HOM_SectionIICoverageType), CInt(_CoverageCodeId)) = True Then
                    '    _HOM_CoverageType = CInt(_CoverageCodeId)
                    'End If
                    'If System.Enum.IsDefined(GetType(DFR_SectionIICoverageType), CInt(_CoverageCodeId)) = True Then
                    '    _DFR_CoverageType = CInt(_CoverageCodeId)
                    'End If
                    'updated 12/4/2013
                    '_CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, _CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
                    'updated 12/20/2013 to send enum text
                    _CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, System.Enum.GetName(GetType(SectionIICoverageType), _CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
                    'If System.Enum.TryParse(Of HOM_SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, _CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType), _HOM_CoverageType) = False Then
                    'updated 12/23/2013 to send enum text
                    If System.Enum.TryParse(Of HOM_SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, System.Enum.GetName(GetType(SectionIICoverageType), _CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType), _HOM_CoverageType) = False Then
                        _HOM_CoverageType = HOM_SectionIICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(HOM_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, _CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType)) = True Then
                    '    _HOM_CoverageType = System.Enum.Parse(GetType(HOM_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, _CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType))
                    'End If
                    'If System.Enum.TryParse(Of DFR_SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, _CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType), _DFR_CoverageType) = False Then
                    'updated 12/23/2013 to send enum text
                    If System.Enum.TryParse(Of DFR_SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, System.Enum.GetName(GetType(SectionIICoverageType), _CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType), _DFR_CoverageType) = False Then
                        _DFR_CoverageType = DFR_SectionIICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(DFR_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, _CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType)) = True Then
                    '    _DFR_CoverageType = System.Enum.Parse(GetType(DFR_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, _CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType))
                    'End If

                    'added 5/20/2015 for FAR
                    If System.Enum.TryParse(Of FAR_SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, System.Enum.GetName(GetType(SectionIICoverageType), _CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType), _FAR_CoverageType) = False Then
                        _FAR_CoverageType = FAR_SectionIICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(FAR_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, _CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType)) = True Then
                    '    _FAR_CoverageType = System.Enum.Parse(GetType(FAR_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, _CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType))
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
        Public Property HOM_CoverageType As HOM_SectionIICoverageType 'added 8/16/2013 to differentiate HOM from DFR
            Get
                Return _HOM_CoverageType
            End Get
            Set(value As HOM_SectionIICoverageType)
                _HOM_CoverageType = value
                If _HOM_CoverageType <> Nothing AndAlso _HOM_CoverageType <> HOM_SectionIICoverageType.None Then
                    '_CoverageCodeId = CInt(_HOM_CoverageType).ToString
                    'If System.Enum.IsDefined(GetType(SectionIICoverageType), CInt(_CoverageCodeId)) = True Then
                    '    _CoverageType = CInt(_CoverageCodeId)
                    'End If
                    'updated 12/4/2013
                    '_CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, _HOM_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
                    'updated 12/20/2013 to send enum text
                    _CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, System.Enum.GetName(GetType(HOM_SectionIICoverageType), _HOM_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
                    'If System.Enum.TryParse(Of SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, _HOM_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType), _CoverageType) = False Then
                    'updated 12/23/2013 to send enum text
                    If System.Enum.TryParse(Of SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, System.Enum.GetName(GetType(HOM_SectionIICoverageType), _HOM_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType), _CoverageType) = False Then
                        '_CoverageType = HOM_SectionIICoverageType.None
                        'updated 5/20/2015
                        _CoverageType = SectionIICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, _HOM_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType)) = True Then
                    '    _CoverageType = System.Enum.Parse(GetType(SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, _HOM_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType))
                    'End If
                    'previous logic wasn't setting this...
                    'If System.Enum.TryParse(Of DFR_SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, _HOM_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType), _DFR_CoverageType) = False Then
                    'updated 12/23/2013 to send enum text
                    If System.Enum.TryParse(Of DFR_SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, System.Enum.GetName(GetType(HOM_SectionIICoverageType), _HOM_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType), _DFR_CoverageType) = False Then
                        _DFR_CoverageType = DFR_SectionIICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(DFR_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, _HOM_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType)) = True Then
                    '    _DFR_CoverageType = System.Enum.Parse(GetType(DFR_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, _HOM_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType))
                    'End If

                    'added 5/20/2015 for FAR
                    If System.Enum.TryParse(Of FAR_SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, System.Enum.GetName(GetType(HOM_SectionIICoverageType), _HOM_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType), _FAR_CoverageType) = False Then
                        _FAR_CoverageType = FAR_SectionIICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(FAR_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, _HOM_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType)) = True Then
                    '    _FAR_CoverageType = System.Enum.Parse(GetType(FAR_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType, _HOM_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType))
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
        Public Property DFR_CoverageType As DFR_SectionIICoverageType 'added 8/16/2013 to differentiate DFR from HOM
            Get
                Return _DFR_CoverageType
            End Get
            Set(value As DFR_SectionIICoverageType)
                _DFR_CoverageType = value
                If _DFR_CoverageType <> Nothing AndAlso _DFR_CoverageType <> DFR_SectionIICoverageType.None Then
                    '_CoverageCodeId = CInt(_DFR_CoverageType).ToString
                    'If System.Enum.IsDefined(GetType(SectionIICoverageType), CInt(_CoverageCodeId)) = True Then
                    '    _CoverageType = CInt(_CoverageCodeId)
                    'End If
                    'updated 12/4/2013
                    '_CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, _DFR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
                    'updated 12/20/2013 to send enum text
                    _CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, System.Enum.GetName(GetType(DFR_SectionIICoverageType), _DFR_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
                    'If System.Enum.TryParse(Of SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, _DFR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType), _CoverageType) = False Then
                    'updated 12/23/2013 to send enum text
                    If System.Enum.TryParse(Of SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, System.Enum.GetName(GetType(DFR_SectionIICoverageType), _DFR_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType), _CoverageType) = False Then
                        '_CoverageType = HOM_SectionIICoverageType.None
                        'updated 5/20/2015
                        _CoverageType = SectionIICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, _DFR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType)) = True Then
                    '    _CoverageType = System.Enum.Parse(GetType(SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, _DFR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType))
                    'End If
                    'previous logic wasn't setting this...
                    'If System.Enum.TryParse(Of HOM_SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, _DFR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType), _HOM_CoverageType) = False Then
                    'updated 12/23/2013 to send enum text
                    If System.Enum.TryParse(Of HOM_SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, System.Enum.GetName(GetType(DFR_SectionIICoverageType), _DFR_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType), _HOM_CoverageType) = False Then
                        _HOM_CoverageType = HOM_SectionIICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(HOM_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, _DFR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType)) = True Then
                    '    _HOM_CoverageType = System.Enum.Parse(GetType(HOM_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, _DFR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType))
                    'End If

                    'added 5/20/2015 for FAR
                    If System.Enum.TryParse(Of FAR_SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, System.Enum.GetName(GetType(DFR_SectionIICoverageType), _DFR_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType), _FAR_CoverageType) = False Then
                        _FAR_CoverageType = FAR_SectionIICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(FAR_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, _DFR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType)) = True Then
                    '    _FAR_CoverageType = System.Enum.Parse(GetType(FAR_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType, _DFR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType))
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
        Public Property FAR_CoverageType As FAR_SectionIICoverageType 'added 5/19/2015
            Get
                Return _FAR_CoverageType
            End Get
            Set(value As FAR_SectionIICoverageType)
                _FAR_CoverageType = value
                If _FAR_CoverageType <> Nothing AndAlso _FAR_CoverageType <> FAR_SectionIICoverageType.None Then
                    '_CoverageCodeId = CInt(_FAR_CoverageType).ToString
                    'If System.Enum.IsDefined(GetType(SectionIICoverageType), CInt(_CoverageCodeId)) = True Then
                    '    _CoverageType = CInt(_CoverageCodeId)
                    'End If
                    'updated 12/4/2013
                    '_CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, _FAR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
                    'updated 12/20/2013 to send enum text
                    _CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, System.Enum.GetName(GetType(FAR_SectionIICoverageType), _FAR_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
                    'If System.Enum.TryParse(Of SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, _FAR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType), _CoverageType) = False Then
                    'updated 12/23/2013 to send enum text
                    If System.Enum.TryParse(Of SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, System.Enum.GetName(GetType(FAR_SectionIICoverageType), _FAR_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType), _CoverageType) = False Then
                        _CoverageType = SectionIICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, _FAR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType)) = True Then
                    '    _CoverageType = System.Enum.Parse(GetType(SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, _FAR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType))
                    'End If
                    'previous logic wasn't setting this...
                    'If System.Enum.TryParse(Of HOM_SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, _FAR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType), _HOM_CoverageType) = False Then
                    'updated 12/23/2013 to send enum text
                    If System.Enum.TryParse(Of HOM_SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, System.Enum.GetName(GetType(FAR_SectionIICoverageType), _FAR_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType), _HOM_CoverageType) = False Then
                        _HOM_CoverageType = HOM_SectionIICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(HOM_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, _FAR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType)) = True Then
                    '    _HOM_CoverageType = System.Enum.Parse(GetType(HOM_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, _FAR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType))
                    'End If

                    'added 5/20/2015
                    If System.Enum.TryParse(Of DFR_SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, System.Enum.GetName(GetType(FAR_SectionIICoverageType), _FAR_CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType), _DFR_CoverageType) = False Then
                        _DFR_CoverageType = DFR_SectionIICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(DFR_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, _FAR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType)) = True Then
                    '    _DFR_CoverageType = System.Enum.Parse(GetType(DFR_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType, _FAR_CoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType))
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
                    'If System.Enum.IsDefined(GetType(SectionIICoverageType), CInt(_CoverageCodeId)) = True Then
                    '    _CoverageType = CInt(_CoverageCodeId)
                    '    'added 8/16/2013
                    '    If System.Enum.IsDefined(GetType(HOM_SectionIICoverageType), CInt(_CoverageCodeId)) = True Then
                    '        _HOM_CoverageType = CInt(_CoverageCodeId)
                    '    End If
                    '    If System.Enum.IsDefined(GetType(DFR_SectionIICoverageType), CInt(_CoverageCodeId)) = True Then
                    '        _DFR_CoverageType = CInt(_CoverageCodeId)
                    '    End If
                    'End If
                    'updated 12/4/2013
                    If System.Enum.TryParse(Of SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType), _CoverageType) = False Then
                        _CoverageType = HOM_SectionIICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType)) = True Then
                    '    _CoverageType = System.Enum.Parse(GetType(SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType))
                    'End If
                    If System.Enum.TryParse(Of HOM_SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType), _HOM_CoverageType) = False Then
                        _HOM_CoverageType = HOM_SectionIICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(HOM_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType)) = True Then
                    '    _HOM_CoverageType = System.Enum.Parse(GetType(HOM_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.HOM_CoverageType))
                    'End If
                    If System.Enum.TryParse(Of DFR_SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType), _DFR_CoverageType) = False Then
                        _DFR_CoverageType = DFR_SectionIICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(DFR_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType)) = True Then
                    '    _DFR_CoverageType = System.Enum.Parse(GetType(DFR_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.DFR_CoverageType))
                    'End If

                    'added 5/20/2015 for FAR
                    If System.Enum.TryParse(Of FAR_SectionIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType), _FAR_CoverageType) = False Then
                        _FAR_CoverageType = FAR_SectionIICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(FAR_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType)) = True Then
                    '    _FAR_CoverageType = System.Enum.Parse(GetType(FAR_SectionIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.FAR_CoverageType))
                    'End If
                End If
            End Set
        End Property
        Public Property Premium As String
            Get
                'Return _Premium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_Premium)
            End Get
            Set(value As String)
                _Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Premium)
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
        Public Property Name As QuickQuoteName
            Get
                SetObjectsParent(_Name)
                Return _Name
            End Get
            Set(value As QuickQuoteName)
                _Name = value
                SetObjectsParent(_Name)
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
        Public Property NumberOfPersonsReceivingCare As String
            Get
                Return _NumberOfPersonsReceivingCare
            End Get
            Set(value As String)
                _NumberOfPersonsReceivingCare = value
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
        Public Property NumberOfFullTimeEmployees_180plus_days As String
            Get
                Return _NumberOfFullTimeEmployees_180plus_days
            End Get
            Set(value As String)
                _NumberOfFullTimeEmployees_180plus_days = value
            End Set
        End Property
        Public Property NumberOfPartTimeEmployees_41_to_180_days As String
            Get
                Return _NumberOfPartTimeEmployees_41_to_180_days
            End Get
            Set(value As String)
                _NumberOfPartTimeEmployees_41_to_180_days = value
            End Set
        End Property
        Public Property NumberOfPartTimeEmployees_40_or_less_days As String
            Get
                Return _NumberOfPartTimeEmployees_40_or_less_days
            End Get
            Set(value As String)
                _NumberOfPartTimeEmployees_40_or_less_days = value
            End Set
        End Property
        Public Property EstimatedNumberOfHead As String
            Get
                Return _EstimatedNumberOfHead
            End Get
            Set(value As String)
                _EstimatedNumberOfHead = value
            End Set
        End Property
        Public Property BusinessType As String
            Get
                Return _BusinessType
            End Get
            Set(value As String)
                _BusinessType = value
            End Set
        End Property
        Public Property InitialFarmPremises As Boolean
            Get
                Return _InitialFarmPremises
            End Get
            Set(value As Boolean)
                _InitialFarmPremises = value
            End Set
        End Property
        Public Property EventFrom As String
            Get
                Return _EventFrom
            End Get
            Set(value As String)
                _EventFrom = value
                qqHelper.ConvertToShortDate(_EventFrom)
            End Set
        End Property
        Public Property EventTo As String
            Get
                Return _EventTo
            End Get
            Set(value As String)
                _EventTo = value
                qqHelper.ConvertToShortDate(_EventTo)
            End Set
        End Property
        Public Property BusinessName As String
            Get
                Return _BusinessName
            End Get
            Set(value As String)
                _BusinessName = value
            End Set
        End Property
        Public Property AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05631}")
                Return _AdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _AdditionalInterests = value
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05631}")
            End Set
        End Property
        Public Property NavigationPeriodEffDate As String
            Get
                Return _NavigationPeriodEffDate
            End Get
            Set(value As String)
                _NavigationPeriodEffDate = value
                qqHelper.ConvertToShortDate(_NavigationPeriodEffDate)
            End Set
        End Property
        Public Property NavigationPeriodExpDate As String
            Get
                Return _NavigationPeriodExpDate
            End Get
            Set(value As String)
                _NavigationPeriodExpDate = value
                qqHelper.ConvertToShortDate(_NavigationPeriodExpDate)
            End Set
        End Property
        Public Property NumberOfEmployees As String
            Get
                Return _NumberOfEmployees
            End Get
            Set(value As String)
                _NumberOfEmployees = value
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

        'added 6/8/2015 for Farm
        Public Property EstimatedReceipts As String
            Get
                Return _EstimatedReceipts
            End Get
            Set(value As String)
                _EstimatedReceipts = value
                qqHelper.ConvertToLimitFormat(_EstimatedReceipts)
            End Set
        End Property
        Public Property NumberOfDomesticEmployees As String
            Get
                Return _NumberOfDomesticEmployees
            End Get
            Set(value As String)
                _NumberOfDomesticEmployees = value
            End Set
        End Property
        Public Property NumberOfEvents As String
            Get
                Return _NumberOfEvents
            End Get
            Set(value As String)
                _NumberOfEvents = value
            End Set
        End Property
        Public Property NumberOfStalls As String
            Get
                Return _NumberOfStalls
            End Get
            Set(value As String)
                _NumberOfStalls = value
            End Set
        End Property
        'added 6/12/2015 for Farm
        Public Property BusinessPursuitTypeId As String 'static data
            Get
                Return _BusinessPursuitTypeId
            End Get
            Set(value As String)
                _BusinessPursuitTypeId = value
            End Set
        End Property
        'added 10/8/2015 for Farm
        Public Property IncreasedLimitId As String
            Get
                Return _IncreasedLimitId
            End Get
            Set(value As String)
                _IncreasedLimitId = value
            End Set
        End Property
        Public Property EventEffDate As String
            Get
                Return _EventFrom
            End Get
            Set(value As String)
                _EventFrom = value
            End Set
        End Property

        Public Property EventExpDate As String
            Get
                Return _EventTo
            End Get
            Set(value As String)
                _EventTo = value
            End Set
        End Property

        'added 11/1/2021
        Public Property AddedDate As String
            Get
                Return _AddedDate
            End Get
            Set(value As String)
                _AddedDate = value
            End Set
        End Property
        Public Property LastModifiedDate As String
            Get
                Return _LastModifiedDate
            End Get
            Set(value As String)
                _LastModifiedDate = value
            End Set
        End Property
        Public Property PCAdded_Date As String
            Get
                Return _PCAdded_Date
            End Get
            Set(value As String)
                _PCAdded_Date = value
            End Set
        End Property
        Public Property AddedImageNum As String 'added 7/31/2019
            Get
                Return _AddedImageNum
            End Get
            Set(value As String)
                _AddedImageNum = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _CoverageType = SectionIICoverageType.None
            _CoverageCodeId = ""
            _Premium = ""
            _IncreasedLimit = ""
            _IncludedLimit = "" 'added 9/10/2014
            _TotalLimit = "" 'added 9/10/2014
            _Description = ""
            _Name = New QuickQuoteName
            _Name.NameAddressSourceId = "10022" 'Section Coverage
            _Address = New QuickQuoteAddress
            _NumberOfPersonsReceivingCare = ""
            _NumberOfFamilies = ""
            _NumberOfFullTimeEmployees_180plus_days = ""
            _NumberOfPartTimeEmployees_41_to_180_days = ""
            _NumberOfPartTimeEmployees_40_or_less_days = ""
            _EstimatedNumberOfHead = ""
            _BusinessType = ""
            _InitialFarmPremises = False
            _EventFrom = ""
            _EventTo = ""
            _BusinessName = ""
            '_AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            _AdditionalInterests = Nothing 'added 8/4/2014
            _NavigationPeriodEffDate = ""
            _NavigationPeriodExpDate = ""
            _NumberOfEmployees = ""
            _HOM_CoverageType = HOM_SectionIICoverageType.None
            _DFR_CoverageType = DFR_SectionIICoverageType.None
            _FAR_CoverageType = FAR_SectionIICoverageType.None 'added 5/20/2015

            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False 'added 4/29/2014

            '_SectionCoverageNum = "" 'added 10/14/2014 for reconciliation; removed 10/29/2018
            _SectionCoverageNumGroup = New QuickQuoteDiamondNumGroup(Me) 'added 10/29/2018

            'added 6/8/2015 for Farm
            _EstimatedReceipts = ""
            _NumberOfDomesticEmployees = ""
            _NumberOfEvents = ""
            _NumberOfStalls = ""
            'added 6/12/2015 for Farm
            _BusinessPursuitTypeId = ""
            'added 10/8/2015 for Farm
            _IncreasedLimitId = ""

            ' added 11/1/2021
            _AddedDate = ""
            _LastModifiedDate = ""
            _PCAdded_Date = ""
            _AddedImageNum = ""

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

        Public Overrides Function ToString() As String 'added 6/29/2015
            Dim str As String = ""
            Dim myList As New List(Of String)
            If Me IsNot Nothing Then
                If String.IsNullOrWhiteSpace(CoverageCodeId) = False Then
                    str = "CoverageCodeId: " & CoverageCodeId & " (" & CoverageType & ")"
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
                    If _Name IsNot Nothing Then
                        _Name.Dispose()
                        _Name = Nothing
                    End If
                    If _Address IsNot Nothing Then
                        _Address.Dispose()
                        _Address = Nothing
                    End If
                    If _NumberOfPersonsReceivingCare IsNot Nothing Then
                        _NumberOfPersonsReceivingCare = Nothing
                    End If
                    If _NumberOfFamilies IsNot Nothing Then
                        _NumberOfFamilies = Nothing
                    End If
                    If _NumberOfFullTimeEmployees_180plus_days IsNot Nothing Then
                        _NumberOfFullTimeEmployees_180plus_days = Nothing
                    End If
                    If _NumberOfPartTimeEmployees_41_to_180_days IsNot Nothing Then
                        _NumberOfPartTimeEmployees_41_to_180_days = Nothing
                    End If
                    If _NumberOfPartTimeEmployees_40_or_less_days IsNot Nothing Then
                        _NumberOfPartTimeEmployees_40_or_less_days = Nothing
                    End If
                    If _EstimatedNumberOfHead IsNot Nothing Then
                        _EstimatedNumberOfHead = Nothing
                    End If
                    If _BusinessType IsNot Nothing Then
                        _BusinessType = Nothing
                    End If
                    If _InitialFarmPremises <> Nothing Then
                        _InitialFarmPremises = Nothing
                    End If
                    If _EventFrom IsNot Nothing Then
                        _EventFrom = Nothing
                    End If
                    If _EventTo IsNot Nothing Then
                        _EventTo = Nothing
                    End If
                    If _BusinessName IsNot Nothing Then
                        _BusinessName = Nothing
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
                    If _NavigationPeriodEffDate IsNot Nothing Then
                        _NavigationPeriodEffDate = Nothing
                    End If
                    If _NavigationPeriodExpDate IsNot Nothing Then
                        _NavigationPeriodExpDate = Nothing
                    End If
                    If _NumberOfEmployees IsNot Nothing Then
                        _NumberOfEmployees = Nothing
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

                    'added 6/8/2015 for Farm
                    qqHelper.DisposeString(_EstimatedReceipts)
                    qqHelper.DisposeString(_NumberOfDomesticEmployees)
                    qqHelper.DisposeString(_NumberOfEvents)
                    qqHelper.DisposeString(_NumberOfStalls)
                    'added 6/12/2015 for Farm
                    qqHelper.DisposeString(_BusinessPursuitTypeId)
                    'added 10/8/2015 for Farm
                    qqHelper.DisposeString(_IncreasedLimitId)

                    ' added 11/1/2021
                    qqHelper.DisposeString(_AddedDate)
                    qqHelper.DisposeString(_LastModifiedDate)
                    qqHelper.DisposeString(_PCAdded_Date)
                    qqHelper.DisposeString(_AddedImageNum)

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
