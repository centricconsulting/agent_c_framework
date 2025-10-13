using System.Collections.Generic;
using System.Linq;
using DCO = Diamond.Common.Objects;

#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.CommonObjects.OMP.FAR
{
    [System.Serializable]
    public class FarLocation : Location
    {
        public List<FarmBuilding> Buildings { get; set; }
        public string FarmingType { get; set; }
        public string AcreageDescription { get; set; }
        public int Acres { get; set; }

        public FarLocation() { }
        internal FarLocation(DCO.Policy.Location dLocation) : base(dLocation)
        {
            if (dLocation != null)
            {
                if (dLocation.BarnsBuildings != null && dLocation.BarnsBuildings.Any())
                {
                    this.Buildings = new List<FarmBuilding>();
                    foreach (var b in dLocation.BarnsBuildings)
                    {
                        Buildings.Add(new FarmBuilding(b));
                    }
                }
                if (dLocation.Acreages != null && dLocation.Acreages.Any())
                {
                    if (dLocation.Acreages[0] != null)
                    {
                        this.AcreageDescription = dLocation.Acreages[0].Description;
                        this.Acres = dLocation.Acreages[0].Acreage;
                    }
                }

                //Farm TYpe
                if (dLocation.FarmTypeBees)
                {
                    FarmingType = "Bees";
                }
                else
                {
                    if (dLocation.FarmTypeDairy)
                    {
                        this.FarmingType = "Dairy";
                    }
                    else
                    {
                        if (dLocation.FarmTypeFeedLot)
                        {
                            this.FarmingType = "Feed Lot";
                        }
                        else
                        {
                            if (dLocation.FarmTypeFieldCrops)
                            {
                                this.FarmingType = "Field Crops";
                            }
                            else
                            {
                                if (dLocation.FarmTypeFlowers)
                                {
                                    this.FarmingType = "Flowers";
                                }
                                else
                                {
                                    if (dLocation.FarmTypeFruits)
                                    {
                                        this.FarmingType = "Fruits";
                                    }
                                    else
                                    {
                                        if (dLocation.FarmTypeFurbearingAnimals)
                                        {
                                            this.FarmingType = "Fur Bearing Animals";
                                        }
                                        else
                                        {
                                            if (dLocation.FarmTypeGreenhouses)
                                            {
                                                this.FarmingType = "Greenhouses";
                                            }
                                            else
                                            {
                                                if (dLocation.FarmTypeHobby)
                                                {
                                                    this.FarmingType = "Hobby Farm";
                                                }
                                                else
                                                {
                                                    if (dLocation.FarmTypeHorse)
                                                    {
                                                        this.FarmingType = "Horses";
                                                    }
                                                    else
                                                    {
                                                        if (dLocation.FarmTypeLivestock)
                                                        {
                                                            this.FarmingType = "Livestock";
                                                        }
                                                        else
                                                        {
                                                            if (dLocation.FarmTypeMushrooms)
                                                            {
                                                                this.FarmingType = "Mushrooms";
                                                            }
                                                            else
                                                            {
                                                                if (dLocation.FarmTypeNurseryStock)
                                                                {
                                                                    this.FarmingType = "Nursery Stock";
                                                                }
                                                                else
                                                                {
                                                                    if (dLocation.FarmTypeNuts)
                                                                    {
                                                                        this.FarmingType = "Nuts";
                                                                    }
                                                                    else
                                                                    {
                                                                        if (dLocation.FarmTypePoultry)
                                                                        {
                                                                            this.FarmingType = "Poultry";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (dLocation.FarmTypeSod)
                                                                            {
                                                                                this.FarmingType = "Sod";
                                                                            }
                                                                            else
                                                                            {
                                                                                if (dLocation.FarmTypeSwine)
                                                                                {
                                                                                    this.FarmingType = "Swine";
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (dLocation.FarmTypeTobacco)
                                                                                    {
                                                                                        this.FarmingType = "Tobacco";
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (dLocation.FarmTypeTurkey)
                                                                                        {
                                                                                            this.FarmingType = "Turkey";
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (dLocation.FarmTypeVegetables)
                                                                                            {
                                                                                                this.FarmingType = "Vegetables";
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (dLocation.FarmTypeVineyards)
                                                                                                {
                                                                                                    this.FarmingType = "Vineyards";
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (dLocation.FarmTypeWorms)
                                                                                                    {
                                                                                                        this.FarmingType = "Worms";
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        this.FarmingType = "Other";
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //Farm Type END
            }
#if DEBUG
            else
            {
                Debugger.Break();
            }
#endif
        }
    }
}