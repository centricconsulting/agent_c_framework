using System;

namespace IFM.DataServicesCore.CommonObjects.Enums
{
    [System.Serializable]
    public class Enums
    {
        public enum MessageType : int
        {
            None = 0,
            GeneralMessage = 1,
            ValidationMessage = 2
        }

        public enum MessageSeverityType : int
        {
            None = 0,
            Warning = 1,
            StandardError = 2,
            FullStopError = 3
        }

        public enum PolicyInformationVerificationLevel : int
        {
            None = 1,
            PolicyAndName = 10,
            PolicyFull = 20,

            //StaffVerification = 30
            OneTimePaymentByName = 40,

            OneTimePaymentByOnlinePaymentNumber = 50,
            OneTimePaymentByAccountNumber = 60
        }

        public enum ClaimLossType : int
        {
            Farm = 0,
            Home = 1,
            Business = 2,
            Business_Auto = 3,
            Auto_Personal = 4,
            Other_Loss = 5,
            Person = 6,
            Property = 7
        }

        public enum PayPlans : int
        {
            annual = 12,
            semiAnnual = 13,
            quarterly = 14,
            monthly = 15,
            rcc = 18,
            rEft = 19,
            accountBillMonthly = 24,
            accountBillCCMontly = 25,
            accountBillEFTMonthly = 26
        }
        public enum BillingTransactionType
        {
            None = 0,
            PayPlanChange = 1,
            RefundOptionChange = 2,
            SubmitRefundChecks = 3,
            ManageBillingAccounts = 4,
            LinkPolicyToBillingAccount = 5,
            UnlinkPolicyFromBillingAccount = 6,
            CreateBillingAccount = 7,
            DeleteBillingAccount = 8,
            PayorChange = 9,
            EditCreditCardInfo = 10,
            EditEftInfo = 11,
            EditBlackoutInfo = 12,
            ManualInvoice = 13,
            RecalculateInstallsAfterDivRefund = 14,
            RecalculateFutureInstallments = 15,
            ChangeInstallmentDeductionDay = 16,
            ReferredToCollections = 17,
            ExtendCancelDate = 18,
            ChangeInvoiceDueDate = 19,
            AcceptRenewalOffer = 20,
            ChangeDoNotReinstate = 21,
            ChangeAutomaticRefundPayee = 22,
            ModifyPolicyInstallmentSchedule = 23,
            AddAccountServiceCharges = 24,
            RemoveAccountServiceCharges = 25,
            ExcludeFromCollections = 26,
            SuppressInvoices = 27,
            AllowElectronicPayments = 28,
            EditFutureScheduledPayments = 29
        }

        public enum APIPaymentTypeId
        {
            NA,
            CreditCard,
            Echeck,
            RCC,
            EFT,
            AgencyEFT,
            WalletCreditCard,
            WalletEcheck
        }
        public enum PolicyQuoteProcessingType
        {
            NewBusiness = 1,
            Change = 2
        }

        public enum PolicyQuotingApplication
        {
            VR = 1,
            MemberPortal = 2
        }

        public enum DwellingType
        {
            NotAvailable = 0,
            OneFamilyDwelling = 1,
            TwoFamilyDwelling = 2,
            ThreeFamilyDwelling = 3,
            FourFamilyDwelling = 4,
            Apartment1to10Units = 5,
            Farm = 6,
            VacantLand = 7,
            LessorsRisk = 8,
            OtherBusinessProperty = 9,
            Frame = 10,
            Masonry = 11,
            FireResistant = 12,
            MasonryVeneer = 13,
            RentersPolicy = 14,
            CondominiumUnit = 15,
            ManufacturedSingleWide = 16,
            ManufacturedDoubleWide = 17,
            SiteBuiltFrame = 18,
            SiteBuiltMasonry = 19,
            IncidentalBusinessExposuresOffices = 20,
            IncidentalBusinessExposuresRentalDwelling = 21,
            Type1 = 22,
            Type2 = 23,
            Type3 = 24,
            SuperiorConstructionFireResistive = 25,
            SuperiorConstructionMasonryNonCombustible = 26,
            SuperiorConstructionNonCombustible = 27
        }


