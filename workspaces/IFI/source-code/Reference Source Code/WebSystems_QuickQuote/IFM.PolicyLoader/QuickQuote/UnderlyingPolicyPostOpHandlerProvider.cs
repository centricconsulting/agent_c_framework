using QuickQuote.CommonObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QuickQuote.CommonMethods.QuickQuoteHelperClass;
using static QuickQuote.CommonObjects.QuickQuoteObject;
using QQHelper = QuickQuote.CommonMethods.QuickQuoteHelperClass;
using QQObject = QuickQuote.CommonObjects.QuickQuoteObject;
using QuickQuote.CommonObjects.Umbrella;


namespace IFM.PolicyLoader.QuickQuote
{
    /// <summary>
    /// Without dependency injection in place, this provider allow you to register implementations with it
    /// The current implementation registers the desired implementations available in this file
    /// </summary>
    public class UnderlyingPolicyPostOpHandlerProvider : IPostOperationHandlerProvider
    {
        private List<IPostOperationHandler>
            _registeredHandlers = new List<IPostOperationHandler>(_commonHandlers);

        public UnderlyingPolicyPostOpHandlerProvider()
        {

        }
        public UnderlyingPolicyPostOpHandlerProvider(params IPostOperationHandler[] availableHandlers)
        {
            _registeredHandlers.Clear(); //only use the supplied handlers
            _registeredHandlers.AddRange(availableHandlers);
        }
        public void RegisterHandler(Func<QuickQuoteObject, bool> canProvideFunc, Func<QuickQuoteObject, QuickQuoteUnderlyingPolicy> handleImplFunc)
        {
            if (canProvideFunc is null)
            {
                throw new ArgumentException("Parameter canProvideFunc cannot be null");
            }
            if (handleImplFunc is null)
            {
                throw new ArgumentException("Parameter handleImplFunc cannot be null");
            }
            _registeredHandlers.Add(new PostOpHandlerWrapper(canProvideFunc, handleImplFunc));
        }

        public void RegisterHandler(IPostOperationHandler handlerImpl)
        {
            if (handlerImpl is null)
            {
                throw new ArgumentException("Parameter handlerImpl cannopt be null");
            }

            _registeredHandlers.Add(handlerImpl);
        }

        public void UnregisterHandler(Type handlerImplementation)
        { 
            if(handlerImplementation.GetInterface("IFM.PolicyLoader.QuickQuote.IPostOperationHandler", true) is null)
            {
                throw new ArgumentException("Parameter handlerImplementation must implement IPostOperationHandler");
            }

            if (handlerImplementation.IsInterface || handlerImplementation.IsAbstract)
            {
                throw new ArgumentException("Parameter handlerImplementation cannot be an interface or abstract class");
            }
            _registeredHandlers.RemoveAll(poh =>
            {
                return poh.GetType() == handlerImplementation;
            });

        }

        public void UnregisterHandler(IPostOperationHandler handlerImpl)
        {
            if (handlerImpl is null)
            {
                throw new ArgumentException("Parameter handlerImpl cannot be null");
            }

            _registeredHandlers.Remove(handlerImpl);
        }


        public bool CanProvideFor(QuickQuoteObject testData)
        {
            return _registeredHandlers.Any(handler => handler.CanHandle(testData));
        }

        public IPostOperationHandler RetrieveHandlerForData(QuickQuoteObject testData)
        {
            IPostOperationHandler handler = null;
            //verify 
            var potentialHandlers = _registeredHandlers.Where(h => h.CanHandle(testData));

            if (potentialHandlers.Any())
            {
                if (potentialHandlers.Count() > 1)
                {
                    handler = new CompositePostOpHandler(potentialHandlers.ToArray());
                }
                else
                {
                    handler = potentialHandlers.First();
                }
            }
           
            return handler;
        }

        #region "initial implementations"

        protected static List<IPostOperationHandler> _commonHandlers = new List<IPostOperationHandler>();

        //Would prefer to use DI but this pattern will work as well for now
        static UnderlyingPolicyPostOpHandlerProvider()
        {
            _commonHandlers.Add(new PersonalAutoHandler());
            _commonHandlers.Add(new HomeownersHandler());
            _commonHandlers.Add(new FarmHandler_NoPrimaryResidence());
            _commonHandlers.Add(new WorkersCompHandler());
            _commonHandlers.Add(new CommercialAutoHandler());
            _commonHandlers.Add(new DwellingFireHandler_NoPersonalLiability());
            _commonHandlers.Add(new StopGapEmployersLiabilityHandler());
        }

        #endregion

        /// <summary>
        /// Wraps lammda expression/func based handler implementations in a IPostOperationHandler object
        /// </summary>
        protected class PostOpHandlerWrapper : IPostOperationHandler
        {
            private readonly Func<QuickQuoteObject, bool> _filter;
            private readonly Func<QuickQuoteObject, QuickQuoteUnderlyingPolicy> _implementation;

            public PostOpHandlerWrapper(Func<QuickQuoteObject, QuickQuoteUnderlyingPolicy> implementation)
            {
                _implementation = implementation;
            }
            public PostOpHandlerWrapper(Func<QuickQuoteObject, bool> filter, Func<QuickQuoteObject, QuickQuoteUnderlyingPolicy> implementation)
            {
                _filter = filter;
                _implementation = implementation;
            }

            public bool CanHandle(QuickQuoteObject requestData)
            {
                return _filter?.Invoke(requestData) ?? true;
            }

            public OperationResult PerformHandling(OperationRequest opRequest)
            {
                QuickQuoteUnderlyingPolicy implResult;
                OperationResult retval = new OperationResult { Success = false };

                try
                {
                    implResult = _implementation.Invoke(opRequest.Data);
                    retval.Data = implResult;
                    retval.Success = true;
                }
                catch (Exception ex)
                {
                    retval.AddMessage(ex.Message);
                }

                return retval;
            }
        }

        /// <summary>
        /// Used a runtime when more than one handler is able to handle a particular case.
        /// Can also be used to pre-emptively tie multiple handlers together
        /// </summary>
        protected class CompositePostOpHandler : IPostOperationHandler
        {
            private readonly IEnumerable<IPostOperationHandler> _handlers;

            public CompositePostOpHandler(params IPostOperationHandler[] handlers)
            {
                _handlers = handlers;
            }

            public bool CanHandle(QQObject requestData)
            {
                return _handlers.Any(h => h.CanHandle(requestData));
            }

            public OperationResult PerformHandling(OperationRequest opRequest)
            {
                OperationResult retval = null;
                retval = _handlers.Where(h => h.CanHandle(opRequest.Data))
                                  .Aggregate<IPostOperationHandler, OperationResult>(null,
                                  (prevAggResult, h) =>
                                            {

                                                OperationResult newAggResult = h.PerformHandling(opRequest);

                                                // PeformHandling should never return null, but just in case
                                                if (newAggResult != null)
                                                {
                                                    newAggResult.Previous = prevAggResult?.AsObjectResult();
                                                }

                                                if (prevAggResult != null)
                                                {
                                                    prevAggResult.Next = newAggResult?.AsObjectResult();
                                                }

                                                return prevAggResult ?? newAggResult;
                                            });
                return retval;
            }
        }

    }

    public class PersonalAutoHandler : IPostOperationHandler
    {
        protected const int _YOUTHFUL_DRIVER_MAX_AGE_YEARS = 25;
        protected const int _YOUTHFUL_DRIVER_TEEN_COLLEGE_MAX_AGE_YEARS = 21;

