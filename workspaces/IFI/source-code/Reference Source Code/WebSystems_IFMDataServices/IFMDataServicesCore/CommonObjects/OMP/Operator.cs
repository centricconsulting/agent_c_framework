using System.Collections.Generic;
using System.Linq;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class Operator : ModelBase
    {
        public Name Name { get; set; }
        public Address Address { get; set; }
        public List<LossHistory> LossHistory { get; set; }

        public Operator() { }

        internal Operator(DCO.Policy.Operator dOper)
        {
            this.Name = new Name(dOper.Name, false);
            this.Address = new Address(dOper.Address);
            if (dOper.LossHistories != null && dOper.LossHistories.Any())
            {
                this.LossHistory = new List<LossHistory>();
                foreach (var l in dOper.LossHistories)
                {
                    this.LossHistory.Add(new LossHistory(l));
                }
            }
        }

        public override string ToString()
        {
            return this.Name != null ? Name.ToString() : "Name is NUll";
        }
    }
}