        public enum State
        {
            ALASKA = 1,
            ALABAMA = 2,
            ARKANSAS = 3,
            ARIZONA = 4,
            CALIFORNIA = 5,
            COLORADO = 6,
            CONNECTICUT = 7,
            DELAWARE = 9,
            FLORIDA = 10,
            GEORGIA = 11,
            HAWAII = 12,
            IOWA = 13,
            IDAHO = 14,
            ILLINOIS = 15,
            INDIANA = 16,
            KANSAS = 17,
            KENTUCKY = 18,
            LOUISIANA = 19,
            MASSACHUSETTS = 20,
            MARYLAND = 21,
            MAINE = 22,
            MICHIGAN = 23,
            MINNESOTA = 24,
            MISSOURI = 25,
            MISSISSIPPI = 26,
            MONTANA = 27,
            NORTH_CAROLINA = 28,
            NORTH_DAKOTA = 29,
            NEBRASKA = 30,
            NEW_HAMPSHIRE = 31,
            NEW_JERSEY = 32,
            NEW_MEXICO = 33,
            NEVADA = 34,
            NEW_YORK = 35,
            OHIO = 36,
            OKLAHOMA = 37,
            OREGON = 38,
            PENNSYLVANIA = 39,
            RHODE_ISLAND = 40,
            SOUTH_CAROLINA = 41,
            SOUTH_DAKOTA = 42,
            TENNESSEE = 43,
            TEXAS = 44,
            UTAH = 45,
            VIRGINIA = 46,
            VERMONT = 47,
            WASHINGTON = 48,
            WISCONSIN = 49,
            WEST_VIRGINIA = 50,
            WYOMING = 51
        }

        public enum ClaimLocationOfLoss
        {
            OnPremises = 1,
            OffPremises = 2,
            Unknown = 3
        }
        public enum ClaimLossIndicatorType
        {
            None = 0,
            PartialLoss = 1,
            TotalLoss = 2
        }

