using System.Collections.Generic;
using System.Linq;
using IFM.DataServicesCore.CommonObjects.OMP.PPA;
using System;
using DCO = Diamond.Common.Objects;

#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.CommonObjects.OMP.CAP
{
    [System.Serializable]
    public class CAPSupplementalInfo : ModelBase
    {
        public List<Driver> Drivers { get; set; }
        public List<Vehicle> Vehicles { get; set; }
        public List<PrintForm> IdCards { get; set; } = new List<PrintForm>();

        public CAPSupplementalInfo() { }

        internal CAPSupplementalInfo(BasicPolicyInformation pol, DCO.Policy.Image image, bool isMultiState)
        {
            if (!isMultiState)
            {
                if (image?.LOB?.RiskLevel?.Drivers != null)
                {
                    Drivers = new List<Driver>();
                    foreach (var d in image.LOB.RiskLevel.Drivers)
                    {
                        Drivers.Add(new Driver(d));
                    }
                }
                else
                {
#if !DEBUG
                global::IFM.IFMErrorLogging.LogIssue($"Drivers was null. Policy #{((pol != null) ? pol.PolicyNumber : string.Empty)}", "IFMDATASERVICES -> CAPSupplementalInfo -> CAPSupplementalInfo");
#else
                    Debugger.Break();
#endif
                }

                if (image?.LOB?.RiskLevel?.Vehicles != null)
                {
                    Vehicles = new List<Vehicle>();
                    foreach (var v in image.LOB.RiskLevel.Vehicles)
                    {
                        Vehicles.Add(new Vehicle(v));
                    }
                }
                else
                {
#if !DEBUG
                global::IFM.IFMErrorLogging.LogIssue($"Vehicles was null. Policy #{((pol != null) ? pol.PolicyNumber : string.Empty)}", "IFMDATASERVICES -> CAPSupplementalInfo -> CAPSupplementalInfo");
#else
                    Debugger.Break();
#endif
                }
            }
            else
            {
                Drivers = new List<Driver>();
                Vehicles = new List<Vehicle>();
                foreach (var p in image.PackageParts.Skip(1))
                {

                    if (p.LOB?.RiskLevel?.Drivers != null)
                    {

                        foreach (var d in p.LOB.RiskLevel.Drivers)
                        {
                            Drivers.Add(new Driver(d));
                        }
                    }
                    else
                    {
#if !DEBUG
                global::IFM.IFMErrorLogging.LogIssue($"Drivers was null. Policy #{((pol != null) ? pol.PolicyNumber : string.Empty)}", "IFMDATASERVICES -> CAPSupplementalInfo -> CAPSupplementalInfo");
#else
                        Debugger.Break();
#endif
                    }
                    if (p.LOB?.RiskLevel?.Vehicles != null)
                    {

                        foreach (var v in p.LOB.RiskLevel.Vehicles)
                        {
                            Vehicles.Add(new Vehicle(v));
                        }
                    }
                    else
                    {
#if !DEBUG
                global::IFM.IFMErrorLogging.LogIssue($"Vehicles was null. Policy #{((pol != null) ? pol.PolicyNumber : string.Empty)}", "IFMDATASERVICES -> CAPSupplementalInfo -> CAPSupplementalInfo");
#else
                        Debugger.Break();
#endif
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
            else
            {
                if (pol?.PrintForms != null)
                {
                   // IdCards = (from f in pol.PrintForms where f.Description.ToLower().Contains("auto id ") select f).ToList();
                    foreach (var f in pol.PrintForms)
                    {

                        if (f.Description.ToLower().Contains("auto id ") || f.Description.ToLower().Contains("identification card"))
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
                else
                {
#if !DEBUG
                global::IFM.IFMErrorLogging.LogIssue($"Print history was null so could not get autoid cards. Policy #{((pol != null) ? pol.PolicyNumber : string.Empty)}", "IFMDATASERVICES -> CAPSupplementalInfo -> CAPSupplementalInfo");
#else
                    Debugger.Break();
#endif
                }
            }
        }
    }
}