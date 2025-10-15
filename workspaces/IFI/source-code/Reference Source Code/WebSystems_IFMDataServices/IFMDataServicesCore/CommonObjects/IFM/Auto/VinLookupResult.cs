using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraSpreadsheet.Layout;
using IFM.DataServicesCore.BusinessLogic;
using IFM.DataServicesCore.CommonObjects.OMP;
using IFM.PrimitiveExtensions;
using DCE = Diamond.Common.Enums;
using DCO = Diamond.Common.Objects;
using DCS = Diamond.Common.Services;
using VILT = Diamond.Common.Enums.VehicleInfoLookupType;
using QQCN = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName;
using QQPN = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName;
using Diamond.Common.ChoicePoint.Library.ProductTransactionRecords.Inquiry.Mvr;
using Diamond.Common.Utility;
using static Diamond.Common.Utility.Redis;

namespace IFM.DataServicesCore.CommonObjects.IFM.Auto
{
    public class VinLookupResult
    {
        public string Vin { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string BodyTypeId { get; set; }
        public string BodyTypeText { get; set; }
        public string DiamondBodyTypeId { get; set; }
        public string AntiTheftDescription { get; set; }
        public string AntiTheftTypeId { get; set; }
        public string RestraintDescription { get; set; }
        public string RestraintTypeId { get; set; }

        public string AntiLockDescription { get; set; }
        public string AntiLockTypeId { get; set; }
        public string CollisionSymbol { get; set; }
        public string CompSymbol { get; set; }
        public string LiabilitySymbol { get; set; }
        public string BodilyInjurySymbol { get; set; }
        public string MedPaySymbol { get; set; }
        public string PropertyDamageSymbol { get; set; }
        public string PerformanceTypeDescription { get; set; }
        public string PerformanceTypeId { get; set; }
        public VILT.VehicleInfoLookupType ResultVendor { get; set; } = VILT.VehicleInfoLookupType.NA;
        public int ResultVendorId { get; set; }
        public string Description { get; set; }
        public string ISOBodyStyle { get; set; }
        public string ISOBodyStyleId { get; set; }
        public string CylinderCount { get; set; }
        public string CylinderDescription { get; set; }
        public string ClassDescription { get; set; }
        public decimal CostNew { get; set; }
        public string Size { get; set; }
        public string RatingType { get; set; }
    }

    public class VinLookup : ModelBase
    {
        private static List<VILT.VehicleInfoLookupIndicatorType> ItemsToFix { get; set; } = new List<VILT.VehicleInfoLookupIndicatorType>();
        private static List<VILT.VehicleInfoLookupIndicatorType> ItemsNotFound { get; set; } = new List<VILT.VehicleInfoLookupIndicatorType>();

