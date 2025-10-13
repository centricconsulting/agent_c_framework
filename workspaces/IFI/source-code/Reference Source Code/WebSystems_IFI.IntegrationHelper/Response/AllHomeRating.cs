using System;
using System.Collections.Generic;
using System.Text;

namespace IFI.Integrations.Response
{
    public class AllHomeRating
    {
        public IFI.Integrations.Response.Common.ServiceResult<WtwSis> WtwSis { get; set; }
        public IFI.Integrations.Response.Common.ServiceResult<CapeHom> CapeHom { get; set; }
    }
}
