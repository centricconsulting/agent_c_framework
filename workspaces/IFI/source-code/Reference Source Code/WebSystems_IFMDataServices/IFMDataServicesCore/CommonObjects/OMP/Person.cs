using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class Person
    {
        public CommonObjects.OMP.Name Name { get; set; }
        public CommonObjects.OMP.Address Address { get; set; }
        public List<CommonObjects.OMP.Phone> Phone { get; set; }
        public List<CommonObjects.OMP.Email> Email { get; set; }
    }
}
