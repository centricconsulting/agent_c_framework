using IFI.Integrations.Request.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFI.Integrations.Response
{
    public class AllCommercialPrefill
    {
        public IFI.Integrations.Response.Common.ServiceResult<ComFirmoGraphicsPrefill> Firmographics { get; set; }
        public List<IFI.Integrations.Response.Common.ServiceResult<ComPropertyPrefill>> Property { get; set; }
    }
}
