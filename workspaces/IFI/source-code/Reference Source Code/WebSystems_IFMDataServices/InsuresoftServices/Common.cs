using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCS = Diamond.Common.Services;

namespace Insuresoft.DiamondServices
{
    public static class Common
    {
        public static DCS.DiamondSecurityToken Token {
            get
            { return DCS.Proxies.ProxyBase.DiamondSecurityToken; }
        }

        public static void SetDiamondToken(DCS.DiamondSecurityToken Token)
        {
            DCS.Proxies.ProxyBase.DiamondSecurityToken = Token;
        }

    }
}