        public static List<VinLookupResult> GetMakeModelYearOrVinVehicleInfo(string Vin, string make, string model, int year, DateTime effectiveDate, int versionId)
        {
            var lst = new List<DCO.VehicleInfoLookup.VehicleInfoLookupResults>();
            var results = new List<VinLookupResult>();
            try
            {
                var vinReq = new DCS.Messages.VinService.VehicleInfoLookup.Request();
                var vinRes = new DCS.Messages.VinService.VehicleInfoLookup.Response();
                var vins = new List<string>();

                if (effectiveDate < DateTime.Now.AddDays(-90))
                {
                    effectiveDate = DateTime.Now;
                }
                if (BusinessLogic.AppConfig.UseRAPAApiForVehicleSymbolLookup.TryToGetBoolean() && versionId >= 245)
                {
                    vinReq.RequestData.LookupSource = VILT.VehicleInfoLookupType.ModelIsoRapaApi;
                    //All required for RAPA API to work properly
                    vinReq.RequestData.VersionId = versionId;
                    vinReq.RequestData.TEffDate = (DCO.InsDateTime)effectiveDate;
                    vinReq.RequestData.VehicleNum = new DCO.IdValue(0); //Just needs to be set to something, may only be needed for logging purposes //This one is not needed but will set anyways. VR sets this too
                    vinReq.RequestData.PolicyId = 0; //Just needs to be set to something, may only be needed for logging purposes
                    vinReq.RequestData.PolicyImageNum = new DCO.IdValue(0); //Just needs to be set to something, may only be needed for logging purposes
                    global::IFM.DataServicesCore.BusinessLogic.Diamond.Login.LoginNow(AppConfig.PrintUserName, AppConfig.PrintUserPassword);
                }
                else
                {
                    vinReq.RequestData.LookupSource = VILT.VehicleInfoLookupType.ModelISORAPA;
                }
                
                if (Vin.IsNullEmptyOrWhitespace())
                {
                    vinReq.RequestData.Year = year;
                    vinReq.RequestData.Type = DCE.Vin.MakeModelLookupType.LookupUsingYearMakeModel;
                    vinReq.RequestData.Model = model.Trim();
                    if (vinReq.RequestData.LookupSource == VILT.VehicleInfoLookupType.ModelIsoRapaApi)
                    {
                        var ymmLookup = new YearMakeModelLookup(year, make, model, versionId);
                        if (ymmLookup?.ISOMakeCode?.IsNotNullEmptyOrWhitespace() == true)
                            vinReq.RequestData.Make = ymmLookup.ISOMakeCode;
                    }
                    else
                    {
                        if (make.ToUpper().Contains("nissan".ToUpper()))
                        {
                            make = "nissan/datsun".ToUpper();
                        }
                        vinReq.RequestData.Make = make.Trim();
                    }
                }
                else
                {
                    vinReq.RequestData.Vin = Vin.Trim().ToUpper();
                    vinReq.RequestData.Type = DCE.Vin.MakeModelLookupType.LookupUsingVin;
                }
                using (var proxy = new DCS.Proxies.VinServiceProxy())
                {
                    vinRes = proxy.VehicleInfoLookup(vinReq);
                }

                if (vinRes?.ResponseData?.VehicleInfoLookupResults?.Count > 0)
                {
                    int companyStateLobId = 1;
                    if (versionId > 0 && versionId != 128) //Many items default to using 128 for versionId. CSL = 1 when versionID is 128, lets not do a lookup if it was defaulted to this.
                    {
                        companyStateLobId = GetCompanyStateLobId(versionId);
                    }
                    foreach (var lr in vinRes.ResponseData.VehicleInfoLookupResults)
                    {
                        lst.Add(lr);
                        var result = new VinLookupResult();
                        if (vins.Contains(lr.Vin) == false)
                        {
                            result.Description = lr.Description;
                            result.Make = lr.Make.ToUpper();
                            result.Model = lr.Model.ToUpper();
                            result.Year = lr.Year;
                            result.Vin = lr.Vin;
                            result.BodyTypeId = lr.BodyTypeId.ToString();
                            result.BodyTypeText = lr.BodyType;
                            result.ResultVendor = (VILT.VehicleInfoLookupType)lr.VehicleInfoLookupTypeId;
                            //result.ISOBodyStyle = ConvertVinLookupBodyStyleToVRBodyStyle(lr.ISOBodyStyle); //Legacy - pretty sure SetDiamondBodyTypeId() will do this more accurately; Leaving here just in case;
                            result.ResultVendorId = lr.VehicleInfoLookupTypeId;
                           
                            result.CylinderCount = lr.Cylinders; //' Added 5-10-16 Matt A;
                            result.CylinderDescription = lr.CylindersDescription; //' Added 5-10-16 Matt A
                            result.ClassDescription = lr.ClassDescription;

                            var mappings = GetVehicleInfoMappings(lr, companyStateLobId);
                            SetMappingItems(result, mappings);
                            FixMappingItems(result, lr);
                            result.DiamondBodyTypeId = SetDiamondBodyTypeId(lr.BodyType, lr.ISOBodyStyle, result.ISOBodyStyleId);

                            result.CostNew = lr.CostNew;
                            result.Size = ConvertVinLookupSizeToVRSize(lr.VehicleType, lr.StyleCode, lr.GrossVehicleWeight);
                            result.RatingType = ConvertVinLookupVehicleTypeToVRVehicleRatingType(lr.VehicleType);

                            if (result.Model.ToUpper() == "FORTWO" && result.Make.ToUpper() == "UNDETERMINED") //'''Matt A 8-10-2016 needed to fix 'Smart' cars
                                result.Make = "SMART";
                            if (vinReq.RequestData.LookupSource != VILT.VehicleInfoLookupType.ModelIsoRapaApi)
                            {
                                var symbolResults = PerformVehicleSymbolPlanLookup(lr.ModelISOId, versionId);
                                DCO.VehicleSymbolPlanLookup.VehicleSymbolPlanLookupResults libSymbol = null;

                                if (symbolResults != null)
                                    libSymbol = (from s in symbolResults where s.VehicleSymbolCoverageTypeId == 3 select s).FirstOrDefault();

                                if (libSymbol != null)
                                {
                                    result.LiabilitySymbol = libSymbol.Symbol;
                                    
                                }
                                result.CompSymbol = lr.IsoCompSymbol.Trim();
                                result.CollisionSymbol = lr.IsoCollisionSymbol.Trim();
                                result.BodilyInjurySymbol = lr.RAPABodilyInjurySymbol.Trim();
                                result.MedPaySymbol = lr.RAPAMedPaySymbol.Trim();
                                result.PropertyDamageSymbol = lr.RAPAPropertyDamageSymbol.Trim();
                            }
                            else
                            {
                                //RAPA API seems to work a little differently and return the Body Style for the Body Type now.... if they match, lets set Body Type to The old value we are used to.
                                if (result.BodyTypeText == result.ISOBodyStyle)
                                {
                                    result.BodyTypeText = ConvertVinLookupBodyStyleToVRBodyStyle(result.ISOBodyStyle);
                                }
                                //MP UI is using BodyTypeId and sometimes this is getting returned as 0 even though we have determined the DiamondBodyTypeId to be something sometimes.
                                if (result.BodyTypeId.TryToGetInt32() == 0 && result.DiamondBodyTypeId.TryToGetInt32() > 0)
                                {
                                    result.BodyTypeId = result.DiamondBodyTypeId;
                                }
                                result.LiabilitySymbol = lr.RAPACSLSymbol.Trim();
                                result.CompSymbol = lr.RAPAComprehensiveSymbol.Trim();
                                result.CollisionSymbol = lr.RAPACollisionSymbol.Trim();
                                result.BodilyInjurySymbol = lr.RAPABodilyInjurySymbol.Trim();
                                result.MedPaySymbol = lr.RAPAMedPaySymbol.Trim();
                                result.PropertyDamageSymbol = lr.RAPAPropertyDamageSymbol.Trim();
                            }

                            vins.Add(lr.Vin);
                            results.Add(result);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                IFMErrorLogging.LogException(ex, "IFMDataServices -> Core -> CommonObjects -> Auto -> VinLookupResults.cs");
            }

            return (from r in results orderby r.Model select r).ToList();
        }

        private static int GetCompanyStateLobId(Int32 versionId)
        {
            // could use the static data file if it was updated
            int csl = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(BusinessLogic.AppConfig.ConnDiamond))
            {
                conn.Open();
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT V.companystatelob_id FROM  dbo.[Version] V WHERE V.version_id = @versionId";
                    cmd.Parameters.AddWithValue("@versionId", versionId);
                    using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            csl = reader.GetInt32(0);
                        }
                    }
                }
            }
            return csl;
        }

