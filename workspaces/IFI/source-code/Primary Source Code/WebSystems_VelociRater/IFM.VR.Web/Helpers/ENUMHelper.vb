Public Class ENUMHelper
    Enum VehicleBodyType
        bodyType_Car = 14
        bodyType_SUV = 41
        bodyType_PickupWOCamper = 40
        bodyType_PickupWCamper = 39
        bodyType_Van = 16
        bodyType_RecTrailer = 19
        bodyType_OtherTrailer = 20
        bodyType_AntiaueAuto = 22
        bodyType_ClassicAuto = 24
        bodyType_MotorHome = 18
        bodyType_Motorcycle = 42
    End Enum

    Enum DriverRelationshipType
        Policyholder1 = 8
        Policyholder2 = 5
        PolicyholderChild = 2
        PolicyholderParentGuardian = 4
        PolicyholderOther = 7
        PolicyholderUnrelated = 11
    End Enum

    Enum AutoLiabilityType
        SplitLimit = 0
        CSL = 1
    End Enum

    Enum VehiclePolicyType
        fullCoverage = 0
        liabilityOnly = 1
        compOnly = 2 ' no longer valid with BUg 8418 - Matt A 3-27-17
        namedNonOwner = 3
        trailer = 4
    End Enum

    Enum PolicyTypeId
        NA
        None
        AutomobileLiability
        PersonalLiability
        WatercraftLiability
        AircraftLiability
        ProfessionalLiability
        InvestmentPropertyLiability
        RecreationalVehicleLiabilityAuto
        RecreationalVehicleLiabilityHome
        OwnersMulti
        ApartmentUnit
        CondominiumUnit
        Manufactured
        SiteBuilt
        OwnersPossesive
        Renters
        Owned
        Rented
        LifeEstate
        LongTermContract
        VacantOrUnoccupied
        BusinessExposuresLiability
        RealEstateLiability
        RecreationalVehicleLiability
        EmployersLiability
        CommercialFarmLiability
        HomeBasedBusinessLiability
        MotorcycleLiability
        FarmPersonalLiability
        PremisesLiability
        NoPrimaryPolicy
        Owner
        NamedNonOwner
        NamedOperatorGovernmentEmployee
        Individual
        Partnership
        Corporation
        IndividualOther
        Other
        Auto
        Home
        Watercraft
        Motorcycle
        AllOther
        RecreationalVehicleLiabilityAuto2
        RecreationalVehicleLiabilityHome2
        Voluntary
        Ceded
        NonOwned
        FarmLiability
        RecreationalVehicle
        MiscellaneousLiability
        ConventionallyBuilt
        Modular
        Prefab
        Panelized
        MobileHome
        Log
        EmployersLiabilityCoverageB
        Standard
        Preferred
        GeneralLiability
        GarageOwnersLiability
        BusinessOwnersLiability
        WorkersCompLiability
        CPPStandard
        CPPPreferred
        POPStandard
        POPPreferred
        EmployersLiabilityStopGap
        AGGLAgribusinessLiability
        AGGLFarmCommercialLiability
        CoverageQEmployersLiability
        CoverageVEmployersLiability
        HiredAndNonOwnedAutomobileLiability
        DwellingFireLiability
        CommercialAutoLiability
    End Enum
End Class
