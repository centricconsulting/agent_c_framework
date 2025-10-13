using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.Utils.Drawing.Helpers.NativeMethods;
using DCS = Diamond.Common.Services;
using DCE = Diamond.Common.Enums;
using DCO = Diamond.Common.Objects;
using IFM.DataServicesCore.BusinessLogic;

namespace IFM.DataServicesCore.CommonObjects.IFM.Auto
{
    public class YearMakeModelLookup
    {
        public YearMakeModelLookup(int year, int versionId)
        {
            Year = year;
            VersionId = versionId;
        }
        public YearMakeModelLookup(int year, string make, int versionId)
        {
            Year = year;
            Make = make;
            VersionId = versionId;
            ISOMakeCode = GetISOMakeCode();
        }
        public YearMakeModelLookup(int year, string make, string model, int versionId)
        {
            Year = year;
            Make = make;
            Model = model;
            VersionId = versionId;
            ISOMakeCode = GetISOMakeCode();
        }
        public int Year { get; set; }
        private string _make = "";
        public string Make { 
            get 
            { 
                return _make;
            }
            set 
            {
                if (value.Equals("chevy", StringComparison.OrdinalIgnoreCase))
                {
                    _make = "Chevrolet";
                }
                else
                {
                    _make = value;
                }
            }
        }
        public string ISOMakeCode { get; set; }
        public string Model { get; set; }
        public int VersionId { get; set; }
        private DCE.VehicleInfoLookupType.VehicleInfoLookupType VehicleInfoLookupType { 
            get
            {
                if (BusinessLogic.AppConfig.UseRAPAApiForVehicleSymbolLookup.TryToGetBoolean() && VersionId >= 245)
                {
                    return DCE.VehicleInfoLookupType.VehicleInfoLookupType.ModelIsoRapaApi;
                }
                else
                {
                    return DCE.VehicleInfoLookupType.VehicleInfoLookupType.ModelISORAPA;
                }
            }
        }

        private string GetISOMakeCode()
        {
            //Key is Make Description aka Full Make name. Value is the ISOMake aka abbreviated make.
            var makeDict = GetMakes();
            var foundMakes = new List<KeyValuePair<string, string>>();
            var foundModels = new List<KeyValuePair<string, string>>();

            if (makeDict.Count > 0)
            {
                foundMakes = makeDict.Where(x => x.Value.Equals(Make, StringComparison.OrdinalIgnoreCase)).ToList();
                if (foundMakes == null || foundMakes.Count == 0)
                {
                    foundMakes = makeDict.Where(x => x.Key.Equals(Make, StringComparison.OrdinalIgnoreCase)).ToList();
                    if (foundMakes == null || foundMakes.Count == 0)
                    {
                        foundMakes = makeDict.Where(x => x.Key.ToUpper().Contains(Make.ToUpper())).ToList();
                    }
                }
            }

            if (foundMakes?.Count > 0)
            {
                if (foundMakes.Count > 1)
                {
                    foreach (var foundMake in foundMakes)
                    {
                        foundModels.AddRange(GetModelsForISOMakeLookup(foundMake.Value).ToList());
                    }
                }
                else
                {
                    //foundModels = GetModelsForISOMakeLookup(foundMakes[0].Value).ToList(); //Should be no reason to look at models... just return the Make
                    return foundMakes[0].Value;
                }

                //If we have multiple potential Makes, we are looking at all the models and picking the best match based on the users Make text.
                //The value passed back is the ISOMakeCode associated with the model selected
                if (foundModels?.Count > 0)
                {
                    var ModelExactMatch = foundModels.FirstOrDefault(x => x.Key.Equals(Model, StringComparison.OrdinalIgnoreCase)).Value;
                    if (ModelExactMatch.IsNotNullEmptyOrWhitespace())
                    {
                        return ModelExactMatch;
                    }

                    var SearchTextContainsFoundModelsItem = foundModels.FirstOrDefault(x => Model.ToUpper().Contains(x.Key.ToUpper())).Value;
                    if (SearchTextContainsFoundModelsItem.IsNotNullEmptyOrWhitespace())
                    {
                        return SearchTextContainsFoundModelsItem;
                    }

                    var FoundModelsContainsSearchTextItem = foundModels.FirstOrDefault(x => x.Key.ToUpper().Contains(Model.ToUpper())).Value;
                    if (FoundModelsContainsSearchTextItem.IsNotNullEmptyOrWhitespace())
                    {
                        return FoundModelsContainsSearchTextItem;
                    }
                }
            }
            return null;
        }