        private static string ConvertVinLookupBodyStyleToVRBodyStyle(string ISOBodyStyle)
        {
            string VRBodyStyle = "";
            List<string> pickupWithOutCamperTerms = new List<string> { "pickup", "pkp", "shrt bed" };
            List<string> carTerms = new List<string> { "sedan", "coupe", "wagon", "hatchback", "conv", "rdstr", "hchbk", "hrdtp", "sed", "wag" };
            List<string> vanTerms = new List<string> { "van" };
            List<string> suvTerms = new List<string> { "utility", "util", "utl" };

            if (string.IsNullOrWhiteSpace(ISOBodyStyle) == false)
            {
                if (pickupWithOutCamperTerms.Any(x => ISOBodyStyle.Contains(x.ToUpper())))
                    VRBodyStyle = "PICKUP W/O CAMPER";
                else
                {
                    if (carTerms.Any(x => ISOBodyStyle.Contains(x.ToUpper())))
                        VRBodyStyle = "CAR";
                    else
                    {
                        if (vanTerms.Any(x => ISOBodyStyle.Contains(x.ToUpper())))
                            VRBodyStyle = "VAN";
                        else
                        {
                            if (suvTerms.Any(x => ISOBodyStyle.Contains(x.ToUpper())))
                                VRBodyStyle = "SUV";
                        }
                    }
                }
            }

            if(VRBodyStyle == "")
            {
                VRBodyStyle = "CAR";
            }

            return VRBodyStyle;
        }