        public enum FNOLClaimLossType
        {
            AccidentalDischargeOrLeakage = 87, // not used in Diamond
            AircraftDamage = 144, // not used in Diamond
            AllOther = 88, // not used in Diamond
            AllOtherPhysicalDamage = 89, // not used in Diamond
            Alteration = 157, // not used in Diamond
            AnimalDamage = 65, // not used in Diamond; use 165 or 254
            ApplianceRelatedWaterDamage = 90, // not used in Diamond
            BackupOfSewerOrDrain = 41,
            BodilyInjuryAutoBI = 152, // not used in Diamond
            BroadFormPerilsAutoPAPDOrOTC = 86, // not used in Diamond
            Collapse = 42,
            CollisionMultipleAutos = 113, // not used in Diamond
            CollisionOther = 114, // not used in Diamond
            CollisionSingleAuto = 115, // not used in Diamond
            CollisionUpsetOverturn = 91, // not used in Diamond
            ComprehensiveAnimal = 116, // not used in Diamond
            ComprehensiveEarthquake = 117, // not used in Diamond
            ComprehensiveExplosion = 118, // not used in Diamond
            ComprehensiveFallingObjectsOrMissiles = 119, // not used in Diamond
            ComprehensiveFire = 120, // not used in Diamond
            ComprehensiveFlood = 121, // not used in Diamond
            ComprehensiveGlassRepair = 122, // not used in Diamond
            ComprehensiveGlassReplace = 123, // not used in Diamond
            ComprehensiveHail = 124, // not used in Diamond
            ComprehensiveLabor = 125, // not used in Diamond
            ComprehensiveOther = 126, // not used in Diamond
            ComprehensiveOtherTransportation = 127, // not used in Diamond
            ComprehensiveOtherWater = 128, // not used in Diamond
            ComprehensiveRiotOrCivilCommotion = 129, // not used in Diamond
            ComprehensiveTheftPartial = 130, // not used in Diamond
            ComprehensiveTheftTotal = 131, // not used in Diamond
            ComprehensiveTowing = 132, // not used in Diamond
            ComprehensiveVandalismAndMischief = 133, // not used in Diamond
            ComprehensiveWind = 134, // not used in Diamond
            ComputerFraud = 161, // not used in Diamond
            Contamination = 143, // not used in Diamond
            ContaminationPollution = 92, // not used in Diamond
            Conversion = 162,
            CreditCard = 93, // not used in Diamond
            DamageToPropertyOfOthers = 94, // not used in Diamond
            DeathAndDisabilityOrFuneral = 135, // not used in Diamond
            DestructionOfDataOrPrograms = 164, // not used in Diamond
            DogBiteLiability = 95, // not used in Diamond
            EarthMovement = 96, // not used in Diamond
            Earthquake = 63,
            EmployeeTheft = 160, // not used in Diamond
            EnvironmentalSpill = 136,
            EnvironmentalMiscellaneous = 46, // just Environmental in Diamond
            EnvironmentalMold = 44, // not used in Diamond
            EnvironmentalOther = 43, // not used in Diamond
            EnvironmentalPetroleum = 45, // not used in Diamond
            EquipmentBreakdown = 47, // not used in Diamond; use 166 or 248
            Explosion = 48, // not used in Diamond; use 167 or 197
            ExtendedCoveragePerils = 97, // not used in Diamond
            FalseAlarm = 66,
            Fire = 37,
            FireAutoPAPDOrOTC = 84, // not used in Diamond
            Flood = 98, // not used in Diamond; use 200
            Forgery = 142,
            ForgeryOrAlteration = 158, // not used in Diamond
            FreezingWaterAndSubsequentWater = 99, // not used in Diamond
            GlassBreakage = 50, // not used in Diamond; use 168 or 249
            Hail = 39,
            IdentityTheft = 51, // not used in Diamond; use 169
            InlandMarine = 52, // not used in Diamond
            IntakeofForeignObject = 100, // not used in Diamond; use 215 (IntakeOfForeignObjects)
            Landslide = 62,
            LiabilityAllOther = 101, // not used in Diamond
            LiabilityAbuseOrMolestation = 147, // not used in Diamond
            LiabilityAdvertising = 153,
            LiabilityAnimalOrLivestock = 81, // not used in Diamond; use 170
            LiabilityBodilyInjury = 69, // not used in Diamond
            LiabilityBusinessPursuits = 73,
            LiabilityCollision = 74, // not used in Diamond
            LiabilityDogBite = 71, // not used in Diamond; use LiabilityPetBite (171 or 239)
            LiabilityEnvironmental = 75, // not used in Diamond; use 172
            LiabilityEnvironmentalMiscellaneous = 79, // not used in Diamond
            LiabilityEnvironmentalMold = 76, // not used in Diamond
            LiabilityEnvironmentalPetroleum = 77, // not used in Diamond
            LiabilityEnvironmentalSolidWaste = 78, // not used in Diamond
            LiabilityErrorsOrOmissions = 148, // not used in Diamond
            LiabilityGlass = 80, // not used in Diamond
            LiabilityMedPay = 68, // not used in Diamond
            LiabilityMiscellaneous = 82, // not used in Diamond
            LiabilityOtherAnimal = 154, // not used in Diamond
            LiabilityOverspray = 149, // not used in Diamond
            LiabilityOwnersProtectiveLiability = 155, // not used in Diamond
            LiabilityPersonal = 156, // not used in Diamond
            LiabilityPersonalTheft = 83, // not used in Diamond
            LiabilityPropertyDamage = 70, // not used in Diamond
            LiabilitySlipandFall = 72, // not used in Diamond; use 174
            LiabilityWatercraft = 150,
            Lightning = 53,
            Livestock = 102, // not used in Diamond
            LossAdjustmentExpense = 103, // not used in Diamond
            MedicalPayments = 104, // not used in Diamond
            MineSubsidence = 54, // not used in Diamond; use 175 or 250
            Miscellaneous = 55, // not used in Diamond
            Mold = 105, // not used in Diamond
            MysteriousDisappearance = 67,
            MysteriousDisappearanceInvolvingScheduledProperty = 106, // not used in Diamond
            NotAvailable = 0,
            PowerInterruptionOffPremise = 58, // not used in Diamond
            PowerInterruptionOnPremise = 57, // not used in Diamond
            RiotOrCivilCommotion = 145, // not used in Diamond; use 183 or 205
            Sinkhole = 64,
            SlipOrFallLiability = 107, // not used in Diamond
            Smoke = 40,
            SprinklerLeakage = 146, // not used in Diamond
            Suffocation = 151, // not used in Diamond
            TheftOther = 49, // just Theft in Diamond
            TheftofMoney = 56, // not used in Diamond
            TheftofMoneyandSecurities = 159, // not used in Diamond
            TheftInvolvingScheduledProperty = 108, // not used in Diamond
            TheftAutoPAPDOrOTC = 85, // not used in Diamond
            TheftOrBurglary = 109, // not used in Diamond
            UnauthorizedReproduction = 163, // not used in Diamond
            UninsuredMotoristBodilyInjury = 137, // not used in Diamond
            UninsuredMotoristPropertyDamage = 138, // not used in Diamond
            VandalismandMaliciousMischief = 59,
            VehicleDamage = 60, // not used in Diamond; use 176
            WaterDamage = 61, // not used in Diamond; could use WaterDamageOther (177)
            Watercraft = 110, // not used in Diamond
            WeatherRelatedWaterDamage = 111, // not used in Diamond; could use WaterDamageWindDriven (181)
            WindOrTornado = 38,
            WorkersCompensation = 112, // not used in Diamond; use 218 or 231, also WorkComp (232)

