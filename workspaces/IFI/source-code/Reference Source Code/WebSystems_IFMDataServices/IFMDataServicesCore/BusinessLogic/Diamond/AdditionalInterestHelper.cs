using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCO = Diamond.Common.Objects;
using IDS = Insuresoft.DiamondServices;
using IFM.PrimitiveExtensions;
using QuickQuote.CommonMethods;
using Newtonsoft.Json.Serialization;
using IFM.DataServicesCore.CommonObjects;

namespace IFM.DataServicesCore.BusinessLogic.Diamond
{
    public static class AdditionalInterestHelper
    {
        public static List<DCO.Policy.AdditionalInterestList> AdditionalInterestLookup(DataServicesCore.CommonObjects.OMP.AdditionalInterest ai, bool AttemptToGetUniqueEntriesOnly = true)
        {
            using (var aiLookup = IDS.AdditionalInterestService.LookupLoad())
            {
                List<DCO.Policy.AdditionalInterestList> returnList = null;
                if (ai.Name != null)
                {
                    aiLookup.RequestData.Name = ai.Name.IsPersonalName == true ? ai.Name.DisplayName : ai.Name.CommercialName;
                    if (ai.Address != null)
                    {
                        int stateID = 0;

                        if (ai.Address.StateId > 0 && ai.Address.StateAbbrev.HasValue())
                        {
                            stateID = ai.Address.StateId;
                        }
                        else if (ai.Address.StateAbbrev.HasValue())
                        {
                            var myModelBase = new IFM.DataServicesCore.CommonObjects.OMP.ModelBase();
                            stateID = CommonObjects.IFM.StaticData.StaticDataHelper.GetStaticDataValueForTextAsInt(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.StateId, ai.Address.StateAbbrev.ToUpper());
                        }
                        else if (ai.Address.State.HasValue())
                        {
                            var myModelBase = new IFM.DataServicesCore.CommonObjects.OMP.ModelBase();
                            stateID = CommonObjects.IFM.StaticData.StaticDataHelper.GetStaticDataValueForText2(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.StateId, ai.Address.State).TryToGetInt32();
                        }

                        if (stateID > 0)
                        {
                            aiLookup.RequestData.StateId = stateID;
                        }
                        if (ai.Address.City.HasValue())
                        {
                            aiLookup.RequestData.City = ai.Address.City;
                        }
                        if (ai.Address.Zip.HasValue())
                        {
                            aiLookup.RequestData.Zip = ai.Address.Zip;
                        }
                    }

                    var invoke = aiLookup.Invoke();
                    DCO.InsCollection<DCO.Policy.AdditionalInterestList> myList = null;
                    if (invoke?.DiamondResponse?.ResponseData?.AdditionalInterestLists?.Count > 0)
                    {
                        myList = invoke?.DiamondResponse?.ResponseData?.AdditionalInterestLists;
                    }

                    if(myList?.Count > 0)
                    {
                        var filteredList = myList.ToList();
                        if (ai.Address.HouseNumber.IsNumeric())
                        {
                            filteredList = filteredList.FindAll(x => x.Address.HouseNumber.StringsAreEqual(ai.Address.HouseNumber));
                        }
                        if (ai.Address.StreetName.HasValue() && myList?.Count > 0)
                        {
                            filteredList = filteredList.FindAll(x => x.Address.StreetName.StringsAreEqual(ai.Address.StreetName));
                        }
                        if (ai.Address.PoBox.HasValue() && myList?.Count > 0)
                        {
                            filteredList = filteredList.FindAll(x => x.Address.POBox.StringsAreEqual(ai.Address.PoBox));
                        }
                        if (ai.Address.County.HasValue() && myList?.Count > 0)
                        {
                            filteredList = filteredList.FindAll(x => x.Address.County.StringsAreEqual(ai.Address.County));
                        }

                        if (filteredList?.Count > 0)
                        {
                            if(filteredList.Count == 1)
                            {
                                return filteredList;
                            }
                            else
                            {
                                if (AttemptToGetUniqueEntriesOnly == true)
                                {
                                    returnList = filteredList?.DistinctBy(a => new { a.AdditionalInterestListId.Id }).ToList();
                                    if (returnList?.Count > 1)
                                    {
                                        returnList = returnList?.DistinctBy(a => new { a.FormattedDisplayAddress, a.FormattedDisplayName })?.OrderBy(x => x.AdditionalInterestListId.Id).ToList();
                                    }
                                }
                                else
                                {
                                    returnList = filteredList?.OrderBy(x => x.AdditionalInterestListId.Id).ToList();
                                }

                                if (returnList?.Count > 1)
                                {
                                    var returnListWithoutOther = returnList.FindAll(x => x.Address.Other == "");
                                    if (returnListWithoutOther?.Count > 0)
                                    {
                                        returnList = returnListWithoutOther;
                                    }
                                }
                            }
                        }
                    }
                }
                return returnList;
            }
        }

        public static List<DataServicesCore.CommonObjects.OMP.AdditionalInterest> AdditionalInterestLookup_MPAI(DataServicesCore.CommonObjects.OMP.AdditionalInterest ai, bool AttemptToGetUniqueEntriesOnly = true)
        {
            var aiList = AdditionalInterestLookup(ai, AttemptToGetUniqueEntriesOnly);
            var newList = ObjectConversions.DiamondAdditionalInterestListsToMPAdditionalInterests(aiList);
            return newList?.Count > 0 ? newList : null;
        }

        public static List<QuickQuote.CommonObjects.QuickQuoteAdditionalInterest> AdditionalInterestLookup_QQ(DataServicesCore.CommonObjects.OMP.AdditionalInterest ai, bool AttemptToGetUniqueEntriesOnly = true)
        {
            var aiList = AdditionalInterestLookup(ai, AttemptToGetUniqueEntriesOnly);
            var newList = ObjectConversions.DiamondAdditionalInterestListsToQQAdditionalInterests(aiList);
            return newList?.Count > 0 ? newList : null;
        }
    }
}