        public static DCO.InsCollection<DCS.Messages.VinService.GetVehicleInfoLookupMappings.MappingItem> GetVehicleInfoMappings(DCO.VehicleInfoLookup.VehicleInfoLookupResults lr, int CompanyStateLobId)
        {
            if (lr != null && CompanyStateLobId > 0)
            {
                var request = new DCS.Messages.VinService.GetVehicleInfoLookupMappings.Request();
                var response = new DCS.Messages.VinService.GetVehicleInfoLookupMappings.Response();
                if (request != null)
                {
                    var myList = new DCO.InsCollection<DCS.Messages.VinService.GetVehicleInfoLookupMappings.MappingItem>();
                    myList.Add(CreateMappingItem(VILT.VehicleInfoLookupIndicatorType.AntiLock, lr.AntiLockBrakes, CompanyStateLobId, lr.VehicleInfoLookupTypeId));
                    myList.Add(CreateMappingItem(VILT.VehicleInfoLookupIndicatorType.AntiTheft, lr.AntiTheft, CompanyStateLobId, lr.VehicleInfoLookupTypeId));
                    myList.Add(CreateMappingItem(VILT.VehicleInfoLookupIndicatorType.Restraint, lr.Restraint, CompanyStateLobId, lr.VehicleInfoLookupTypeId));
                    myList.Add(CreateMappingItem(VILT.VehicleInfoLookupIndicatorType.BodyType, lr.ISOBodyStyle, CompanyStateLobId, lr.VehicleInfoLookupTypeId));
                    myList.Add(CreateMappingItem(VILT.VehicleInfoLookupIndicatorType.Perform, lr.Perform, CompanyStateLobId, lr.VehicleInfoLookupTypeId));
                    request.RequestData.MappingItems = myList;
                    using (var proxy = new DCS.Proxies.VinServiceProxy())
                    {
                        response = proxy.GetVehicleInfoLookupMappings(request);
                        if (response?.ResponseData?.MappingItems?.Count > 0)
                        {
                            return response.ResponseData.MappingItems;
                        }
                    }
                }
            }
            return null;
        }

        public static DCS.Messages.VinService.GetVehicleInfoLookupMappings.MappingItem CreateMappingItem(VILT.VehicleInfoLookupIndicatorType IndicatorTypeId, string Indicator, int CompanyStateLobId, int VehicleLookupTypeId)
        {
            var item = new DCS.Messages.VinService.GetVehicleInfoLookupMappings.MappingItem();
            item.IndicatorTypeId = IndicatorTypeId;
            item.LookupTypeId = (VILT.VehicleInfoLookupType)VehicleLookupTypeId;
            item.Indicator = Indicator;
            item.CompanyStateLobId = CompanyStateLobId;
            return item;
        }

        private static void SetMappingItems(VinLookupResult result, DCO.InsCollection<DCS.Messages.VinService.GetVehicleInfoLookupMappings.MappingItem> mappings)
        {
            if (result != null && mappings?.Count > 0)
            {
                SetMappingItem(result, mappings, VILT.VehicleInfoLookupIndicatorType.AntiLock);
                SetMappingItem(result, mappings, VILT.VehicleInfoLookupIndicatorType.AntiTheft);
                SetMappingItem(result, mappings, VILT.VehicleInfoLookupIndicatorType.Perform);
                SetMappingItem(result, mappings, VILT.VehicleInfoLookupIndicatorType.BodyType);
                SetMappingItem(result, mappings, VILT.VehicleInfoLookupIndicatorType.Restraint);
            }
        }

