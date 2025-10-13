using IFM.Configuration.Extensions;
using IFM.ControlFlags;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace IFM.VR.Flags.App
{
    public class DependencyInjection:IFeatureFlag
    {
        private const string USE_MEDI = "VR_DI_USE_MicrosoftExtensionsDependencyInjection";
        public bool UseMEDI => ConfigurationManager.AppSettings.As<bool>(USE_MEDI);
    }
}
