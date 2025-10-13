using IFM.PrimativeExtensions;
using QuickQuote.CommonObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class Optional_Coverages_Hom_Validator
    {
        public const string ValidationListID = "{32EE4A3D-9FF9-4BDB-8B74-AEBBF04371F1}";
        public const string QuoteIsNull = "{4302F5D2-EE01-4858-A93A-9A1B26DD284E}";
        public const string UnExpectedLobType = "{E91734F6-30A0-4A19-859F-67A5503817B2}";
        public const string NoLocation = "{969D514A-85DE-4352-AF6C-CDE29BA9FCC8}";

        // Inflation Guard
        public const string InflationGuard = "{16F3E4A0-B6BC-48EE-A7EC-F10CF2F25EE6}";

        public const string BackupSewDrain = "{16F3E4A0-B6BC-48EE-A7EC-F10CYUF25EH6}";

        public const string MineSubsidenceConflict = "{2240876D-E93B-47FA-A069-7683CD239E41}";

        public const string CovASpecificed_ACVConflic = "{4129D8F0-805E-448D-A60B-6914BFA52C48}";

        //HOM 2011 Upgrade
        public const string WaterDamage = "{C0C61A42-4CA6-4DF4-B091-8B375EEBABAF}"; //Added 1/16/18 for HOM Upgrade MLW
        public const string IdentityFraudHOM = "{8B34ED75-CC94-47A6-95B1-4184708BAD70}"; //Added 1/17/18 for HOM Upgrade MLW
        public const string HomePlusEnhance = "{2DB4C4A0-07BD-492C-81AD-7E491EA33F77}"; //Added 1/17/18 for HOM Upgrade MLW
        public const string SpecialCoverageConflict = "{6F371498-8BEF-4ACE-9632-B26827552FAA}"; //Added 1/17/18 for HOM Upgrade MLW
        public const string GreenUpgrades = "{8A35040C-42DA-43ED-A560-EC4BDC998EDB}"; //Added 1/26/18 for HOM Upgrade MLW

        public static string GetHomeVersion(QuickQuote.CommonObjects.QuickQuoteObject quote)
        {
            QuickQuote.CommonMethods.QuickQuoteHelperClass qqh = new QuickQuote.CommonMethods.QuickQuoteHelperClass();
            DateTime effectiveDate = DateTime.Today;
            string eDate = "";
            string HomeVersion = "";
            if (quote != null)
            {
                if (quote.EffectiveDate != null && quote.EffectiveDate != "")
                {
                    effectiveDate = Convert.ToDateTime(quote.EffectiveDate);
                }
                eDate = Convert.ToString(effectiveDate);
                if (qqh.doUseNewVersionOfLOB(quote, QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, Convert.ToDateTime("7/1/2018")))
                {
                    HomeVersion = "After20180701";
                }
                else
                {
                    HomeVersion = "Before20180701";
                }
            }
            return HomeVersion;
        }

        public static Validation.ObjectValidation.ValidationItemList ValidateHOMOptCoverages(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            DateTime CyberEffDate = DateTime.Now;
            if (!System.Configuration.ConfigurationManager.AppSettings["VR_Home_Cyber_EffDate"].Equals(null)) {
                CyberEffDate = Convert.ToDateTime(System.Configuration.ConfigurationManager.AppSettings["VR_Home_Cyber_EffDate"]);
            }
            else
            {
                CyberEffDate = Convert.ToDateTime("9/1/2020");
            }

            //Added 5/24/2022 for task 74106 MLW
            DateTime HPEEWaterBackupEffDate = DateTime.Now;
            if (!System.Configuration.ConfigurationManager.AppSettings["VR_HOM_HPEE_WaterBU_EffectiveDate"].Equals(null))
            {
                HPEEWaterBackupEffDate = Convert.ToDateTime(System.Configuration.ConfigurationManager.AppSettings["VR_HOM_HPEE_WaterBU_EffectiveDate"]);
            }
            else
            {
                HPEEWaterBackupEffDate = Convert.ToDateTime("9/1/2022");
            }
            bool HPEEWaterBackupEnabled = false;
            if (!System.Configuration.ConfigurationManager.AppSettings["VR_HOM_HPEE_WaterBU_Enabled"].Equals(null))
            {
                HPEEWaterBackupEnabled = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["VR_HOM_HPEE_WaterBU_Enabled"]);
            }

            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                {
                    if (quote.Locations != null && quote.Locations.Any() && quote.Locations[0] != null)
                    {
                        var MyLocation = quote.Locations[0];
                        string HomeVersion = GetHomeVersion(quote); //Added 1/16/18 for HOM Upgrade MLW

                        if (MyLocation.SectionICoverages != null) {
                            // Backup Sew and Drain - must have Home Owners Enhancement (1010 or 1019) to have this coverage
                            bool hasEnhancement = false;
                            // if (Convert.ToDateTime(quote.EffectiveDate) < CyberEffDate || (MyLocation.FormTypeId == "25" || MyLocation.StructureTypeId == "2" || MyLocation.OccupancyCodeId.EqualsAny("4","5")))

                            // New Logic - Bug 64025
                            // Determine if either of the Home Ownews Enhancements (1010 or 1019) are on the quote
                            hasEnhancement = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.HomeownerEnhancementEndorsement).Any();
                            if (!hasEnhancement)
                            {
                                hasEnhancement = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.HomeownerEnhancementEndorsement1019).Any();
                            }

                            // Old Logic - Bug 64025
                            //if (Convert.ToDateTime(quote.EffectiveDate) < CyberEffDate || (MyLocation.FormTypeId == "25" || MyLocation.StructureTypeId == "2" ))
                            //    {
                            //    hasEnhancement = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.HomeownerEnhancementEndorsement).Any();
                            //}
                            //else
                            //{
                            //    hasEnhancement = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.HomeownerEnhancementEndorsement1019).Any();
                            //}
                            var hasBackup = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.BackupSewersAndDrains).Any();

                            //Updated 5/25/2022 for task 74106 MLW
                            if (!HPEEWaterBackupEnabled || Convert.ToDateTime(quote.EffectiveDate) < HPEEWaterBackupEffDate) 
                            { 
                                if (hasBackup && hasEnhancement == false)
                                    if (HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                    {
                                        valList.Add(new ObjectValidation.ValidationItem("Water Backup is only available with the HomeOwners Enhancement.", BackupSewDrain, false));
                                    }
                                    else
                                    {
                                        valList.Add(new ObjectValidation.ValidationItem("Backup Sew and Drain is only available with the HomeOwners Enhancement.", BackupSewDrain, false));
                                    }
                            }
                            //if (hasBackup && hasEnhancement == false)
                            //    if (HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                            //    {
                            //        valList.Add(new ObjectValidation.ValidationItem("Water Backup is only available with the HomeOwners Enhancement.", BackupSewDrain, false));
                            //    }
                            //    else
                            //    {
                            //        valList.Add(new ObjectValidation.ValidationItem("Backup Sew and Drain is only available with the HomeOwners Enhancement.", BackupSewDrain, false));
                            //    }
                            if (hasBackup == false && hasEnhancement)
                                if (HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                {
                                    valList.Add(new ObjectValidation.ValidationItem("HomeOwners Enhancement is required to have Water Backup coverage.", BackupSewDrain, false));
                                }
                                else
                                {
                                    valList.Add(new ObjectValidation.ValidationItem("HomeOwners Enhancement is required to have Backup Sew and Drain coverage.", BackupSewDrain, false));
                                }

                            //Added 1/16/18 for HOM Upgrade MLW
                            if (HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                            {
                                // Water Damage - must have Home Owners Plus Enhancement (1017 or 1020) to have this coverage
                                bool hasPlusEnhancement = false;
                                // New Logic - Bug 64025
                                hasPlusEnhancement = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.HomeownersPlusEnhancementEndorsement).Any();
                                if (!hasPlusEnhancement)
                                {
                                    hasPlusEnhancement = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.HomeownersPlusEnhancementEndorsement1020).Any();
                                }
                                // Old Logic - Bug 64025
                                //if (Convert.ToDateTime(quote.EffectiveDate) < CyberEffDate || (MyLocation.FormTypeId == "25" || MyLocation.StructureTypeId == "2" || MyLocation.OccupancyCodeId.EqualsAny("4","5")))
                                //{
                                //    hasPlusEnhancement = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.HomeownersPlusEnhancementEndorsement).Any();
                                //}
                                //else
                                //{
                                //    hasPlusEnhancement = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.HomeownersPlusEnhancementEndorsement1020).Any();
                                //}
                                var hasWaterDamage = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.WaterDamage).Any();
                                if (hasWaterDamage && (!hasPlusEnhancement))
                                    valList.Add(new ObjectValidation.ValidationItem("Water Damage is only available with the Homeowners Plus Enhancement.", WaterDamage, false));

                                //if (hasWaterDamage == false && !hasPlusEnhancement)
                                //    valList.Add(new ObjectValidation.ValidationItem("HomeOwners Plus Enhancement is required to have Water Damage coverage.", WaterDamage, false));

                                //cannot have both homeowner enhancement and homeowner plus enhancement
                                if (hasEnhancement && hasPlusEnhancement)
                                    valList.Add(new ObjectValidation.ValidationItem("Having both Homeowners Enhancement and Homeowners Plus Enhancement at the same time is invalid.", HomePlusEnhance, false));
                            
                                var hasIdentityFraud = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.IdentityFraudExpenseHOM0455).Any();
                                //cannot have both homeowner plus enhancement and identity fraud expense
                                if (hasIdentityFraud && hasPlusEnhancement)
                                    valList.Add(new ObjectValidation.ValidationItem("Having both Homeowners Plus Enhancement and Identity Fraud Expense at the same time is invalid.", IdentityFraudHOM, false));
 
                                //cannot have both homeowner enhancement and identity fraud expense
                                if (hasIdentityFraud && hasEnhancement)
                                    valList.Add(new ObjectValidation.ValidationItem("Having both Homeowners Enhancement and Identity Fraud Expense at the same time is invalid.", IdentityFraudHOM, false));

                                //Added 1/17/18 for HOM Upgrade MLW
                                if (MyLocation.FormTypeId == "25")
                                {
                                    if (MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.SpecialComputerCoverage).Any() && MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.SpecialPersonalProperty).Any())
                                    {
                                        // you cannot have both Special Computer Coverage and Special Personal Property Coverage at the same time
                                        valList.Add(new ObjectValidation.ValidationItem("Having both Special Computer Coverage and Special Personal Property Coverage at the same time is invalid.", SpecialCoverageConflict));
                                    }

                                    //this doesn't allow Other Structures on the Residence Premises alone - need code like homeowner enhancement
                                    //if (MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB).Any() && MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.Cov_B_Related_Private_Structures).Any())
                                    //{
                                    //    // you cannot have both Mine Subsidence A and B and Other Structures on the Residence Premises at the same time
                                    //    valList.Add(new ObjectValidation.ValidationItem("Having both Mine Subsidence A and B and Other Structures on the Residence Premises at the same time is invalid.", MineSubsidenceConflict));
                                    //}
                                    var hasMineAandB = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB).Any();
                                    //Updated 5/23/18 for Bugs 26818 and 26819 - Coverage code changed from 70303 OtherStructuresOnTheResidencePremises to 70064 Cov_B_Related_Private_Structures MLW
                                    //var hasOtherStructureOn = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.OtherStructuresOnTheResidencePremises).Any();
                                    var hasOtherStructureOn = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.Cov_B_Related_Private_Structures).Any();
                                    if (hasMineAandB && hasOtherStructureOn == false)
                                        valList.Add(new ObjectValidation.ValidationItem("Other Structures on the Residence Premises is required with Mine Subsidence A and B.", MineSubsidenceConflict, false));
                                }
                                //Added 1/17/18 for HOM Upgrade MLW
                                if (MyLocation.FormTypeId.EqualsAny("23", "24", "26") || (MyLocation.FormTypeId == "22" && MyLocation.StructureTypeId != "2"))
                                {
                                    // Green Upgrades only allowed when Personal Property Replacement Cost is selected
                                    var hasPersPropReplCost = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.PersonalPropertyReplacement).Any();
                                    var hasGreenUpgrades = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.GreenUpgrades).Any();
                                    if (hasGreenUpgrades && hasPersPropReplCost == false)
                                        valList.Add(new ObjectValidation.ValidationItem("Green Upgrades are only available with Personal Property Replacement Cost.", GreenUpgrades, false));
                                }
                            }

                            if (MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA).Any() && MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB).Any())
                            {
                                // you can't have both at the same time
                                valList.Add(new ObjectValidation.ValidationItem("Having both Mine Subsidence A and Mine Subsidence A and B at the same time is invalid.", MineSubsidenceConflict));
                            }

                            if (MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlement).Any() && MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.Home_CoverageASpecialCoverage).Any())
                            {
                                // you can't have Cov A - Specified Additional and ACV Loss Settlement at the same time
                                valList.Add(new ObjectValidation.ValidationItem("Cov.A - Specified Additional Amount of Insurance is not available with Actual Cash Value Loss Settlement.", CovASpecificed_ACVConflic, true));
                            }

                            if (valType != ValidationItem.ValidationType.issuance)
                            {
                                // Inflation Guard removal warning
                                List<QuickQuoteSectionICoverage> acv = new List<QuickQuoteSectionICoverage>();
                                List<QuickQuoteSectionICoverage> windHail = new List<QuickQuoteSectionICoverage>();

                                acv = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlement);
                                windHail = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing);

                                var InflationGaurdCov = MyLocation.SectionICoverages.FindAll(s => s.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.Inflation_Guard);

                                if (HomeVersion != "After20180701" && MyLocation.FormTypeId.NotEqualsAny("22", "23", "24", "25", "26"))                               
                                {
                                    if (acv.Count > 0 || windHail.Count > 0 && InflationGaurdCov.Any())
                                        valList.Add(new ObjectValidation.ValidationItem("Inflation Guard is not available with Actual Cash Value Loss Settlement. Coverage removed.", InflationGuard, true));
                                }
                            }

                        }
                    }
                    else
                    {
                        valList.Add(new ObjectValidation.ValidationItem("No Location", NoLocation));
                    }
                }
                else
                {
                    valList.Add(new ObjectValidation.ValidationItem("Invalid LOB type", UnExpectedLobType));
                }
            }
            else
            {
                valList.Add(new ObjectValidation.ValidationItem("Quote is null", QuoteIsNull));
            }
            return valList;
        }
    }
}