        private static void SetMappingItem(VinLookupResult result, DCO.InsCollection<DCS.Messages.VinService.GetVehicleInfoLookupMappings.MappingItem> mappings, VILT.VehicleInfoLookupIndicatorType IndicatorTypeId)
        {
            if (mappings?.Count > 0)
            {
                var mapping = mappings.Find(x => x.IndicatorTypeId == IndicatorTypeId);
                if(mapping != null)
                {
                    switch (IndicatorTypeId)
                    {
                        case VILT.VehicleInfoLookupIndicatorType.AntiLock:
                            result.AntiLockTypeId = mapping.MappingValue.ToString();
                            result.AntiLockDescription = GetStaticDataTextForValue(QQCN.QuickQuoteVehicle, QQPN.AntiLockTypeId, mapping.MappingValue.ToString());
                            break;
                        case VILT.VehicleInfoLookupIndicatorType.AntiTheft:
                            result.AntiTheftTypeId = mapping.MappingValue.ToString();
                            result.AntiTheftDescription = GetStaticDataTextForValue(QQCN.QuickQuoteVehicle, QQPN.AntiTheftTypeId, mapping.MappingValue.ToString());
                            break;
                        case VILT.VehicleInfoLookupIndicatorType.Perform:
                            result.PerformanceTypeId = mapping.MappingValue.ToString();
                            result.PerformanceTypeDescription = GetStaticDataTextForValue(QQCN.QuickQuoteVehicle, QQPN.PerformanceTypeId, mapping.MappingValue.ToString());
                            break;
                        case VILT.VehicleInfoLookupIndicatorType.BodyType:
                            result.ISOBodyStyleId = mapping.MappingValue.ToString();
                            result.ISOBodyStyle = mapping.Indicator;
                            if(mapping.IndicatorId > 0 && mapping.IndicatorId != result.BodyTypeId.TryToGetInt32())
                            {
                                result.BodyTypeId = mapping.IndicatorId.ToString();
                            }
                            break;
                        case VILT.VehicleInfoLookupIndicatorType.Restraint:
                            result.RestraintTypeId = mapping.MappingValue.ToString();
                            result.RestraintDescription = GetStaticDataTextForValue(QQCN.QuickQuoteVehicle, QQPN.RestraintTypeId, mapping.MappingValue.ToString());
                            break;
                    }
                }
                else
                {
                    ItemsNotFound.Add(IndicatorTypeId);
                }
            }
        }

        private static void FixMappingItems(VinLookupResult result, DCO.VehicleInfoLookup.VehicleInfoLookupResults lr)
        {
            if (ItemsNotFound.Count > 0)
            {
                foreach (var item in ItemsNotFound)
                {
                    switch (item)
                    {
                        //Not really sure what to do if we don't find an item in the lookup table.... but this is where you would do it.
                        case VILT.VehicleInfoLookupIndicatorType.AntiLock:
                            result.AntiLockTypeId = "0";
                            result.AntiLockDescription = "None";
                            break;
                        case VILT.VehicleInfoLookupIndicatorType.AntiTheft:
                            result.AntiTheftTypeId = "0";
                            result.AntiTheftDescription = "None";
                            break;
                        case VILT.VehicleInfoLookupIndicatorType.BodyType:
                            result.ISOBodyStyleId = "0";
                            result.ISOBodyStyle = lr.ISOBodyStyle.IsNotNullEmptyOrWhitespace() ? lr.ISOBodyStyle : "None";
                            break;
                        case VILT.VehicleInfoLookupIndicatorType.Perform:
                            //result.PerformanceTypeId = "0";
                            //result.PerformanceTypeDescription = "N/A";
                            
                            //Looks like diamond defaults these to Standard... Can we do better till they fix their mappings? Maybe?
                            ItemsToFix.Add(VILT.VehicleInfoLookupIndicatorType.Perform);
                            result.PerformanceTypeId = 1.ToString();
                            result.PerformanceTypeDescription = "Standard";
                            break;
                        case VILT.VehicleInfoLookupIndicatorType.Restraint:
                            result.RestraintTypeId = "0";
                            result.RestraintDescription = "None";
                            break;
                    }
                }
            }
            if (ItemsToFix.Count > 0)
            {
                foreach (var item in ItemsToFix)
                {
                    switch(item)
                    {
                        //Most of this is placeholder in case other things need tweeks.
                        case VILT.VehicleInfoLookupIndicatorType.AntiLock:
                            break;
                        case VILT.VehicleInfoLookupIndicatorType.AntiTheft:
                            break;
                        case VILT.VehicleInfoLookupIndicatorType.BodyType:
                            break;
                        case VILT.VehicleInfoLookupIndicatorType.Perform:
                            string perf = lr.PerformanceDescription.Replace(" Performance", "");
                            int perfId = GetStaticDataValueForTextAsInt(QQCN.QuickQuoteVehicle, QQPN.PerformanceTypeId, perf);
                            if(perfId > 0)
                            {
                                result.PerformanceTypeDescription = perf;
                                result.PerformanceTypeId = perfId.ToString();
                            }//If this doesn't work, we will just go with the default of Standard. There are others I am not unsure how we would translate. Turbo, Hybrid, Hemi, Intermediate, etc.
                            break;
                        case VILT.VehicleInfoLookupIndicatorType.Restraint:
                            break;
                    }
                }
            }
        }

