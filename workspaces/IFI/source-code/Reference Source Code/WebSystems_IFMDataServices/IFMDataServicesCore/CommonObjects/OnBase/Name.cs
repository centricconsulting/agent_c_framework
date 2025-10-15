using System;
//using Mapster;
using IFM.PrimitiveExtensions;

#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.CommonObjects.OnBase
{
    [System.Serializable]
    public class Name : ModelBase
    {
        public string DisplayName
        {
            get { return this.TypeId == 1 ? $"{this.FirstName} {this.LastName}".Trim() : this.CommercialName; }
        }

        public string FullDisplayName
        {
            get { return this.TypeId == 1 ? $"{this.PrefixName} {this.FirstName} {this.MiddleName} {this.LastName} {this.SuffixName}".Trim().Replace("  ", " ") : this.CommercialName; }
        }

        public bool IsPersonalName
        {
            get { return this.TypeId == 1; }
            set { }
        }

        public Int32 TypeId { get; set; }
        public string PrefixName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SuffixName { get; set; }

        //[AdaptMember("CommercialName1")]
        public string CommercialName { get; set; }


        public Name() { }


        public override string ToString()
        {
            return this.DisplayName;
        }
    }
}