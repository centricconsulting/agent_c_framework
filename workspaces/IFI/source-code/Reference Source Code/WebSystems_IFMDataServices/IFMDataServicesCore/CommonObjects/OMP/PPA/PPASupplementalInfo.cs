using System;
using System.Collections.Generic;
using DCO = Diamond.Common.Objects;

#if DEBUG

using System.Diagnostics;

#endif

using System.Linq;
using System.Web.UI.WebControls;

namespace IFM.DataServicesCore.CommonObjects.OMP.PPA
{
    [System.Serializable]
    public class PPASupplementalInfo : ModelBase
    {
        public bool HasAutoPlusEnhancementCoverage { get; set; }
        public bool HasAutoEnhancementCoverage { get; set; }
        public List<Driver> Drivers { get; set; }
        public List<Vehicle> Vehicles { get; set; }
        public List<PrintForm> IdCards { get; set; } = new List<PrintForm>();
        public PPASupplementalInfo() { }
        internal PPASupplementalInfo(BasicPolicyInformation pol, DCO.Policy.Image image, bool isMutliState)
        {
            if (!isMutliState)
            {
                if (image?.LOB != null)
                {
                    var riskLevel = image.LOB.RiskLevel;
                    var policyLevel = image.LOB.PolicyLevel;
                    if (riskLevel != null)
                    {
                        if (riskLevel.Drivers != null)
                        {
                            Drivers = new List<Driver>();
                            foreach (var d in riskLevel.Drivers)
                            {
                                Drivers.Add(new Driver(d));
                            }
                        }
                        else
                        {
#if !DEBUG
                global::IFM.IFMErrorLogging.LogIssue($"IFMDATASERVICES -> PPASupplementalInfo -> PPASupplementalInfo - Drivers was null. Policy #{((image != null) ? image.PolicyNumber : string.Empty)}");
#else
                            Debugger.Break();
#endif
                        }

                        if (riskLevel.Vehicles != null)
                        {
                            Vehicles = new List<Vehicle>();
                            foreach (var v in riskLevel.Vehicles)
                            {
                                Vehicles.Add(new Vehicle(v));
                            }
                        }
                        else
                        {
#if !DEBUG
                    global::IFM.IFMErrorLogging.LogIssue($"Vehicles was null. Policy #{((image != null) ? image.PolicyNumber : string.Empty)}");
#else
                            Debugger.Break();
#endif
                        }
                    }

                    if (policyLevel != null)
                    {
                        List<int> foundCovs = null;
                        List<int> autoCoverages = new List<int>();
                        autoCoverages.Add(80443); //Auto Plus Enhancement
                        autoCoverages.Add(80094); //Auto Enhancement

                        if (policyLevel.Coverages?.Count > 0)
                        {
                            foundCovs = autoCoverages.FindAll(ac => policyLevel.Coverages.Find(c => c.CoverageCodeId == ac && (c.Checkbox == true || c.WrittenPremium > 0)) != null);
                        }

                        foreach (var cov in foundCovs)
                        {
                            switch (cov)
                            {
                                case 80443:
                                    HasAutoPlusEnhancementCoverage = true;
                                    break;
                                case 80094:
                                    HasAutoEnhancementCoverage = true;
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                Drivers = new List<Driver>();
                Vehicles = new List<Vehicle>();
                foreach (var p in image.PackageParts.Skip(1))
                {
                    if (p?.LOB != null)
                    {
                        var riskLevel = p.LOB.RiskLevel;
                        var policyLevel = p.LOB.PolicyLevel;
                        if (riskLevel != null)
                        {
                            if (riskLevel.Drivers != null)
                            {
                                foreach (var d in riskLevel.Drivers)
                                {
                                    Drivers.Add(new Driver(d));
                                }
                            }
                            else
                            {
#if !DEBUG
                global::IFM.IFMErrorLogging.LogIssue($"IFMDATASERVICES -> PPASupplementalInfo -> PPASupplementalInfo - Drivers was null. Policy #{((image != null) ? image.PolicyNumber : string.Empty)}");
#else
                                Debugger.Break();
#endif
                            }

                            if (riskLevel.Vehicles != null)
                            {

                                foreach (var v in riskLevel.Vehicles)
                                {
                                    Vehicles.Add(new Vehicle(v));
                                }
                            }
                            else
                            {
#if !DEBUG
                    global::IFM.IFMErrorLogging.LogIssue($"Vehicles was null. Policy #{((image != null) ? image.PolicyNumber : string.Empty)}");
#else
                                Debugger.Break();
#endif
                            }
                        }

                        if (policyLevel != null)
                        {
                            List<int> foundCovs = null;
                            List<int> autoCoverages = new List<int>();
                            autoCoverages.Add(80443); //Auto Plus Enhancement
                            autoCoverages.Add(80094); //Auto Enhancement

                            if (policyLevel.Coverages?.Count > 0)
                            {
                                foundCovs = autoCoverages.FindAll(ac => policyLevel.Coverages.Find(c => c.CoverageCodeId == ac && (c.Checkbox == true || c.WrittenPremium > 0)) != null);
                            }

                            foreach (var cov in foundCovs)
                            {
                                switch (cov)
                                {
                                    case 80443:
                                        HasAutoPlusEnhancementCoverage = true;
                                        break;
                                    case 80094:
                                        HasAutoEnhancementCoverage = true;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            if (pol?.PrintFormsHistory != null && pol.PrintFormsHistory.Any())
            {
                //IdCards = (from f in pol.PrintFormsHistory where f.Description.ToLower().Contains("auto id ") select f).ToList();
                foreach (var f in pol.PrintFormsHistory)
                {


                    if (f.Description.ToLower().Contains("auto id ") || f.Description.ToLower().Contains("identification card"))
                    {

                        if (f.UnitDescription.BruteForceInt32() == 0)
                        {
                            foreach (var vehicle in this.Vehicles)
                            {
                                IdCards.Add(new PrintForm()
                                {
                                    PrintRecipientId = f.PrintRecipientId,
                                    PrintDate = f.PrintDate,
                                    Description = f.Description,
                                    PrintXmlId = f.PrintXmlId,
                                    FormNumber = f.FormNumber,
                                    PolicyFormNumber = f.PolicyFormNumber,
                                    PolicyId = f.PolicyId,
                                    PolicyImageNum = f.PolicyImageNum,
                                    PrintJobId = f.PrintJobId,
                                    PrintUrl = f.PrintUrl,
                                    VehicleNum = f.VehicleNum,
                                    UnitDescription = f.UnitDescription,
                                    Make = vehicle.Make,
                                    Model = vehicle.Model,
                                    Year = vehicle.Year
                                });
                            }
                        }
                        else
                        {
                            IdCards.Add(new PrintForm()
                            {
                                PrintRecipientId = f.PrintRecipientId,
                                PrintDate = f.PrintDate,
                                Description = f.Description,
                                PrintXmlId = f.PrintXmlId,
                                FormNumber = f.FormNumber,
                                PolicyFormNumber = f.PolicyFormNumber,
                                PolicyId = f.PolicyId,
                                PolicyImageNum = f.PolicyImageNum,
                                PrintJobId = f.PrintJobId,
                                PrintUrl = f.PrintUrl,
                                VehicleNum = f.VehicleNum,
                                UnitDescription = f.UnitDescription,
                                Make = (from p in this.Vehicles where p.VehicleNum.Equals(f.VehicleNum) select p.Make).FirstOrDefault(),
                                Model = (from p in this.Vehicles where p.VehicleNum.Equals(f.VehicleNum) select p.Model).FirstOrDefault(),
                                Year = (from p in this.Vehicles where p.VehicleNum.Equals(f.VehicleNum) select p.Year).FirstOrDefault()
                            });
                        }
                    }
                }
            }
            else
            {
                if (pol?.PrintForms != null)
                {
                 //   IdCards = (from f in pol.PrintForms where f.Description.ToLower().Contains("auto id ") select f).ToList();
                    foreach (var f in pol.PrintForms)
                    {

                        if (f.Description.ToLower().Contains("auto id ") || f.Description.ToLower().Contains("identification card"))
                        {
                            if (f.UnitDescription.BruteForceInt32() == 0) {
                                foreach (var vehicle in this.Vehicles)
                                {
                                    IdCards.Add(new PrintForm()
                                    {
                                        PrintRecipientId = f.PrintRecipientId,
                                        PrintDate = f.PrintDate,
                                        Description = f.Description,
                                        PrintXmlId = f.PrintXmlId,
                                        FormNumber = f.FormNumber,
                                        PolicyFormNumber = f.PolicyFormNumber,
                                        PolicyId = f.PolicyId,
                                        PolicyImageNum = f.PolicyImageNum,
                                        PrintJobId = f.PrintJobId,
                                        PrintUrl = f.PrintUrl,
                                        VehicleNum = f.VehicleNum,
                                        UnitDescription = f.UnitDescription,
                                        Make = vehicle.Make,
                                        Model = vehicle.Model,
                                        Year = vehicle.Year
                                    });
                                }
                            }
                            else
                            {
                                IdCards.Add(new PrintForm()
                                {
                                    PrintRecipientId = f.PrintRecipientId,
                                    PrintDate = f.PrintDate,
                                    Description = f.Description,
                                    PrintXmlId = f.PrintXmlId,
                                    FormNumber = f.FormNumber,
                                    PolicyFormNumber = f.PolicyFormNumber,
                                    PolicyId = f.PolicyId,
                                    PolicyImageNum = f.PolicyImageNum,
                                    PrintJobId = f.PrintJobId,
                                    PrintUrl = f.PrintUrl,
                                    VehicleNum = f.VehicleNum,
                                    UnitDescription = f.UnitDescription,
                                    Make = (from p in this.Vehicles where p.VehicleNum.Equals(f.VehicleNum) select p.Make).FirstOrDefault(),
                                    Model = (from p in this.Vehicles where p.VehicleNum.Equals(f.VehicleNum) select p.Model).FirstOrDefault(),
                                    Year = (from p in this.Vehicles where p.VehicleNum.Equals(f.VehicleNum) select p.Year).FirstOrDefault()
                                });
                            }


                        }
                    }
                }
                else
                {
#if !DEBUG
                global::IFM.IFMErrorLogging.LogIssue($"Drivers was null. Policy #{((image != null) ? image.PolicyNumber : string.Empty)}");
#else
                    Debugger.Break();
#endif
                }
            }
        }

        internal PPASupplementalInfo(BasicPolicyInformation pol, DCO.Policy.Image image, DCO.Policy.UnderlyingPolicy uPolicy)
        {
            if (uPolicy.PolicyInfos?[0] != null)
            {
                var polInfo = uPolicy.PolicyInfos[0];
                if (polInfo.Drivers != null)
                {
                    Drivers = new List<Driver>();
                    foreach (var d in polInfo.Drivers)
                    {
                        Drivers.Add(new Driver(d));
                    }
                }
                else
                {
#if !DEBUG
                    global::IFM.IFMErrorLogging.LogIssue($"Vehicles was null. Policy #{((image != null) ? image.PolicyNumber : string.Empty)}");
#else
                    Debugger.Break();
#endif
                }

                if (polInfo.Vehicles != null)
                {
                    Vehicles = new List<Vehicle>();
                    foreach (var v in polInfo.Vehicles)
                    {
                        Vehicles.Add(new Vehicle(v));
                    }
                }
                else
                {
#if !DEBUG
                    global::IFM.IFMErrorLogging.LogIssue($"Drivers was null. Policy #{((image != null) ? image.PolicyNumber : string.Empty)}");
#else
                    Debugger.Break();
#endif
                }

                List<int> foundCovs = null;
                List<int> autoCoverages = new List<int>();
                autoCoverages.Add(80443); //Auto Plus Enhancement
                autoCoverages.Add(80094); //Auto Enhancement

                if (polInfo.Coverages?.Count > 0)
                {
                    foundCovs = autoCoverages.FindAll(ac => polInfo.Coverages.Find(c => c.CoverageCodeId == ac && (c.Checkbox == true || c.WrittenPremium > 0)) != null);
                }

                foreach (var cov in foundCovs)
                {
                    switch (cov)
                    {
                        case 80443:
                            HasAutoPlusEnhancementCoverage = true;
                            break;
                        case 80094:
                            HasAutoEnhancementCoverage = true;
                            break;
                    }
                }
            }

            if (pol?.PrintFormsHistory != null && pol.PrintFormsHistory.Any())
            {
                IdCards = (from f in pol.PrintFormsHistory where f.Description.ToLower().Contains("auto id ") select f).ToList();
            }
            else
            {
                if (pol?.PrintForms != null)
                {
                    IdCards = (from f in pol.PrintForms where f.Description.ToLower().Contains("auto id ") select f).ToList();
                }
                else
                {
#if !DEBUG
                global::IFM.IFMErrorLogging.LogIssue($"IFMDATASERVICES -> PPASupplementalInfo -> PPASupplementalInfo - Print history was null so no Auto Id Cards. Policy #{((image != null) ? image.PolicyNumber : string.Empty)}");
#else
                    Debugger.Break();
#endif
                }
            }
        }
    }
}