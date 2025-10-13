using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFM.DataServicesCore.BusinessLogic.OMP
{
    public static class DiamondLogin
    {
        public static bool OMPLogin()
        {
            if (IFM.DataServicesCore.BusinessLogic.Diamond.Login.IsLoggedIN() == false)
            {
                IFM.DataServicesCore.BusinessLogic.Diamond.Login.LoginNow(AppConfig.PrintUserName, AppConfig.PrintUserPassword);
            }
            return IFM.DataServicesCore.BusinessLogic.Diamond.Login.IsLoggedIN();
        }
    }
}