        public Dictionary<string, string> GetMakes()
        {
            try
            {
                global::IFM.DataServicesCore.BusinessLogic.Diamond.Login.LoginNow(AppConfig.PrintUserName, AppConfig.PrintUserPassword);
                var makesReq = new DCS.Messages.VinService.LoadMakeModelLookUpCombo.Request();
                var makesRes = new DCS.Messages.VinService.LoadMakeModelLookUpCombo.Response();

                makesReq.RequestData.PolicyId = 0;
                makesReq.RequestData.PolicyImageNum = 0;
                makesReq.RequestData.VehicleNum = new DCO.IdValue(0);
                makesReq.RequestData.VersionId = VersionId;
                makesReq.RequestData.Year = Year;
                makesReq.RequestData.ComboType = (int)DCE.Vin.MakeModelLookupComboType.Make;
                makesReq.RequestData.VehicleInfoLookupTypeId = VehicleInfoLookupType;

                using (var proxy = new DCS.Proxies.VinServiceProxy())
                {
                    makesRes = proxy.LoadMakeModelLookUpCombo(makesReq);
                }

                if (makesRes?.ResponseData?.Items?.Count > 0)
                {
                    return makesRes.ResponseData.Items.ToDictionary(x => x.Description, x => x.Code);
                }
                else
                {
                    return null;
                }
            }
            catch
            {

            }
            return null;
        }

        public List<string> GetModels()
        {
            try
            {
                global::IFM.DataServicesCore.BusinessLogic.Diamond.Login.LoginNow(AppConfig.PrintUserName, AppConfig.PrintUserPassword);
                var modelsReq = new DCS.Messages.VinService.LoadMakeModelLookUpCombo.Request();
                var modelsRes = new DCS.Messages.VinService.LoadMakeModelLookUpCombo.Response();

                modelsReq.RequestData.PolicyId = 0;
                modelsReq.RequestData.PolicyImageNum = 0;
                modelsReq.RequestData.VehicleNum = new DCO.IdValue(0);
                modelsReq.RequestData.VersionId = VersionId;
                modelsReq.RequestData.Year = Year;
                modelsReq.RequestData.Make = ISOMakeCode;
                modelsReq.RequestData.ComboType = (int)DCE.Vin.MakeModelLookupComboType.Model;
                modelsReq.RequestData.VehicleInfoLookupTypeId = VehicleInfoLookupType;
                using (var proxy = new DCS.Proxies.VinServiceProxy())
                {
                    modelsRes = proxy.LoadMakeModelLookUpCombo(modelsReq);
                }

                if (modelsRes?.ResponseData?.Items?.Count > 0)
                {
                    return modelsRes.ResponseData.Items.Select(x => x.Description).ToList();
                }
                else
                {
                    return null;
                }
            }
            catch
            {

            }
            return null;
        }

        public Dictionary<string, string> GetModelsForISOMakeLookup(string myISOMakeCode)
        {
            try
            {
                var modelsReq = new DCS.Messages.VinService.LoadMakeModelLookUpCombo.Request();
                var modelsRes = new DCS.Messages.VinService.LoadMakeModelLookUpCombo.Response();

                modelsReq.RequestData.PolicyId = 0;
                modelsReq.RequestData.PolicyImageNum = 0;
                modelsReq.RequestData.VehicleNum = new DCO.IdValue(0);
                modelsReq.RequestData.VersionId = VersionId;
                modelsReq.RequestData.Year = Year;
                modelsReq.RequestData.Make = myISOMakeCode;
                modelsReq.RequestData.ComboType = (int)DCE.Vin.MakeModelLookupComboType.Model;
                modelsReq.RequestData.VehicleInfoLookupTypeId = VehicleInfoLookupType;
                using (var proxy = new DCS.Proxies.VinServiceProxy())
                {
                    modelsRes = proxy.LoadMakeModelLookUpCombo(modelsReq);
                }

                if (modelsRes?.ResponseData?.Items?.Count > 0)
                {
                    return modelsRes.ResponseData.Items.ToDictionary(x => x.Description, x => myISOMakeCode);
                }
                else
                {
                    return null;
                }
            }
            catch
            {

            }
            return null;
        }
    }
}