            // updated 8/3/2017 w/ missing values
            Aircraft = 184,
            AnimalCollision = 195,
            AnimalDamage2 = 165,
            AnimalDamage3 = 254,
            // BackupOfSewerOrDrain = 41
            BOP = 229,
            Burglary = 245,
            CGL = 230,
            // Collapse = 42
            Collapse2 = 247,
            CollisionUpsetOrOverturn = 213,
            CollisionUpsetOrOverturn2 = 258,
            CommercialAuto = 227,
            CommercialGarage = 228,
            // Conversion = 162
            DebrisRemoval = 187,
            DebrisRemoval2 = 257,
            DwellingFire = 225,
            // Earthquake = 63
            Earthquake2 = 196,
            EmployeeDishonesty = 246,
            EmployersLiability = 221,
            EmployersLiability2 = 233,
            Environmental = 46,
            // EnvironmentalSpill = 136
            EquipmentBreakdown2 = 166,
            EquipmentBreakdown3 = 248,
            Explosion2 = 167,
            Explosion3 = 197,
            FallingObjects = 185,
            FallingObjects2 = 255,
            FallingObjectsOrMissiles = 198,
            // FalseAlarm = 66
            FalseAlarm2 = 261,
            FamilyMedical = 220,
            Farmowners = 226,
            // Fire = 37
            Fire2 = 199,
            Flood2 = 200,
            FloodMachineryAndImplements = 214,
            // Forgery = 142
            Freezing = 182,
            GarageKeepersLiability = 235,
            GlassBreakage2 = 168,
            GlassBreakage3 = 249,
            GlassRepair = 201,
            GlassReplace = 202,
            // Hail = 39
            Hail2 = 203,
            Homeowners = 224,
            IdentityTheft2 = 169,
            IntakeOfForeignObjects = 215,
            // Landslide = 62
            // LiabilityAdvertising = 153
            LiabilityAnimalOrLivestock2 = 170,
            // LiabilityBusinessPursuits = 73
            LiabilityCompletedOps = 241,
            LiabilityDamageToPropertyOfOthers = 173,
            LiabilityDamageToPropertyOfOthers2 = 260,
            LiabilityEnvironmental2 = 172,
            LiabilityFallingObjects = 243,
            LiabilityIntellectualProperty = 238,
            LiabilityLoadingOrUnloading = 236,
            LiabilityMiscVehicle = 188,
            LiabilityNOC = 189,
            LiabilityPersonalInjury = 192,
            LiabilityPersonalInjury2 = 242,
            LiabilityPetBite = 171,
            LiabilityPetBite2 = 239,
            LiabilityProducts = 240,
            LiabilityProductsOrCompletedOps = 237,
            LiabilityServingOfAlcohol = 191,
            LiabilitySlipAndFall2 = 174,
            LiabilityTrampoline = 190,
            // LiabilityWatercraft = 150
            // Lightning = 53
            LivestockOther = 216,
            LivestockCollision = 217,
            LoadingOrUnloading = 234,
            LostStone = 222,
            MineSubsidence2 = 175,
            MineSubsidence3 = 250,
            MultiVehicleAccident = 193,
            // MysteriousDisappearance = 67
            NonMovingVehicle = 212,
            OtherNOC = 178,
            OtherNOC2 = 211,
            OtherNOC3 = 251,
            PersonalAuto = 223,
            PowerInterruption = 179,
            PowerInterruption2 = 252,
            RiotOrCivilCommotion2 = 183,
            RiotOrCivilCommotion3 = 205,
            RoadbedCollision = 219,
            RoadbedCollision2 = 259,
            Robbery = 244,
            SingleVehicleAccident = 194,
            // Sinkhole = 64
            // Smoke = 40
            Theft = 49,
            TheftPartsOrContents = 206,
            TheftTotalVehicle = 207,
            TowingAndLabor = 208,
            UnderinsuredMotorist = 262,
            VandalismAndMischief = 209,
            // VandalismAndMaliciousMischief = 59
            VehicleDamage2 = 176,
            Water = 204,
            WaterDamageAppliance = 180,
            WaterDamageOther = 177,
            WaterDamageWindDriven = 181,
            WeightOfIceSnowOrSleet = 186,
            WeightOfIceSnowOrSleet2 = 256,
            Wind = 210,
            // WindOrTornado = 38
            WorkComp = 232,
            WorkersCompensation2 = 218,
            WorkersCompensation3 = 231
        }
       public enum ClaimLossType_AllValid
        {
            Aircraft = 184,
            AnimalCollision = 195,
            AnimalDamage = 165,
            AnimalDamage2 = 254,
            BackupOfSewerOrDrain = 41,
            BOP = 229,
            Burglary = 245,
            CGL = 230,
            Collapse = 42,
            Collapse2 = 247,
            CollisionUpsetOrOverturn = 213,
            CollisionUpsetOrOverturn2 = 258,
            CommercialAuto = 227,
            CommercialGarage = 228,
            Conversion = 162,
            DebrisRemoval = 187,
            DebrisRemoval2 = 257,
            DwellingFire = 225,
            Earthquake = 63,
            Earthquake2 = 196,
            EmployeeDishonesty = 246,
            EmployersLiability = 221,
            EmployersLiability2 = 233,
            Environmental = 46,
            EnvironmentalSpill = 136,
            EquipmentBreakdown = 166,
            EquipmentBreakdown2 = 248,
            Explosion = 167,
            Explosion2 = 197,
            FallingObjects = 185,
            FallingObjects2 = 255,
            FallingObjectsOrMissiles = 198,
            FalseAlarm = 66,
            FalseAlarm2 = 261,
            FamilyMedical = 220,
            Farmowners = 226,
            Fire = 37,
            Fire2 = 199,
            Flood = 200,
            FloodMachineryAndImplements = 214,
            Forgery = 142,
            Freezing = 182,
            GarageKeepersLiability = 235,
            GlassBreakage = 168,
            GlassBreakage2 = 249,
            GlassRepair = 201,
            GlassReplace = 202,
            Hail = 39,
            Hail2 = 203,
            Homeowners = 224,
            IdentityTheft = 169,
            IntakeOfForeignObjects = 215,
            Landslide = 62,
            LiabilityAdvertising = 153,
            LiabilityAnimalOrLivestock = 170,
            LiabilityBusinessPursuits = 73,
            LiabilityCompletedOps = 241,
            LiabilityDamageToPropertyOfOthers = 173,
            LiabilityDamageToPropertyOfOthers2 = 260,
            LiabilityEnvironmental = 172,
            LiabilityFallingObjects = 243,
            LiabilityIntellectualProperty = 238,
            LiabilityLoadingOrUnloading = 236,
            LiabilityMiscVehicle = 188,
            LiabilityNOC = 189,
            LiabilityPersonalInjury = 192,
            LiabilityPersonalInjury2 = 242,
            LiabilityPetBite = 171,
            LiabilityPetBite2 = 239,
            LiabilityProducts = 240,
            LiabilityProductsOrCompletedOps = 237,
            LiabilityServingOfAlcohol = 191,
            LiabilitySlipAndFall = 174,
            LiabilityTrampoline = 190,
            LiabilityWatercraft = 150,
            Lightning = 53,
            LivestockOther = 216,
            LivestockCollision = 217,
            LoadingOrUnloading = 234,
            LostStone = 222,
            MineSubsidence = 175,
            MineSubsidence2 = 250,
            MultiVehicleAccident = 193,
            MysteriousDisappearance = 67,
            NonMovingVehicle = 212,
            OtherNOC = 178,
            OtherNOC2 = 211,
            OtherNOC3 = 251,
            PersonalAuto = 223,
            PowerInterruption = 179,
            PowerInterruption2 = 252,
            RiotOrCivilCommotion = 183,
            RiotOrCivilCommotion2 = 205,
            RoadbedCollision = 219,
            RoadbedCollision2 = 259,
            Robbery = 244,
            SingleVehicleAccident = 194,
            Sinkhole = 64,
            Smoke = 40,
            Theft = 49,
            TheftPartsOrContents = 206,
            TheftTotalVehicle = 207,
            TowingAndLabor = 208,
            UnderinsuredMotorist = 262,
            VandalismAndMischief = 209,
            VandalismAndMaliciousMischief = 59,
            VehicleDamage = 176,
            Water = 204,
            WaterDamageAppliance = 180,
            WaterDamageOther = 177,
            WaterDamageWindDriven = 181,
            WeightOfIceSnowOrSleet = 186,
            WeightOfIceSnowOrSleet2 = 256,
            Wind = 210,
            WindOrTornado = 38,
            WorkComp = 232,
            WorkersCompensation = 218,
            WorkersCompensation2 = 231
        }
       public enum PolicyLobType
        {
            AutoPersonal = 1,
            CommercialAuto = 20,
            CommercialBOP = 25,
            CommercialCrime = 26,
            CommercialGarage = 24,
            CommercialGeneralLiability = 9,
            CommercialInlandMarine = 29,
            CommercialPackage = 23,
            CommercialProperty = 28,
            CommercialUmbrella = 27,
            DwellingFirePersonal = 3,
            Farm = 17,
            HomePersonal = 2,
            InlandMarinePersonal = 16,
            NotAssigned = 0,
            UmbrellaPersonal = 14,
            WorkersComp = 21
        }
      public enum ClaimLossType_All
        {
            AccidentalDischargeOrLeakage = 87,
            Aircraft = 184,
            AircraftDamage = 144,
            AllOther = 88,
            AllOtherPhysicalDamage = 89,
            Alteration = 157,
            AnimalCollision = 195,
            AnimalDamage = 65,
            AnimalDamage2 = 165,
            AnimalDamage3 = 254,
            ApplianceRelatedWaterDamage = 90,
            BackupOfSewerOrDrain = 41,
            BodilyInjuryAutoBI = 152,
            BOP = 229,
            BroadFormPerilsAutoPAPDOrOTC = 86,
            Burglary = 245,
            CGL = 230,
            Collapse = 42,
            Collapse2 = 247,
            CollisionMultipleAutos = 113,
            CollisionOther = 114,
            CollisionSingleAuto = 115,
            CollisionUpsetOrOverturn = 91,
            CollisionUpsetOrOverturn2 = 213,
            CollisionUpsetOrOverturn3 = 258,
            CommercialAuto = 227,
            CommercialGarage = 228,
            ComprehensiveAnimal = 116,
            ComprehensiveEarthquake = 117,
            ComprehensiveExplosion = 118,
            ComprehensiveFallingObjectsOrMissiles = 119,
            ComprehensiveFire = 120,
            ComprehensiveFlood = 121,
            ComprehensiveGlassRepair = 122,
            ComprehensiveGlassReplace = 123,
            ComprehensiveHail = 124,
            ComprehensiveLabor = 125,
            ComprehensiveOther = 126,
            ComprehensiveOtherTransportation = 127,
            ComprehensiveOtherWater = 128,
            ComprehensiveRiotOrCivilCommotion = 129,
            ComprehensiveTheftPartial = 130,
            ComprehensiveTheftTotal = 131,
            ComprehensiveTowing = 132,
            ComprehensiveVandalismAndMischief = 133,
            ComprehensiveWind = 134,
            ComputerFraud = 161,
            Contamination = 143,
            ContaminationOrPollution = 92,
            Conversion = 162,
            CreditCard = 93,
            DamageToPropertyOfOthers = 94,
            DeathAndDisabilityOrFuneral = 135,
            DebrisRemoval = 187,
            DebrisRemoval2 = 257,
            DestructionOfDataOrPrograms = 164,
            DogBiteLiability = 95,
            DwellingFire = 225,
            Earthmovement = 96,
            Earthquake = 63,
            Earthquake2 = 196,
            EmployeeDishonesty = 246,
            EmployeeTheft = 160,
            EmployersLiability = 221,
            EmployersLiability2 = 233,
            Environmental = 46,
            EnvironmentalMiscellaneous = 139,
            EnvironmentalSpill = 136,
            EnvironmentalMold = 44,
            EnvironmentalOther = 43,
            EnvironmentalPetroleum = 45,
            EquipmentBreakdown = 47,
            EquipmentBreakdown2 = 166,
            EquipmentBreakdown3 = 248,
            Explosion = 48,
            Explosion2 = 167,
            Explosion3 = 197,
            ExtendedCoveragePerils = 97,
            FallingObjects = 185,
            FallingObjects2 = 255,
            FallingObjectsOrMissiles = 198,
            FalseAlarm = 66,
            FalseAlarm2 = 261,
            FamilyMedical = 220,
            Farmowners = 226,
            Fire = 37,
            Fire2 = 199,
            FireAutoPAPDOrOTC = 84,
            Flood = 98,
            Flood2 = 200,
            FloodMachineryAndImplements = 214,
            Forgery = 142,
            ForgeryOrAlteration = 158,
            Freezing = 182,
            FreezingWaterAndSubsequentWater = 99,
            GarageKeepersLiability = 235,
            GlassBreakage = 50,
            GlassBreakage2 = 168,
            GlassBreakage3 = 249,
            GlassRepair = 201,
            GlassReplace = 202,
            Hail = 39,
            Hail2 = 203,
            Homeowners = 224,
            IdentityTheft = 51,
            IdentityTheft2 = 169,
            InlandMarine = 52,
            IntakeOfForeignObject = 100,
            IntakeOfForeignObjects = 215,
            Landslide = 62,
            LiabilityAllOther = 101,
            LiabilityAbuseOrMolestation = 147,
            LiabilityAdvertising = 153,
            LiabilityAnimalOrLivestock = 81,
            LiabilityAnimalOrLivestock2 = 170,
            LiabilityBodilyInjury = 69,
            LiabilityBusinessPursuits = 73,
            LiabilityCollision = 74,
            LiabilityCompletedOps = 241,
            LiabilityDamageToPropertyOfOthers = 173,
            LiabilityDamageToPropertyOfOthers2 = 260,
            LiabilityDogBite = 71,
            LiabilityEnvironmental = 75,
            LiabilityEnvironmental2 = 172,
            LiabilityEnvironmentalMiscellaneous = 79,
            LiabilityEnvironmentalMold = 76,
            LiabilityEnvironmentalPetroleum = 77,
            LiabilityEnvironmentalSolidWaste = 78,
            LiabilityErrorsOrOmissions = 148,
            LiabilityFallingObjects = 243,
            LiabilityGlass = 80,
            LiabilityIntellectualProperty = 238,
            LiabilityLoadingOrUnloading = 236,
            LiabilityMedPay = 68,
            LiabilityMiscVehicle = 188,
            LiabilityMiscellaneous = 82,
            LiabilityNOC = 189,
            LiabilityOtherAnimal = 154,
            LiabilityOverspray = 149,
            LiabilityOwnersProtectiveLiability = 155,
            LiabilityPersonal = 156,
            LiabilityPersonalInjury = 192,
            LiabilityPersonalInjury2 = 242,
            LiabilityPersonalTheft = 83,
            LiabilityPetBite = 171,
            LiabilityPetBite2 = 239,
            LiabilityProducts = 240,
            LiabilityProductsOrCompletedOps = 237,
            LiabilityPropertyDamage = 70,
            LiabilityServingOfAlcohol = 191,
            LiabilitySlipAndFall = 72,
            LiabilitySlipAndFall2 = 174,
            LiabilityTrampoline = 190,
            LiabilityWatercraft = 150,
            LiabilityBodilyInjury2 = 140,
            LiabilityPropertyDamage2 = 141,
            Lightning = 53,
            Livestock = 102,
            LivestockOther = 216,
            LivestockCollision = 217,
            LoadingOrUnloading = 234,
            LossAdjustmentExpense = 103,
            LostStone = 222,
            MedicalPayments = 104,
            MineSubsidence = 54,
            MineSubsidence2 = 175,
            MineSubsidence3 = 250,
            Miscellaneous = 55,
            Mold = 105,
            MultiVehicleAccident = 193,
            MysteriousDisappearance = 67,
            MysteriousDisappearanceInvolvingScheduledProperty = 106,
            NOrA = 0,
            NonMovingVehicle = 212,
            OtherNOC = 178,
            OtherNOC2 = 211,
            OtherNOC3 = 251,
            PersonalAuto = 223,
            PowerInterruption = 179,
            PowerInterruption2 = 252,
            PowerInterruptionOffPremise = 58,
            PowerInterruptionOnPremise = 57,
            RiotOrCivilCommotion = 183,
            RiotOrCivilCommotion2 = 145,
            RiotOrCivilCommotion3 = 205,
            RoadbedCollision = 219,
            RoadbedCollision2 = 259,
            Robbery = 244,
            SingleVehicleAccident = 194,
            Sinkhole = 64,
            SlipOrFallLiability = 107,
            Smoke = 40,
            SprinklerLeakage = 146,
            Suffocation = 151,
            Theft = 49,
            TheftPartsOrContents = 206,
            TheftTotalVehicle = 207,
            TheftOfMoney = 56,
            TheftOfMoneyAndSecurities = 159,
            TheftOrInvolvingScheduledProperty = 108,
            TheftAutoPAPDOrOTC = 85,
            TheftOrBurglary = 109,
            TowingAndLabor = 208,
            UnauthorizedReproduction = 163,
            UnderinsuredMotorist = 262,
            UninsuredMotoristBodilyInjury = 137,
            UninsuredMotoristPropertyDamage = 138,
            VandalismAndMischief = 209,
            VandalismAndMaliciousMischief = 59,
            VehicleDamage = 60,
            VehicleDamage2 = 176,
            Water = 204,
            WaterDamage = 61,
            WaterDamageAppliance = 180,
            WaterDamageOther = 177,
            WaterDamageWindDriven = 181,
            Watercraft = 110,
            WeatherRelatedWaterDamage = 111,
            WeightOfIceSnowOrSleet = 186,
            WeightOfIceSnowOrSleet2 = 256,
            Wind = 210,
            WindOrTornado = 38,
            WorkComp = 232,
            WorkersCompensation = 218,
            WorkersCompensation2 = 231,
            WorkersCompensation3 = 112
        }