        private static string SetDiamondBodyTypeId(string BodyType, string ISOBodyStyle, string ISOBodyStyleID)
        {
            string VRBodyType = "";
            string VRBodyTypeID = "";

            if(ISOBodyStyleID.TryToGetInt32() > 0)
            {
                var StaticDataBodyStyleValue = GetStaticDataTextForValue(QQCN.QuickQuoteVehicle, QQPN.BodyTypeId, ISOBodyStyleID);
                if (StaticDataBodyStyleValue.IsNotNullEmptyOrWhitespace())
                {
                    return ISOBodyStyleID;
                }
            }

            if(BodyType.IsNullEmptyOrWhitespace())
            {
                VRBodyTypeID = GetStaticDataValueForText(QQCN.QuickQuoteVehicle, QQPN.BodyTypeId, BodyType);
                if (VRBodyTypeID.TryToGetInt32() > 0)
                {
                    return VRBodyTypeID;
                }
            }

            if (String.IsNullOrWhiteSpace(ISOBodyStyle) == false)
            {
                VRBodyType = ConvertVinLookupBodyStyleToVRBodyStyle(ISOBodyStyle);
                VRBodyTypeID = GetStaticDataValueForText(QQCN.QuickQuoteVehicle, QQPN.BodyTypeId, VRBodyType);
            }

            return VRBodyTypeID;
        }

        public static DCO.InsCollection<DCO.VehicleSymbolPlanLookup.VehicleSymbolPlanLookupResults> PerformVehicleSymbolPlanLookup(int vehiclelookupId, int versionId)
        {
            var request = BuildVehicleSymbolPlanLookupRequest(vehiclelookupId, versionId);
            if(request != null)
            {
                DCS.Messages.VinService.VehicleSymbolPlanLookup.Response response = null;
                using (var proxy = new DCS.Proxies.VinServiceProxy())
                {
                    response = proxy.VehicleSymbolPlanLookup(request);
                    if(response?.ResponseData?.VehicleSymbolPlanLookupResults?.Count > 0)
                    {
                        return response.ResponseData.VehicleSymbolPlanLookupResults;
                    }
                }
            }
            return null;
        }

        public static DCS.Messages.VinService.VehicleSymbolPlanLookup.Request BuildVehicleSymbolPlanLookupRequest(int lookupId, int versionId)
        {
            VILT.VehicleInfoLookupType lookupSource = VILT.VehicleInfoLookupType.ModelISORAPA;
            DCS.Messages.VinService.VehicleSymbolPlanLookup.Request request = new DCS.Messages.VinService.VehicleSymbolPlanLookup.Request();
            request.RequestData.LookupSource = lookupSource;
            request.RequestData.ModelISOId = lookupId;
            request.RequestData.VersionId = versionId;
            request.RequestData.MakeModelLookupType = DCE.Vin.MakeModelLookupType.LookupUsingVin;
            return request;
        }

        private static string ConvertVinLookupVehicleTypeToVRVehicleRatingType(string vehicleType)
        {
            switch (vehicleType.ToUpper())
            {
                case "C": //Trailer
                    return "9"; //Truck, Trailer, Tractor
                case "T": //Tractor or Truck
                    return "9"; //Truck, Trailer, Tractor
                case "P": //Car
                    return "1"; //Private Passenger Type
                default:
                    return "";
            }
        }

        private static string ConvertVinLookupSizeToVRSize(string vehicleType, string styleCode, decimal grossVehicleWeight)
        {
            switch (vehicleType.ToUpper())
            {
                case "C": //Trailer
                    return "Trailer Types";
                case "T": //Truck or Tractor
                    if (styleCode == "TR")
                    {
                        string lessThanOrEqualTo45000 = "Heavy Truck-Tractors < or equal 45,000 Pounds GVW";
                        string greaterThan45000 = "Extra Heavy Truck-Tractors > 45,000 Pounds GVW";
                        return grossVehicleWeight <= 45000 ? lessThanOrEqualTo45000 : greaterThan45000;
                    }
                    else
                    {
                        if (grossVehicleWeight <= 10000)
                            return "Light Truck < or equal 10,000 Pounds GVW";
                        else if (grossVehicleWeight >= 10001 && grossVehicleWeight <= 20000)
                            return "Medium Truck 10,001 to 20,000 Pounds GVW";
                        else if (grossVehicleWeight >= 20001 && grossVehicleWeight <= 45000)
                            return "Heavy Truck 20,001 to 45,000 Pounds GVW";
                        else
                            return "Extra Heavy Truck > 45,000 Pounds GVW";
                    }
                default:
                    return "";
            }
        }
    }
}