        protected static QQHelper _helper = new QQHelper();
        protected static QuickQuoteStaticDataList _policyVehicleTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.BodyTypeId);
        protected static QuickQuoteStaticDataList _umbrellaVehicleTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaVehicleTypeId);
        protected static QuickQuoteStaticDataList _policyLiabilityLimits = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.Liability_UM_UIM_LimitId);
        protected static QuickQuoteStaticDataList _policyPropertyDamageLimits = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.PropertyDamageLimitId);
        protected static QuickQuoteStaticDataList _policyBodilyInjuryLiabilityLimits = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.BodilyInjuryLiabilityLimitId);
        protected static TimeSpan YoutfulDriver_MaxAge = new TimeSpan(25 * 365 - 1, 23, 59, 59);
        public bool CanHandle(QQObject requestData)
        {
            return (requestData.LobType == QuickQuoteLobType.AutoPersonal);
        }

        public OperationResult PerformHandling(OperationRequest opRequest)
        {
            var qq = opRequest.Data.GoverningStateQuoteFor();
            var retval = new OperationResult();
            bool limitsHaveBeenInitialized = false;

            try
            {
                if (qq != null)
                {
                    var retvalData = new QuickQuoteUnderlyingPolicy()
                    {
                        EffectiveDate = qq.EffectiveDate,
                        ExpirationDate = qq.ExpirationDate,
                        CompanyTypeId = "1",
                        PrimaryPolicyNumber = qq.PolicyNumber,
                        LobId = $"{Diamond_UnderlyingPolicyLobId.PersonalAuto:d}"
                    };

                    var pInfo = new PolicyInfo(retvalData)
                    {
                        TypeId = $"{DiamondPolicyTypeId.AutomobileLiability:d}"
                    };
                    //drivers
                    if (qq.Drivers?.Any() ?? false)
                    {
                        var ratedDrivers = qq.Drivers.Where(d => d.DriverExcludeTypeId == "1");
                        if (ratedDrivers?.Any() ?? false)
                        {
                            pInfo.Drivers = new List<Driver>();
                            pInfo.Drivers.AddRange(ratedDrivers.Select(d =>
                            {

                                var drvr = new Driver
                                {
                                    Name = new Name
                                    {
                                        FirstName = d.Name.FirstName,
                                        LastName = d.Name.LastName,
                                        MaritalStatusId = d.Name.MaritalStatusId,
                                        DLN = d.Name.DriversLicenseNumber,
                                        DLDate = d.Name.DriversLicenseDate,
                                        DLStateId = d.Name.DriversLicenseStateId,
                                        NameAddressSourceId = d.Name.NameAddressSourceId,
                                        SexId = d.Name.SexId,
                                        BirthDate = d.Name.BirthDate,
                                        NameId = d.Name.NameId,
                                        PolicyId = qq.PolicyId,
                                        PolicyImageNum = qq.PolicyImageNum,
                                        TypeId = d.Name.TypeId,
                                        TaxTypeId = d.Name.TaxTypeId,
                                        TaxNumber = d.Name.TaxNumber,
                                        //PolicyItemNumber = d.Name.NameNum,
                                    },
                                    PolicyId = qq.PolicyId,
                                    PolicyImageNum = qq.PolicyImageNum,
                                    //PolicyItemNumber = d.DriverNum,
                                    SetParent = pInfo
                                };
                                drvr.Name.SetParent = drvr;

                                return drvr;
                            }));
                        }

                        var contextEffectiveDate = DateTime.Parse(qq.EffectiveDate);

                        var drivers_Youthful = pInfo.Drivers.Select(drv => new
                        {
                            Driver = drv,
                            DOB = DateTime.Parse(drv.Name.BirthDate)
                        })
                        .Where(stub => stub.DOB.AddYears(_YOUTHFUL_DRIVER_MAX_AGE_YEARS) > contextEffectiveDate)
                        .GroupBy(stub => new
                        {
                            Under21 = stub.DOB.AddYears(_YOUTHFUL_DRIVER_TEEN_COLLEGE_MAX_AGE_YEARS) > contextEffectiveDate
                        });
                        //TODO: we don't use the YouthfulOperatorTypeId anywhere else, si I am hard coding it for now. We should extract it out to some usable set of constants
                        //Also, all of our values are string per convention, but in later iterations, the types should match their intent
                        /**
                         * Age Range | TypeId
                         * ----------|---------
                         * 16-20     | 2
                         * 21-24     | 3
                         */
                        if (drivers_Youthful.Any())
                        {
                            pInfo.YouthfulOperators = new List<YouthfulOperator>();

                            foreach (var drvStub in drivers_Youthful)
                            {
                                pInfo.YouthfulOperators.Add(new YouthfulOperator
                                {
                                    YouthfulOperatorTypeId = drvStub.Key.Under21 ? "2" : "3",
                                    YouthfulOperatorCount = $"{drvStub.Count()}",
                                    PolicyId = qq.PolicyId,
                                    PolicyImageNum = qq.PolicyImageNum,
                                    SetParent = pInfo
                                });
                            }
                        }
                    }

                    //vehicles
                    if (qq.Vehicles?.Any() ?? false)
                    {
                        pInfo.Vehicles = new List<Vehicle>();

                        foreach (var vehicleProto in qq.Vehicles.ToList().GroupBy(v => v.BodyTypeId))
                        {
                            var itemCount = 0;
                            var uVehicle = new Vehicle
                            {
                                PolicyId = qq.PolicyId,
                                PolicyImageNum = qq.PolicyImageNum,
                                EffectiveDate = qq.EffectiveDate,
                                SetParent = pInfo
                            };

                            var origVehicleType = _policyVehicleTypes.Options.FirstOrDefault(vt => vt.Value == vehicleProto.Key);

                            switch (origVehicleType.Text.Trim())
                            {
                                case "Car":
                                case "Pickup w/Camper":
                                case "Pickup w/o Camper":
                                case "Van":
                                case "SUV":
                                case "Antique Auto":
                                case "Classic Auto":
                                    uVehicle.TypeId = _umbrellaVehicleTypes.Options
                                                                .FirstOrDefault(vt => vt.Text.Equals("Private Passenger", StringComparison.CurrentCultureIgnoreCase))
                                                                ?.Value;

                                    break;
                                case "Motor Home":
                                    uVehicle.TypeId = _umbrellaVehicleTypes.Options
                                                                .FirstOrDefault(vt => vt.Text.Equals("Motorhome", StringComparison.CurrentCultureIgnoreCase))
                                                                ?.Value;
                                    break;
                                case "Motorcycle":
                                    uVehicle.TypeId = _umbrellaVehicleTypes.Options
                                                                .FirstOrDefault(vt => vt.Text.Equals("Motorcycle", StringComparison.CurrentCultureIgnoreCase))
                                                                ?.Value;

                                    break;
                                default:
                                    continue; //don't add the vehicle if it doesn't map, skips rest of code
                            }

                            foreach (var vehicle in vehicleProto)
                            {
                                if (CheckAndSetPolicyInfoLimits(vehicle, pInfo, limitsHaveBeenInitialized))
                                {
                                    itemCount++;
                                    limitsHaveBeenInitialized = true;
                                }
                            }

                            if (itemCount > 0)
                            {
                                uVehicle.NumberOfItems = $"{itemCount}";
                                pInfo.Vehicles.Add(uVehicle);
                            }
                        }


                        //pInfo.CombinedSingleLimit = _policyLiabilityLimits.Options
                        //                                .FirstOrDefault(pd => pd.Value.Equals(qq.Vehicles.FirstOrDefault().Liability_UM_UIM_LimitId, StringComparison.CurrentCultureIgnoreCase))
                        //                                ?.Text;
                        //pInfo.PropertyDamageLimit = _policyPropertyDamageLimits.Options
                        //                                .FirstOrDefault(pd => pd.Value.Equals(qq.Vehicles.FirstOrDefault().PropertyDamageLimitId, StringComparison.CurrentCultureIgnoreCase))
                        //                                ?.Text;

                        //var bodilyInjury = _policyBodilyInjuryLiabilityLimits.Options
                        //                        .FirstOrDefault(bi => bi.Value.Equals(qq.Vehicles.FirstOrDefault()?.BodilyInjuryLiabilityLimitId, StringComparison.CurrentCultureIgnoreCase))
                        //                        ?.Text.Split('/');

                        //pInfo.BodilyInjuryLimitA = bodilyInjury?[0];
                        //pInfo.BodilyInjuryLimitB = bodilyInjury?[1];

                        retvalData.PolicyInfos.Add(pInfo);
                    }

                    retval.Data = retvalData;
                    retval.Success = true;
                }
            }
            catch (Exception e)
            {
                retval.Success = false;
                retval.AddMessage(e.Message);
            }
            return retval;

        }

        private bool CheckAndSetPolicyInfoLimits(QuickQuoteVehicle qqVehicle, PolicyInfo policyInfo, bool checkLimitsOnly = false)
        {
            bool hasValidLimits = false;
            var csl =_policyLiabilityLimits.Options.FirstOrDefault(pd => pd.Value.Equals(qqVehicle.Liability_UM_UIM_LimitId, StringComparison.CurrentCultureIgnoreCase))
                                                    ?.Text;
            var pdl = _policyPropertyDamageLimits.Options.FirstOrDefault(pd => pd.Value.Equals(qqVehicle.PropertyDamageLimitId, StringComparison.CurrentCultureIgnoreCase))
                                                         ?.Text;

            var bodilyInjury = _policyBodilyInjuryLiabilityLimits.Options.FirstOrDefault(bi => bi.Value.Equals(qqVehicle.BodilyInjuryLiabilityLimitId, StringComparison.CurrentCultureIgnoreCase))
                                                                         ?.Text.Split('/');

            var bdlA = bodilyInjury?[0];
            var bdlB = bodilyInjury?[1];

            csl = _helper.DiamondAmountFormat(csl);
            pdl = _helper.DiamondAmountFormat(pdl);
            bdlA = _helper.DiamondAmountFormat(bdlA);
            bdlB = _helper.DiamondAmountFormat(bdlB);

            hasValidLimits = int.TryParse(csl, out _) ||
                             (int.TryParse(pdl, out _) &&
                              int.TryParse(bdlA, out _) &&
                              int.TryParse(bdlB, out _));

            if (!checkLimitsOnly)
            {
                policyInfo.CombinedSingleLimit = csl;
                policyInfo.PropertyDamageLimit = pdl;
                policyInfo.BodilyInjuryLimitA = bdlA;
                policyInfo.BodilyInjuryLimitB = bdlB;
            }

            return hasValidLimits;
        }
    }

    public class HomeownersHandler : IPostOperationHandler
    {
        protected static QQHelper _helper = new QQHelper();
        protected static QuickQuoteStaticDataList _policyOccupancyTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.OccupancyCodeId);
        protected static QuickQuoteStaticDataList _umbrellaPersonalLiabilityTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.PersonalLiabilityTypeId);
        protected static QuickQuoteStaticDataList _policyRvWaterCraftTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuotePropertyName.RvWatercraftTypeId);
        protected static QuickQuoteStaticDataList _umbrellaMiscellaneousLiabilityTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.MiscellaneousLiabilityTypeId);
        protected static QuickQuoteStaticDataList _umbrellaDwellingTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DwellingTypeId);
        protected static QuickQuoteStaticDataList _umbrellaProfessionalLiabilityTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.ProfessionalLiabilityId);
        protected static QuickQuoteStaticDataList _policyPersonalLiabilityLimits = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.PersonalLiabilityLimitId);
        protected static QuickQuoteStaticDataList _umbrellaRecreationalVehicleTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaRecreationalBodyTypeId);
        protected static QuickQuoteStaticDataList _umbrellaWatercraftTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaWaterCraftTypeId);

        public bool CanHandle(QQObject requestData)
        {
            return (requestData.LobType == QuickQuoteLobType.HomePersonal);
        }

        public OperationResult PerformHandling(OperationRequest opRequest)
        {
            var qq = opRequest.Data.GoverningStateQuoteFor();
            var retval = new OperationResult();

            try
            {
                if (qq != null)
                {
                    var retvalData = new QuickQuoteUnderlyingPolicy()
                    {
                        EffectiveDate = qq.EffectiveDate,
                        ExpirationDate = qq.ExpirationDate,
                        CompanyTypeId = "1",
                        PrimaryPolicyNumber = qq.PolicyNumber,
                        LobId = $"{Diamond_UnderlyingPolicyLobId.Home:d}"
                    };

                    PolicyInfo pInfo = null;
                    var personalLiabilityLimitForAll = _policyPersonalLiabilityLimits.Options.FirstOrDefault(p => p.Value == qq.PersonalLiabilityLimitId).Text;
                    //Personal Liability
                    if (qq.Locations.Any())
                    {
                        pInfo = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.PersonalLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll
                        };
                        pInfo.PersonalLiabilities = new List<PersonalLiability>();

                        foreach (var liabilityLocationProto in qq.Locations)
                        {
                            var uPersonalLiability = new PersonalLiability
                            {
                                Acreage = long.TryParse(liabilityLocationProto.Acreage, out long convertedAc) ? $"{convertedAc}" : "0",
                                NumberOfItems = $"1",
                                IncidentalFarmingOnPremises = (liabilityLocationProto.SectionIICoverages
                                                                      ?.Any(s2cov => s2cov?.CoverageType == QuickQuoteSectionIICoverage.SectionIICoverageType.IncidentalFarmersPersonalLiability)
                                                                      ?? false)
                                                                      .ToString(),
                                IncidentalFarmingOffPremises = (liabilityLocationProto.SectionIICoverages
                                                                      ?.Any(s2cov => s2cov?.CoverageType == QuickQuoteSectionIICoverage.SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises)
                                                                      ?? false)
                                                                      .ToString(),
                                Address = new Address(liabilityLocationProto.Address),
                                SetParent = pInfo
                            };

                            var origOccupanyType = _policyOccupancyTypes.Options.FirstOrDefault(vt => vt.Value.Equals(liabilityLocationProto.OccupancyCodeId, StringComparison.CurrentCultureIgnoreCase));

                            switch (origOccupanyType?.Text)
                            {
                                case "Owner":
                                case "Vacant":
                                case "Under Construction":
                                case "Tenant":
                                case "Primary":
                                    uPersonalLiability.TypeId = _umbrellaPersonalLiabilityTypes.Options
                                                                .FirstOrDefault(vt => vt.Text.Equals("Primary Residence", StringComparison.CurrentCultureIgnoreCase))
                                                                ?.Value;
                                    break;
                                case "Seasonal":
                                case "Secondary":
                                    uPersonalLiability.TypeId = _umbrellaPersonalLiabilityTypes.Options
                                                                .FirstOrDefault(vt => vt.Text.Equals("Secondary Residence", StringComparison.CurrentCultureIgnoreCase))
                                                                ?.Value;
                                    break;
                                default:
                                    continue; //don't add location if it doesn't map                                    
                            }
                            pInfo.PersonalLiabilities.Add(uPersonalLiability);
                        }

                        foreach (var otherOccupiedByInsured in qq.Locations.SelectMany(loc => loc.SectionIICoverages ?? Enumerable.Empty<QuickQuoteSectionIICoverage>())
                                                                                   .Where(sec => sec.CoverageType == QuickQuoteSectionIICoverage.SectionIICoverageType.OtherLocationOccupiedByInsured))
                        {
                            var uPersonalLiability = new PersonalLiability
                            {
                                Address = new Address(otherOccupiedByInsured.Address),
                                Acreage = "0",
                                NumberOfItems = "1",
                                SetParent = pInfo
                            };
                            uPersonalLiability.TypeId = _umbrellaPersonalLiabilityTypes.Options
                                                                .FirstOrDefault(vt => vt.Text.Equals("Secondary Residence", StringComparison.CurrentCultureIgnoreCase))
                                                                ?.Value;
                            pInfo.PersonalLiabilities.Add(uPersonalLiability);
                        }
                        retvalData.PolicyInfos.Add(pInfo);
                    }

                    //RvWaterCraft
                    var rvWatercraftLocations = qq.Locations.Where(loc => loc.RvWatercrafts?.Any() ?? false);
                    if (rvWatercraftLocations.Any())
                    {
                        var pInfoW = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.WatercraftLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            Watercrafts = new List<Watercraft>()
                        };

                        var pInfoRv = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.RecreationalVehicleLiabilityHome:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            RecreationalVehicles = new List<RecreationalVehicle>()
                        };

                        var rvWaterCraftsWithLiability = rvWatercraftLocations.SelectMany(l => l.RvWatercrafts ?? Enumerable.Empty<QuickQuoteRvWatercraft>())
                                                                              .Where(rvw => rvw.HasLiability || rvw.HasLiabilityOnly);
                        foreach (var rvWatercraftProto in rvWaterCraftsWithLiability)
                        //.GroupBy(rvw => rvw.RvWatercraftTypeId))
                        {

                            var rvwType = _policyRvWaterCraftTypes.Options.FirstOrDefault(rvw => rvw.Value == rvWatercraftProto.RvWatercraftTypeId);
                            switch (rvwType?.Text)
                            {
                                case "Watercraft":
                                case "Sailboat":
                                case "Jet Skis & Waverunners":
                                    var wcType = _umbrellaWatercraftTypes.Options.FirstOrDefault(wc => wc.GetExtraElementOrNothing("Value2")?.nvp_value == rvWatercraftProto.RvWatercraftTypeId)?.Value;

                                    pInfoW.Watercrafts.Add(new Watercraft
                                    {
                                        TypeId = wcType,
                                        NumberOfItems = $"1",
                                        Horsepower = long.TryParse(rvWatercraftProto.HorsepowerCC, out long convertedHp) ? $"{convertedHp}" : "0",
                                        Length = long.TryParse(rvWatercraftProto.Length, out long convertedLen) ? $"{convertedLen}" : "0",
                                        PolicyId = qq.PolicyId,
                                        PolicyImageNum = qq.PolicyImageNum,
                                        EffectiveDate = qq.EffectiveDate,
                                        //PolicyItemNumber = $"{pInfo.WaterCrafts.Count + 1}",
                                        SetParent = pInfoW
                                    });
                                    break;
                                case "Golf Cart":
                                case "Snowmobile - Named Perils":
                                case "Snowmobile - Special Coverage":
                                case "4 Wheel ATV":
                                    var rvType = _umbrellaRecreationalVehicleTypes.Options.FirstOrDefault(wc => wc.GetExtraElementOrNothing("Value2")?.nvp_value == rvWatercraftProto.RvWatercraftTypeId)?.Value;
                                    var existingRVOfType = pInfoRv.RecreationalVehicles.FirstOrDefault(rv => rv.TypeId == rvType);

                                    if (existingRVOfType == null)
                                    {
                                        pInfoRv.RecreationalVehicles.Add(new RecreationalVehicle
                                        {
                                            TypeId = rvType,
                                            NumberOfItems = $"1",
                                            PolicyId = qq.PolicyId,
                                            PolicyImageNum = qq.PolicyImageNum,
                                            EffectiveDate = qq.EffectiveDate,
                                            //PolicyItemNumber = $"{pInfo.RecreationalVehicles.Count + 1}",
                                            SetParent = pInfoRv
                                        });
                                    }
                                    else
                                    {
                                        existingRVOfType.NumberOfItems = $"{Convert.ToInt32(existingRVOfType.NumberOfItems) + 1}";
                                    }
                                    break;
                                default:
                                    //don't add the vehicle if it doesn't map
                                    break;
                            }
                        }

                        if (pInfoW.Watercrafts.Any())
                        {
                            retvalData.PolicyInfos.Add(pInfoW);
                        }
                        if (pInfoRv.RecreationalVehicles.Any())
                        {
                            retvalData.PolicyInfos.Add(pInfoRv);
                        }
                    }

                    //Miscellaneous
                    var miscLocations = qq.Locations.Where(loc => loc.SwimmingPoolHotTubSurcharge);
                    if (miscLocations.Any())
                    {
                        pInfo = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.MiscellaneousLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            MiscellaneousLiabilities = new List<MiscellaneousLiability>()
                        };

                        pInfo.MiscellaneousLiabilities.Add(new MiscellaneousLiability
                        {
                            TypeId = _umbrellaMiscellaneousLiabilityTypes.Options.FirstOrDefault(ml => ml.Text == "Swimming Pool")?.Value,
                            NumberOfItems = $"{miscLocations.Count()}",
                            PolicyId = qq.PolicyId,
                            PolicyImageNum = qq.PolicyImageNum,
                            EffectiveDate = qq.EffectiveDate,
                            //PolicyItemNumber = $"{pInfo.MiscellaneousLiabilities.Count + 1}",
                            SetParent = pInfo
                        });

                    }

                    miscLocations = qq.Locations.Where(loc => loc.TrampolineSurcharge);
                    if (miscLocations.Any())
                    {
                        if (pInfo?.MiscellaneousLiabilities is null)
                        {
                            pInfo = new PolicyInfo(retvalData)
                            {
                                TypeId = $"{DiamondPolicyTypeId.MiscellaneousLiability:d}",
                                PersonalLiabilityLimit = personalLiabilityLimitForAll,
                                MiscellaneousLiabilities = new List<MiscellaneousLiability>()
                            };
                        }

                        pInfo.MiscellaneousLiabilities.Add(new MiscellaneousLiability
                        {
                            TypeId = _umbrellaMiscellaneousLiabilityTypes.Options.FirstOrDefault(ml => ml.Text == "Trampoline")?.Value,
                            NumberOfItems = $"{miscLocations.Count()}",
                            PolicyId = qq.PolicyId,
                            PolicyImageNum = qq.PolicyImageNum,
                            EffectiveDate = qq.EffectiveDate,
                            //PolicyItemNumber = $"{pInfo.MiscellaneousLiabilities.Count + 1}",
                            SetParent = pInfo
                        });
                    }

                    if (pInfo?.MiscellaneousLiabilities?.Any() ?? false)
                    {
                        retvalData.PolicyInfos.Add(pInfo);
                    }
                    //Investment Property
                    //section II
                    var investmentS2 = qq.Locations.Where(l => l.SectionIICoverages?.Any(s2covs => s2covs.CoverageType == QuickQuoteSectionIICoverage.SectionIICoverageType.AdditionalResidenceRentedToOther) ?? false);

                    if (investmentS2.Any())
                    {
                        pInfo = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.InvestmentPropertyLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            InvestmentProperties = new List<Location>()
                        };

                        foreach (var inv in investmentS2)
                        {
                            var invLoc = new Location
                            {
                                Address = new Address(inv.Address),
                                TypeId = _umbrellaDwellingTypes.Options.FirstOrDefault(dt => dt.Text.Contains("Rental"))?.Value,
                                NumberOfItems = inv.SectionIICoverages.Count(s2covs => s2covs.CoverageType == QuickQuoteSectionIICoverage.SectionIICoverageType.AdditionalResidenceRentedToOther).ToString(),
                                PolicyId = qq.PolicyId,
                                PolicyImageNum = qq.PolicyImageNum,
                                EffectiveDate = qq.EffectiveDate,
                                SetParent = pInfo
                            };
                            invLoc.Address.SetParent = invLoc;
                            pInfo.InvestmentProperties.Add(invLoc);
                        }
                    }
                    //section I and II
                    var investmentS1_2 = qq.Locations.Where(l => l.SectionIAndIICoverages?.Any(s1_2covs => s1_2covs.MainCoverageType == QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures) ?? false);
                    if (investmentS1_2.Any())
                    {
                        if (pInfo?.InvestmentProperties is null)
                        {
                            pInfo = new PolicyInfo(retvalData)
                            {
                                TypeId = $"{DiamondPolicyTypeId.InvestmentPropertyLiability:d}",
                                PersonalLiabilityLimit = personalLiabilityLimitForAll,
                                InvestmentProperties = new List<Location>()
                            };
                        }
                        foreach (var inv in investmentS1_2)
                        {
                            var invLoc = new Location
                            {
                                Address = new Address(inv.Address),
                                TypeId = _umbrellaDwellingTypes.Options.FirstOrDefault(dt => dt.Text.Contains("Offices"))?.Value,
                                NumberOfItems = inv.SectionIAndIICoverages.Count(s1_2covs => s1_2covs.MainCoverageType == QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures).ToString(),
                                PolicyId = qq.PolicyId,
                                PolicyImageNum = qq.PolicyImageNum,
                                EffectiveDate = qq.EffectiveDate,
                                SetParent = pInfo
                            };
                            invLoc.Address.SetParent = invLoc;
                            pInfo.InvestmentProperties.Add(invLoc);
                        }
                    }

                    if (pInfo?.InvestmentProperties?.Any() ?? false)
                    {
                        retvalData.PolicyInfos.Add(pInfo);
                    }

                    //professional liability
                    var profS2TeacherA = qq.Locations.SelectMany(l => l.SectionIICoverages ?? Enumerable.Empty<QuickQuoteSectionIICoverage>())
                                                     .Where(s2covs => s2covs?.CoverageType == QuickQuoteSectionIICoverage.SectionIICoverageType.BusinessPursuits_Teacher_Other_IncludingCorporalPunishment);

                    if (profS2TeacherA.Any())
                    {
                        pInfo = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.ProfessionalLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            ProfessionalLiabilities = new List<ProfessionalLiability>()
                        };

                        pInfo.ProfessionalLiabilities.Add(new ProfessionalLiability
                        {
                            TypeId = _umbrellaProfessionalLiabilityTypes.Options.FirstOrDefault(dt => dt.Text.Contains("Teacher Including Corporal Punishment"))?.Value,
                            NumberOfItems = $"{profS2TeacherA.Count()}",
                            PolicyId = qq.PolicyId,
                            PolicyImageNum = qq.PolicyImageNum,
                            EffectiveDate = qq.EffectiveDate,
                            SetParent = pInfo
                        });

                    }

                    var profS2TeacherB = qq.Locations.SelectMany(l => l.SectionIICoverages ?? Enumerable.Empty<QuickQuoteSectionIICoverage>())
                                                     .Where(s2covs => s2covs?.CoverageType == QuickQuoteSectionIICoverage.SectionIICoverageType.BusinessPursuits_Teacher_Other_ExcludingCorporalPunishment);

                    if (profS2TeacherB.Any())
                    {
                        if (pInfo?.ProfessionalLiabilities is null)
                        {
                            pInfo = new PolicyInfo(retvalData)
                            {
                                TypeId = $"{DiamondPolicyTypeId.ProfessionalLiability:d}",
                                PersonalLiabilityLimit = personalLiabilityLimitForAll,
                                ProfessionalLiabilities = new List<ProfessionalLiability>()
                            };
                        }

                        pInfo.ProfessionalLiabilities.Add(new ProfessionalLiability
                        {
                            TypeId = _umbrellaProfessionalLiabilityTypes.Options.FirstOrDefault(dt => dt.Text.Contains("Teacher Excluding Corporal Punishment"))?.Value,
                            NumberOfItems = $"{profS2TeacherB.Count()}",
                            PolicyId = qq.PolicyId,
                            PolicyImageNum = qq.PolicyImageNum,
                            EffectiveDate = qq.EffectiveDate,
                            SetParent = pInfo
                        });
                    }

                    if (pInfo?.ProfessionalLiabilities?.Any() ?? false)
                    {
                        retvalData.PolicyInfos.Add(pInfo);
                    }


                    retval.Data = retvalData;
                    retval.Success = true;
                }
            }
            catch (Exception e)
            {
                retval.Success = false;
                retval.AddMessage(e.Message);
            }
            return retval;
        }
    }

    [Obsolete("Not used anymore due to requirements changes.")]
    public class FarmHandler : IPostOperationHandler
    {
        protected static QQHelper _helper = new QQHelper();
        protected static QuickQuoteStaticDataList _policyOccurrenceLiabilityLimits = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.OccurrenceLiabilityLimitId);
        protected static QuickQuoteStaticDataList _umbrellaPersonalLiabilityTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.PersonalLiabilityTypeId);
        protected static QuickQuoteStaticDataList _umbrellaAnnualReceiptTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.AnnualReceiptsTypeId);
        protected const string _customFarmingWithSpraying_CoverageCodeId = "80115";
        protected const string _customFarmingWithoutSpraying_CoverageCodeId = "70129";
        protected static QuickQuoteStaticDataList __policyAcreageTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteAcreage, QuickQuotePropertyName.LocationAcreageTypeId) ??
                                                                         new QuickQuoteStaticDataList()
                                                                         {
                                                                             Options = (new[] {
                                                                                            new {
                                                                                                value =  "-1",
                                                                                                text =  "N/A"
                                                                                            },
                                                                                            new {
                                                                                            value =  "0",
                                                                                            text =  "N/A"
                                                                                            },
                                                                                            new {
                                                                                            value =  "1",
                                                                                            text =  "Primary Location"
                                                                                            },
                                                                                            new {
                                                                                            value =  "2",
                                                                                            text =  "Additional Location"
                                                                                            },
                                                                                            new {
                                                                                            value =  "3",
                                                                                            text =  "Acreage Only"
                                                                                            } })
                                                                                        .Select(lat => new QuickQuoteStaticDataOption
                                                                                        {
                                                                                            Text = lat.text,
                                                                                            Value = $"{lat.value}"
                                                                                        })
                                                                                        .ToList()
                                                                         };
        protected static string _FarmFormTypeTypeN_A = _helper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.FormTypeId,
                                                                                            "", lob: QuickQuoteLobType.Farm);
        protected static QuickQuoteStaticDataList _policyRvWaterCraftTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuotePropertyName.RvWatercraftTypeId);
        protected static QuickQuoteStaticDataList _umbrellaMiscellaneousLiabilityTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.MiscellaneousLiabilityTypeId);
        protected static QuickQuoteStaticDataList _umbrellaRecreationalVehicleTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaRecreationalBodyTypeId);
        protected static QuickQuoteStaticDataList _umbrellaWatercraftTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaWaterCraftTypeId);

        public bool CanHandle(QQObject requestData)
        {
            return (requestData.LobType == QuickQuoteLobType.Farm);
        }

        public OperationResult PerformHandling(OperationRequest opRequest)
        {
            var qq = opRequest.Data.GoverningStateQuoteFor();
            var retval = new OperationResult();

            try
            {
                if (qq != null)
                {
                    var retvalData = new QuickQuoteUnderlyingPolicy()
                    {
                        EffectiveDate = qq.EffectiveDate,
                        ExpirationDate = qq.ExpirationDate,
                        Company = qq.CompanyId,
                        PrimaryPolicyNumber = qq.PolicyNumber,
                        LobId = $"{Diamond_UnderlyingPolicyLobId.Farm:d}",
                    };

                    PolicyInfo pInfo = null;

                    var personalLiabilityLimitForAll = _policyOccurrenceLiabilityLimits.Options.FirstOrDefault(p => p.Value == qq.OccurrenceLiabilityLimitId)?.Text;

                    if (!string.IsNullOrWhiteSpace(personalLiabilityLimitForAll))
                    {
                        pInfo = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.PersonalLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            //AggregateLiabilityLimit = qq.ManualAggregateLiabilityLimit,
                            //OccurrenceLiabilityLimit = personalLiabilityLimitForAll
                        };
                        //personal liability
                        if (qq.Locations.Any())
                        {
                            pInfo.PersonalLiabilities = new List<PersonalLiability>();
                            var acreageLocations = qq.Locations.SelectMany(loc => loc.Acreages)
                                                               .Select(acr =>
                                                               {
                                                                   var parent = acr.Parent as QuickQuoteLocation;
                                                                   return new
                                                                   {
                                                                       Acreage = acr.Acreage,
                                                                       Address = parent.Address,
                                                                       LocationType = __policyAcreageTypes.Options.FirstOrDefault(opt => opt.Value == acr.LocationAcreageTypeId)?.Text,
                                                                       isSelectOMatic = (parent.ProgramType == "Select-O-Matic" &&
                                                                                         parent.FormTypeId == _FarmFormTypeTypeN_A &&
                                                                                         parent.PrimaryResidence)
                                                                   };
                                                               });
                            //.GroupBy(stub => new
                            //{
                            //    stub.isSelectOMatic,
                            //    stub.LocationType
                            //});

                            foreach (var protoLoc in acreageLocations)
                            {
                                var uPersonalLiability = new PersonalLiability
                                {
                                    Address = new Address(protoLoc.Address),
                                    // Acreage = protoLoc.Sum(stub => double.Parse(stub.Acreage)).ToString(),
                                    NumberOfItems = $"1",
                                    SetParent = pInfo
                                };

                                switch (protoLoc.LocationType)
                                {
                                    case "Primary Location" when protoLoc.isSelectOMatic == false:
                                        uPersonalLiability.TypeId = _umbrellaPersonalLiabilityTypes.Options
                                                                    .FirstOrDefault(vt => vt.Text.Equals("Primary Residence", StringComparison.CurrentCultureIgnoreCase))
                                                                    ?.Value;
                                        break;
                                    case "Acreage Only" when protoLoc.isSelectOMatic == false:
                                        uPersonalLiability.TypeId = _umbrellaPersonalLiabilityTypes.Options
                                                                    .FirstOrDefault(vt => vt.Text.Equals("Primary Residence", StringComparison.CurrentCultureIgnoreCase))
                                                                    ?.Value;
                                        break;
                                    case "Additional Location" when protoLoc.isSelectOMatic == false:
                                        uPersonalLiability.TypeId = _umbrellaPersonalLiabilityTypes.Options
                                                                    .FirstOrDefault(vt => vt.Text.Equals("Secondary Residence", StringComparison.CurrentCultureIgnoreCase))
                                                                    ?.Value;
                                        break;
                                    default:
                                        if (protoLoc.isSelectOMatic)
                                        {
                                            uPersonalLiability.TypeId = _umbrellaPersonalLiabilityTypes.Options
                                                                        .FirstOrDefault(vt => vt.Text.Equals("Farm No Dwelling", StringComparison.CurrentCultureIgnoreCase))
                                                                        ?.Value;
                                        }
                                        break;
                                }

                                if (double.TryParse(protoLoc.Acreage, out double converted))
                                {
                                    uPersonalLiability.Acreage = $"{converted}";
                                }

                                pInfo.PersonalLiabilities.Add(uPersonalLiability);
                            }
                        }
                        retvalData.PolicyInfos.Add(pInfo);
                    }
                    //farm liability
                    var customFarmingLocations = qq.Locations.SelectMany(loc => loc.SectionCoverages ?? Enumerable.Empty<QuickQuoteSectionCoverage>())
                                                                .Where(sec => sec.Coverages.Any(cov => cov.CoverageCodeId == _customFarmingWithSpraying_CoverageCodeId ||
                                                                                                     cov.CoverageCodeId == _customFarmingWithoutSpraying_CoverageCodeId))
                                                                .Select(sec =>
                                                                {
                                                                    var parent = sec.Parent as QuickQuoteLocation;
                                                                    double.TryParse(sec.EstimatedReceipts, out var receiptValue);
                                                                    return new
                                                                    {
                                                                        Address = parent.Address,
                                                                        CustomFarmingWithSpraying = sec.Coverages.Any(cov => cov.CoverageCodeId == _customFarmingWithSpraying_CoverageCodeId),
                                                                        ReceiptsUnderFiveThousand = (receiptValue < 5000.00)
                                                                    };
                                                                })
                                                                .GroupBy(stub => new
                                                                {
                                                                    stub.ReceiptsUnderFiveThousand,
                                                                    stub.CustomFarmingWithSpraying
                                                                });
                    if (customFarmingLocations.Any())
                    {
                        pInfo = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.FarmLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            //AggregateLiabilityLimit = qq.ManualAggregateLiabilityLimit, 'not needed 5/20/2021
                            //OccurrenceLiabilityLimit = personalLiabilityLimitForAll, 'not needed 5/20/2021
                            FarmLiabilities = new List<FarmLiability>()
                        };

                        foreach (var protoLoc in customFarmingLocations)//this could be a single linq expression but it would hurt readability
                        {
                            var uFarmLiability = new FarmLiability
                            { //the literal strings, should be constants
                                AnnualReceiptsTypeId = _umbrellaAnnualReceiptTypes.Options.FirstOrDefault(opt => opt.Text == (protoLoc.Key.ReceiptsUnderFiveThousand ? "Receipts $5,000 or less" : "Receipts over $5,000"))?.Value,
                                HerbicidesPesticidesUsed = $"{protoLoc.Key.CustomFarmingWithSpraying}",
                                NumberOfItems = $"{protoLoc.Count()}",
                                SetParent = pInfo
                            };

                            pInfo.FarmLiabilities.Add(uFarmLiability);
                        }

                        retvalData.PolicyInfos.Add(pInfo);
                    }

                    //added 5/11/2021 KLJ 

                    //RvWaterCraft
                    var rvWatercraftLocations = qq.Locations.Where(loc => loc.RvWatercrafts?.Any() ?? false);
                    if (rvWatercraftLocations.Any())
                    {
                        var pInfoW = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.WatercraftLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            Watercrafts = new List<Watercraft>()
                        };

                        var pInfoRv = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.RecreationalVehicleLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            RecreationalVehicles = new List<RecreationalVehicle>()
                        };

                        foreach (var rvWatercraftProto in rvWatercraftLocations.SelectMany(l => l.RvWatercrafts ?? Enumerable.Empty<QuickQuoteRvWatercraft>())
                                                                               .GroupBy(rvw => rvw.RvWatercraftTypeId))
                        {

                            var rvwType = _policyRvWaterCraftTypes.Options.FirstOrDefault(rvw => rvw.Value == rvWatercraftProto.Key);

                            switch (rvwType?.Text)
                            {
                                case "Watercraft":
                                case "Sailboat":
                                case "Jet Skis & Waverunners":
                                    pInfoW.Watercrafts.Add(new Watercraft
                                    {
                                        TypeId = _umbrellaWatercraftTypes.Options.FirstOrDefault(wc => wc.GetExtraElementOrNothing("Value2")?.nvp_value == rvWatercraftProto.Key)?.Value,
                                        NumberOfItems = $"{rvWatercraftProto.Count()}",
                                        Horsepower = rvWatercraftProto.Sum(rvw => !string.IsNullOrWhiteSpace(rvw.HorsepowerCC) ? long.Parse(rvw.HorsepowerCC) : 0.00).ToString(),
                                        Length = rvWatercraftProto.Sum(rvw => !string.IsNullOrWhiteSpace(rvw.Length) ? long.Parse(rvw.Length) : 0.00).ToString(),
                                        PolicyId = qq.PolicyId,
                                        PolicyImageNum = qq.PolicyImageNum,
                                        EffectiveDate = qq.EffectiveDate,
                                        //PolicyItemNumber = $"{pInfo.WaterCrafts.Count + 1}",
                                        SetParent = pInfoW
                                    });
                                    break;
                                case "Golf Cart":
                                case "Snowmobile - Named Perils":
                                case "Snowmobile - Special Coverage":
                                case "4 Wheel ATV":
                                    pInfoRv.RecreationalVehicles.Add(new RecreationalVehicle
                                    {
                                        TypeId = _umbrellaRecreationalVehicleTypes.Options.FirstOrDefault(wc => wc.GetExtraElementOrNothing("Value2")?.nvp_value == rvWatercraftProto.Key)?.Value,
                                        NumberOfItems = $"{rvWatercraftProto.Count()}",
                                        PolicyId = qq.PolicyId,
                                        PolicyImageNum = qq.PolicyImageNum,
                                        EffectiveDate = qq.EffectiveDate,
                                        //PolicyItemNumber = $"{pInfo.RecreationalVehicles.Count + 1}",
                                        SetParent = pInfoRv
                                    });
                                    break;
                                default:
                                    //don't add the vehicle if it doesn't map
                                    break;
                            }
                        }
                        if (pInfoW.Watercrafts.Any())
                        {
                            retvalData.PolicyInfos.Add(pInfoW);
                        }
                        if (pInfoRv.RecreationalVehicles.Any())
                        {
                            retvalData.PolicyInfos.Add(pInfoRv);
                        }
                    }

                    //Miscellaneous
                    var miscLocations = qq.Locations.Where(loc => loc.SwimmingPoolHotTubSurcharge);
                    if (miscLocations.Any())
                    {
                        pInfo = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.MiscellaneousLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            MiscellaneousLiabilities = new List<MiscellaneousLiability>()
                        };

                        pInfo.MiscellaneousLiabilities.Add(new MiscellaneousLiability
                        {
                            TypeId = _umbrellaMiscellaneousLiabilityTypes.Options.FirstOrDefault(ml => ml.Text == "Swimming Pool")?.Value,
                            NumberOfItems = $"{miscLocations.Count()}",
                            PolicyId = qq.PolicyId,
                            PolicyImageNum = qq.PolicyImageNum,
                            EffectiveDate = qq.EffectiveDate,
                            //PolicyItemNumber = $"{pInfo.RecreationalVehicles.Count + 1}",
                            SetParent = pInfo
                        });

                    }

                    miscLocations = qq.Locations.Where(loc => loc.TrampolineSurcharge);
                    if (miscLocations.Any())
                    {
                        if (pInfo?.MiscellaneousLiabilities is null)
                        {
                            pInfo = new PolicyInfo(retvalData)
                            {
                                TypeId = $"{DiamondPolicyTypeId.MiscellaneousLiability:d}",
                                PersonalLiabilityLimit = personalLiabilityLimitForAll,
                                MiscellaneousLiabilities = new List<MiscellaneousLiability>()
                            };
                        }

                        pInfo.MiscellaneousLiabilities.Add(new MiscellaneousLiability
                        {
                            TypeId = _umbrellaMiscellaneousLiabilityTypes.Options.FirstOrDefault(ml => ml.Text == "Trampoline")?.Value,
                            NumberOfItems = $"{miscLocations.Count()}",
                            PolicyId = qq.PolicyId,
                            PolicyImageNum = qq.PolicyImageNum,
                            EffectiveDate = qq.EffectiveDate,
                            //PolicyItemNumber = $"{pInfo.RecreationalVehicles.Count + 1}",
                            SetParent = pInfo
                        });
                    }

                    if (pInfo?.MiscellaneousLiabilities?.Any() ?? false)
                    {
                        retvalData.PolicyInfos.Add(pInfo);
                    }


                    retval.Data = retvalData;
                    retval.Success = true;
                }
            }
            catch (Exception e)
            {
                retval.Success = false;
                retval.AddMessage(e.Message);
            }
            return retval;
        }
    }

    public class FarmHandler_NoPrimaryResidence : IPostOperationHandler
    {
        protected static QQHelper _helper = new QQHelper();
        protected static QuickQuoteStaticDataList _policyOccurrenceLiabilityLimits = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.OccurrenceLiabilityLimitId);
        protected static QuickQuoteStaticDataList _umbrellaPersonalLiabilityTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.PersonalLiabilityTypeId);
        protected static QuickQuoteStaticDataList _umbrellaAnnualReceiptTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.AnnualReceiptsTypeId);

        //Custom farming constants
        protected const string _CUSTOM_FARMING_WITH_SPRAYING_COVERAGE_CODE_ID = "80115";
        protected const string _CUSTOM_FARMING_WITHOUT_SPRAYING_COVERAGE_CODE_ID = "70129";

        //professional liability constants
        protected const string _PROF_LIABILITY_TEACHER_OTHER_INCLUDING_CORPORAL_PUNISHMENT_CODE_ID = "20055";
        protected const string _PROF_LIABILITY_TEACHER_OTHER_EXCLUDING_CORPORAL_PUNISHMENT_CODE_ID = "20054";
        protected static QuickQuoteStaticDataList __policyAcreageTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteAcreage, QuickQuotePropertyName.LocationAcreageTypeId) ??
                                                                         new QuickQuoteStaticDataList()
                                                                         {
                                                                             Options = (new[] {
                                                                                            new {
                                                                                                value =  "-1",
                                                                                                text =  "N/A"
                                                                                            },
                                                                                            new {
                                                                                            value =  "0",
                                                                                            text =  "N/A"
                                                                                            },
                                                                                            new {
                                                                                            value =  "1",
                                                                                            text =  "Primary Location"
                                                                                            },
                                                                                            new {
                                                                                            value =  "2",
                                                                                            text =  "Additional Location"
                                                                                            },
                                                                                            new {
                                                                                            value =  "3",
                                                                                            text =  "Acreage Only"
                                                                                            } })
                                                                                        .Select(lat => new QuickQuoteStaticDataOption
                                                                                        {
                                                                                            Text = lat.text,
                                                                                            Value = $"{lat.value}"
                                                                                        })
                                                                                        .ToList()
                                                                         };
        //Policy Acreage Constants
        protected const string _ACREAGE_TYPE_NA_BLANK = "-1";
        protected const string _ACREAGE_TYPE_NA = "0";
        protected const string _ACREAGE_TYPE_PRIMARY_LOCATION = "1";
        protected const string _ACREAGE_TYPE_ADITIONAL_LOCATION = "2";
        protected const string _ACREAGE_TYPE_ACREAGE_ONLY = "3";
        protected const string _OPTIONAL_COVERAGE_ADDITIONAL_RESIDENCE_PREMISES_COVERAGE_CODE_ID = "40044";
        protected const string _OPTIONAL_COVERAGE_ADDITIONAL_RESIDENCE_RENTED_TO_OTHERS_COVERAGE_CODE_ID = "40045";
        protected const string _PROGRAM_TYPE_ID_FARM_SELECT_O_MATIC = "7";
        protected const string _PROGRAM_TYPE_ID_FARM_lIABILITY = "8";
        protected static string _FORM_TYPE_ID_N_A = "13";

        protected static QuickQuoteStaticDataList _policyRvWaterCraftTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuotePropertyName.RvWatercraftTypeId);
        protected static QuickQuoteStaticDataList _umbrellaMiscellaneousLiabilityTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.MiscellaneousLiabilityTypeId);
        protected static QuickQuoteStaticDataList _umbrellaRecreationalVehicleTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaRecreationalBodyTypeId);
        protected static QuickQuoteStaticDataList _umbrellaWatercraftTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaWaterCraftTypeId);
        protected static QuickQuoteStaticDataList _umbrellaProfessionalLiabilityTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.ProfessionalLiabilityId);
        protected static QuickQuoteStaticDataList _umbrellaDwellingTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DwellingTypeId);

        public bool CanHandle(QQObject requestData)
        {
            return (requestData.LobType == QuickQuoteLobType.Farm);
        }

        public OperationResult PerformHandling(OperationRequest opRequest)
        {
            var qq = opRequest.Data.GoverningStateQuoteFor();
            var retval = new OperationResult();

            (bool hasPrimary, bool hasAdditional, bool hasAcreageOnly, long totalAcres)
                extractFromAcreages(List<QuickQuoteAcreage> acreages)
            {
                var retData = (hasPrimary: false, hasAdditional: false, hasAcreageOnly: false, totalAcres: 0L);

                foreach (var ad in acreages)
                {
                    switch (ad.LocationAcreageTypeId)
                    {
                        case _ACREAGE_TYPE_PRIMARY_LOCATION:
                            retData.hasPrimary = true;
                            break;
                        //case _ACREAGE_TYPE_ADITIONAL_LOCATION:
                        //    retData.hasAdditional = true;
                        //    break;
                        case _ACREAGE_TYPE_ACREAGE_ONLY:
                            retData.hasAcreageOnly = true;
                            break;
                        default:
                            break;
                    }
                    if (long.TryParse(ad.Acreage, out long converted))
                        retData.totalAcres += converted;
                }

                return retData;
            }

            try
            {
                if (qq != null)
                {

                    var retvalData = new QuickQuoteUnderlyingPolicy()
                    {
                        EffectiveDate = qq.EffectiveDate,
                        ExpirationDate = qq.ExpirationDate,
                        CompanyTypeId = "1",
                        PrimaryPolicyNumber = qq.PolicyNumber,
                        LobId = $"{Diamond_UnderlyingPolicyLobId.Farm:d}"
                    };

                    PolicyInfo pInfo = null;

                    var personalLiabilityLimitForAll = _policyOccurrenceLiabilityLimits.Options.FirstOrDefault(p => p.Value == qq.OccurrenceLiabilityLimitId)?.Text;

                    //personal liability
                    if (!string.IsNullOrWhiteSpace(personalLiabilityLimitForAll))
                    {

                        
                        if (qq.Locations.Any())
                        {
                            pInfo = new PolicyInfo(retvalData)
                            {
                                TypeId = $"{DiamondPolicyTypeId.PersonalLiability:d}",
                                PersonalLiabilityLimit = personalLiabilityLimitForAll,
                                //AggregateLiabilityLimit = qq.ManualAggregateLiabilityLimit,  'not needed 5/20/2021
                                //OccurrenceLiabilityLimit = personalLiabilityLimitForAll   'not needed 5/20/2021
                            };

                            pInfo.PersonalLiabilities = new List<PersonalLiability>();
                            var acreageLocations = qq.Locations.Select(loc =>
                                                               {
                                                                   var extracted = extractFromAcreages(loc.Acreages);
                                                                   return new
                                                                   {
                                                                       AcreageData = extracted,
                                                                       Address = loc.Address,
                                                                       isSelectOMatic = (loc.ProgramTypeId == _PROGRAM_TYPE_ID_FARM_SELECT_O_MATIC &&
                                                                                         loc.FormTypeId == _FORM_TYPE_ID_N_A &&
                                                                                         loc.PrimaryResidence),
                                                                       isSecondary = extracted.hasAdditional,
                                                                       isFarmLiability = (loc.ProgramTypeId == _PROGRAM_TYPE_ID_FARM_lIABILITY)

                                                                   };
                                                               });
                            long totalAcrossForPolicy = 0;

                            foreach (var protoLoc in acreageLocations)
                            {
                                var uPersonalLiability = new PersonalLiability
                                {
                                    Address = new Address(protoLoc.Address),
                                    Acreage = $"{protoLoc.AcreageData.totalAcres}",
                                    NumberOfItems = "1",
                                    SetParent = pInfo
                                };

                                totalAcrossForPolicy += protoLoc.AcreageData.totalAcres;

                                //order is important in the following block! do not rearranmge
                                switch (protoLoc)
                                {
                                    case var _ when protoLoc.isFarmLiability:
                                        uPersonalLiability.TypeId = _umbrellaPersonalLiabilityTypes.Options
                                                                    .FirstOrDefault(vt => vt.Text.Equals("Farm No Dwelling", StringComparison.CurrentCultureIgnoreCase))
                                                                    ?.Value;
                                        break;
                                    case var _ when protoLoc.AcreageData.hasPrimary && protoLoc.isSelectOMatic == false:
                                        uPersonalLiability.TypeId = _umbrellaPersonalLiabilityTypes.Options
                                                                    .FirstOrDefault(vt => vt.Text.Equals("Farm with One Dwelling", StringComparison.CurrentCultureIgnoreCase))
                                                                    ?.Value;
                                        break;
                                    //case var _ when protoLoc.isSecondary && protoLoc.isSelectOMatic == false:
                                    //    uPersonalLiability.TypeId = _umbrellaPersonalLiabilityTypes.Options
                                    //                                .FirstOrDefault(vt => vt.Text.Equals("Secondary Residence", StringComparison.CurrentCultureIgnoreCase))
                                    //                                ?.Value;
                                    //    break;
                                    case var _ when protoLoc.AcreageData.hasAcreageOnly && protoLoc.isSelectOMatic == false:
                                        uPersonalLiability.TypeId = _umbrellaPersonalLiabilityTypes.Options
                                                                    .FirstOrDefault(vt => vt.Text.Equals("Primary Residence", StringComparison.CurrentCultureIgnoreCase))
                                                                    ?.Value;
                                        break;
                                    case var _ when protoLoc.isSelectOMatic:
                                        uPersonalLiability.TypeId = _umbrellaPersonalLiabilityTypes.Options
                                                                    .FirstOrDefault(vt => vt.Text.Equals("Farm No Dwelling", StringComparison.CurrentCultureIgnoreCase))
                                                                    ?.Value;
                                        break;
                                    default:
                                        continue;
                                }


                                pInfo.PersonalLiabilities.Add(uPersonalLiability);
                            }

                            foreach (var additionalOccupiedByInsured in qq.Locations.SelectMany(loc => loc.SectionCoverages ?? Enumerable.Empty<QuickQuoteSectionCoverage>())
                                                                                    .Where(sec => sec.Coverages?.Any(cov => cov.CoverageCodeId == _OPTIONAL_COVERAGE_ADDITIONAL_RESIDENCE_PREMISES_COVERAGE_CODE_ID)
                                                                                                              ?? false))
                            {
                                var uPersonalLiability = new PersonalLiability
                                {
                                    Address = new Address(additionalOccupiedByInsured.Address),
                                    Acreage = "0",
                                    NumberOfItems = "1",
                                    SetParent = pInfo
                                };
                                uPersonalLiability.TypeId = _umbrellaPersonalLiabilityTypes.Options
                                                                    .FirstOrDefault(vt => vt.Text.Equals("Secondary Residence", StringComparison.CurrentCultureIgnoreCase))
                                                                    ?.Value;
                                pInfo.PersonalLiabilities.Add(uPersonalLiability);
                            }


                            /**
                             * HACK
                             * requested hack to cause a specific extra charge to occur rather than fix the Rate Book implementation
                             * HACK
                            **/
                            var firstLocation = pInfo.PersonalLiabilities.FirstOrDefault();
                            if (firstLocation != null)
                                firstLocation.Acreage = $"{totalAcrossForPolicy}";

                        }
                        retvalData.PolicyInfos.Add(pInfo);
                    }
                    //retvalData.PolicyInfos.Add(pInfo);

                    //farm liability
                    {
                        pInfo = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.FarmLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            //AggregateLiabilityLimit = qq.ManualAggregateLiabilityLimit,  'not needed 5/20/2021
                            //OccurrenceLiabilityLimit = personalLiabilityLimitForAll,   'not needed 5/20/2021
                            FarmLiabilities = new List<FarmLiability>()
                        };

                        var customFarmingLocations = qq.Locations.SelectMany(loc => loc.SectionCoverages ?? Enumerable.Empty<QuickQuoteSectionCoverage>())
                                                                    .Where(sec => sec.Coverages?.Any(cov => cov.CoverageCodeId == _CUSTOM_FARMING_WITH_SPRAYING_COVERAGE_CODE_ID ||
                                                                                                         cov.CoverageCodeId == _CUSTOM_FARMING_WITHOUT_SPRAYING_COVERAGE_CODE_ID)
                                                                                               ?? false)
                                                                    .Select(sec =>
                                                                    {
                                                                        var parent = sec.Parent as QuickQuoteLocation;
                                                                        double.TryParse(sec.EstimatedReceipts, out var receiptValue);
                                                                        return new
                                                                        {
                                                                            Address = parent.Address,
                                                                            CustomFarmingWithSpraying = sec.Coverages.Any(cov => cov.CoverageCodeId == _CUSTOM_FARMING_WITH_SPRAYING_COVERAGE_CODE_ID),
                                                                            ReceiptsUnderFiveThousand = (receiptValue < 5000.00)
                                                                        };
                                                                    })
                                                                    .GroupBy(stub => new
                                                                    {
                                                                        stub.ReceiptsUnderFiveThousand,
                                                                        stub.CustomFarmingWithSpraying
                                                                    });
                        if (customFarmingLocations.Any())
                        {


                            foreach (var protoLoc in customFarmingLocations)//this could be a single linq expression but it would hurt readability
                            {
                                var uFarmLiability = new FarmLiability
                                { //the literal strings, should be constants
                                    AnnualReceiptsTypeId = _umbrellaAnnualReceiptTypes.Options.FirstOrDefault(opt => opt.Text == (protoLoc.Key.ReceiptsUnderFiveThousand ? "Receipts $5,000 or less" : "Receipts over $5,000"))?.Value,
                                    HerbicidesPesticidesUsed = $"{protoLoc.Key.CustomFarmingWithSpraying}",
                                    NumberOfItems = $"{protoLoc.Count()}",
                                    SetParent = pInfo
                                };

                                pInfo.FarmLiabilities.Add(uFarmLiability);
                            }
                        }
                        //we have to add a Farm Liability policy info for every Farm underlying policy so we can get the Underlying Limit amount into Diamond
                        retvalData.PolicyInfos.Add(pInfo);
                    }
                    //added 5/11/2021 KLJ 

                    //RvWaterCraft
                    {
                        var rvWatercraftLocations = qq.Locations.Where(loc => loc.RvWatercrafts?.Any() ?? false);
                        if (rvWatercraftLocations.Any())
                        {
                            var pInfoW = new PolicyInfo(retvalData)
                            {
                                TypeId = $"{DiamondPolicyTypeId.WatercraftLiability:d}",
                                PersonalLiabilityLimit = personalLiabilityLimitForAll,
                                Watercrafts = new List<Watercraft>()
                            };

                            var pInfoRv = new PolicyInfo(retvalData)
                            {
                                TypeId = $"{DiamondPolicyTypeId.RecreationalVehicleLiability:d}",
                                PersonalLiabilityLimit = personalLiabilityLimitForAll,
                                RecreationalVehicles = new List<RecreationalVehicle>()
                            };

                            var rvWaterCraftsWithLiability = rvWatercraftLocations.SelectMany(l => l.RvWatercrafts ?? Enumerable.Empty<QuickQuoteRvWatercraft>())
                                                                                  .Where(rvw => rvw.HasLiability || rvw.HasLiabilityOnly);
                            foreach (var rvWatercraftProto in rvWaterCraftsWithLiability)
                            //.GroupBy(rvw => rvw.RvWatercraftTypeId))
                            {

                                var rvwType = _policyRvWaterCraftTypes.Options.FirstOrDefault(rvw => rvw.Value == rvWatercraftProto.RvWatercraftTypeId);
                                switch (rvwType?.Text)
                                {
                                    case "Watercraft":
                                    case "Sailboat":
                                    case "Jet Skis & Waverunners":
                                        var wcType = _umbrellaWatercraftTypes.Options.FirstOrDefault(wc => wc.GetExtraElementOrNothing("Value2")?.nvp_value == rvWatercraftProto.RvWatercraftTypeId)?.Value;

                                        pInfoW.Watercrafts.Add(new Watercraft
                                        {
                                            TypeId = wcType,
                                            NumberOfItems = $"1",
                                            Horsepower = long.TryParse(rvWatercraftProto.HorsepowerCC, out long convertedHp) ? $"{convertedHp}" : "0",
                                            Length = long.TryParse(rvWatercraftProto.Length, out long convertedLen) ? $"{convertedLen}" : "0",
                                            PolicyId = qq.PolicyId,
                                            PolicyImageNum = qq.PolicyImageNum,
                                            EffectiveDate = qq.EffectiveDate,
                                            //PolicyItemNumber = $"{pInfo.WaterCrafts.Count + 1}",
                                            SetParent = pInfoW
                                        });
                                        break;
                                    case "Golf Cart":
                                    case "Snowmobile - Named Perils":
                                    case "Snowmobile - Special Coverage":
                                    case "4 Wheel ATV":
                                        var rvType = _umbrellaRecreationalVehicleTypes.Options.FirstOrDefault(wc => wc.GetExtraElementOrNothing("Value2")?.nvp_value == rvWatercraftProto.RvWatercraftTypeId)?.Value;
                                        var existingRVOfType = pInfoRv.RecreationalVehicles.FirstOrDefault(rv => rv.TypeId == rvType);

                                        if (existingRVOfType == null)
                                        {
                                            pInfoRv.RecreationalVehicles.Add(new RecreationalVehicle
                                            {
                                                TypeId = rvType,
                                                NumberOfItems = $"1",
                                                PolicyId = qq.PolicyId,
                                                PolicyImageNum = qq.PolicyImageNum,
                                                EffectiveDate = qq.EffectiveDate,
                                                //PolicyItemNumber = $"{pInfo.RecreationalVehicles.Count + 1}",
                                                SetParent = pInfoRv
                                            });
                                        }
                                        else
                                        {
                                            existingRVOfType.NumberOfItems = $"{Convert.ToInt32(existingRVOfType.NumberOfItems) + 1}";
                                        }
                                        break;
                                    default:
                                        //don't add the vehicle if it doesn't map
                                        break;
                                }
                            }

                            if (pInfoW.Watercrafts.Any())
                            {
                                retvalData.PolicyInfos.Add(pInfoW);
                            }
                            if (pInfoRv.RecreationalVehicles.Any())
                            {
                                retvalData.PolicyInfos.Add(pInfoRv);
                            }
                        }
                    }
                    //Miscellaneous
                    {
                        var miscLocations = qq.Locations.Where(loc => loc.SwimmingPoolHotTubSurcharge);
                        if (miscLocations.Any())
                        {
                            pInfo = new PolicyInfo(retvalData)
                            {
                                TypeId = $"{DiamondPolicyTypeId.MiscellaneousLiability:d}",
                                PersonalLiabilityLimit = personalLiabilityLimitForAll,
                                MiscellaneousLiabilities = new List<MiscellaneousLiability>()
                            };

                            pInfo.MiscellaneousLiabilities.Add(new MiscellaneousLiability
                            {
                                TypeId = _umbrellaMiscellaneousLiabilityTypes.Options.FirstOrDefault(ml => ml.Text == "Swimming Pool")?.Value,
                                NumberOfItems = $"{miscLocations.Sum(loc => Math.Max(loc.SwimmingPoolHotTubSurcharge_NumberOfUnits, 1))}",
                                PolicyId = qq.PolicyId,
                                PolicyImageNum = qq.PolicyImageNum,
                                EffectiveDate = qq.EffectiveDate,
                                //PolicyItemNumber = $"{pInfo.RecreationalVehicles.Count + 1}",
                                SetParent = pInfo
                            });

                        }

                        miscLocations = qq.Locations.Where(loc => loc.TrampolineSurcharge);
                        if (miscLocations.Any())
                        {
                            if (pInfo?.MiscellaneousLiabilities is null)
                            {
                                pInfo = new PolicyInfo(retvalData)
                                {
                                    TypeId = $"{DiamondPolicyTypeId.MiscellaneousLiability:d}",
                                    PersonalLiabilityLimit = personalLiabilityLimitForAll,
                                    MiscellaneousLiabilities = new List<MiscellaneousLiability>()
                                };
                            }

                            pInfo.MiscellaneousLiabilities.Add(new MiscellaneousLiability
                            {
                                TypeId = _umbrellaMiscellaneousLiabilityTypes.Options.FirstOrDefault(ml => ml.Text == "Trampoline")?.Value,
                                NumberOfItems = $"{miscLocations.Sum(loc => Math.Max(loc.TrampolineSurcharge_NumberOfUnits, 1))}",
                                PolicyId = qq.PolicyId,
                                PolicyImageNum = qq.PolicyImageNum,
                                EffectiveDate = qq.EffectiveDate,
                                SetParent = pInfo
                            });
                        }

                        if (pInfo?.MiscellaneousLiabilities?.Any() ?? false)
                        {
                            retvalData.PolicyInfos.Add(pInfo);
                        }

                    }
                    //professional liability
                    {
                        var profS2TeacherA = qq.Locations.SelectMany(l => l.SectionIICoverages ?? Enumerable.Empty<QuickQuoteSectionIICoverage>())
                                                         .Where(lcov => lcov?.CoverageCodeId == _PROF_LIABILITY_TEACHER_OTHER_INCLUDING_CORPORAL_PUNISHMENT_CODE_ID);

                        if (profS2TeacherA.Any())
                        {
                            pInfo = new PolicyInfo(retvalData)
                            {
                                TypeId = $"{DiamondPolicyTypeId.ProfessionalLiability:d}",
                                PersonalLiabilityLimit = personalLiabilityLimitForAll,
                                ProfessionalLiabilities = new List<ProfessionalLiability>()
                            };

                            pInfo.ProfessionalLiabilities.Add(new ProfessionalLiability
                            {
                                TypeId = _umbrellaProfessionalLiabilityTypes.Options.FirstOrDefault(dt => dt.Text.Contains("Teacher Including Corporal Punishment"))?.Value,
                                NumberOfItems = $"{profS2TeacherA.Count()}",
                                PolicyId = qq.PolicyId,
                                PolicyImageNum = qq.PolicyImageNum,
                                EffectiveDate = qq.EffectiveDate,
                                SetParent = pInfo
                            });

                        }

                        var profS2TeacherB = qq.Locations.SelectMany(l => l.SectionIICoverages ?? Enumerable.Empty<QuickQuoteSectionIICoverage>())
                                                         .Where(lcov => lcov?.CoverageCodeId == _PROF_LIABILITY_TEACHER_OTHER_EXCLUDING_CORPORAL_PUNISHMENT_CODE_ID);

                        if (profS2TeacherB.Any())
                        {
                            if (pInfo?.ProfessionalLiabilities is null)
                            {
                                pInfo = new PolicyInfo(retvalData)
                                {
                                    TypeId = $"{DiamondPolicyTypeId.ProfessionalLiability:d}",
                                    PersonalLiabilityLimit = personalLiabilityLimitForAll,
                                    ProfessionalLiabilities = new List<ProfessionalLiability>()
                                };
                            }

                            pInfo.ProfessionalLiabilities.Add(new ProfessionalLiability
                            {
                                TypeId = _umbrellaProfessionalLiabilityTypes.Options.FirstOrDefault(dt => dt.Text.Contains("Teacher Excluding Corporal Punishment"))?.Value,
                                NumberOfItems = $"{profS2TeacherB.Count()}",
                                PolicyId = qq.PolicyId,
                                PolicyImageNum = qq.PolicyImageNum,
                                EffectiveDate = qq.EffectiveDate,
                                SetParent = pInfo
                            });
                        }

                        if (pInfo?.ProfessionalLiabilities?.Any() ?? false)
                        {
                            retvalData.PolicyInfos.Add(pInfo);
                        }
                    }
                    //Investment Property
                    {
                        var investments = qq.Locations.SelectMany(loc => loc.SectionCoverages ?? Enumerable.Empty<QuickQuoteSectionCoverage>())
                                                      .Where(sec => sec.Coverages?.Any(cov => cov.CoverageCodeId == _OPTIONAL_COVERAGE_ADDITIONAL_RESIDENCE_RENTED_TO_OTHERS_COVERAGE_CODE_ID)
                                                                    ?? false);

                        if (investments.Any())
                        {
                            pInfo = new PolicyInfo(retvalData)
                            {
                                TypeId = $"{DiamondPolicyTypeId.InvestmentPropertyLiability:d}",
                                PersonalLiabilityLimit = personalLiabilityLimitForAll,
                                InvestmentProperties = new List<Location>()
                            };

                            foreach (var inv in investments)
                            {
                                var invLoc = new Location
                                {
                                    Address = new Address(inv.Address),
                                    TypeId = _umbrellaDwellingTypes.Options.FirstOrDefault(dt => dt.Text.Contains("Rental"))?.Value,
                                    NumberOfItems = $"{1}",
                                    PolicyId = qq.PolicyId,
                                    PolicyImageNum = qq.PolicyImageNum,
                                    EffectiveDate = qq.EffectiveDate,
                                    SetParent = pInfo
                                };
                                invLoc.Address.SetParent = invLoc;
                                pInfo.InvestmentProperties.Add(invLoc);
                            }

                            retvalData.PolicyInfos.Add(pInfo);
                        }


                    }
                    retval.Data = retvalData;
                    retval.Success = true;
                }
            }
            catch (Exception e)
            {
                retval.Success = false;
                retval.AddMessage(e.Message);
            }
            return retval;
        }
    }

    public class StopGapEmployersLiabilityHandler : IPostOperationHandler
    {
        protected static QQHelper _helper = new QQHelper();


        public bool CanHandle(QQObject requestData)
        {
            return (requestData.QuickQuoteState == QuickQuoteState.Ohio) && (requestData.LobType == QuickQuoteLobType.Farm);
        }

        public OperationResult PerformHandling(OperationRequest opRequest)
        {
            var qq = opRequest.Data.GoverningStateQuoteFor();
            var retval = new OperationResult();
            try
            {
                if (qq != null && !string.IsNullOrWhiteSpace(qq.StopGapLimitId))
                {
                    var retvalData = new QuickQuoteUnderlyingPolicy()
                    {
                        EffectiveDate = qq.EffectiveDate,
                        ExpirationDate = qq.ExpirationDate,
                        CompanyTypeId = "1",
                        PrimaryPolicyNumber = qq.PolicyNumber,
                        LobId = $"{Diamond_UnderlyingPolicyLobId.StopGapEmployersLiability_Ohio:d}"
                    };

                    var pInfo = new PolicyInfo(retvalData)
                    {
                        TypeId = $"{DiamondPolicyTypeId.EmployersLiabilityStopGap:d}"
                    };

                    var liabilityLimitText = _helper.GetStaticDataTextForValueAndState(QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.StopGapLimitId, QuickQuoteState.Ohio, qq.StopGapLimitId);
                    var liabilitySplit = liabilityLimitText.Split('/').Select(val => $"{val},000");
                    pInfo.EachAccident = liabilitySplit.ElementAtOrDefault(0);
                    pInfo.DiseasePolicyLimit = liabilitySplit.ElementAtOrDefault(1);
                    pInfo.DiseaseEachEmployee = liabilitySplit.ElementAtOrDefault(2);

                    retvalData.PolicyInfos.Add(pInfo);
                    retval.Data = retvalData;
                    retval.Success = true;
                }
            }
            catch (Exception e)
            {
                retval.Success = false;
                retval.AddMessage(e.Message);
            }
            return retval;
        }
    }


    public class WorkersCompHandler : IPostOperationHandler
    {
        protected static QQHelper _helper = new QQHelper();
        protected static QuickQuoteStaticDataList _umbrellaCAPELIds = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaCombinedAccidentPolicyEmployeeLimitId);
        protected static readonly string _UMBRELLA_CAPEL_ID_N_A = _umbrellaCAPELIds.Options.FirstOrDefault(opt => opt.Text.Equals("N/A", StringComparison.CurrentCultureIgnoreCase))?.Value;
        public bool CanHandle(QQObject requestData)
        {
            return (requestData.LobType == QuickQuoteLobType.WorkersCompensation);
        }

        public OperationResult PerformHandling(OperationRequest opRequest)
        {
            var qq = opRequest.Data.GoverningStateQuoteFor();
            var retval = new OperationResult();
            try
            {
                if (qq != null && !string.IsNullOrWhiteSpace(qq.EmployersLiability))
                {
                    var retvalData = new QuickQuoteUnderlyingPolicy()
                    {
                        EffectiveDate = qq.EffectiveDate,
                        ExpirationDate = qq.ExpirationDate,
                        CompanyTypeId = "1",
                        PrimaryPolicyNumber = qq.PolicyNumber,
                        LobId = $"{Diamond_UnderlyingPolicyLobId.WorkersComp:d}"
                    };

                    var pInfo = new PolicyInfo(retvalData)
                    {
                        TypeId = $"{DiamondPolicyTypeId.WorkersCompLiability:d}"
                    };

                    var liabilityValue = qq.EmployersLiability?.Trim();
                    var liabilitySplit = liabilityValue.Split('/').Select(val => $"{val},000");
                    pInfo.EachAccident = liabilitySplit.ElementAtOrDefault(0);
                    pInfo.DiseasePolicyLimit = liabilitySplit.ElementAtOrDefault(1);
                    pInfo.DiseaseEachEmployee = liabilitySplit.ElementAtOrDefault(2);
                    pInfo.CombinedAccidentPolicyEmployeeLimitId = _umbrellaCAPELIds.Options
                                                                        .FirstOrDefault(opt => opt.Text.Equals(liabilityValue, StringComparison.CurrentCultureIgnoreCase))
                                                                        ?.Value??_UMBRELLA_CAPEL_ID_N_A;

                    retvalData.PolicyInfos.Add(pInfo);
                    retval.Data = retvalData;
                    retval.Success = true;
                }
            }
            catch (Exception e)
            {
                retval.Success = false;
                retval.AddMessage(e.Message);
            }
            return retval;
        }
    }

    //public class WorkersCompHandler_CompositeLimit : IPostOperationHandler
    //{
    //    protected static QQHelper _helper = new QQHelper();

    //    public bool CanHandle(QQObject requestData)
    //    {
    //        return (requestData.LobType == QuickQuoteLobType.WorkersCompensation);
    //    }

    //    public OperationResult PerformHandling(OperationRequest opRequest)
    //    {
    //        var qq = opRequest.Data.GoverningStateQuoteFor();
    //        var retval = new OperationResult();
    //        try
    //        {
    //            if (qq != null && !string.IsNullOrWhiteSpace(qq.EmployersLiability))
    //            {
    //                var retvalData = new QuickQuoteUnderlyingPolicy()
    //                {
    //                    EffectiveDate = qq.EffectiveDate,
    //                    ExpirationDate = qq.ExpirationDate,
    //                    CompanyTypeId = "1",
    //                    PrimaryPolicyNumber = qq.PolicyNumber,
    //                    LobId = $"{Diamond_UnderlyingPolicyLobId.WorkersComp:d}"
    //                };

    //                var pInfo = new PolicyInfo(retvalData)
    //                {
    //                    TypeId = $"{DiamondPolicyTypeId.WorkersCompLiability:d}"
    //                };

    //                var liabilitySplit = qq.EmployersLiability?.Trim().Split('/').Select(val => $"{val},000");
    //                pInfo.EachAccident = liabilitySplit.ElementAtOrDefault(0);
    //                pInfo.DiseasePolicyLimit = liabilitySplit.ElementAtOrDefault(1);
    //                pInfo.DiseaseEachEmployee = liabilitySplit.ElementAtOrDefault(2);
    //                pInfo
    //                retvalData.PolicyInfos.Add(pInfo);
    //                retval.Data = retvalData;
    //                retval.Success = true;
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            retval.Success = false;
    //            retval.AddMessage(e.Message);
    //        }
    //        return retval;
    //    }
    //}

    public class CommercialAutoHandler : IPostOperationHandler
    {
        protected const int _YOUTHFUL_DRIVER_MAX_AGE_YEARS = 25;
        protected const int _YOUTHFUL_DRIVER_TEEN_COLLEGE_MAX_AGE_YEARS = 21;

        protected static QQHelper _helper = new QQHelper();
        protected static QuickQuoteStaticDataList _policyVehicleRatingTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.VehicleRatingTypeId);
        protected static QuickQuoteStaticDataList _policyVehicleSizeTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.SizeTypeId);
        protected static QuickQuoteStaticDataList _umbrellaVehicleTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaVehicleTypeId);
        protected static QuickQuoteStaticDataList _policyLiabilityLimits = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.Liability_UM_UIM_LimitId);


        public bool CanHandle(QQObject requestData)
        {
            return (requestData.LobType == QuickQuoteLobType.CommercialAuto);
        }

        public OperationResult PerformHandling(OperationRequest opRequest)
        {
            var qq = opRequest.Data.GoverningStateQuoteFor();
            var retval = new OperationResult();
            bool limitsHaveBeenInitialized = false;


            try
            {
                if (qq != null)
                {
                    var retvalData = new QuickQuoteUnderlyingPolicy()
                    {
                        EffectiveDate = qq.EffectiveDate,
                        ExpirationDate = qq.ExpirationDate,
                        CompanyTypeId = "1",
                        PrimaryPolicyNumber = qq.PolicyNumber,
                        LobId = $"{Diamond_UnderlyingPolicyLobId.CommercialAuto:d}"
                    };

                    var pInfo = new PolicyInfo(retvalData)
                    {
                        TypeId = $"{DiamondPolicyTypeId.CommercialAutoLiability:d}"
                    };
                    //drivers
                    if (qq.Drivers?.Any() ?? false)
                    {
                        pInfo.Drivers = new List<Driver>();
                        pInfo.Drivers.AddRange(qq.Drivers.Select(d =>
                        {

                            var drvr = new Driver
                            {
                                Name = new Name
                                {
                                    FirstName = d.Name.FirstName,
                                    LastName = d.Name.LastName,
                                    MaritalStatusId = d.Name.MaritalStatusId,
                                    DLN = d.Name.DriversLicenseNumber,
                                    DLDate = d.Name.DriversLicenseDate,
                                    DLStateId = d.Name.DriversLicenseStateId,
                                    NameAddressSourceId = d.Name.NameAddressSourceId,
                                    SexId = d.Name.SexId,
                                    BirthDate = d.Name.BirthDate,
                                    NameId = d.Name.NameId,
                                    PolicyId = d.Name.PolicyId,
                                    PolicyImageNum = d.Name.PolicyImageNum,
                                    TypeId = d.Name.TypeId,
                                    TaxTypeId = d.Name.TaxTypeId,
                                    TaxNumber = d.Name.TaxNumber
                                    //PolicyItemNumber = d.Name.NameNum
                                },
                                PolicyId = d.PolicyId,
                                PolicyImageNum = d.PolicyImageNum,
                                //PolicyItemNumber = d.DriverNum,
                                SetParent = pInfo
                            };
                            drvr.Name.SetParent = drvr;
                            return drvr;
                        }));

                        var contextEffectiveDate = DateTime.Parse(qq.EffectiveDate);

                        var drivers_Youthful = pInfo.Drivers.Select(drv => new
                        {
                            Driver = drv,
                            DOB = DateTime.Parse(drv.Name.BirthDate)
                        })
                        .Where(stub => stub.DOB.AddYears(_YOUTHFUL_DRIVER_MAX_AGE_YEARS) > contextEffectiveDate)
                        .GroupBy(stub => new
                        {
                            Under21 = stub.DOB.AddYears(_YOUTHFUL_DRIVER_TEEN_COLLEGE_MAX_AGE_YEARS) > contextEffectiveDate
                        });
                        //TODO: we don't use the YouthfulOperatorTypeId anywhere else, si I am hard coding it for now. We should extract it out to some usable set of constants
                        //Also, all of our values are string per convention, but in later iterations, the types should match their intent
                        /**
                         * Age Range | TypeId
                         * ----------|---------
                         * 16-20     | 2
                         * 21-24     | 3
                         */
                        if (drivers_Youthful.Any())
                        {
                            pInfo.YouthfulOperators = new List<YouthfulOperator>();

                            foreach (var drvStub in drivers_Youthful)
                            {
                                pInfo.YouthfulOperators.Add(new YouthfulOperator
                                {
                                    YouthfulOperatorTypeId = drvStub.Key.Under21 ? "2" : "3",
                                    YouthfulOperatorCount = $"{drvStub.Count()}",
                                    PolicyId = qq.PolicyId,
                                    PolicyImageNum = qq.PolicyImageNum,
                                    SetParent = pInfo
                                });
                            }
                        }
                    }

                    //vehicles
                    if (qq.Vehicles?.Any() ?? false)
                    {
                        pInfo.Vehicles = new List<Vehicle>();

                        foreach (var vehicleProto in
                                    qq.Vehicles.GroupBy(v => new
                                    {
                                        v.VehicleRatingTypeId,
                                        v.SizeTypeId
                                    }))
                        {
                            var itemCount = 0;
                            var uVehicle = new Vehicle
                            {
                                PolicyId = qq.PolicyId,
                                PolicyImageNum = qq.PolicyImageNum,
                                EffectiveDate = qq.EffectiveDate,
                                SetParent = pInfo
                            };

                            var origVehicleType = _policyVehicleRatingTypes.Options.FirstOrDefault(vt => vt.Value == vehicleProto.Key.VehicleRatingTypeId);
                            var origVehicleSize = _policyVehicleSizeTypes.Options.FirstOrDefault(vt => vt.Value == vehicleProto.Key.SizeTypeId);

                            if (origVehicleType?.Text is string ratingType)
                            {
                                switch (ratingType)
                                {
                                    case "Private Passenger Type":
                                        uVehicle.TypeId = _umbrellaVehicleTypes.Options
                                                                    .FirstOrDefault(vt => vt.Text.Equals("Private Passenger", StringComparison.CurrentCultureIgnoreCase))
                                                                    ?.Value;
                                        break;
                                    case "Truck, Tractor, or Trailer" when origVehicleSize?.Value is string truckSize:
                                        switch (truckSize)
                                        {
                                            case "3":
                                            case "8":
                                            case "18":
                                                uVehicle.TypeId = _umbrellaVehicleTypes.Options
                                                                    .FirstOrDefault(vt => vt.Text.Equals("Private Passenger", StringComparison.CurrentCultureIgnoreCase))
                                                                    ?.Value;
                                                break;
                                            case "4":
                                            case "9":
                                            case "19":
                                                uVehicle.TypeId = _umbrellaVehicleTypes.Options
                                                                        .FirstOrDefault(vt => vt.Text.Equals("Farm Trucks - Medium", StringComparison.CurrentCultureIgnoreCase))
                                                                        ?.Value;
                                                break;
                                            case "5":
                                            case "10":
                                            case "20":
                                                uVehicle.TypeId = _umbrellaVehicleTypes.Options
                                                                        .FirstOrDefault(vt => vt.Text.Equals("Farm Trucks - Heavy", StringComparison.CurrentCultureIgnoreCase))
                                                                        ?.Value;
                                                break;
                                            case "6":
                                            case "11":
                                            case "21":
                                                uVehicle.TypeId = _umbrellaVehicleTypes.Options
                                                                        .FirstOrDefault(vt => vt.Text.Equals("Farm Trucks - Extra Heavy", StringComparison.CurrentCultureIgnoreCase))
                                                                        ?.Value;
                                                break;
                                            case "12":
                                            case "13":
                                            case "22":
                                            case "23":
                                                uVehicle.TypeId = _umbrellaVehicleTypes.Options
                                                                        .FirstOrDefault(vt => vt.Text.Equals("Tractor/Trailer", StringComparison.CurrentCultureIgnoreCase))
                                                                        ?.Value;
                                                break;
                                            default:
                                                continue; //don't map any other size types
                                        }
                                        break;
                                    case "Mobile Home":
                                        uVehicle.TypeId = _umbrellaVehicleTypes.Options
                                                                    .FirstOrDefault(vt => vt.Text.Equals("Motorhome", StringComparison.CurrentCultureIgnoreCase))
                                                                    ?.Value;
                                        break;
                                    case "Motorcycle":
                                        uVehicle.TypeId = _umbrellaVehicleTypes.Options
                                                                    .FirstOrDefault(vt => vt.Text.Equals("Motorcycle", StringComparison.CurrentCultureIgnoreCase))
                                                                    ?.Value;
                                        break;
                                    default:
                                        continue;//don't add the vehicle if it doesn't map                                        
                                }

                                foreach (var vehicle in vehicleProto)
                                {
                                    if (CheckAndSetPolicyInfoLimits(vehicle, pInfo, limitsHaveBeenInitialized))
                                    {
                                        itemCount++;
                                        limitsHaveBeenInitialized = true;
                                    }
                                }
                                if (itemCount > 0)
                                {
                                    uVehicle.NumberOfItems = $"{itemCount}";
                                    pInfo.Vehicles.Add(uVehicle);
                                }
                            }
                        }

                        //pInfo.CombinedSingleLimit = _policyLiabilityLimits.Options
                        //                                .FirstOrDefault(pd => pd.Value.Equals(qq.Vehicles.FirstOrDefault()?.Liability_UM_UIM_LimitId, StringComparison.CurrentCultureIgnoreCase))
                        //                                ?.Text;

                        retvalData.PolicyInfos.Add(pInfo);
                        retval.Data = retvalData;
                        retval.Success = true;
                    }
                }
            }
            catch (Exception e)
            {
                retval.Success = false;
                retval.AddMessage(e.Message);
            }
            return retval;
        }

        private bool CheckAndSetPolicyInfoLimits(QuickQuoteVehicle qqVehicle, PolicyInfo policyInfo, bool checkLimitsOnly = false)
        {
            bool hasValidLimits = false;
            var csl = _policyLiabilityLimits.Options.FirstOrDefault(pd => pd.Value.Equals(qqVehicle.Liability_UM_UIM_LimitId, StringComparison.CurrentCultureIgnoreCase))
                                                    ?.Text;
            csl = _helper.DiamondAmountFormat(csl);            
            hasValidLimits = int.TryParse(csl, out _);

            if (!checkLimitsOnly)
            {
                policyInfo.CombinedSingleLimit = csl;
            }

            return hasValidLimits;
        }
    }

    [Obsolete("Not used anymore due to requirements changes.")]
    public class DwellingFireHandler : IPostOperationHandler
    {
        protected static QQHelper _helper = new QQHelper();
        protected static QuickQuoteStaticDataList _policyOccupancyTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.OccupancyCodeId);
        protected static IEnumerable<QuickQuoteStaticDataOption> _policyUsageTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.UsageTypeId)
                                                                             .Options
                                                                             .Join(new[] { "Seasonal", "Non-Seasonal" },
                                                                                   opt => opt.Text, u => u, (opt, u) => opt);
        protected static QuickQuoteStaticDataList _umbrellaPersonalLiabilityTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.PersonalLiabilityTypeId);
        protected static QuickQuoteStaticDataList _policyRvWaterCraftTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuotePropertyName.RvWatercraftTypeId);
        protected static QuickQuoteStaticDataList _umbrellaMiscellaneousLiabilityTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.MiscellaneousLiabilityTypeId);
        protected static QuickQuoteStaticDataList _umbrellaDwellingTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DwellingTypeId);
        protected static QuickQuoteStaticDataList _umbrellaProfessionalLiabilityTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.ProfessionalLiabilityId);
        protected static QuickQuoteStaticDataList _policyPersonalLiabilityLimits = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.PersonalLiabilityLimitId);
        protected static QuickQuoteStaticDataList _umbrellaRecreationalVehicleTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaRecreationalBodyTypeId);
        protected static QuickQuoteStaticDataList _umbrellaWatercraftTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaWaterCraftTypeId);

        public bool CanHandle(QQObject requestData)
        {
            return (requestData.LobType == QuickQuoteLobType.DwellingFirePersonal);
        }

        public OperationResult PerformHandling(OperationRequest opRequest)
        {
            var qq = opRequest.Data.GoverningStateQuoteFor();
            var retval = new OperationResult();

            try
            {
                if (qq != null)
                {
                    var retvalData = new QuickQuoteUnderlyingPolicy()
                    {
                        EffectiveDate = qq.EffectiveDate,
                        ExpirationDate = qq.ExpirationDate,
                        Company = qq.CompanyId,
                        PrimaryPolicyNumber = qq.PolicyNumber,
                        LobId = $"{Diamond_UnderlyingPolicyLobId.DwellingFire:d}"
                    };

                    PolicyInfo pInfo = null;
                    var personalLiabilityLimitForAll = _policyPersonalLiabilityLimits.Options.FirstOrDefault(p => p.Value == qq.PersonalLiabilityLimitId).Text;
                    //Personal Liability
                    if (qq.Locations.Any())
                    {
                        pInfo = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.DwellingFireLiability:D}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll
                        };
                        pInfo.PersonalLiabilities = new List<PersonalLiability>();

                        //this may be more readable?
                        var dfLocations = (from loc in qq.Locations
                                           join usageType in _policyUsageTypes
                                           on loc.UsageTypeId equals usageType.Value
                                           group new
                                           {
                                               location = loc,
                                               usage = usageType.Text
                                           }
                                           by loc.OccupancyCodeId);

                        foreach (var liabilityLocationProto in dfLocations)
                        {
                            var plFirst = liabilityLocationProto.FirstOrDefault();

                            var uPersonalLiability = new PersonalLiability
                            {
                                NumberOfItems = $"{liabilityLocationProto.Count()}",
                                Address = new Address(plFirst.location.Address),
                                SetParent = pInfo
                            };
                            uPersonalLiability.Address.SetParent = uPersonalLiability;

                            var origOccupanyType = _policyOccupancyTypes.Options.FirstOrDefault(vt => vt.Value.Equals(liabilityLocationProto.Key, StringComparison.CurrentCultureIgnoreCase));

                            switch (origOccupanyType.Text.Trim())
                            {
                                case "Owner Occupied" when plFirst.usage == "Non-Seasonal":
                                case "Tenant Occupied" when plFirst.usage == "Non-Seasonal":
                                    uPersonalLiability.TypeId = _umbrellaPersonalLiabilityTypes.Options
                                                                .FirstOrDefault(vt => vt.Text.Equals("Primary Residence", StringComparison.CurrentCultureIgnoreCase))
                                                                ?.Value;
                                    break;
                                case "Owner Occupied" when plFirst.usage == "Seasonal":
                                case "Tenant Occupied" when plFirst.usage == "Seasonal":
                                    uPersonalLiability.TypeId = _umbrellaPersonalLiabilityTypes.Options
                                                                .FirstOrDefault(vt => vt.Text.Equals("Secondary Residence", StringComparison.CurrentCultureIgnoreCase))
                                                                ?.Value;
                                    break;
                                default:
                                    continue; //don't add location if it doesn't map
                                    break;
                            }
                            pInfo.PersonalLiabilities.Add(uPersonalLiability);
                        }
                        retvalData.PolicyInfos.Add(pInfo);
                    }

                    //RvWaterCraft
                    var rvWatercraftLocations = qq.Locations.Where(loc => loc.RvWatercrafts?.Any() ?? false);
                    if (rvWatercraftLocations.Any())
                    {
                        var pInfoW = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.WatercraftLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            Watercrafts = new List<Watercraft>()
                        };

                        var pInfoRv = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.RecreationalVehicleLiabilityHome:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            RecreationalVehicles = new List<RecreationalVehicle>()
                        };

                        foreach (var rvWatercraftProto in rvWatercraftLocations.SelectMany(l => l.RvWatercrafts ?? Enumerable.Empty<QuickQuoteRvWatercraft>())
                                                                               .GroupBy(rvw => rvw.RvWatercraftTypeId))
                        {

                            var rvwType = _policyRvWaterCraftTypes.Options.FirstOrDefault(rvw => rvw.Value == rvWatercraftProto.Key);

                            switch (rvwType?.Text)
                            {
                                case "Watercraft":
                                case "Sailboat":
                                case "Jet Skis & Waverunners":
                                    pInfoW.Watercrafts.Add(new Watercraft
                                    {
                                        TypeId = _umbrellaWatercraftTypes.Options.FirstOrDefault(wc => wc.GetExtraElementOrNothing("Value2")?.nvp_value == rvWatercraftProto.Key)?.Value,
                                        NumberOfItems = $"{rvWatercraftProto.Count()}",
                                        Horsepower = rvWatercraftProto.Sum(rvw => !string.IsNullOrWhiteSpace(rvw.HorsepowerCC) ? long.Parse(rvw.HorsepowerCC) : 0.00).ToString(),
                                        Length = rvWatercraftProto.Sum(rvw => !string.IsNullOrWhiteSpace(rvw.Length) ? long.Parse(rvw.Length) : 0.00).ToString(),
                                        PolicyId = qq.PolicyId,
                                        PolicyImageNum = qq.PolicyImageNum,
                                        EffectiveDate = qq.EffectiveDate,
                                        //PolicyItemNumber = $"{pInfo.WaterCrafts.Count + 1}",
                                        SetParent = pInfo
                                    });
                                    break;
                                case "Golf Cart":
                                case "Snowmobile - Named Perils":
                                case "Snowmobile - Special Coverage":
                                case "4 Wheel ATV":
                                    pInfoRv.RecreationalVehicles.Add(new RecreationalVehicle
                                    {
                                        TypeId = _umbrellaRecreationalVehicleTypes.Options.FirstOrDefault(wc => wc.GetExtraElementOrNothing("Value2")?.nvp_value == rvWatercraftProto.Key)?.Value,
                                        NumberOfItems = $"{rvWatercraftProto.Count()}",
                                        PolicyId = qq.PolicyId,
                                        PolicyImageNum = qq.PolicyImageNum,
                                        EffectiveDate = qq.EffectiveDate,
                                        //PolicyItemNumber = $"{pInfo.RecreationalVehicles.Count + 1}",
                                        SetParent = pInfo
                                    });
                                    break;
                                default:
                                    //don't add the vehicle if it doesn't map
                                    break;
                            }
                        }
                        if (pInfoW.Watercrafts.Any())
                        {
                            retvalData.PolicyInfos.Add(pInfoW);
                        }
                        if (pInfoRv.RecreationalVehicles.Any())
                        {
                            retvalData.PolicyInfos.Add(pInfoRv);
                        }
                    }

                    //Miscellaneous
                    var miscLocations = qq.Locations.Where(loc => loc.SwimmingPoolHotTubSurcharge);
                    if (miscLocations.Any())
                    {
                        pInfo = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.MiscellaneousLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            MiscellaneousLiabilities = new List<MiscellaneousLiability>()
                        };

                        pInfo.MiscellaneousLiabilities.Add(new MiscellaneousLiability
                        {
                            TypeId = _umbrellaMiscellaneousLiabilityTypes.Options.FirstOrDefault(ml => ml.Text == "Swimming Pool")?.Value,
                            NumberOfItems = $"{miscLocations.Count()}",
                            PolicyId = qq.PolicyId,
                            PolicyImageNum = qq.PolicyImageNum,
                            EffectiveDate = qq.EffectiveDate,
                            //PolicyItemNumber = $"{pInfo.RecreationalVehicles.Count + 1}",
                            SetParent = pInfo
                        });

                    }

                    miscLocations = qq.Locations.Where(loc => loc.TrampolineSurcharge);
                    if (miscLocations.Any())
                    {
                        if (pInfo?.MiscellaneousLiabilities is null)
                        {
                            pInfo = new PolicyInfo(retvalData)
                            {
                                TypeId = $"{DiamondPolicyTypeId.MiscellaneousLiability:d}",
                                PersonalLiabilityLimit = personalLiabilityLimitForAll,
                                MiscellaneousLiabilities = new List<MiscellaneousLiability>()
                            };
                        }

                        pInfo.MiscellaneousLiabilities.Add(new MiscellaneousLiability
                        {
                            TypeId = _umbrellaMiscellaneousLiabilityTypes.Options.FirstOrDefault(ml => ml.Text == "Trampoline")?.Value,
                            NumberOfItems = $"{miscLocations.Count()}",
                            PolicyId = qq.PolicyId,
                            PolicyImageNum = qq.PolicyImageNum,
                            EffectiveDate = qq.EffectiveDate,
                            //PolicyItemNumber = $"{pInfo.RecreationalVehicles.Count + 1}",
                            SetParent = pInfo
                        });
                    }

                    if (pInfo?.MiscellaneousLiabilities?.Any() ?? false)
                    {
                        retvalData.PolicyInfos.Add(pInfo);
                    }
                    //Investment Property
                    //section II
                    var investmentS2 = qq.Locations.Where(l => l.SectionIICoverages?.Any(s2covs => s2covs.CoverageType == QuickQuoteSectionIICoverage.SectionIICoverageType.AdditionalResidenceRentedToOther) ?? false);

                    if (investmentS2.Any())
                    {
                        pInfo = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.InvestmentPropertyLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            InvestmentProperties = new List<Location>()
                        };

                        foreach (var inv in investmentS2)
                        {
                            var invLoc = new Location
                            {
                                Address = new Address(inv.Address),
                                TypeId = _umbrellaDwellingTypes.Options.FirstOrDefault(dt => dt.Text.Contains("Rental"))?.Value,
                                NumberOfItems = inv.SectionIICoverages.Count(s2covs => s2covs.CoverageType == QuickQuoteSectionIICoverage.SectionIICoverageType.AdditionalResidenceRentedToOther).ToString(),
                                PolicyId = qq.PolicyId,
                                PolicyImageNum = qq.PolicyImageNum,
                                EffectiveDate = qq.EffectiveDate,
                                SetParent = pInfo
                            };
                            invLoc.Address.SetParent = invLoc;
                            pInfo.InvestmentProperties.Add(invLoc);

                        }
                    }
                    //section I and II
                    var investmentS1_2 = qq.Locations.Where(l => l.SectionIAndIICoverages?.Any(s1_2covs => s1_2covs.MainCoverageType == QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures) ?? false);
                    if (investmentS1_2.Any())
                    {
                        if (pInfo?.InvestmentProperties is null)
                        {
                            pInfo = new PolicyInfo(retvalData)
                            {
                                TypeId = $"{DiamondPolicyTypeId.InvestmentPropertyLiability:d}",
                                PersonalLiabilityLimit = personalLiabilityLimitForAll,
                                InvestmentProperties = new List<Location>()
                            };
                        }
                        foreach (var inv in investmentS1_2)
                        {
                            var invLoc = new Location
                            {
                                Address = new Address(inv.Address),
                                TypeId = _umbrellaDwellingTypes.Options.FirstOrDefault(dt => dt.Text.Contains("Offices"))?.Value,
                                NumberOfItems = inv.SectionIAndIICoverages.Count(s1_2covs => s1_2covs.MainCoverageType == QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures).ToString(),
                                PolicyId = qq.PolicyId,
                                PolicyImageNum = qq.PolicyImageNum,
                                EffectiveDate = qq.EffectiveDate,
                            };
                            invLoc.Address.SetParent = invLoc;
                            pInfo.InvestmentProperties.Add(invLoc);
                        }
                    }

                    if (pInfo?.InvestmentProperties?.Any() ?? false)
                    {
                        retvalData.PolicyInfos.Add(pInfo);
                    }

                    //professional liability
                    var profS2TeacherA = qq.Locations.SelectMany(l => l.SectionIICoverages ?? Enumerable.Empty<QuickQuoteSectionIICoverage>())
                                                     .Where(s2covs => s2covs?.CoverageType == QuickQuoteSectionIICoverage.SectionIICoverageType.BusinessPursuits_Teacher_Other_IncludingCorporalPunishment);

                    if (profS2TeacherA.Any())
                    {
                        pInfo = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.ProfessionalLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            ProfessionalLiabilities = new List<ProfessionalLiability>()
                        };

                        pInfo.ProfessionalLiabilities.Add(new ProfessionalLiability
                        {
                            TypeId = _umbrellaProfessionalLiabilityTypes.Options.FirstOrDefault(dt => dt.Text.Contains("Teacher Including Corporal Punishment"))?.Value,
                            NumberOfItems = $"{profS2TeacherA.Count()}",
                            PolicyId = qq.PolicyId,
                            PolicyImageNum = qq.PolicyImageNum,
                            EffectiveDate = qq.EffectiveDate,
                            SetParent = pInfo
                        });

                    }

                    var profS2TeacherB = qq.Locations.SelectMany(l => l.SectionIICoverages ?? Enumerable.Empty<QuickQuoteSectionIICoverage>())
                                                     .Where(s2covs => s2covs?.CoverageType == QuickQuoteSectionIICoverage.SectionIICoverageType.BusinessPursuits_Teacher_Other_ExcludingCorporalPunishment);

                    if (profS2TeacherB.Any())
                    {
                        if (pInfo?.ProfessionalLiabilities is null)
                        {
                            pInfo = new PolicyInfo(retvalData)
                            {
                                TypeId = $"{DiamondPolicyTypeId.ProfessionalLiability:d}",
                                PersonalLiabilityLimit = personalLiabilityLimitForAll,
                                ProfessionalLiabilities = new List<ProfessionalLiability>()
                            };
                        }

                        pInfo.ProfessionalLiabilities.Add(new ProfessionalLiability
                        {
                            TypeId = _umbrellaProfessionalLiabilityTypes.Options.FirstOrDefault(dt => dt.Text.Contains("Teacher Excluding Corporal Punishment"))?.Value,
                            NumberOfItems = $"{profS2TeacherB.Count()}",
                            PolicyId = qq.PolicyId,
                            PolicyImageNum = qq.PolicyImageNum,
                            EffectiveDate = qq.EffectiveDate,
                            SetParent = pInfo
                        });
                    }

                    if (pInfo?.ProfessionalLiabilities?.Any() ?? false)
                    {
                        retvalData.PolicyInfos.Add(pInfo);
                    }


                    retval.Data = retvalData;
                    retval.Success = true;
                }
            }
            catch (Exception e)
            {
                retval.Success = false;
                retval.AddMessage(e.Message);
            }
            return retval;
        }
    }

    public class DwellingFireHandler_NoPersonalLiability : IPostOperationHandler
    {
        protected static QQHelper _helper = new QQHelper();
        protected static QuickQuoteStaticDataList _policyOccupancyTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.OccupancyCodeId);
        protected static IEnumerable<QuickQuoteStaticDataOption> _policyUsageTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.UsageTypeId)
                                                                             .Options
                                                                             .Join(new[] { "Seasonal", "Non-Seasonal" },
                                                                                   opt => opt.Text, u => u, (opt, u) => opt);
        protected static QuickQuoteStaticDataList _umbrellaPersonalLiabilityTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.PersonalLiabilityTypeId);
        protected static QuickQuoteStaticDataList _policyRvWaterCraftTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuotePropertyName.RvWatercraftTypeId);
        protected static QuickQuoteStaticDataList _umbrellaMiscellaneousLiabilityTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.MiscellaneousLiabilityTypeId);
        protected static QuickQuoteStaticDataList _umbrellaDwellingTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DwellingTypeId);
        protected static QuickQuoteStaticDataList _umbrellaProfessionalLiabilityTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.ProfessionalLiabilityId);
        protected static QuickQuoteStaticDataList _policyPersonalLiabilityLimits = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.PersonalLiabilityLimitId);
        protected static QuickQuoteStaticDataList _umbrellaRecreationalVehicleTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaRecreationalBodyTypeId);
        protected static QuickQuoteStaticDataList _umbrellaWatercraftTypes = _helper.GetStaticDataList(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaWaterCraftTypeId);

        public bool CanHandle(QQObject requestData)
        {
            return (requestData.LobType == QuickQuoteLobType.DwellingFirePersonal);
        }

        public OperationResult PerformHandling(OperationRequest opRequest)
        {
            var qq = opRequest.Data.GoverningStateQuoteFor();
            var retval = new OperationResult();

            try
            {
                if (qq != null)
                {
                    var retvalData = new QuickQuoteUnderlyingPolicy()
                    {
                        EffectiveDate = qq.EffectiveDate,
                        ExpirationDate = qq.ExpirationDate,
                        CompanyTypeId = "1",
                        PrimaryPolicyNumber = qq.PolicyNumber,
                        LobId = $"{Diamond_UnderlyingPolicyLobId.DwellingFire:d}"
                    };

                    PolicyInfo pInfo = null;
                    var personalLiabilityLimitForAll = _policyPersonalLiabilityLimits.Options.FirstOrDefault(p => p.Value == qq.PersonalLiabilityLimitId).Text;

                    //personal liability limit
                    pInfo = new PolicyInfo(retvalData)
                    {
                        TypeId = $"{DiamondPolicyTypeId.PersonalLiability:d}",
                        PersonalLiabilityLimit = personalLiabilityLimitForAll
                    };

                    retvalData.PolicyInfos.Add(pInfo);


                    var rentalDwellingTypeId = _umbrellaDwellingTypes.Options.FirstOrDefault(dt => dt.Text.Contains("Rental"))?.Value;
                    //No Personal Liability --> will now be an Investment Property
                    if (qq.Locations.Any())
                    {
                        pInfo = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.InvestmentPropertyLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll
                        };
                        pInfo.InvestmentProperties = new List<Location>();

                        //this may be more readable?
                        var dfLocations = (from loc in qq.Locations
                                           join usageType in _policyUsageTypes
                                           on loc.UsageTypeId equals usageType.Value
                                           select new
                                           {
                                               location = loc,
                                               usage = usageType.Text
                                           });


                        foreach (var liabilityLocationProto in dfLocations)
                        {

                            var uInvestment = new Location
                            {
                                NumberOfItems = $"{1}",
                                NumberOfUnitsId = $"{1}", //the value is the same as the id for QuickQuoteLocation>NumberOfUnitsId
                                Address = new Address(liabilityLocationProto.location.Address),
                                PolicyId = qq.PolicyId,
                                PolicyImageNum = qq.PolicyImageNum,
                                EffectiveDate = qq.EffectiveDate,
                                SetParent = pInfo,
                                TypeId = rentalDwellingTypeId
                            };
                            uInvestment.Address.SetParent = uInvestment;

                            pInfo.InvestmentProperties.Add(uInvestment);
                        }
                        retvalData.PolicyInfos.Add(pInfo);
                    }

                    //RvWaterCraft
                    var rvWatercraftLocations = qq.Locations.Where(loc => loc.RvWatercrafts?.Any() ?? false);
                    if (rvWatercraftLocations.Any())
                    {
                        var pInfoW = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.WatercraftLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            Watercrafts = new List<Watercraft>()
                        };

                        var pInfoRv = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.RecreationalVehicleLiabilityHome:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            RecreationalVehicles = new List<RecreationalVehicle>()
                        };

                        foreach (var rvWatercraftProto in rvWatercraftLocations.SelectMany(l => l.RvWatercrafts ?? Enumerable.Empty<QuickQuoteRvWatercraft>())
                                                                              .Where(rvw => rvw.HasLiability || rvw.HasLiabilityOnly))
                        //.GroupBy(rvw => rvw.RvWatercraftTypeId))
                        {

                            var rvwType = _policyRvWaterCraftTypes.Options.FirstOrDefault(rvw => rvw.Value == rvWatercraftProto.RvWatercraftTypeId);
                            switch (rvwType?.Text)
                            {
                                case "Watercraft":
                                case "Sailboat":
                                case "Jet Skis & Waverunners":
                                    var wcType = _umbrellaWatercraftTypes.Options.FirstOrDefault(wc => wc.GetExtraElementOrNothing("Value2")?.nvp_value == rvWatercraftProto.RvWatercraftTypeId)?.Value;

                                    pInfoW.Watercrafts.Add(new Watercraft
                                    {
                                        TypeId = wcType,
                                        NumberOfItems = $"1",
                                        Horsepower = long.TryParse(rvWatercraftProto.HorsepowerCC, out long convertedHp) ? $"{convertedHp}" : "0",
                                        Length = long.TryParse(rvWatercraftProto.Length, out long convertedLen) ? $"{convertedLen}" : "0",
                                        PolicyId = qq.PolicyId,
                                        PolicyImageNum = qq.PolicyImageNum,
                                        EffectiveDate = qq.EffectiveDate,
                                        //PolicyItemNumber = $"{pInfo.WaterCrafts.Count + 1}",
                                        SetParent = pInfoW
                                    });
                                    break;
                                case "Golf Cart":
                                case "Snowmobile - Named Perils":
                                case "Snowmobile - Special Coverage":
                                case "4 Wheel ATV":
                                    var rvType = _umbrellaRecreationalVehicleTypes.Options.FirstOrDefault(wc => wc.GetExtraElementOrNothing("Value2")?.nvp_value == rvWatercraftProto.RvWatercraftTypeId)?.Value;
                                    var existingRVOfType = pInfoRv.RecreationalVehicles.FirstOrDefault(rv => rv.TypeId == rvType);

                                    if (existingRVOfType == null)
                                    {
                                        pInfoRv.RecreationalVehicles.Add(new RecreationalVehicle
                                    {
                                        TypeId = rvType,
                                        NumberOfItems = $"1",
                                        PolicyId = qq.PolicyId,
                                        PolicyImageNum = qq.PolicyImageNum,
                                        EffectiveDate = qq.EffectiveDate,
                                        //PolicyItemNumber = $"{pInfo.RecreationalVehicles.Count + 1}",
                                        SetParent = pInfoRv
                                    });
                                    }
                                    else
                                    {
                                        existingRVOfType.NumberOfItems = $"{Convert.ToInt32(existingRVOfType.NumberOfItems) + 1}";
                                    }
                                    break;
                                default:
                                    //don't add the vehicle if it doesn't map
                                    break;
                            }
                        }

                        if (pInfoW.Watercrafts.Any())
                        {
                            retvalData.PolicyInfos.Add(pInfoW);
                        }
                        if (pInfoRv.RecreationalVehicles.Any())
                        {
                            retvalData.PolicyInfos.Add(pInfoRv);
                        }
                    }

                    //Miscellaneous
                    var miscLocations = qq.Locations.Where(loc => loc.SwimmingPoolHotTubSurcharge);
                    if (miscLocations.Any())
                    {
                        pInfo = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.MiscellaneousLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            MiscellaneousLiabilities = new List<MiscellaneousLiability>()
                        };

                        pInfo.MiscellaneousLiabilities.Add(new MiscellaneousLiability
                        {
                            TypeId = _umbrellaMiscellaneousLiabilityTypes.Options.FirstOrDefault(ml => ml.Text == "Swimming Pool")?.Value,
                            NumberOfItems = $"{miscLocations.Count()}",
                            PolicyId = qq.PolicyId,
                            PolicyImageNum = qq.PolicyImageNum,
                            EffectiveDate = qq.EffectiveDate,
                            //PolicyItemNumber = $"{pInfo.RecreationalVehicles.Count + 1}",
                            SetParent = pInfo
                        });

                    }

                    miscLocations = qq.Locations.Where(loc => loc.TrampolineSurcharge);
                    if (miscLocations.Any())
                    {
                        if (pInfo?.MiscellaneousLiabilities is null)
                        {
                            pInfo = new PolicyInfo(retvalData)
                            {
                                TypeId = $"{DiamondPolicyTypeId.MiscellaneousLiability:d}",
                                PersonalLiabilityLimit = personalLiabilityLimitForAll,
                                MiscellaneousLiabilities = new List<MiscellaneousLiability>()
                            };
                        }

                        pInfo.MiscellaneousLiabilities.Add(new MiscellaneousLiability
                        {
                            TypeId = _umbrellaMiscellaneousLiabilityTypes.Options.FirstOrDefault(ml => ml.Text == "Trampoline")?.Value,
                            NumberOfItems = $"{miscLocations.Count()}",
                            PolicyId = qq.PolicyId,
                            PolicyImageNum = qq.PolicyImageNum,
                            EffectiveDate = qq.EffectiveDate,
                            //PolicyItemNumber = $"{pInfo.RecreationalVehicles.Count + 1}",
                            SetParent = pInfo
                        });
                    }

                    if (pInfo?.MiscellaneousLiabilities?.Any() ?? false)
                    {
                        retvalData.PolicyInfos.Add(pInfo);
                    }
                    //Investment Property

                    ////section II
                    //var investmentS2 = qq.Locations.Where(l => l.SectionIICoverages?.Any(s2covs => s2covs.CoverageType == QuickQuoteSectionIICoverage.SectionIICoverageType.AdditionalResidenceRentedToOther) ?? false);

                    //if (investmentS2.Any())
                    //{
                    //    pInfo = new PolicyInfo(retvalData)
                    //    {
                    //        TypeId = $"{DiamondPolicyTypeId.InvestmentPropertyLiability:d}",
                    //        PersonalLiabilityLimit = personalLiabilityLimitForAll,
                    //        InvestmentProperties = new List<Location>()
                    //    };

                    //    foreach (var inv in investmentS2)
                    //    {
                    //        var invLoc = new Location
                    //        {
                    //            Address = new Address(inv.Address),
                    //            TypeId = _umbrellaDwellingTypes.Options.FirstOrDefault(dt => dt.Text.Contains("Rental"))?.Value,
                    //            NumberOfItems = inv.SectionIICoverages.Count(s2covs => s2covs.CoverageType == QuickQuoteSectionIICoverage.SectionIICoverageType.AdditionalResidenceRentedToOther).ToString(),
                    //            PolicyId = qq.PolicyId,
                    //            PolicyImageNum = qq.PolicyImageNum,
                    //            EffectiveDate = qq.EffectiveDate,
                    //            SetParent = pInfo
                    //        };
                    //        invLoc.Address.SetParent = invLoc;
                    //        pInfo.InvestmentProperties.Add(invLoc);

                    //    }
                    //}

                    //section I and II
                    var investmentS1_2 = qq.Locations.Where(l => l.SectionIAndIICoverages?.Any(s1_2covs => s1_2covs.MainCoverageType == QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures) ?? false);
                    if (investmentS1_2.Any())
                    {

                        pInfo = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.InvestmentPropertyLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            InvestmentProperties = new List<Location>()
                        };

                        foreach (var inv in investmentS1_2)
                        {
                            var invLoc = new Location
                            {
                                Address = new Address(inv.Address),
                                TypeId = _umbrellaDwellingTypes.Options.FirstOrDefault(dt => dt.Text.Contains("Offices"))?.Value,
                                NumberOfItems = inv.SectionIAndIICoverages.Count(s1_2covs => s1_2covs.MainCoverageType == QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures).ToString(),
                                PolicyId = qq.PolicyId,
                                PolicyImageNum = qq.PolicyImageNum,
                                EffectiveDate = qq.EffectiveDate,
                            };
                            invLoc.Address.SetParent = invLoc;
                            pInfo.InvestmentProperties.Add(invLoc);
                        }

                        retvalData.PolicyInfos.Add(pInfo);
                    }



                    //professional liability
                    var profS2TeacherA = qq.Locations.SelectMany(l => l.SectionIICoverages ?? Enumerable.Empty<QuickQuoteSectionIICoverage>())
                                                     .Where(s2covs => s2covs?.CoverageType == QuickQuoteSectionIICoverage.SectionIICoverageType.BusinessPursuits_Teacher_Other_IncludingCorporalPunishment);

                    if (profS2TeacherA.Any())
                    {
                        pInfo = new PolicyInfo(retvalData)
                        {
                            TypeId = $"{DiamondPolicyTypeId.ProfessionalLiability:d}",
                            PersonalLiabilityLimit = personalLiabilityLimitForAll,
                            ProfessionalLiabilities = new List<ProfessionalLiability>()
                        };

                        pInfo.ProfessionalLiabilities.Add(new ProfessionalLiability
                        {
                            TypeId = _umbrellaProfessionalLiabilityTypes.Options.FirstOrDefault(dt => dt.Text.Contains("Teacher Including Corporal Punishment"))?.Value,
                            NumberOfItems = $"{profS2TeacherA.Count()}",
                            PolicyId = qq.PolicyId,
                            PolicyImageNum = qq.PolicyImageNum,
                            EffectiveDate = qq.EffectiveDate,
                            SetParent = pInfo
                        });

                    }

                    var profS2TeacherB = qq.Locations.SelectMany(l => l.SectionIICoverages ?? Enumerable.Empty<QuickQuoteSectionIICoverage>())
                                                     .Where(s2covs => s2covs?.CoverageType == QuickQuoteSectionIICoverage.SectionIICoverageType.BusinessPursuits_Teacher_Other_ExcludingCorporalPunishment);

                    if (profS2TeacherB.Any())
                    {
                        if (pInfo?.ProfessionalLiabilities is null)
                        {
                            pInfo = new PolicyInfo(retvalData)
                            {
                                TypeId = $"{DiamondPolicyTypeId.ProfessionalLiability:d}",
                                PersonalLiabilityLimit = personalLiabilityLimitForAll,
                                ProfessionalLiabilities = new List<ProfessionalLiability>()
                            };
                        }

                        pInfo.ProfessionalLiabilities.Add(new ProfessionalLiability
                        {
                            TypeId = _umbrellaProfessionalLiabilityTypes.Options.FirstOrDefault(dt => dt.Text.Contains("Teacher Excluding Corporal Punishment"))?.Value,
                            NumberOfItems = $"{profS2TeacherB.Count()}",
                            PolicyId = qq.PolicyId,
                            PolicyImageNum = qq.PolicyImageNum,
                            EffectiveDate = qq.EffectiveDate,
                            SetParent = pInfo
                        });
                    }

                    if (pInfo?.ProfessionalLiabilities?.Any() ?? false)
                    {
                        retvalData.PolicyInfos.Add(pInfo);
                    }


                    retval.Data = retvalData;
                    retval.Success = true;
                }
            }
            catch (Exception e)
            {
                retval.Success = false;
                retval.AddMessage(e.Message);
            }
            return retval;
        }
    }
}