        public enum ClaimFaultType
        {
            NA = 0,
            AtFault = 1,
            ComparativeFaultInsdlessthan50percent = 2,
            ComparativeFaultInsd50percentormore = 3,
            NotAtFault = 4,
            Undetermined = 5
        }

        public enum FNOL_LOB_Enum
        {
            Auto =1,
            CommercialAuto=2,
            Liability=3,
            Property=4,
        }

        public enum PhoneType
        {
            HomePhone,
            BusinessPhone,
            CellPhone,
            FaxPhone
        }
        public enum Insured1orInsured2
        {
            Any = 0,
            Insured1 = 1,
            Insured2 = 2,
            UnknownOnly = 3
        }
        public enum FirstOrLast
        {
            First = 0,
            Last = 1
        }

        public enum FNOL_Type
        {
            NONE = 0,
            AutoFNOL = 1,
            LiabilityFNOL = 2,
            PropertyFNOL = 3
        }
        public enum PersonType_Enum
        {
           Injured=1,
            OtherVehicleOwner=2,
            OtherVehicleDriver=3,
            Witness=4
        }
        public enum AdjusterCountUpdateIndicator
        {
            Increase,
            Decrease
        }

        public enum SeverityLoss
        {
            MinorLoss =1,
            ModerateLoss =2,
            SevereLoss =3,
            MinorInjury =4,
            ModerateInjury =5,
            SevereInjury =6,
            Fatality =7,
            other=8
        }

    }
}